# Umbraco Look (Alpha)
Look sits on top of [Umbraco Examine](https://our.umbraco.com/documentation/reference/searching/examine/) adding support for: text match highlighting, geospatial querying and tag faceting.
Examine manages the indexes, whilst Look adds 'config-file-free' C# indexing and a lightweight querying API.

[The NuGet Package](https://www.nuget.org/packages/Our.Umbraco.Look) installs a single assembly _Our.Umbraco.Look.dll_ with dependencies on: 

  * Umbraco 7.2.3 (min)
  * Examine 0.1.70 (min)
  * Lucene.Net.Contrib 2.9.4.1 (min)

Namespaces used in examples:
```csharp
using System;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Models;
using Our.Umbraco.Look.Services;
using Our.Umbraco.Look.Models;
```

## Indexing

Look automatically hooks into all Umbraco Exmaine indexers offering the ability to create additional Lucene fields for `name`, `date`, `text`, `tags` and `location` data.
To configure the indexing behaviour, functions can be set via static methods on the LookService (all are optional).
If a function is set and returns a value then custom Lucene field(s) prefixed with "Look_" will be used.
(On a default Umbraco install, the indexers are: "ExternalIndexer", "InternalIndexer" and "InternalMemberIndexer", see [/config/ExamineSettings.config](https://our.umbraco.com/Documentation/Reference/Config/ExamineSettings/)).

The static method definitions on the LookService where indexing functions can be set:

```csharp
// creates case sensitive and case insensitive fields (not analyzed) - for use with NameQuery
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

The model supplied to the indexing functions:

```csharp
public class IndexingContext
{
	/// <summary>
	/// The IPublishedContent of the Content, Media or Member being indexed
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
		LookService.SetNameIndexer(indexingContext => { 
			
			// eg. always return the Name of the IPublishedContent
			return indexingContext.Item.Name; 
		});

		LookService.SetDateIndexer(indexingContext => { 
			
			// eg. news articles use a different date
			if (indexingContext.Item.DocumentTypeAlias == "newsArticle") 
			{
				return indexingItem.GetPropertyValue<DateTime>("releaseDate");
			}

			return indexingContext.Item.UpdateDate; 
		});

		LookService.SetTextIndexer(indexingContext => { 

			// eg. if content, render page and scrape markup
			if (indexingContext.Item.ItemType == PublishedItemType.Content) 
			{				
				// (could pass in httpContext to render page without http web request)
				// return string
			}

			return null; // don't index
		});
		
		LookService.SetTagIndexer(indexingContext => {
			// return LookTag[] or null (see tags section below)

			// eg a nuPicker
			var picker = indexingContext.Item.GetPropertyValue<Picker>("colours");

			return picker
				.PickedKeys
				.Select(x => new LookTag("colour", x))
				.ToArray();
		});
		
		LookService.SetLocationIndexer(indexingContext => {
			// return Location or null

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
together with an Examine Searcher. The LookQuery constructor is used to specify which Examine searcher to use:

```csharp
var lookQuery = new LookQuery(); // use the default searcher (usually "ExternalSearcher")
```

```csharp
var lookQuery = new LookQuery("InternalSearcher"); // use a named searcher
```

All query types are optional, but when set, they become a required clause. All queries will return a LookResult, which has a boolean Success flag property. The flag
is set to true when a query with at least one clause is executed sucessfully.

#### RawQuery

String property for any [Lucene raw query](http://www.lucenetutorial.com/lucene-query-syntax.html) (this can be a way to pass in an Examine built query).

```csharp
lookQuery.RawQuery = "+myField: myValue";
```

#### NodeQuery
A node query is used to set search criteria based on the IPublishedContent type, alias and any Ids that should be excluded (all properties are optional).

```csharp
lookQuery.NodeQuery = new NodeQuery() {
	Types = new PublishedItemType[] { 
		PublishedItemType.Content, 
		PublishedItemType.Media, 
		PublishedItemType.Member 
	},
	Aliases = new string[] { 
		"myDocTypeAlias", 
		"myMediaTypeAlias",
		"myMemberTypeAlias"
	},
	NotIds = new int[] { 123 } // (eg. exclude current page)
};
```

Constructor overloads:

```csharp
NodeQuery(PublishedItemType type)
NodeQuery(string alias)
NodeQuery(params string[] aliases)
NodeQuery(PublishedItemType type, string alias)
NodeQuery(PublishedItemType type, params string[] aliases)
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

Optional constructor params:

```csharp
NameQuery(
	string @is = null, 
	string startsWith = null, 
	string contains = null, 
	string endsWith = null, 
	bool caseSensitive = true)
```

#### DateQuery

A date query is used together with a custom date indexer and enables date range queries (both date properties are optional).

```csharp
lookQuery.DateQuery = new DateQuery() {
	After = new DateTime(2005, 02, 16),
	Before = null
}
```

#### TextQuery

A text query is used together with a custom text indexer and allows for wildcard searching using the analyzer specified by Exmaine.
Highlighting gives the ability to return an html snippet of text indiciating the part of the full text that the match was made on. All properties
are optional, and there is a constructor where all properties are optional (by default GetHighlight and GetText are false).

```csharp
lookQuery.TextQuery = new TextQuery() {
	SearchText = "some text to search for",
	GetHighlight = true, // return highlight extract from the text field containing the search text
	GetText = true // raw text field should be returned (potentially a large document)
}
```

Optional constructor params:

```csharp
TextQuery(string searchText = null, bool getHighlight = false, bool getText = false)
```

#### TagQuery

A tag query is used together with a custom tag indexer.

The All, Any and Not properties expect LookTag[] values (see LookTags section below). 

If there are any query contradictions (such as a tag exsing in both All and Not), then
an empty result is returned with the success flag as false.

The GetFacets string[] indcates which tag groups to return facet counts for.

```csharp
lookQuery.TagQuery = new TagQuery() {
	All = TagQuery.MakeTags("size:large"), // all of these tags
	Any = TagQuery.MakeTags("colour:red", "colour:green", "colour:blue") // at least one of these tags
	Not = TagQuery.MakeTags("colour:black"), // none of these tags, 'not' always takes priority
	GetFacets = new string[] { "colour", "size", "shape" } 
};
```

Optional constructor params:

```csharp
TagQuery(LookTag[] all = null, LookTag[] any = null, LookTag[] not = null, string[] getFacets = null)
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

Optional constructor params:

```csharp
LocationQuery(Location location = null, Distance maxDistance = null)
```

#### SortOn

If not specified then the reults will be sorted on the Lucene score, otherwise sorting can be set by the SortOn enum to use the custom name, date or distance fields.

### Search Results

The search is performed by calling the static Query method on the LookService:

```csharp
var lookResult = LookService.Query(lookQuery); // returns Our.Umbraco.Look.Model.LookResult
```

```csharp
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
A group has can be any string that contains only aphanumberic/underscore chars, as is less than 50 chars (as it is also used to generate a custom Lucene field name).

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
var tag3 = new LookTag("colour", "red"); // tag 'red', in group 'colour'
```

There is also a static helper on the TagQuery model which can be used as a shorthand to create a LookTag array. Eg.

```csharp
var tags = TagQuery.MakeTags("colour:red", "colour:green", "colour:blue", "size:large");
```