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

			// indexingContext.Item is the IPublishedContent of the content, media or member being indexed
			// indexingContext.IndexerName is the string name of the Examine Indexer

			return indexingContext.Item.Name;
		});

		LookService.SetDateIndexer(indexingContext => { return indexingContext.Item.UpdateDate; });

		LookService.SetTextIndexer(indexingContext => {		

			// eg. for content, trigger a web request and scrape markup to index

			return null;
		});

		LookService.SetTagIndexer(indexingContext => {

			// return Our.Umbraco.Look.Models.LookTag[] (or null to not index)

			// A tag can be any string and exists within an optionally specified group.
			// If a group isn't set, then the tag is put into a default un-named group.
			// (using groups allows for targeted facet queries, as each group corresponds
			// with a custom field)
			// eg.
			//	"red" - a tag "red" in the default un-named group
			//	"colour:red" - a tag "red", in group "colour"
			// 
			// A group must contain only alphanumeric / underscore chars and be less 
			// than 50 chars. The first colon in the string is used as the delimeter, 
			// so to use a colon char in a tag (in the default un-named group) it must be 
			// escaped by prefixing with a colon.
			// eg.
			//	":red:green" - a tag "red:green" in the default un-named group
			//	"colour:red:green" - a tag "red:green" in the group "colour"
			//
			// A LookTag can be constructed with a raw string, and there is a static helper
			// as a shorthand to create a LookTag[]
			// eg.
			//	var tag = new LookTag("colour:red"); // raw string
			//	var tags = TagQuery.MakeTags("colour:red", "colour:green", "colour:blue"); 


			// eg a nuPicker
			var picker = indexingContext.Item.GetPropertyValue<Picker>("colours");

			return picker
				.AsPublishedContent()
				.Select(x => new LookTag("colour:" + x.Name))
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
		AllTags = TagQuery.MakeTags("size:large"),
		
		// at least one of these tags must be present (if single tag, then it's deemed mandatory as per AllTags)
		AnyTags = TagQuery.MakeTags("colour:red", "colour:green", "colour:blue")

		// none of these tags must be present 
		// ('not' always takes priority, any query contradictions will return an empty result with message)
		NotTags = TagQuery.MakeTags("colour:black"),

		GetFacets = new string[] { "colour", "size" } // return facet counts for all tags in the colour and size groups
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
