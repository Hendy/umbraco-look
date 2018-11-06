# Umbraco Look (Alpha)
Extends Umbraco Examine adding support for: text match highlighting, geospatial querying and tag faceting.

[The NuGet Package](https://www.nuget.org/packages/Our.Umbraco.Look) installs a single assembly _Our.Umbraco.Look.dll_ with dependencies on: 

  * Umbraco 7.2.3 (min)
  * Examine 0.1.70 (min)
  * Lucene.Net.Contrib 2.9.4.1 (min)


## Indexing

No configuration files need to be changed as Look hooks into all configured Umbraco Exmaine indexers (usually "InternalIndexer", "InternalMemberIndexer" and "ExternalIndexer").

The indexing behaviour is controlled by supplying indexing functions though static methods on the LookService. Each of these functions at index-time will be given the IPublishedContent representation
of the content, media or member being indexed and the name of the current Exmaine Indexer. If an indexer is not set, or returns null, then that field is not indexed, otherwise the value will be indexed into an additional field (prefixed "Look_" for uniqueness).

```csharp
using Our.Umbraco.Look.Services;
using Our.Umbraco.Look.Models;
using Tag = Our.Umbraco.Look.Models.Tag;
```

```csharp
public class ConfigureIndexing : ApplicationEventHandler
{	
	/// <summary>
	/// Umbraco has started event
	/// </summary>
	protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
	{
		LookService.SetNameIndexer(indexingContext => {			

			// the content, media or member being indexed
			var item = indexingContext.Item; // IPublishedContent 

			// the current Examine indexer
			var indexerName = indexingContext.IndexerName; // string

			if (indexerName == "ExternalIndexer")
			{
				return item.Parent.Name + " " + item.Name;
			}
			
			return null; // don't index
		});

		LookService.SetDateIndexer(indexingContext => {
			// return DateTime (or null to not index)

			return indexingContext.Item.UpdateDate;
		});

		LookService.SetTextIndexer(indexingContext => {		
			// return string (or null to not index)
			// eg. if content, trigger web request and scrape markup

			return null;
		});

		LookService.SetTagIndexer(indexingContext => {
			// return Our.Umbraco.Look.Models.Tag[] (or null to not index)
			// eg.

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

## Searching

A Look query consists of any combinations of the following (optional) query types: `RawQuery`, `NodeQuery`, `DateQuery`, `TextQuery`, `TagQuery`, & `LocationQuery` and an Exmaine Searcher.

```csharp
var lookQuery = new LookQuery() // use the default Examine searcher
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

		// all of these tags must be present
		AllTags = new Tag[] { 
				new Tag("tag1"), // tag in the 'nameless group'
				new Tag("group1", "tag2") // tag in a named group
		}, 

		// at least one of these tags must be present
		AnyTags = new Tag[] { 
				new Tag("tag3"), 
				new Tag("tag4") 
		}, 

		// none of these tags must be present
		NotTags = new Tag[] { new Tag("tag6") },

		GetFacets = new string[] { "", "group1" } // facet counts will be returned for tags in the 'name-less' group and group1
	},

	LocationQuery = new LocationQuery() {
		Location = new Location(55.406330, 10.388500), // a location means distance results can be set
		MaxDistance = new Distance(500, DistanceUnit.Miles)  // limits the results to within this distance
	},

	SortOn = SortOn.Distance // other sorts are: Score (default), Name, DateAscending, DateDescending
};

```


### Search Results

```csharp
// perform the search
var lookResults = LookService.Query(lookQuery);

var totalResults = lookResults.Total; // total number of item expected in the lookResults enumerable
var results = lookResults.ToArray(); // returns Our.Umbraco.Look.Models.LookMatch[]
var facets = lookResults.Facets; // returns Our.Umbraco.Look.Models.Facet[]
```

```csharp
public class LookMatch
{
	/// <summary>
	/// The Umbraco node Id of the matched item
	/// </summary>
	public int Id { get; }
	
	/// <summary>
	/// Lazy evaluation of the associated IPublishedContent item
	/// </summary>
	public IPublishedContent Item { get; }
	
	/// <summary>
	/// The custom name field
	/// </summary>
	public string Name { get; }

	/// <summary>
	/// The custom date field
	/// </summary>
	public DateTime? Date { get; }

	/// <summary>
	/// The full text (only returned if specified)
	/// </summary>
	public string Text { get; }

	/// <summary>
	/// Highlight text (containing search text) extracted from the full text
	/// </summary>
	public IHtmlString Highlight { get; }

	/// <summary>
	/// Collection of all tags associated with this item
	/// </summary>
	public Tag[] Tags { get; }

	/// <summary>
	/// The custom location (lat|lng) field
	/// </summary>
	public Location Location { get; }

	/// <summary>
	/// The calculated distance (only returned if a location supplied in query)
	/// </summary>
	public double? Distance { get; }

	/// <summary>
	/// The Lucene score for this match
	/// </summary>
	public float Score { get; }
}
```

```csharp
public class Facet
{
	/// <summary>
	/// The tag
	/// </summary>
	public Tag Tag { get; }

	/// <summary>
	/// The total number of results expected should this tag be added to TagQuery.AllTags on the current query
	/// </summary>
	public int Count { get;  }
}

```