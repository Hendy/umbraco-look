# Umbraco Look (Alpha)
Look sits on top of [Umbraco Examine](https://our.umbraco.com/documentation/reference/searching/examine/) adding support for: text match highlighting, geospatial querying and tag faceting.
Examine manages the indexes, whilst Look adds 'config-file-free' C# indexing and a lightweight querying API.

[The NuGet Package](https://www.nuget.org/packages/Our.Umbraco.Look) installs a single assembly _Our.Umbraco.Look.dll_ with dependencies on: 

  * Umbraco 7.3.0 (min)
  * Examine 0.1.70 (min)
  * Lucene.Net.Contrib 2.9.4.1 (min)


Look can be used in a number of ways. Once installed Look offers .net seams for indexing IPublishedConent, be they content, media, members nodes or properties that return collections of IPublishedContent (eg. Nested Content).



## Indexing

Look automatically hooks into all Umbraco Exmaine indexers offering the ability to create additional Lucene fields for `name`, `date`, `text`, `tags` and `location` data.
To configure the indexing behaviour, functions can be set via static methods on the LookConfiguration class (all are optional).
If a function is set and returns a value then custom Lucene field(s) prefixed with "Look_" will be used.
(On a default Umbraco install, the indexers are: "ExternalIndexer", "InternalIndexer" and "InternalMemberIndexer", see [/config/ExamineSettings.config](https://our.umbraco.com/Documentation/Reference/Config/ExamineSettings/)).

The static properties definitions on LookConfiguration where indexing functions can be set:

```csharp
public static class LookConfiguration
{
	// creates case sensitive and case insensitive fields (not analyzed) - for use with NameQuery
	public static Func<IndexingContext, string> NameIndexer { set; }

	// creates a date & sorting fields - for use with DateQuery
	public static Func<IndexingContext, DateTime?> DateIndexer { set; }

	// creates a text field (analyzed) - for use with TextQuery
	public static Func<IndexingContext, string> TextIndexer { set; }

	// creates a tag field for each tag group - for use with TagQuery
	public static Func<IndexingContext, LookTag[]> TagIndexer { set; }

	// creates multple fields - for use with LocationQuery
	public static Func<IndexingContext, Location> LocationIndexer { set; }
}
```

The model supplied to the indexing functions:

```csharp
public class IndexingContext
{
    /// <summary>
    /// When a detached item is being indexed, this property will be the hosting content, media or member
    /// </summary>
    public IPublishedContent HostItem { get; }

    /// <summary>
    /// The Content, Media, Member or Detacehd item being indexed
    /// </summary>
    public IPublishedContent Item { get; }

    /// <summary>
    /// The name of the Examine indexer into which this item is being indexed
    /// </summary>
    public string IndexerName { get; }
}
```

The index setters would typically be set in an Umbraco startup event (but they can be changed at any-time). Eg.

```csharp
public class ConfigureIndexing : ApplicationEventHandler
{	
	/// <summary>
	/// Umbraco has started event
	/// </summary>
	protected override void ApplicationStarted(
				UmbracoApplicationBase umbracoApplication, 
				ApplicationContext applicationContext)
	{				
		LookConfiguration.NameIndexer = indexingContext => { 
			
			// eg. always return the Name of the IPublishedContent
			return indexingContext.Item.Name; 
		};

		LookConfiguration.DateIndexer = indexingContext => { 
			
			// eg. news articles use a different date
			if (indexingContext.Item.DocumentTypeAlias == "newsArticle") 
			{
				return indexingItem.GetPropertyValue<DateTime>("releaseDate");
			}

			return indexingContext.Item.UpdateDate; 
		};

		LookConfiguration.TextIndexer = indexingContext => { 

			// eg. if content, render page and scrape markup
			if (!indexingContext.IsDetached && indexingContext.Item.ItemType == PublishedItemType.Content) 
			{				
				// (could pass in httpContext to render page without http web request)
				// return string
			}

			return null; // don't index
		};
		
		LookConfiguration.TagIndexer = indexingContext => {
			// return LookTag[] or null (see tags section below)

			// eg a nuPicker
			var picker = indexingContext.Item.GetPropertyValue<Picker>("colours");

			return picker
				.PickedKeys
				.Select(x => new LookTag("colour", x))
				.ToArray();
		};
		
		LookConfiguration.LocationIndexer = indexingContext => {
			// return Location or null

			// eg. using Terratype			 
			var terratype = indexingContext.Item.GetPropertyValue<Terratype.Models.Model>("location");
			var terratypeLatLng = terratype.Position.ToWgs84();

			return new Location(terratypeLatLng.Latitude, terratypeLatLng.Longitude);
		};
	}
}
```

## Searching

A LookQuery consists of any combinations of these query types: `ExamineQuery`, `RawQuery`, `NodeQuery`, `NameQuery`, `DateQuery`, `TextQuery`, `TagQuery`, & `LocationQuery`
together with an Examine Searcher. The LookQuery constructor is used to specify which Examine searcher to use:

```csharp
var lookQuery = new LookQuery(); // use the default searcher (usually "ExternalSearcher")
```

```csharp
var lookQuery = new LookQuery("InternalSearcher"); // use a named searcher
```

All query types are optional, but when set, they become a required clause. All queries will return a LookResult, which has a boolean Success flag property. The flag
is set to true when a query with at least one clause is executed sucessfully.

#### ExamineQuery

Examine ISearchCriteria can be passed into a LookQuery.

```charp
lookQuery.ExamineQuery = myExamineQuery.Compile();
```

#### RawQuery

String property for any [Lucene raw query](http://www.lucenetutorial.com/lucene-query-syntax.html) (this can be a way to pass in an Examine built query).

```csharp
lookQuery.RawQuery = "+myField: myValue";
```

#### NodeQuery
A node query is used to set search criteria based on the IPublishedContent type, alias and any Ids that should be excluded (all properties are optional).

```csharp
lookQuery.NodeQuery = new NodeQuery() {
	Types = new [] { 
		PublishedItemType.Content, 
		PublishedItemType.Media, 
		PublishedItemType.Member 
	},
	DetachedQuery = DetachedQuery.IncludeDetached, // enum options
	Cultures = new [] {
		new CultureInfo("fr")	
	},
	Aliases = new [] { 
		"myDocTypeAlias", 
		"myMediaTypeAlias",
		"myMemberTypeAlias"
	},
	Ids = new [] { 1,2,3 },
	Keys = new [] { 
		Guid.Parse("dc890492-4571-4701-8085-b874837d597a"), 
		Guid.Parse("9f60f10f-74ea-4323-98bb-13b6f6423ad6"),
		Guid.Parse("88a9e4e3-d4cb-4641-aff3-8579f1d60399")
	}
	NotIds = new [] { 123 }, // (eg. exclude current page)
	NotKeys = new [] { Guid.Parse("6bb24ed2-9466-422f-a9d4-27a805db2d47") }
};
```

#### NameQuery
A name query is used together with a custom name indexer and enables string comparrison queries (wildcards are not allowed and all properties are optional).
If a name query is contradictory (for example, Is = "Must be this" and StartsWith = "Something else"), then
an empty result will be returned with the Success flag being false. The NameQuery also has constructor overloads 
for all properties where each is optional (defaulting to a case sensitive search).

```csharp
lookQuery.NameQuery = new NameQuery() {
	Is = "Abc123Xyz",
	StartsWith = "Abc",
	Contains = "123",
	EndsWith = "Xyz",
	CaseSensitive = true // applies to all: Is, StartsWith, Contains & EndsWith
};
```

#### DateQuery

A date query is used together with a custom date indexer and enables date range queries (both date properties are optional).

```csharp
lookQuery.DateQuery = new DateQuery() {
	After = new DateTime(2005, 02, 16),
	Before = null,
	Boundary = DateBoundary.Inclusive
}
```

#### TextQuery

A text query is used together with a custom text indexer and allows for wildcard searching using the analyzer specified by Exmaine.
Highlighting gives the ability to return an html snippet of text indiciating the part of the full text that the match was made on. All properties
are optional, and there is a constructor where all properties are optional (by default GetHighlight and GetText are false).

```csharp
lookQuery.TextQuery = new TextQuery() {
	SearchText = "some text to search for",
	GetHighlight = true // return highlight extract from the text field containing the search text
}
```

#### TagQuery

A tag query is used together with a custom tag indexer.

The All, Any and Not properties expect LookTag[] values (see LookTags section below). 

If there are any query contradictions (such as a tag exsing in both All and Not), then
an empty result is returned with the success flag as false.

The FacetOn proeperty is used to specify how tag faceting is caluculated (see Facets section below).

```csharp
lookQuery.TagQuery = new TagQuery() {
	All = TagQuery.MakeTags("size:large"), // all of these tags
	Any = new LookTag[][] { TagQuery.MakeTags("colour:red", "colour:green", "colour:blue") } // at least one of these tags
	Not = TagQuery.MakeTags("colour:black"), // none of these tags, 'not' always takes priority,
	FacetOn = new TagFacetQuery("colour", "size", "shape")
};
```

#### LocationQuery

A location query is used together with a custom location indexer. If a Location alone is set, then all nodes which 
have a location indexed will have a distance value returned. However if a MaxDistance is also set, then only nodes
within that range are returned.

```csharp
lookQuery.LocationQuery = new LocationQuery() {
	Location = new Location(55.406330, 10.388500),
	MaxDistance = new Distance(500, DistanceUnit.Miles)
};
```

#### SortOn

If not specified then the reults will be sorted on the Lucene score, otherwise sorting can be set by the SortOn enum to use the custom name, date or distance fields.

### Search Results

The search can be performed by calling the Search method on the LookQuery object:

```csharp
var lookResult = lookQuery.Search();
```

When the query is performed, the source LookQuery model is compiled, such that it can be useful to hold onto a reference for any subsequent paging queries. The 
LookResult model returned implements Examine.ISearchResults so that it can be integrated into existing site architecture, whilst the Matches property will
return the results enumerable as strongly typed LookMatch objects.


```csharp
public class LookResult : ISearchResults
{
	/// <summary>
	/// When true, indicates the Look Query was parsed and executed correctly
	/// </summary>
	public bool Success { get; }
	
	/// <summary>
	/// Expected total number of results expected in the result enumerable
	/// </summary>
	public int TotalItemCount { get; }

	/// <summary>
	/// Get the results enumerable as LookMatch objects
	/// </summary>
	public IEnumerable<LookMatch> Matches { get; }

	/// <summary>
	/// Any returned facets
	/// </summary>
	public Facet[] Facets { get; }
}
```

Whilst the enumeration on the LookResult returns items as Exmaine SearchResult, they can be cast to LookMatch objects:

```csharp
public class LookMatch : SearchResult
{
	/// <summary>
	/// Lazy evaluation of the hosting IPublishedContent (if Item is detached, otherwise this will be null)
	/// </summary>
	public IPublishedContent HostItem { get; }

	/// <summary>
	/// Lazy evaluation of the associated IPublishedContent
	/// </summary>
	public IPublishedContent Item { get; }
	
	/// <summary>
	/// Unique key to this IPublishedContent
	/// </summary>
	public Guid Key { get; }

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
	/// The tags that would be added into the TagQuery.All clause
	/// </summary>
	public LookTag[] Tags { get; }

	/// <summary>
	/// The total number of results expected should this facet be applied to the curent query
	/// </summary>
	public int Count { get;  }
}
```

```csharp
public class LookTag
{
	/// <summary>
	/// The tag group - must contain only alphanumeric / underscore chars and be less than 50 chars.
	/// </summary>
	public string Group { get; set; }

	/// <summary>
	/// The tag name - this can be any string.
	/// </summary>
	public string Tag { get; set; }
}
```

### LookTags

A tag can be any string and exists within an optionally specified group (if a group isn't set, then the tag is put into a default un-named group - String.Empty).
A group string must only contain aphanumberic/underscore chars, and be less than 50 chars (as it is also used to generate a custom Lucene field name).

A LookTag can be constructed from specified group and tag values:

```csharp
LookTag(string group, string name)
```

or from a raw string value:

```csharp
LokTag(string value)
````

When constructing from a raw string value, the first colon char ':' is used as an optional delimiter between a group and tag.
eg.

```csharp
var tag1 = new LookTag("red"); // tag 'red', in default un-named group
var tag2 = new LookTag(":red"); // tag 'red', in default un-named group
var tag2 = new LookTag("colour:red"); // tag 'red', in group 'colour'
```

There is also a static helper on the TagQuery model which can be used as a shorthand to create a LookTag array. Eg.

```csharp
var tags = TagQuery.MakeTags("colour:red", "colour:green", "colour:blue", "size:large");
```

### Facets

Look interprets facets to mean: "if the current query asked something slightly different (the facet being the difference), then how many results would be returned instead ?".


Namespaces used in examples:
```csharp
using System;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Models;
using Examine;
using Examine.SearchCriteria;
using Our.Umbraco.Look;
```
