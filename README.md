# Umbraco Look (Alpha)
Extends Umbraco Examine adding support for: text match highlighting, geospatial querying and tag faceting.

[The NuGet Package](https://www.nuget.org/packages/Our.Umbraco.Look) installs a single assembly _Our.Umbraco.Look.dll_ with dependencies on: 

  * Umbraco 7.2.3 (min)
  * Examine 0.1.70 (min)
  * Lucene.Net.Contrib 2.9.4.1 (min)

## Indexing

Look will add the following (optional) fields to each document in an Examine managed index: Name, Date, Text, Tags and a Location. (Each field is prefixed with Our.Umbraco.Look to ensure uniqueness)
  
No configuration files need to be changed as Look will hook into the default Umbraco External indexer and searcher, otherwise the following appSetting keys can be set in the web.config:

```xml
<appSettings>
	<add key="Our.Umbraco.Look.IndexerName" value="MyIndexer" />
	<add key="Our.Umbraco.Look.SearcherName" value="MySearcher" />
</appSettings>
```

To configure indexing there are static methods on the `LookService` class which accept functions taking a parameter of IPublishedContent (ipc) that return the typed value to be indexed (all are optional).

```csharp
using Our.Umbraco.Look.Services;
using Umbraco.Core;

public class ConfigureIndexing : ApplicationEventHandler
{	
	/// <summary>
	/// Umbraco has started event
	/// </summary>
	protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
	{
		LookService.SetNameIndexer(ipc => {			
			// return string or null
		});

		LookService.SetDateIndexer(ipc => {
			// return DateTime or null
		});

		LookService.SetTextIndexer(ipc => {		
			// return string or null
		});

		LookService.SetTagIndexer(ipc => {
			// return string[] or null
		});

		LookService.SetLocationIndexer(ipc => {
			// return Our.Umbraco.Look.Model.Location or null
			// eg. return new Location(55.406330, 10.388500);		

            var terratype = ipc.GetProperty("location").Value as Terratype.Models.Model;

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

A Look query consists of any combinations of the following (optional) query types: `RawQuery`, `NodeQuery`, `DateQuery`, `TextQuery`, `TagQuery`, & `LocationQuery`.


```csharp
using Our.Umbraco.Look.Models;  
using Our.Umbraco.Look.Services;  

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
		AllTags = new string[] { "tag1", "tag2" }, // both tag1 and tag2 are required
		AnyTags = new string[] { "tag3", "tag4", "tag5" }, // at least one of these tags is required
		// TODO: NotTags = new string[] { "tag6" }, // results must not have any of these tags
		GetFacets = true // facet counts will be returned for tags
	},

	LocationQuery = new LocationQuery() {
		Location = new Location(55.406330, 10.388500), // a location means distance results can be set
		MaxDistance = new Distance(500, DistanceUnit.Miles)  // limits the results to within this distance
	},

	SortOn = SortOn.Distance // other sorts are: Score (default), Name, DateAscending, DateDescending
};

// perform the search
var lookResults = LookService.Query(lookQuery);
```

### Search Results

```csharp
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
