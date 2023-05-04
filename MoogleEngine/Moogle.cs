namespace MoogleEngine;

using System;
using System.IO;
using System.Text;
using System.Numerics;
using System.Text.Json;

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
  public static int LongestCommonPrefix(string a, string b)
  {
    int cnt = 0;
    for (int i = 0; i < Math.Min(a.Length, b.Length); i++)
    {
      if (a[i] == b[i])
      {
        cnt++;
      }
      else
      {
        break;
      }
    }
    return cnt;
  }
  public static double Distance(string a, string b)
  {
    int lcp = LongestCommonPrefix(a, b);
    if (lcp == 0) return EditDistance(a, b);
    return (double)EditDistance(a, b) / (double)LongestCommonPrefix(a, b);
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

  public static string Capitalize(string word)
  {
    if (word.Length == 0) return word;

    string res = word[0].ToString().ToUpper() + word.Substring(1);

    return res;
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
  public static string[] GetNeed(string[] words)
  {
    List<string> res = new List<string>();
    for (int i = 0; i < words.Length; i++)
    {
      if (words[i].Length > 0 && words[i][0] == '^')
      {
        res.Add(Tokenizer(words[i]));
      }
    }
    return res.ToArray();
  }
  public static string[] GetForbidden(string[] words)
  {
    List<string> res = new List<string>();
    for (int i = 0; i < words.Length; i++)
    {
      if (words[i].Length > 0 && words[i][0] == '!')
      {
        res.Add(Tokenizer(words[i]));
      }
    }
    return res.ToArray();
  }

  public static (string, int)[] GetMore(string[] words)
  {
    List<(string, int)> res = new List<(string, int)>();
    for (int i = 0; i < words.Length; i++)
    {
      int cnt = 0;
      while (cnt < words[i].Length && words[i][cnt] == '*')
      {
        cnt++;
      }
      if (cnt != 0)
      {
        res.Add((Tokenizer(words[i]), cnt));
      }
    }
    return res.ToArray();
  }

  public static (string, string)[] GetNear(string[] words)
  {
    List<(string, string)> res = new List<(string, string)>();
    for (int i = 0; i < words.Length; i++)
    {
      if (i - 1 >= 0 && i + 1 < words.Length && words[i] == "~")
      {
        res.Add((Tokenizer(words[i - 1]), Tokenizer(words[i + 1])));
      }
    }
    return res.ToArray();
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
  public List<Dictionary<string, int>> frequency = new List<Dictionary<string, int>>();
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

    if (CanGet())
    {
      GetInfo();
      return;
    }

    DeleteInfo();

    try
    {
      Console.WriteLine("Charging all documents...\n");

      string[] directories = Directory.GetFiles(path, "*.txt");
      int currentIndex = 0;
      foreach (var dir in directories)
      {
        string name = Path.GetFileNameWithoutExtension(dir);
        string text = File.ReadAllText(dir, Encoding.UTF8);
        this.documents.Add(text);
        this.fdocuments.Add(Utils.NormalizeText(text));
        this.documentTitle[currentIndex] = Path.GetFileNameWithoutExtension(dir);
        currentIndex++;
      }

      this.numberOfDocuments = documents.Count();

      Console.WriteLine("Processing TF-IDF for documents...\n");

      for (int i = 0; i < numberOfDocuments; i++)
      {
        TF.Add(new Dictionary<string, double>());
        frequency.Add(new Dictionary<string, int>());
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

      SaveInfo();

      Console.WriteLine("All is working fine!");
    }
    catch (Exception e)
    {
      Console.WriteLine($"The process failed: {e.ToString()}");
    }
  }

  public bool CanGet(string database = "../Database")
  {
    if (Directory.GetFiles(database, "*.json").Length != 8)
    {
      return false;
    }

    string[] books = Directory.GetFiles(path, "*.txt");

    if (File.Exists(database + "/documentTitle.json"))
    {
      Dictionary<int, string> dic = new Dictionary<int, string>();
      string jsonString = File.ReadAllText(database + "/documentTitle.json");
      dic = JsonSerializer.Deserialize<Dictionary<int, string>>(jsonString)!;

      Dictionary<string, bool> exist = new Dictionary<string, bool>();
      foreach (var dir in books)
      {
        string name = Path.GetFileNameWithoutExtension(dir);
        exist[name] = true;
      }

      foreach (var dir in dic.Values)
      {
        if (!exist.ContainsKey(dir))
        {
          Console.WriteLine($"Not in books {dir}");
          return false;
        }
      }

      foreach (var dir in exist.Keys)
      {
        if (!dic.ContainsValue(dir))
        {
          Console.WriteLine($"Not in documentTitle {dir}");
          return false;
        }
      }

      return true;
    }

    return false;
  }

  public void SaveInfo(string database = "../Database")
  {
    File.WriteAllText(database + "/TF.json", JsonSerializer.Serialize(TF, new JsonSerializerOptions() { WriteIndented = true }));
    File.WriteAllText(database + "/IDF.json", JsonSerializer.Serialize(IDF, new JsonSerializerOptions() { WriteIndented = true }));
    File.WriteAllText(database + "/relevance.json", JsonSerializer.Serialize(relevance, new JsonSerializerOptions() { WriteIndented = true }));
    File.WriteAllText(database + "/frequency.json", JsonSerializer.Serialize(frequency, new JsonSerializerOptions() { WriteIndented = true }));
    File.WriteAllText(database + "/vocabulary.json", JsonSerializer.Serialize(vocabulary, new JsonSerializerOptions() { WriteIndented = true }));
    File.WriteAllText(database + "/documents.json", JsonSerializer.Serialize(documents, new JsonSerializerOptions() { WriteIndented = true }));
    File.WriteAllText(database + "/fdocuments.json", JsonSerializer.Serialize(fdocuments, new JsonSerializerOptions() { WriteIndented = true }));
    File.WriteAllText(database + "/documentTitle.json", JsonSerializer.Serialize(documentTitle, new JsonSerializerOptions() { WriteIndented = true }));
  }

  public void GetInfo(string database = "../Database")
  {
    string jsonString = File.ReadAllText(database + "/TF.json");
    this.TF = JsonSerializer.Deserialize<List<Dictionary<string, double>>>(jsonString)!;
    jsonString = File.ReadAllText(database + "/IDF.json");
    this.IDF = JsonSerializer.Deserialize<Dictionary<string, double>>(jsonString)!;
    jsonString = File.ReadAllText(database + "/relevance.json");
    this.relevance = JsonSerializer.Deserialize<List<Dictionary<string, double>>>(jsonString)!;
    jsonString = File.ReadAllText(database + "/frequency.json");
    this.frequency = JsonSerializer.Deserialize<List<Dictionary<string, int>>>(jsonString)!;
    jsonString = File.ReadAllText(database + "/vocabulary.json");
    this.vocabulary = JsonSerializer.Deserialize<Dictionary<string, List<int>>>(jsonString)!;
    jsonString = File.ReadAllText(database + "/documents.json");
    this.documents = JsonSerializer.Deserialize<List<string>>(jsonString)!;
    jsonString = File.ReadAllText(database + "/fdocuments.json");
    this.fdocuments = JsonSerializer.Deserialize<List<List<string>>>(jsonString)!;
    jsonString = File.ReadAllText(database + "/documentTitle.json");
    this.documentTitle = JsonSerializer.Deserialize<Dictionary<int, string>>(jsonString)!;
    this.numberOfDocuments = documents.Count();
  }

  public void DeleteInfo(string database = "../Database")
  {
    File.Delete(database + "/TF.json");
    File.Delete(database + "/IDF.json");
    File.Delete(database + "/relevance.json");
    File.Delete(database + "/frequency.json");
    File.Delete(database + "/vocabulary.json");
    File.Delete(database + "/documents.json");
    File.Delete(database + "/fdocuments.json");
    File.Delete(database + "/documentTitle.json");
  }

  private void ProcessDocuments(List<string> doc, int index)
  {
    for (int i = 0; i < doc.Count(); i++)
    {
      string word = doc[i];

      if (!TF[index].ContainsKey(word)) TF[index].Add(word, 0.0);
      if (!frequency[index].ContainsKey(word)) frequency[index].Add(word, 0);
      if (!vocabulary.ContainsKey(word)) vocabulary.Add(word, new List<int>());

      TF[index][word] += 1.0;
      frequency[index][word] += 1;

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

  public string Suggestion(string query)
  {
    char[] splitters = { ' ', '\t', '\n' };
    string[] words = query.Split(splitters, StringSplitOptions.RemoveEmptyEntries);

    string res = "";
    for (int i = 0; i < words.Length; i++)
    {
      if (words[i] == "~" || words[i] == "!" || words[i] == "*" || words[i] == "^")
      {
        if (i != 0) res += " ";
        res += words[i];
        continue;
      }

      string str = Utils.Tokenizer(words[i]);

      double minDist = 100000;
      foreach (var term in vocabulary.Keys)
      {
        if (Utils.Distance(Utils.Tokenizer(words[i]), term) < minDist)
        {
          minDist = Utils.Distance(Utils.Tokenizer(words[i]), term);
          str = term;
        }
      }

      if (i != 0) res += " ";

      int ind = 0;
      while (ind < words[i].Length && !Char.IsLetterOrDigit(words[i][ind]))
      {
        res += words[i][ind].ToString();
        ind++;
      }

      if (ind < words[i].Length && Char.IsUpper(words[i][ind]))
      {
        str = Utils.Capitalize(str);
      }

      res += str;

      ind = words[i].Length - 1;
      while (ind >= 0 && !Char.IsLetterOrDigit(words[i][ind]))
      {
        res += words[i][ind].ToString();
        ind--;
      }
    }

    return res;
  }

  private double OperatorIn(string[] words, int index)
  {
    for (int i = 0; i < words.Length; i++)
    {
      if (!vocabulary[words[i]].Contains(index))
      {
        return 0.0;
      }
    }

    return 1.0;
  }

  private double OperatorNotIn(string[] words, int index)
  {
    for (int i = 0; i < words.Length; i++)
    {
      if (vocabulary[words[i]].Contains(index))
      {
        return 0.0;
      }
    }

    return 1.0;
  }

  private double OperatorMore((string, int)[] words, int index)
  {
    double res = 1.0;
    for (int i = 0; i < words.Length; i++)
    {
      string word = words[i].Item1;
      int more = words[i].Item2;
      if (vocabulary[word].Contains(index))
      {
        res *= more * Math.Log(frequency[index][word]);
      }
    }

    return res;
  }

  private double OperatorNear((string, string)[] words, int index)
  {
    double res = 1.0;
    for (int i = 0; i < words.Length; i++)
    {
      string a = words[i].Item1, b = words[i].Item2;
      int lastA = -1, lastB = -1, minDist = 100000;
      for (int j = 0; j < fdocuments[index].Count(); j++)
      {
        string cur = fdocuments[index][j];
        if (cur == a) lastA = j;
        if (cur == b) lastB = j;
        if (lastA != -1 && lastB != -1)
        {
          minDist = Math.Min(minDist, Math.Abs(lastA - lastB));
        }
      }
      if (minDist == 0) minDist = 1;
      res *= 2.0 / (Math.Log(minDist) + 1);
    }

    return res;
  }

  public double ComputeRelevance(ref Dictionary<string, double> queryVec, int index,
    string[] need, string[] forb, (string, int)[] more, (string, string)[] near)
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

    if (den == 0.0)
    {
      return 0.0;
    }

    double res = num / den;
    res *= OperatorIn(need, index);
    res *= OperatorNotIn(forb, index);
    res *= OperatorMore(more, index);
    res *= OperatorNear(near, index);
    return res;
  }
}

public static class SearchEngine
{
  public static TFIDFAnalyzer allDocuments = new TFIDFAnalyzer("../Content");

  private static SearchItem CalculateSnippet(SearchItem item, string query, int len = 100)
  {
    char[] splitters = { ' ', ',', '.', ':', ';', '\t', '\n' };
    string[] text = item.Snippet.Split(splitters, StringSplitOptions.RemoveEmptyEntries);
    string[] queryText = query.Split(splitters, StringSplitOptions.RemoveEmptyEntries);

    Queue<int> window = new Queue<int>();
    int r = -1, snippetPos = 0, maxCnt = -1, curCnt = 0;
    for (int l = 0; l < text.Length; l++)
    {
      while (r + 1 < text.Length && r - l < len)
      {
        r++;

        int f = 0;
        for (int i = 0; i < queryText.Length; i++)
        {
          if (Utils.AreSimilar(queryText[i], text[r]))
          {
            f += 1;
          }
        }

        curCnt += f;
        window.Enqueue(f);
      }

      if (maxCnt < curCnt)
      {
        maxCnt = curCnt;
        snippetPos = l;
      }

      if (window.Count() > 0)
      {
        curCnt -= window.Peek();
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

    List<(double, int)> items = new List<(double, int)>();
    for (int i = 0; i < allDocuments.numberOfDocuments; i++)
    {
      double similarity = allDocuments.ComputeRelevance(ref QTF, i, need, forb, more, near);
      if (similarity == 0) continue;
      Console.WriteLine($"{allDocuments.documentTitle[i]} with similarity {similarity}");
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

    return (res, suggest);
  }
}

public static class Moogle
{
  public static SearchResult Query(string query)
  {
    Console.WriteLine("Processing new query...\n");

    var watch = System.Diagnostics.Stopwatch.StartNew();

    var res = SearchEngine.FindItems(query);

    Console.WriteLine($"Time elapsed (s): {watch.ElapsedMilliseconds / 1000}\n");

    SearchItem[] items = res.Item1.ToArray();

    return new SearchResult(items, res.Item2);
  }
}
