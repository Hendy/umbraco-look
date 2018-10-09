# Umbraco Look
Umbraco Lucene indexer and searcher with support for text match highlighting and geospacial queries.

Distributed as a single dll _Our.Umbraco.Look_

## Indexing

For each Umbraco node, Look will index the following data:

A text field - used to store all text associated with a node, and as the source for a text highlight
A tag field - used to store a collection of string tags associated with a node
A date field - used to associate a date with a node (defaults to the node.UpdatedDate)
A name field - used to associate a name with a node (defaults to the node.Name)
A location field - used to store latitude & longitude associated with a node

To confiure indexing there are static methods on Our.Umbraco.Look.Services.LookIndexService which accept functions returning the type to be indexed.

Eg.

	using Our.Umbraco.Look.Services;

	// return a string (or null)
	LookIndexService.SetTextIndexer(publishedContent => 
	{
		if (publishedContent.DocumentTypeAlias == "myDocTypeAlias")
		{
			return "my text to index";
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
		
		return null;
	});

	// return a DateTime obj (or null)
	LookIndexService.SetDateIndexer(publishedContent => 
	{
		return DateTime.MinValue;
	});

	// return a Our.Umbraco.Look.Models.Location obj (or null)
	LookIndexService.SetLocationIndexer(publishedContent => {

		return new Lcoation(55.406330,10.388500);		
	
	});

## Searching

