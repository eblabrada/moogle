namespace MoogleEngine;

public static class SearchEngine
{
  public static TFIDFAnalyzer allDocuments = new TFIDFAnalyzer("../Content");

  private static SearchItem CalculateSnippet(SearchItem item, string query, int len = 100)
  {
    char[] splitters = { ' ', ',', '.', ':', ';', '\t', '\n' };
    string[] text = item.Snippet.Split(splitters, StringSplitOptions.RemoveEmptyEntries);
    string[] queryText = query.Split(splitters, StringSplitOptions.RemoveEmptyEntries);

    Queue<(int, double)> window = new Queue<(int, double)>();
    int r = -1, snippetPos = 0, maxCnt = -1, curCnt = 0;
    double minDist = -1, curDist = 0;
    for (int l = 0; l < text.Length; l++)
    {
      while (r + 1 < text.Length && r - l < len)
      {
        r++;

        int f = 0; double g = 0;
        for (int i = 0; i < queryText.Length; i++)
        {
          if (Utils.AreSimilar(queryText[i], text[r]))
          {
            f += 1;
            g += Utils.Distance(queryText[i], text[r]);
          }
        }

        curCnt += f;
        curDist += g;
        window.Enqueue((f, g));
      }

      if (maxCnt < curCnt)
      {
        maxCnt = curCnt;
        snippetPos = l;
      }
      else if (maxCnt == curCnt && curCnt != 0)
      {
        if (minDist == -1 || minDist > curDist / curCnt)
        {
          minDist = curDist / curCnt;
          snippetPos = l;
        }
      }

      if (window.Count() > 0)
      {
        curCnt -= window.Peek().Item1;
        curDist -= window.Peek().Item2;
        window.Dequeue();
      }
    }

    string snippet = "";

    for (int i = snippetPos; i < Math.Min(text.Length, snippetPos + len); i++)
    {
      snippet = snippet + text[i] + " ";
    }

    return new SearchItem(item.Title, snippet, item.Score);
  }

  public static (List<SearchItem>, string) FindItems(string query)
  {
    string suggest = allDocuments.Suggestion(query);

    Dictionary<string, double> QTF = new Dictionary<string, double>();

    char[] splitters = { ' ', ',', '.', ':', ';', '\t', '\n' };
    string[] words = query.Split(splitters, StringSplitOptions.RemoveEmptyEntries);
    string[] need = Utils.GetNeed(words);
    string[] forb = Utils.GetForbidden(words);
    (string, int)[] more = Utils.GetMore(words);
    (string, string)[] near = Utils.GetNear(words);

    for (int i = 0; i < words.Length; i++)
    {
      string word = Utils.Tokenizer(words[i]);
      if (word.Length == 0)
      {
        continue;
      }

      if (!QTF.ContainsKey(word)) QTF.Add(word, 0.0);
      QTF[word] = 1;
    }

    foreach (string term in QTF.Keys)
    {
      QTF[term] /= (double)words.Length;
    }

    bool flag = false;

    List<(double, int)> items = new List<(double, int)>();
    for (int i = 0; i < allDocuments.numberOfDocuments; i++)
    {
      double similarity = allDocuments.ComputeRelevance(ref QTF, i, need, forb, more, near);
      if (similarity == 0) continue;

      if (flag == false)
      {
        Console.WriteLine("The results are:");
        flag = true;
      }

      Console.WriteLine($"  * {allDocuments.documentTitle[i]} with similarity {similarity}");
      items.Add((similarity, i));
    }

    if (flag == false)
    {
      Console.WriteLine("No results for this query :( Try with the suggestions");
    }

    items.Sort(); items.Reverse();
    List<SearchItem> res = new List<SearchItem>();
    for (int i = 0; i < items.Count(); i++)
    {
      string title = allDocuments.documentTitle[items[i].Item2];
      string snippet = allDocuments.documents[items[i].Item2];

      res.Add(CalculateSnippet(new SearchItem(title, snippet, (float)items[i].Item1), query));
    }
    return (res, suggest);
  }
}
