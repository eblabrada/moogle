namespace MoogleEngine;

public static class Moogle
{
  public static SearchResult Query(string query, bool alsoSuggestions = true)
  {
    Console.WriteLine("Processing new query...\n");

    var watch = System.Diagnostics.Stopwatch.StartNew();

    var res = SearchEngine.FindItems(query);
    
    if (alsoSuggestions == true)
    {
      Console.WriteLine("\nQuerying also for suggestions...\n");

      Dictionary<string, bool> areUsed = new Dictionary<string, bool>();
      foreach (var x in res.Item1)
      {
        areUsed[x.Title] = true;
      }

      var res2 = SearchEngine.FindItems(res.Item2, 2.0);

      foreach (var x in res2.Item1)
      {
        if (areUsed.ContainsKey(x.Title))
        {
          continue;
        }
        res.Item1.Add(x);
      }
    }

    Console.WriteLine($"\nTime elapsed (s): {watch.ElapsedMilliseconds / 1000}\n");

    SearchItem[] items = res.Item1.ToArray();

    return new SearchResult(items, res.Item2);
  }
}
