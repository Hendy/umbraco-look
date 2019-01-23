# Look (Beta) for Umbraco Examine
Look sits on top of [Umbraco Examine](https://our.umbraco.com/documentation/reference/searching/examine/) adding support for: text match highlighting, geospatial querying, tag faceting and the indexing of detached IPublishedContent.

[The NuGet Package](https://www.nuget.org/packages/Our.Umbraco.Look) installs a single assembly _Our.Umbraco.Look.dll_ with dependencies on: 

  * Umbraco 7.3.0 (min)
  * Examine 0.1.70 (min)
  * Lucene.Net.Contrib 2.9.4.1 (min)

 ## Indexing

Look can be used index additional `name`, `date`, `text`, `tag` and `location` data into Lucene indexes, in two ways:

Firstly, once installed, it offers .Net seams for adding such data into any configured Umbraco Examine indexers (referred to as 'hook' indexing). 
This can be useful as no configuration files need to be changed as it can hook into the default _External_, _Internal_, and _InternalMember_ indexers (or any others that have been setup).

Secondly Look includes an Examine indexer that when configured (see [Examine configuration](https://our.umbraco.com/Documentation/Reference/Config/ExamineSettings/)) can 
also index properties that return collections of IPublishedContent (eg. Nested Content) in additional to the Umbraco content, media and member nodes (referred to as 'look' indexing).

To configure the indexing behaviour for either of the two ways, functions can be set via static methods on the LookConfiguration class (all are optional).
If a function is set and returns a value then custom Lucene field(s) prefixed with "Look_" will be used to store that value.

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
	/// The name of the Examine indexer into which this item is being indexed
	/// </summary>
	public string IndexerName { get; }

	/// <summary>
	/// The Content, Media, Member or Detached item being indexed (always has a value)
	/// </summary>
	public IPublishedContent Item { get; }

	/// <summary>
	/// When a detached item is being indexed, this property will be the hosting content, media or member 
	/// (this value will null when the item being indexed is not Detached)
	/// </summary>
	public IPublishedContent HostItem { get; }
}
```
[Example Indexing Code](../../wiki/Example-Indexing)


## Searching

Searching is performed using a configured (Umbraco or Look) Examine Searcher and can be done using the Exmaine API, or with the Look API.

To be able to use the additional query types that Look introduces (text highlighting, tag faceting etc...) with the Exmaine API, a Look searcher needs to be used
(rather than an Umbraco searcher). This is because a Look searcher will return an object as ISearchCritera that can be cast to LookSearchCriteria where the Look query types can be set.

### Examine API

```csharp
var searcher = ExamineManager.Instance.SearchProviderCollection["MyLookSearcher"];
var searchCriteria = searcher.CreateSearchCriteria();
var lookSearchCriteria = (LookSearchCriteria)searchCriteria;

lookSearchCriteria.NodeQuery = ...
lookSearchCriteria.NameQuery = ...
lookSearchCriteria.DateQuery = ...
lookSearchCriteria.TextQuery = ...
lookSearchCriteria.TagQuery = ...
lookSearchCriteria.LocationQuery = ...
lookSearchCriteria.ExamineQuery = ...
lookSearchCriteria.RawQuery = ...

var results = searcher.Search(searchCriteria);
```

### Look API

The Look API can be used with all searchers. Eg.

```csharp
var lookQuery = new LookQuery(); // use the default Examine searcher (usually "ExternalSearcher")
```

or

```csharp
var lookQuery = new LookQuery("MyLookSearcher"); // use a named Examine searcher
```

```csharp
lookQuery.NodeQuery = ...
lookQuery.NameQuery = ...
lookQuery.DateQuery = ...
lookQuery.TextQuery = ...
lookQuery.TagQuery = ...
lookQuery.LocationQuery = ...
lookQuery.ExamineQuery = ...
lookQuery.RawQuery = ...

var results = lookQuery.Search();
```

### Look Query types

All query types are optional, but when set, they become a required clause. All queries will return a LookResult, which has a boolean Success flag property. The flag
is set to true when a query with at least one clause is executed sucessfully.

#### NodeQuery
A node query is used to specify search criteria based on common properties of IPublishedContent (all properties are optional).

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
If a name query is set (ie, not null), then results must have a name value.

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

A date query is used together with a custom date indexer and enables date range queries (all properties are optional).
If a date query is set (ie, not null), then results must have a date value.

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
are optional).

```csharp
lookQuery.TextQuery = new TextQuery() {
	SearchText = "some text to search for",
	GetHighlight = true // return highlight extract from the text field containing the search text
}
```

#### TagQuery

A tag query is used together with a custom tag indexer (all properties are optional).
If a tag query is set (ie, not null), then each result must have at least one tag.





All = each result must contain all of these tags.

Any = each result must contain at least one tag from each of the collections supplied.

None = each result must not contain any of these tags.

The FacetOn proeperty is used to specify how tag faceting is caluculated (see Facets section below).

```csharp
lookQuery.TagQuery = new TagQuery() {
	All = new[] { new LookTag("size:large") };
	Any = new LookTag[][] { new[] { new LookTag("colour:red"), new LookTag("colour:green") } };
	None = TagQuery.MakeTags("colour:black"), // helper to make collection
	FacetOn = new TagFacetQuery("colour", "size", "shape")
};
```

#### LocationQuery

A location query is used together with a custom location indexer. If a Location alone is set, then all results which 
have a location indexed will have a distance value returned. However if a MaxDistance is also set, then only results
within that range are returned.

```csharp
lookQuery.LocationQuery = new LocationQuery() {
	Location = new Location(55.406330, 10.388500),
	MaxDistance = new Distance(500, DistanceUnit.Miles)
};
```

#### ExamineQuery

Examine ISearchCriteria can be passed into a LookQuery.

```charp
lookQuery.ExamineQuery = myExamineQuery.Compile();
```

#### RawQuery

String property for any [Lucene raw query](http://www.lucenetutorial.com/lucene-query-syntax.html).

```csharp
lookQuery.RawQuery = "+myField: myValue";
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
