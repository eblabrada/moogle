namespace MoogleEngine;

using System;
using System.IO;
using System.Text;

public static class Utils
{
  public static int EditDistance(string a, string b)
  {
    int n = a.Length, m = b.Length;

    int[,] dp = new int[2, m + 1];
    for (int i = 0; i <= m; i++)
    {
      dp[0, i] = i;
    }

    for (int i = 1; i <= n; i++)
    {
      int c = i & 1, o = 1 - (i & 1);
      for (int j = 0; j <= m; j++)
      {
        if (j == 0)
        {
          dp[c, j] = i;
        }
        else
        {
          if (a[i - 1] == b[j - 1])
          {
            dp[c, j] = dp[o, j - 1];
          }
          else
          {
            dp[c, j] = Math.Min(dp[o, j], Math.Min(dp[o, j - 1], dp[c, j - 1])) + 1;
          }
        }
      }
    }

    return dp[n & 1, m];
  }

  public static bool AreSimilar(string a, string b)
  {
    a = a.ToLower(); b = b.ToLower();
    if (b.Length > 1 && EditDistance(a, b) <= 1)
    {
      return true;
    }
    return false;
  }

  public static string Tokenizer(string word)
  {
    string res = "";
    for (int i = 0; i < word.Length; i++)
    {
      if (Char.IsLetterOrDigit(word[i]))
      {
        res += word[i];
      }
    }
    return res.ToLower();
  }

  public static List<string> NormalizeText(string text)
  {
    char[] splitters = { ' ', ',', '.', ':', ';', '\t', '\n' };
    string[] words = text.Split(splitters, StringSplitOptions.RemoveEmptyEntries);
    List<string> res = new List<string>();
    for (int i = 0; i < words.Length; i++)
    {
      string s = Tokenizer(words[i]);
      if (s.Length > 0)
      {
        res.Add(s);
      }
    }
    return res;
  }

  public static double Norm(Dictionary<string, double> vec)
  {
    double res = 0.0;
    foreach (var item in vec)
    {
      double score = item.Value;
      res += score * score;
    }
    return Math.Sqrt(res);
  }
}

public class TFIDFAnalyzer
{
  public List<Dictionary<string, double>> TF = new List<Dictionary<string, double>>();
  public Dictionary<string, double> IDF = new Dictionary<string, double>();
  public List<Dictionary<string, double>> relevance = new List<Dictionary<string, double>>();
  public Dictionary<string, List<int>> vocabulary = new Dictionary<string, List<int>>();
  public string path = "";
  public List<string> documents = new List<string>();
  public List<List<string>> fdocuments = new List<List<string>>();
  public Dictionary<int, string> documentTitle = new Dictionary<int, string>();
  public int numberOfDocuments = 0;

  public TFIDFAnalyzer(string path)
  {
    this.path = path;

    try
    {
      string[] directories = Directory.GetFiles(path, "*.txt");
      int currentIndex = 0;
      foreach (var dir in directories)
      {
        string text = File.ReadAllText(dir, Encoding.UTF8);
        this.documents.Add(text);
        this.fdocuments.Add(Utils.NormalizeText(text));
        this.documentTitle[currentIndex] = Path.GetFileNameWithoutExtension(dir);
        currentIndex++;
      }

      this.numberOfDocuments = documents.Count();

      for (int i = 0; i < numberOfDocuments; i++)
      {
        TF.Add(new Dictionary<string, double>());
        ProcessDocuments(fdocuments[i], i);
      }

      foreach (string term in vocabulary.Keys)
      {
        IDF[term] = Math.Log((double)numberOfDocuments / vocabulary[term].Count());
      }

      for (int i = 0; i < numberOfDocuments; i++)
      {
        relevance.Add(new Dictionary<string, double>());
        foreach (string term in TF[i].Keys)
        {
          relevance[i][term] = TF[i][term] * IDF[term];
        }
      }
    }
    catch (Exception e)
    {
      Console.WriteLine($"The process failed: {e.ToString()}");
    }
  }
  private void ProcessDocuments(List<string> doc, int index)
  {
    for (int i = 0; i < doc.Count(); i++)
    {
      string word = doc[i];

      if (!TF[index].ContainsKey(word)) TF[index].Add(word, 0.0);
      if (!vocabulary.ContainsKey(word)) vocabulary.Add(word, new List<int>());

      TF[index][word] += 1.0;

      if (!vocabulary[word].Contains(index))
      {
        vocabulary[word].Add(index);
      }
    }

    foreach (string term in TF[index].Keys)
    {
      TF[index][term] /= (double)doc.Count();
    }
  }

  public double ComputeRelevance(ref Dictionary<string, double> queryVec, int index)
  {
    double num = 0.0, den = 0.0;
    foreach (var word in queryVec.Keys)
    {
      double docWordScore = 0.0;
      if (relevance[index].ContainsKey(word))
      {
        docWordScore = relevance[index][word];
      }
      double queryWordScore = queryVec[word];
      num += docWordScore * queryWordScore;
    }

    den = Utils.Norm(relevance[index]) * Utils.Norm(queryVec);

    if (den != 0.0)
    {
      return num / den;
    }
    else
    {
      return 0.0;
    }
  }
}

public static class SearchEngine
{
  public static TFIDFAnalyzer allDocuments = new TFIDFAnalyzer("../Content");

  private static SearchItem CalculateSnippet(SearchItem item, string query, int len = 200)
  {
    char[] splitters = { ' ', ',', '.', ':', ';', '\t', '\n' };
    string[] text = item.Snippet.Split(splitters, StringSplitOptions.RemoveEmptyEntries);

    string str = ""; int p = 0;
    while (p < Math.Min(len / 4, text.Length))
    {
      str += text[p] + " ";
      p++;
    }

    string snippet = str + " (...) ";

    string[] queryText = query.Split(splitters, StringSplitOptions.RemoveEmptyEntries);

    int cnt = 0;
    Queue<string> todo = new Queue<string>();
    while (p < text.Length && todo.Count() < len / 2)
    {
      todo.Enqueue(text[p]);
      for (int i = 0; i < queryText.Length; i++)
      {
        if (Utils.AreSimilar(queryText[i], text[p]))
        {
          cnt++;
        }
      }
      p++;
    }

    int position = p, maxCnt = cnt;
    for (int i = p; i < text.Length; i++)
    {
      todo.Enqueue(text[p]);

      bool ok = false;
      for (int j = 0; j < queryText.Length; j++)
      {
        if (Utils.AreSimilar(queryText[j], text[i]))
        {
          cnt++;
        }

        if (Utils.AreSimilar(queryText[j], todo.Peek()))
        {
          ok = true;
        }
      }

      todo.Dequeue();

      if (ok) cnt--;

      if (cnt > maxCnt)
      {
        position = i;
        maxCnt = cnt;
      }
    }

    for (int i = Math.Max(0, position - len / 2); i <= Math.Min(position, text.Length - 1); i++)
    {
      snippet = snippet + text[i] + " ";
    }

    return new SearchItem(item.Title, snippet, item.Score);
  }

  public static List<SearchItem> FindItems(string query)
  {
    Dictionary<string, double> QTF = new Dictionary<string, double>();

    string[] words = query.Split();
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

    List<(double, int)> items = new List<(double, int)>();
    for (int i = 0; i < allDocuments.numberOfDocuments; i++)
    {
      double similarity = allDocuments.ComputeRelevance(ref QTF, i);

      if (similarity == 0) continue;

      Console.WriteLine($"{i} {similarity}");
      items.Add((similarity, i));
    }

    items.Sort(); items.Reverse();

    List<SearchItem> res = new List<SearchItem>();
    for (int i = 0; i < items.Count(); i++)
    {
      string title = allDocuments.documentTitle[items[i].Item2];
      string snippet = allDocuments.documents[items[i].Item2];
      res.Add(CalculateSnippet(new SearchItem(title, snippet, (float)items[i].Item1), query));
    }

    return res;
  }
}

public static class Moogle
{
  public static SearchResult Query(string query)
  {
    List<SearchItem> res = new List<SearchItem>(SearchEngine.FindItems(query));

    SearchItem[] items = res.ToArray();
    return new SearchResult(items, query);
  }
}
