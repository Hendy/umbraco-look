# Look (Beta) for Umbraco v7
Look sits on top of Umbraco Examine adding support for:

* Indexing all IPublishedContent items (each as a Lucene Document) be they: Umbraco Content, Media, Members or properties on them that return IPublishedContent*, eg. Nested Content.

* Tag querying & faceting - query on tags and return facet data for tags.

* Text match highlighting - return fragments of contextual text relevant to the supplied search term.

* Geospatial querying - boundary and location distance querying (this can also be used for filtering / sorting).

*use [PropertyValueConverters](https://our.umbraco.com/documentation/Extending/Property-Editors/value-converters) to return collections of IPublishedContent for any other data to be indexed.

## Installation

There are two NuGet packages:

[Our.Umbraco.Look](https://www.nuget.org/packages/Our.Umbraco.Look) (Required) the core Look indexer & searcher.

[Our.Umbraco.Look.BackOffice](https://www.nuget.org/packages/Our.Umbraco.Look.BackOffice) (Optional) index viewer for back office.

## Configuration

Once installed, by default Look will 'hook' into all Umbraco Examine indexers, augmenting each indexed item with its Guid key and Culture and
fields to enable case-insensitive searches on Name and sorting by UpdateDate, as well as for the node type and its alias. 

This default indexing behaviour is so that the Look querying API can be used 'out of the box' without having to configure anything, 
however all behaviour can all be configured via static properties on the LookConfiguration class. 
(All classes for the API can be found in the Our.Umbraco.Look namespace)

To configure a Look indexer, the Examine configuration files need to be updated:

/config/ExamineIndex.config
```xml
<ExamineLuceneIndexSets>
	<IndexSet SetName="MyLookIndexSet" IndexPath="~/App_Data/TEMP/ExamineIndexes/MyLookIndexSet/" />
</ExamineLuceneIndexSets>
```

/config/ExamineSettings.config
```xml
<Examine>
	<ExamineIndexProviders>
		<providers>
			<add name="MyLookIndexer" type="Our.Umbraco.Look.LookIndexer, Our.Umbraco.Look" />
		</providers>
	</ExamineIndexProviders>
	<ExamineSearchProviders>
		<providers>
			<add name="MyLookSearcher" type="Our.Umbraco.Look.LookSearcher, Our.Umbraco.Look" />
		</providers>
	</ExamineSearchProviders>
</Examine>
```

The static class where the indexing behaviour can be set:

```csharp
public static class LookConfiguration
{
	/// <summary>
	/// 'Hook indexing'
	/// Get or set the names of all the Examine indexers to use.
	/// By default, all Umbraco Examine indexers will be used.
	/// Set to null (or an empty array) not use any Examine indexers. 
	/// </summary>
	public static string[] ExamineIndexers { get; set; }

	/// <summary>
	/// Set configuration for indexer by name
	/// </summary>
	public static Dictionary<string, IndexerConfiguration> IndexerConfiguration { get; }

	/// <summary>
	/// (Optional) Custom method that can be called before the indexing of each IPublishedContent item.
	/// </summary>
	public static Action<IndexingContext> BeforeIndexing { set; }

	/// <summary>
	/// (Optional) Set a custom name indexer.
	/// By default, the IPublishedContent.Name value will be indexed.
	/// </summary>
	public static Func<IndexingContext, string> DefaultNameIndexer { set; }

	/// <summary>
	/// (Optional) Set a custom date indexer.
	/// By default, the IPublishedContent.UpdateDate value will be indexed. 
	/// (Detached items use value from their Host)
	/// </summary>
	public static Func<IndexingContext, DateTime?> DefaultDateIndexer { set; }

	/// <summary>
	/// (Optional) Set a custom text indexer.
	/// By default, no value is indexed.
	/// </summary>
	public static Func<IndexingContext, string> DefaultTextIndexer { set; }

	/// <summary>
	/// (Optional) Set a custom tag indexer.
	/// By default, no value is indexed.
	/// </summary>
	public static Func<IndexingContext, LookTag[]> DefaultTagIndexer { set; }

	/// <summary>
	/// (Optional) Set a custom location indexer.
	/// By default, no value is indexed.
	/// </summary>
	public static Func<IndexingContext, Location> DefaultLocationIndexer { set; }

	/// <summary>
	/// (Optional) Custom method that can be called after the indexing of each IPublishedContent item.
	/// </summary>
	public static Action<IndexingContext> AfterIndexing { set; }

	/// <summary>
    /// Specify which fields to return in the result set.
    /// Defaults to AllFields which will return all Lucene fields for each document, making it fully populate the Fields dictionary property on the SearchResult.
    /// Setting it to LookFieldsOnly reduces the number of Lucene fields returned to the min required to inflate a LookMatch object.
    /// </summary>
    public static RequestFields RequestFields { set; }

	/// <summary>
    /// (Optional) Specify the max number of results to return (defaults to 5000)
    /// </summary>
    public static int MaxResults { set { LookService.SetMaxResults(value); } }
}
```

The indexing context model:

```csharp
public class IndexingContext
{
	/// <summary>
	/// The name of the indexer into which this item is being indexed.
	/// </summary>
	public string IndexerName { get; }

	/// <summary>
	/// The Content, Media, Member or Detached item being indexed (always has a value).
	/// </summary>
	public IPublishedContent Item { get; }

	/// <summary>
	/// When the item being indexed is 'detached', this is the hosting Content, Media or Member.
	/// (this value will null when the item being indexed is not detached)
	/// </summary>
	public IPublishedContent HostItem { get; }

	/// <summary>
	/// Convienience flag to indicate whether the item is a detached item
	/// </summary>
	public bool IsDetached => this.HostItem != null;

	/// <summary>
	/// When called, the indexing of the current item will be cancelled.
	/// If using an Exmaine indexer, then Look will stop adding data from the point of cancellation.
	/// If using a Look indexer, then full cancellation occurs and a Lucene document will not be created.
	/// </summary>
	public void Cancel()
}
```
[Example Indexing Code](../../wiki/Example-Indexing)

There are two extension methods in the Our.Umbraco.Look namespace on the ExmaineManager class: (and the same type of methods can be found on the LookIndexer instance)

```csharp
ExamineManager.ReIndex(IEnumerable<int> ids) { }
```

```csharp
ExamineManager.ReIndex(IEnumerable<IPublishedContent nodes) { }
```


## Searching

Searching is performed using an Examine Searcher and can be done using the Exmaine API, or with the Look API.
The Look API consists of defining the search critera via the setting of pre-described model properties. 
This can be simplier to use than a fluent API, but complex queries can still be performed via the use of tags.

The Look API can be used with all searchers. Eg.

```csharp
var lookQuery = new LookQuery(); // use the default searcher (usually "ExternalSearcher")
```

or

```csharp
var lookQuery = new LookQuery("MyLookSearcher"); // use a named searcher
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
lookQuery.MaxResults = ...

var results = lookQuery.Search();
```

### Look Query Types

All query types are optional, but when set, they become a required clause. 


#### NodeQuery
A node query is used to specify search criteria based on common properties of IPublishedContent (all properties are optional).

```csharp
lookQuery.NodeQuery = new NodeQuery() {

	// must be of this type
	Type = PublishedItemType.Content,
	
	// can be any of these types (the condition above means it must be content)
	TypeAny = new [] { 
		PublishedItemType.Content, 
		PublishedItemType.Media, 
		PublishedItemType.Member 
	},
	
	// options: 
	// IncludeDetached (default - no filtering occurs)
	// ExcludeDetached
	// OnlyDetached
	DetachedQuery = DetachedQuery.IncludeDetached,
	
	// must be of this culture
	Culture = new CultureInfo("fr"),
	
	// can be any of these cultures
	CultureAny = new [] {
		new CultureInfo("fr")	
	},
	
	// must be of this docType/mediaType/memberType alias
	Alias = "myDocTypeAlias",
	
	// can be any of these docType/mediaType/memberType aliases
	AliasAny = new [] { 
		"myDocTypeAlias", 
		"myMediaTypeAlias",
		"myMemberTypeAlias"
	},
	
	// must have any of these ids
	Ids = new [] { 1, 2 },
	
	// must have any of these keys
	Keys = new [] { 
		Guid.Parse("dc890492-4571-4701-8085-b874837d597a"), 
		Guid.Parse("9f60f10f-74ea-4323-98bb-13b6f6423ad6"),
	}
	
	// must not have this id
	NotId = 3, // (eg. exclude current page)
	
	// must not have any of these ids
	NotIds = new [] { 4, 5 },
	
	// must not have this key
	NotKey = Guid.Parse("3e919e10-b702-4478-87ed-4a42ec52b337"),
	
	// must not have any of these keys
	NotKeys = new [] { 
		Guid.Parse("6bb24ed2-9466-422f-a9d4-27a805db2d47"), 
		Guid.Parse("88a9e4e3-d4cb-4641-aff3-8579f1d60399")
	}
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
	
	// applies to all: Is, StartsWith, Contains & EndsWith
	CaseSensitive = true 
};
```

#### DateQuery

A date query is used together with a custom date indexer and enables date range queries (all properties are optional).
If a date query is set (ie, not null), then results must have a date value.

```csharp
lookQuery.DateQuery = new DateQuery() {
	After = new DateTime(2005, 02, 16),
	Before = null,
	
	// options:
	// Inclusive (default)
	// Exclusive
	// BeforeInclusiveAfterExclusive
	// BeforeExclusiveAfterInclusive
	Boundary = DateBoundary.Inclusive
}
```

#### TextQuery

A text query is used together with a custom text indexer and allows for wildcard searching using the analyzer specified by Exmaine.
Highlighting gives the ability to return an html snippet of text indiciating the part of the full text that the match was made on. All properties
are optional).

```csharp
lookQuery.TextQuery = new TextQuery() {

	// query text
	SearchText = "some text to search for",
	
	// flag to indicate whether highlight text should be extracted
	GetHighlight = true
}
```

#### TagQuery

A tag query is used together with a custom tag indexer (all properties are optional).
If a tag query is set then only results with tags are returned.

```csharp
lookQuery.TagQuery = new TagQuery() {    

	// must have this tag
	Has = new LookTag("color:red"),

	// must not have this tag
	Not = new LookTag("colour:white"),

	// must have all these tags
	HasAll = TagQuery.MakeTags("colour:red", "colour:blue"),

	// must have all tags from at least one of these collections
	HasAllOr = new LookTag[][] {
		TagQuery.MakeTags("colour:red", "size:large"),
		TagQuery.MakeTags("colour:red", "size:small")
	}

	// must have at least one of these tags
	HasAny = TagQuery.MakeTags("color:green", "colour:yellow"),

	// must have at least one tag from each collection
	HasAnyAnd = new LookTag[][] { 
		TagQuery.MakeTags("colour:red", "size:large"), 
		TagQuery.MakeTags("colour:red", "size:medium")
	},

	// must not have any of these tags
	NotAny = TagQuery.MakeTags("colour:black"),

	// return facet data for the tag groups
	FacetOn = new TagFacetQuery("colour", "size", "shape")
};
```

##### LookTags

A tag can be any string and exists within an optionally specified group (if a group isn't set, then the tag is put into a default un-named group - String.Empty).
A group string must only contain aphanumberic/underscore chars, and be less than 50 chars (as it is also used to generate a custom Lucene field name).

A LookTag can be constructed from specified group and tag values:

```csharp
LookTag(string group, string name)
```

or from a raw string value:

```csharp
LookTag(string value)
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


#### LocationQuery

A location query is used together with a custom location indexer. 
All properties are optional, but if a LocationQuery is set, then only results with a location will be returned.
A Boundary can be set using two points to define a pane on the latitude/longitude axis.
If a Location is set, then a distance value returned. However if a MaxDistance is also set, then only results
within that range are returned.

```csharp
lookQuery.LocationQuery = new LocationQuery() {
	Boundary = new Boundary(
		new Location(55, 10),
		new Location(56, 11)
	),
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

#### MaxResults

Defaults to 5000 if not set, but can be specified on a per query basis.

#### SortOn

If not specified then the reults will be sorted on the Lucene score, otherwise sorting can be set by the SortOn enum to use the custom name, date or distance fields.

### Search Results

The search can be performed by calling the Search method on the LookQuery object:

```csharp
var lookResult = lookQuery.Search();
```

When the search is performed, the source LookQuery model is compiled (such that it can be useful to hold onto a reference for any subsequent paging queries). 

The LookResult model returned implements Examine.ISearchResults, but extends it with a Matches property that will return the results enumerated as strongly typed LookMatch objects (useful for lazy access to the assocated IPublishedContent item and other data) and a Facets property for any facet results.

```csharp
public class LookResult : Examine.ISearchResults
{
	/// <summary>
	/// When true, indicates the query was parsed and executed correctly
	/// </summary>
	public bool Success { get; }
	
	/// <summary>
	/// The total number of results that could be returned from Lucene
	/// </summary>
	public int TotalItemCount { get; }

	/// <summary>
	/// Get the results enumerable as LookMatch objects
	/// </summary>
	public IEnumerable<LookMatch> Matches { get; }

	/// <summary>
	/// Efficient skipping of matches
	/// </summary>
	public IEnumerable<LookMatch> SkipMatches(int skip) { }

	/// <summary>
	/// Any returned facets
	/// </summary>
	public Facet[] Facets { get; }
}
```

```csharp
public class LookMatch : Examine.SearchResult
{
	/// <summary>
	/// Lazy evaluation of item for the content, media, member or detached item (always has a value)
	/// </summary>
	public IPublishedContent Item { get; }

	/// <summary>
	/// Lazy evaluation of the host item (if the item is detached) otherwize this will be null
	/// </summary>
	public IPublishedContent HostItem { get; }

	/// <summary>
	/// Culture in Umbraco this item is associated with
	/// </summary>
	public CultureInfo CultureInfo { get; set; }

	/// <summary>
	/// Flag to indicate whether this result is a detached item
	/// </summary>
	public bool IsDetached { get; }

	/// <summary>
	/// Guid key of the Content, media, member or detached item
	/// </summary>
	public Guid Key { get; }

	/// <summary>
	/// The docType, mediaType or memberType alias
	/// </summary>
	public string Alias { get; set; }

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
	/// Highlight text (containing search text) extracted from from the full text
	/// </summary>
	public IHtmlString Highlight { get; }

	/// <summary>
	/// All tags associated with this item
	/// </summary>
	public LookTag[] Tags { get; }

	/// <summary>
	/// The custom location (lat|lng) field
	/// </summary>
	public Location Location { get; }

	/// <summary>
	/// Result field for calculated distance
	/// (only used when a location query is set)
	/// </summary>
	public double? Distance { get; }

	/// <summary>
	/// The contextual type: content, media or member (a detached item belongs to its host one of these)
	/// </summary>
	public PublishedItemType PublishedItemType { get; }
}
```
