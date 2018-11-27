# Umbraco Look (Alpha)
Look sits on top of [Umbraco Examine](https://our.umbraco.com/documentation/reference/searching/examine/) adding support for: text match highlighting, geospatial querying and tag faceting.

[The NuGet Package](https://www.nuget.org/packages/Our.Umbraco.Look) installs a single assembly _Our.Umbraco.Look.dll_ with dependencies on: 

  * Umbraco 7.2.3 (min)
  * Examine 0.1.70 (min)
  * Lucene.Net.Contrib 2.9.4.1 (min)

Namespaces in code examples:
```csharp
using Umbraco.Core;
using Umbraco.Core.Models;
using Our.Umbraco.Look.Services;
using Our.Umbraco.Look.Models;
```

## Indexing

Look automatically hooks into all Umbraco Exmaine indexers offering the ability to create additional Lucene fields for `name`, `date`, `text`, `tags` and `location` data.
(The indexers are usually "ExternalIndexer", "InternalIndexer" and "InternalMemberIndexer" by default - see _/config/ExamineSettings.config_).

To configure the indexing behaviour, custom functions can be set via static methods on the LookService (all are optional).
If a custom function is set and returns a value, that value will be indexed into custom Lucene field(s) prefixed with "Look_".

The static method definitions on the LookService where custom indexing functions can be set:

```csharp
// creates both case sensitive and case insensitive fields (not analyzed) - for use with NameQuery
void LookService.SetNameIndexer(Func<IndexingContext, string> nameIndexer)

// creates a date & sorting fields - for use with DateQuery
void LookService.SetDateIndexer(Func<IndexingContext, DateTime?> dateIndexer)

// creates a text field (analyzed) - for use with TextQuery
void LookService.SetTextIndexer(Func<IndexingContext, string> textIndexer)

// creates a tag field for each tag group - for use with TagQuery
void LookService.SetTagIndexer(Func<IndexingContext, LookTag[]> tagIndexer)

// creates multple fields - for use with LocationQuery
void LookService.SetLocationIndexer(Func<IndexingContext, Location> locationIndexer)
```

The model supplied to the custom functions at index-time:

```csharp
public class IndexingContext
{
	/// <summary>
	/// The IPublishedContent of the Content, Media or Member being indexed
	/// </summary>
	public IPublishedContent Item { get; }

	/// <summary>
	/// The name of the Examine indexer into which this node is being indexed
	/// </summary>
	public string IndexerName { get; }
}
```

The index setters would typically be set in an Umbraco startup event (but they can be changed at any-time).

Examples:

```csharp
public class ConfigureIndexing : ApplicationEventHandler
{	
	/// <summary>
	/// Umbraco has started event
	/// </summary>
	protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
	{		

		// return the Name of the IPublishedContent
		LookService.SetNameIndexer(indexingContext => { return indexingContext.Item.Name; });

		// return the UpdateDate of the IPublishedContent
		LookService.SetDateIndexer(indexingContext => { return indexingContext.Item.UpdateDate; });

		// return text string or null
		LookService.SetTextIndexer(indexingContext => { 
			if (indexingContext.Item.ItemType == PublishedItemType.Content) {
				// eg. make web request and scrape markup to index
			}
			return null; 
		});

		// return an Our.Umbraco.Look.Models.LookTag[] or null (see tags section below)
		LookService.SetTagIndexer(indexingContext => {
			// eg a nuPicker
			var picker = indexingContext.Item.GetPropertyValue<Picker>("colours");

			return picker.PickedKeys.Select(x => new LookTag("colour", x)).ToArray();

			// or return TagQuery.MakeTags(picker.PickedKeys.Select(x => "colour" + x))
		});

		// return an Our.Umbraco.Look.Model.Location or null
		LookService.SetLocationIndexer(indexingContext => {
			// eg. using Terratype			 
			var terratype = indexingContext.Item.GetPropertyValue<Terratype.Models.Model>("location");
			var terratypeLatLng = terratype.Position.ToWgs84();

			return new Location(terratypeLatLng.Latitude, terratypeLatLng.Longitude);
		});
	}
}
```

## Searching

A LookQuery consists of any combinations of these query types: `RawQuery`, `NodeQuery`, `NameQuery`, `DateQuery`, `TextQuery`, `TagQuery`, & `LocationQuery`
(all optional), together with an Examine Searcher.

The constructor is used to specify which Examine searcher to use:

```csharp
var lookQuery = new LookQuery(); // use the default searcher (usually "ExternalSearcher")
```

```csharp
var lookQuery = new LookQuery("InternalSearcher"); // use named searcher
```

Example of all query properties (all are optional, and the query type constructors have helper overloads):

```csharp
var lookQuery = new LookQuery("InternalSearcher")
{
	RawQuery = "+path: 1059",

	NodeQuery = new NodeQuery() {
		Types = new PublishedItemType[] { PublishedItemType.Content, PublishedItemType.Media, PublishedItemType.Member },
		Aliases = new string[] { "myDocTypeAlias", "myMediaTypeAlias" },
		NotIds = new int[] { 123 } // (eg. exclude current page)
	},

	NameQuery = new NameQuery() {
		Is = "Abc123Xyz", // the name must be equal to this string
		StartsWith = "Abc", // the name must start with this string
		Contains = "123", // the name must contain this string
		EndsWith = "Xyz",  // the name must end with this string
		CaseSensitive = true // applies to all: Is, StartsWith, Contains & EndsWith
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
		All = TagQuery.MakeTags("size:large"), // all of these tags
		Any = TagQuery.MakeTags("colour:red", "colour:green", "colour:blue") // at least one of these tags
		Not = TagQuery.MakeTags("colour:black"), // none of these tags, 'not' always takes priority, (contradictions return an empty result)
		GetFacets = new string[] { "colour", "size", "shape" } // request counts for all tags in the supplied groups
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
var lookResult = LookService.Query(lookQuery); // returns Our.Umbraco.Look.Model.LookResult

var totalResults = lookResult.Total; // total number of item expected in the lookResult enumerable
var results = lookResult.ToArray(); // enumerates to return Our.Umbraco.Look.Models.LookMatch[]
var facets = lookResult.Facets; // returns Our.Umbraco.Look.Models.Facet[]
```

```csharp
 public class LookResult : IEnumerable<LookMatch>
{
	/// <summary>
	/// When true, indicates the Look Query was parsed and executed correctly
	/// </summary>
	public bool Success { get; }
	
	/// <summary>
	/// Expected total number of results expected in the enumerable of LookMatch results
	/// </summary>
	public int Total { get; }

	/// <summary>
	/// Any returned facets
	/// </summary>
	public Facet[] Facets { get; }
}
```

```csharp
public class LookMatch
{
	/// <summary>
	/// The Umbraco node Id of the matched item
	/// </summary>
	public int Id { get; }
	
	/// <summary>
	/// Lazy evaluation of the associated IPublishedContent
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

```csharp
public class LookTag
{
	/// <summary>
	/// The tag group - this is used as a custom Lucene field, and must contain only alphanumeric / underscore chars and be less than 50 chars.
	/// </summary>
	public string Group { get; set; }

	/// <summary>
	/// The tag name - this can be any string.
	/// </summary>
	public string Tag { get; set; }
}
```

### Tags

A tag can be any string and exists within an optionally specified group. If a group isn't set, then the tag is put into a default un-named group.
A LookTag can be created form a raw string (where the first colon char ':' is used as an optional delimiter between a group/tag) or via a named group, tag overload.
A LookTag array can also be made via a static helper on the TagQuery mode.
eg.

```csharp
var tag1 = new LookTag("red"); // tag 'red', in default un-named group ''
var tag2 = new LookTag("colour:red"); // tag 'red', in group 'colour'
var tag3 = new LookTag("colour", "red"); // tag 'red', in group 'colour'
var tags = TagQuery.MakeTags("colour:red", "colour:green"); // tags 'red' and 'green', both in group 'colour'
```


### Facets

