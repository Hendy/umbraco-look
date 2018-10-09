# Umbraco Look
Umbraco Examine Lucene indexer and searcher with support for text match highlighting and geospacial queries.

[The NuGet Package](https://www.nuget.org/packages/Our.Umbraco.Look) installs a single assembly _Our.Umbraco.Look.dll_.

## Indexing

For each Umbraco node, Look will index the following data:

A text field - used to store all text associated with a node, and as the source for a text highlight  
A tag field - used to store a collection of string tags associated with a node  
A date field - used to associate a date with a node (defaults to the node.UpdatedDate)  
A name field - used to associate a name with a node (defaults to the node.Name)  
A location field - used to store latitude & longitude associated with a node  
  
To configure indexing there are static methods on the Our.Umbraco.Look.Services.LookIndexService class which accept functions returning the typed value to be indexed. (No configuration files need to be changed)

Eg.

	using Our.Umbraco.Look.Services;

	// return a string (or null)
	LookIndexService.SetNameIndexer(publishedContent => {

		if (publishedContent.DocumentTypeAlias == "myDocTypeAlias")
		{
			return "my custom name for myDocTypeAlias to be indexed";
		}

		// fallback to default indexing (or can return null)
		return LookIndexService.DefaultNameIndexer(publishedContent);
	});

	// return a string (or null)
	LookIndexService.SetTextIndexer(publishedContent => {

		if (publishedContent.DocumentTypeAlias == "myDocTypeAlias")
		{
			return "my text for myDocTypeAlias to be indexed";
		}

		// fallback to default indexing (or can return null)
		return LookIndexService.DefaultTextIndexer(publishedContent);
	});

	// return a string array (or null)
	LookIndexService.SetTagIndexer(publishedContent => {

		if (publishedContent.DocumentTypeAlias == "myDocTypeAlias")
		{
			return new string[] { "tag1", "tag2" };
		}
		
		// fallback to default indexing (or can return null)
		return LookIndexService.DefaultTagIndexer(publishedContent);
	});

	// return a DateTime obj (or null)
	LookIndexService.SetDateIndexer(publishedContent => {

		if (publishedContent.DocumentTypeAlias == "myDocTypeAlias")
		{
			return new DateTime(2005, 02, 16);
		}

		// fallback to default indexing (or can return null)
		return LookIndexService.DefaultDateIndexer(publishedContent);
	});

	// return an Our.Umbraco.Look.Models.Location obj (or null)
	LookIndexService.SetLocationIndexer(publishedContent => {

		if (publishedContent.DocumentTypeAlias == "myDocTypeAlias")
		{
			return new Lcoation(55.406330, 10.388500);		
		}

		// currenty there is no default fallback
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
			SearchText = "some text to search for",
			HighlightFragments = 2 // highlight text containing the search term twice should be returned
			HighlightSeparator = " ... ",
			GetText = true // indicate that the raw text field should also be returned
		},

		TagQuery = new TagQuery() {
			AllTags = new string[] { "tag1", "tag2" }, // both tag1 and tag2 are required
			AnyTags = new string[] { "tag3", "tag4" }, // the results must have at lease one of these tags
			GetTags = true // indicate that the tags for each result should also be returned
		},

		NodeQuery = new NodeQuery() {
			TypeAliases = new string[] { "myDocTypeAlias" },
			ExcludeIds = new int[] { 123 } // useful for excluding the current page
		},

		LocationQuery = new LocationQuery() {
			Location = new Location(55.406330, 10.388500), // a location means distance results can be set
			MaxDistance = new Distance(500, DistanceUnit.Miles)  // limits the results to within this distance
		},

		SortOn = SortOn.Distance // other sorts include: Score, Name, Date
	};

	// perform the search
	var lookResults = LookSearchService.Query(lookQuery);

	var totalResults = lookResults.Total; // total number of item expected in the lookResults enumerable
	var results = lookResults.ToArray(); // the lookResults enumerated into an array

### Search Results

A enumeration of the following LookMatch objects are returned:

	public class LookMatch
	{
		/// <summary>
		/// The Umbraco node Id of the matched item
		/// </summary>
		public int Id { get; internal set; }

		/// <summary>
		/// Highlight text (containing search text) extracted from from the full text
		/// </summary>
		public IHtmlString Highlight { get; internal set; }

		/// <summary>
		/// The full text (only returned if specified)
		/// </summary>
		public string Text { get; internal set; }

		/// <summary>
		/// Tag collection (only returned if specified)
		/// </summary>
		public string[] Tags { get; internal set; }

		/// <summary>
		/// The custom date field
		/// </summary>
		public DateTime? Date { get; internal set; }

		/// <summary>
		/// The custom name field
		/// </summary>
		public string Name { get; internal set; }

		/// <summary>
		/// The custom location (lat|lng) field
		/// </summary>
		public Location Location { get; internal set; }

		/// <summary>
		/// Temp field for calculated results
		/// </summary>
		public double? Distance { get; internal set; }

		/// <summary>
		/// The Lucene score for this match
		/// </summary>
		public float Score { get; internal set; }
	}
