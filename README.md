# Umbraco Look (Alpha)
Extends Umbraco Examine adding support for: text match highlighting, geospatial querying and tag faceting.

[The NuGet Package](https://www.nuget.org/packages/Our.Umbraco.Look) installs a single assembly _Our.Umbraco.Look.dll_ with dependencies on: 

  * Umbraco 7.2.3 (min)
  * Examine 0.1.70 (min)
  * Lucene.Net.Contrib 2.9.4.1 (min)

## Indexing

Look can add the following (optional) fields to each document in an Umbraco Examine managed index: Name, Date, Text, Tags and a Location. (Each field is prefixed with Our.Umbraco.Look to ensure uniqueness)
  
No configuration files need to be changed as Look will use default Examine searcher (unless otherwise specified in a Look query).

To configure the indexing behaviour there are static methods on the `LookService` class where (optional) custom indexers can be registered.

Each custom indexer is supplied with an IndexingContext model which contains details as to the IPublishedContent representation of the content, media or member being indexed, together with the name of the Examine index being operated upon.

```csharp
using Our.Umbraco.Look.Services;
using Our.Umbraco.Look.Models;
using Umbraco.Core;

public class ConfigureIndexing : ApplicationEventHandler
{	
	/// <summary>
	/// Umbraco has started event
	/// </summary>
	protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
	{
		LookService.SetNameIndexer(indexingContext => {			
			// return string (or null to not index)
			return indexingContext.Item.Name; // fallback
		});

		LookService.SetDateIndexer(indexingContext => {
			// return DateTime (or null to not index)
			return indexingContext.Item.UpdateDate; // fallback
		});

		LookService.SetTextIndexer(indexingContext => {		
			// return string (or null to not index)
			return null;
		});

		LookService.SetTagIndexer(indexingContext => {
			// return Our.Umbraco.Look.Models.Tag[] (or null to not index)
			return null;
		});

		LookService.SetLocationIndexer(indexingContext => {
			// return Our.Umbraco.Look.Model.Location (or null to not index)
			// eg. return new Location(55.406330, 10.388500);		

			var terratype = indexingContext.Item.GetProperty("location").Value as Terratype.Models.Model;

			if (terratype != null)
			{
				var latLng = terratype.Position.ToWgs84();

				if (latLng != null)
				{
					return new Location(latLng.Latitude, latLng.Longitude);
				}
			}

			return null;			
		});
	}
}

```

The Our.Umbraco.Look.Model.IndexingContext model:

```csharp
public class IndexingContext
{
    /// <summary>
    /// The IPublishedContent representation of the Content, Media or Member being indexed
    /// </summary>
    public IPublishedContent Item { get; }

    /// <summary>
    /// The name of the Examine indexer into which this item is being indexed
    /// </summary>
    public string IndexerName { get; }
}

```

## Searching

A Look query consists of any combinations of the following (optional) query types: `RawQuery`, `NodeQuery`, `DateQuery`, `TextQuery`, `TagQuery`, & `LocationQuery`.

```csharp
using Our.Umbraco.Look.Models;  

var lookQuery = new LookQuery()
{
	RawQuery = "+path: 1059",

	NodeQuery = new NodeQuery() {
		TypeAliases = new string[] { "myDocTypeAlias" },
		NotIds = new int[] { 123 } // (eg. exclude current page)
	},

	DateQuery = new DateQuery() {
		After = new DateTime(2005, 02, 16),
		Before = null
	},

	TextQuery = new TextQuery() {
		SearchText = "some text to search for",
		GetHighlight = true, // return highlight extract from the text field containing the search text
		GetText = true // raw text field should be returned (potentially a large document)
	},

	TagQuery = new TagQuery() {

		AllTags = new Tag[] { 
						new Tag("tag1"), // tag in the 'nameless group'
						new Tag("group1", "tag2") }, // tag in a named group

		AnyTags = new Tag[] { 
						new Tag("tag3"), 
						new Tag("tag4") }, // at least one of these tags (in name-less group) is required

		// TODO: NotTags = new Tag[] { new Tag("tag6") }, // results must not have any of these tags (any tags here that are also in either AllTags or AnyTags, will cause an empty result)

		GetFacets = new string[] { "", "group1" } // facet counts will be returned for tags in the 'name-less' group and group1
	},

	LocationQuery = new LocationQuery() {
		Location = new Location(55.406330, 10.388500), // a location means distance results can be set
		MaxDistance = new Distance(500, DistanceUnit.Miles)  // limits the results to within this distance
	},

	SortOn = SortOn.Distance // other sorts are: Score (default), Name, DateAscending, DateDescending
};

```

By default, a Look query will use the default Examine searcher (usually "ExternalSearcher"), however if you want to query a diffent index, the LookQuery model has an
overloaded constructor where a spcific Exmaine searcher name can be supplied.

```csharp
using Our.Umbraco.Look.Models;  

var lookQuery = new LookQuery("InternalMemberSearcher");

```

### Search Results

```csharp
using Our.Umbraco.Look.Services;

// perform the search
var lookResults = LookService.Query(lookQuery);

var totalResults = lookResults.Total; // total number of item expected in the lookResults enumerable
var results = lookResults.ToArray(); // returns Our.Umbraco.Look.Models.LookMatch[]
var facets = lookResults.Facets; // returns Our.Umbraco.Look.Models.Facet[]

public class LookMatch
{
	/// <summary>
	/// The Umbraco node Id of the matched item
	/// </summary>
	public int Id { get; internal set; }
	
	/// <summary>
	/// The custom name field
	/// </summary>
	public string Name { get; internal set; }

	/// <summary>
	/// The custom date field
	/// </summary>
	public DateTime? Date { get; internal set; }

	/// <summary>
	/// The full text (only returned if specified)
	/// </summary>
	public string Text { get; internal set; }

	/// <summary>
	/// Highlight text (containing search text) extracted from the full text
	/// </summary>
	public IHtmlString Highlight { get; internal set; }

	/// <summary>
	/// Tag collection from the custom tags feild
	/// </summary>
	public string[] Tags { get; internal set; }

	/// <summary>
	/// The custom location (lat|lng) field
	/// </summary>
	public Location Location { get; internal set; }

	/// <summary>
	/// The calculated distance (only returned if a location supplied in query)
	/// </summary>
	public double? Distance { get; internal set; }

	/// <summary>
	/// The Lucene score for this match
	/// </summary>
	public float Score { get; internal set; }
}

public class Facet
{
	/// <summary>
	/// The name of the tag
	/// </summary>
	public string Tag { get; internal set; }

	/// <summary>
	/// The total number of results expected should this tag be added to TagQuery.AllTags on the current query
	/// </summary>
	public int Count { get; internal set; }
}

```