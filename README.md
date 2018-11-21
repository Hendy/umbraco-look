# Umbraco Look (Alpha)
Look sits on top of [Umbraco Examine](https://our.umbraco.com/documentation/reference/searching/examine/) adding support for: text match highlighting, geospatial querying and tag faceting.

[The NuGet Package](https://www.nuget.org/packages/Our.Umbraco.Look) installs a single assembly _Our.Umbraco.Look.dll_ with dependencies on: 

  * Umbraco 7.2.3 (min)
  * Examine 0.1.70 (min)
  * Lucene.Net.Contrib 2.9.4.1 (min)


## Indexing

Look automatically hooks into all Umbraco Exmaine indexers (by default "ExternalIndexer", "InternalIndexer" and "InternalMemberIndexer") where the indexing behaviour can be configured by setting custom functions via static methods on the LookService. 

Each function at index-time will be given the IPublishedContent 
of the content, media or member node being indexed and details about the current Exmaine Indexer.

If an indexing function is not set or it returns null, then that field is not indexed, otherwise the value will be indexed into a custom field (prefixed with "Look_").

```csharp
using Our.Umbraco.Look.Services;
using Our.Umbraco.Look.Models;
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

			// IPublishedContent of the content, media or member being indexed
			var item = indexingContext.Item;

			// string name of the Examine indexer
			var indexerName = indexingContext.IndexerName;

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
			// eg. for content, trigger a web request and scrape markup to index

			return null;
		});

		LookService.SetTagIndexer(indexingContext => {
			// return Our.Umbraco.Look.Models.LookTag[] (or null to not index)

			// eg a nuPicker
			var picker = indexingContext.Item.GetPropertyValue<Picker>("colours");

			return picker
				.AsPublishedContent()
				.Select(x => new LookTag("colour", x.Name))
				.ToArray();
		});

		LookService.SetLocationIndexer(indexingContext => {
			// return Our.Umbraco.Look.Model.Location (or null to not index)
			// eg. return new Location(55.406330, 10.388500);

			var terratype = indexingContext.Item.GetPropertyValue<Terratype.Models.Model>("location");

			var terratypeLatLng = terratype.Position.ToWgs84();

			return new Location(
				terratypeLatLng.Latitude, 
				terratypeLatLng.Longitude);
		});
	}
}

```

## Searching

A Look query consists of any combinations of the following (optional) query types: `RawQuery`, `NodeQuery`, `DateQuery`, `TextQuery`, `TagQuery`, & `LocationQuery` together with an Examine Searcher.

```csharp
var lookQuery = new LookQuery("InternalSearcher") // (omit seracher name to use default, usually "ExternalSearcher")
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
		AllTags = new LookTag[] { new LookTag("size", "large") }, 

		// at least one of these tags must be present
		AnyTags = new LookTag[] { 
			new LookTag("colour", "red"), 
			new LookTag("colour", "green"), 
			new LookTag("colour", "blue")
		}, 

		// none of these tags must be present 
		// 'not' always takes priority ('not' always takes priority - 
		// any query contradictions will return an empty result with message)
		NotTags = new Tag[] { new Tag("colour:black") },

		GetFacets = new string[] { "", "group1" } // return facets for all tags in the 'name-less' group and group1
	},

	LocationQuery = new LocationQuery() {
		Location = new Location(55.406330, 10.388500), // a location means distance results can be set
		MaxDistance = new Distance(500, DistanceUnit.Miles)  // limits the results to within this distance
		// TODO: GetFacets = new Distance[] { }
	},

	SortOn = SortOn.Distance // other sorts are: Score (default), Name, DateAscending, DateDescending
};

```


### Search Results

```csharp
// perform the search
var lookResult = LookService.Query(lookQuery);

var totalResults = lookResult.Total; // total number of item expected in the lookResult enumerable
var results = lookResult.ToArray(); // returns Our.Umbraco.Look.Models.LookMatch[]
var facets = lookResult.Facets; // returns Our.Umbraco.Look.Models.Facet[]
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
	public LookTag[] Tags { get; }

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
	public LookTag Tag { get; }

	/// <summary>
	/// The total number of results expected should this tag be added to TagQuery.AllTags on the current query
	/// </summary>
	public int Count { get;  }
}

```
