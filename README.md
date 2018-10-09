# Umbraco Look
Umbraco Lucene indexer and searcher with support for text match highlighting and geospacial queries.

Distributed as a single dll _Our.Umbraco.Look.dll_

## Indexing

For each Umbraco node, Look will index the following data:

A text field - used to store all text associated with a node, and as the source for a text highlight  
A tag field - used to store a collection of string tags associated with a node  
A date field - used to associate a date with a node (defaults to the node.UpdatedDate)  
A name field - used to associate a name with a node (defaults to the node.Name)  
A location field - used to store latitude & longitude associated with a node  
  
To confiure indexing there are static methods on Our.Umbraco.Look.Services.LookIndexService which accept functions returning the typed value to be indexed.

Eg.

	using Our.Umbraco.Look.Services;

	// return a string (or null)
	LookIndexService.SetTextIndexer(publishedContent => 
	{
		if (publishedContent.DocumentTypeAlias == "myDocTypeAlias")
		{
			return "my text for myDocTypeAlias to be indexed";
		}

		// fall back to default indexing
		return LookIndexService.DefaultTextIndexer(publishedContent);
	});

	// return a string array (or null)
	LookIndexService.SetTagIndexer(publishedContent => 
	{
		if (publishedContent.DocumentTypeAlias == "myDocTypeAlias")
		{
			return new string[] { "tag1", "tag2" };
		}
		
		return LookIndexService.DefaultDateIndexer(publishedContent);
	});

	// return a DateTime obj (or null)
	LookIndexService.SetDateIndexer(publishedContent => 
	{
		if (publishedContent.DocumentTypeAlias == "myDocTypeAlias")
		{
			return new DateTime(2005, 02, 16);
		}

		return LookIndexService.DefaultDateIndexer(publishedContent);
	});

	// return an Our.Umbraco.Look.Models.Location obj (or null)
	LookIndexService.SetLocationIndexer(publishedContent => {

		if (publishedContent.DocumentTypeAlias == "myDocTypeAlias")
		{
			return new Lcoation(55.406330, 10.388500);		
		}

		return null;
	});

## Searching

A Look search consists of any combinations of the following (optional) query types: TextQuery, TagQuery, NodeQuery & LocationQuery (most values are also optional)

Eg.

	using Our.Umbraco.Look.Models;  
	using Our.Umbraco.Look.Services;  

	var lookQuery = new LookQuery()
	{
		TextQuery = new TextQuery() {
			SearchText = "blah",
			HighlightFragments = 1 // specify that highlight text containing the search term once should be returned,
			HighlightSeparator = " ... "
		},

		TagQuery = new TagQuery() {
			AllTags = new string[] { "tag1", "tag2" } // both tag1 and tag2 are required
		},

		NodeQuery = new NodeQuery() {
			TypeAliases = new string[] { "myDocTypeAlias" },
			ExcludeIds = new int[] { 123 } // useful for excluding the curren page
		},

		LocationQuery = new LocationQuery() {
			Location = new Location(55.406330, 10.388500), // passing a location means the distance results can be set
			MaxDistance = new Distance(500, DistanceUnit.Miles)  // limits the results to within this distance
		},

		SortOn = SortOn.Distance
	};

	// perform the search
	var lookResults = LookSearchService.Query(lookQuery);

	var totalResults = lookResults.Total; // total number of item expected in the lookResults enumerable
	var results = lookResults.ToArray(); // the lookResults enumerated into an array
