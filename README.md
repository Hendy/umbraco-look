# Umbraco Look (Alpha)
Umbraco Examine Lucene indexer and searcher with support for text match highlighting and geospatial queries.

[The NuGet Package](https://www.nuget.org/packages/Our.Umbraco.Look) installs a single assembly _Our.Umbraco.Look.dll_ with dependencies on: 

  * Umbraco 7.2.3 (min)
  * Examine 0.1.70 (min)
  * Lucene.Net.Contrib 2.9.4.1 (min)

## Indexing

Look will add the following data to an Examine managed index:

  * A name field - used to associate a name with a node (defaults to the IPublishedContent.Name).  

  * A date field - used to associate a date with a node (defaults to the IPublishedContent.UpdatedDate). 

  * A text field - used to store all text associated with a node into a single field and as the source for any extracted text highlight fragments.
  
  * A tags field - a collection of strings associated with a node (currently all expected to be lowercase & some chars are to be reserved).
    
  * A location field - used to store a latitude & longitude associated with a node (defaults to null).  
  
No configuration files need to be changed as Look will hook into the default Umbraco External indexer and searcher, otherwise the following appSetting keys can be set in the web.config:

```xml
<appSettings>
	<add key="Our.Umbraco.Look.IndexerName" value="MyIndexer" />
	<add key="Our.Umbraco.Look.SearcherName" value="MySearcher" />
</appSettings>
```

To configure the indexing behaviour there are static methods on the `LookIndexService` class which accept functions taking a parameter of IPublishedContent and returning the typed value to be indexed (all are optional).

E.g.:

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
		LookIndexService.SetNameIndexer(ipublishedContent => {			
			// return a string or null or 
			return LookIndexService.DefaultNameIndexer(ipublishedContent);			
		});

		LookIndexService.SetDateIndexer(ipublishedContent => {
			// return DateTime or null or
			return LookIndexService.DefaultDateIndexer(ipublishedContent);
		});

		LookIndexService.SetTextIndexer(ipublishedContent => {		
			// return a string or null or 
			return LookIndexService.DefaultTextIndexer(ipublishedContent);			
		});

		LookIndexService.SetTagIndexer(ipublishedContent => {
			// return string[] or null or 
			return LookIndexService.DefaultTagIndexer(ipublishedContent);
		});

		LookIndexService.SetLocationIndexer(ipublishedContent => {
			// return Our.Umbraco.Look.Model.Location or null
			// eg. return new Location(55.406330, 10.388500);		
			return null;			
		});
	}
}
```

## Searching

A `Look` search consists of any combinations of the following (optional) query types:  `NodeQuery` , `DateQuery`, `TextQuery`, `TagQuery`, & `LocationQuery` (most values are also optional).

E.g.:

```csharp
using Our.Umbraco.Look.Models;  
using Our.Umbraco.Look.Services;  

var lookQuery = new LookQuery()
{
	NodeQuery = new NodeQuery() {
		TypeAliases = new string[] { "myDocTypeAlias" },
		ExcludeIds = new int[] { 123 } // (eg. exclude current page)
	},

	DateQuery = new DateQuery() {
		Before = null,
		After = new DateTime(2005, 02, 16);
	},

	TextQuery = new TextQuery() {
		SearchText = "some text to search for",
		HighlightFragments = 2 // highlight text containing the search term twice should be returned
		HighlightSeparator = " ... ", // text to inject between any search term matches
		GetText = true // indicate that the raw text field should also be returned (potentially a large document)
	},

	TagQuery = new TagQuery() {
		AllTags = new string[] { "tag1", "tag2" }, // both tag1 and tag2 are required
		AnyTags = new string[] { "tag3", "tag4", "tag5" } // at least one of these tags is required
	},

	LocationQuery = new LocationQuery() {
		Location = new Location(55.406330, 10.388500), // a location means distance results can be set
		MaxDistance = new Distance(500, DistanceUnit.Miles)  // limits the results to within this distance
	},

	SortOn = SortOn.Distance // other sorts include: Score, Name, DateAscending, DateDescending
};

// perform the search
var lookResults = LookSearchService.Query(lookQuery);

var totalResults = lookResults.Total; // total number of item expected in the lookResults enumerable
var results = lookResults.ToArray(); // the lookResults enumerated into an array
```

### Search Results

A enumeration of the following `LookMatch` objects are returned:

```csharp
public class LookMatch
{
	/// <summary>
	/// The Lucene score for this match
	/// </summary>
	public float Score { get; internal set; }

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
	/// Highlight text (containing search text) extracted from from the full text
	/// </summary>
	public IHtmlString Highlight { get; internal set; }

	/// <summary>
	/// Tag collection
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
}
```
