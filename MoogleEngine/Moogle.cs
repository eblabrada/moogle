namespace MoogleEngine;

public static class Utils
{
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
        this.documents.Add(File.ReadAllText(dir));
        this.documentTitle[currentIndex] = Path.GetFileNameWithoutExtension(dir);
        currentIndex++;
      }

      this.numberOfDocuments = documents.Count();

      for (int i = 0; i < numberOfDocuments; i++)
      {
        TF.Add(new Dictionary<string, double>());
        ProcessDocuments(documents[i], i);
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
  private void ProcessDocuments(string doc, int index)
  {
    char[] splitters = { ' ', ',', '.', ':', ';', '\t', '\n' };
    string[] words = doc.Split(splitters, StringSplitOptions.RemoveEmptyEntries);
    for (int i = 0; i < words.Length; i++)
    {
      string word = Utils.Tokenizer(words[i]);
      if (word.Length == 0)
      {
        continue;
      }

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
      TF[index][term] /= (double)words.Length;
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

public class Moogle
{
  public static TFIDFAnalyzer AllDocuments = new TFIDFAnalyzer("../Content");

  public static List<(double, int)> findItems(string query)
  {
    Dictionary<string, double> QTF = new Dictionary<string, double>();

    List<string> forbidden = new List<string>();
    List<string> needed = new List<string>();

    string[] words = query.Split();
    for (int i = 0; i < words.Length; i++)
    {
      int importance = 1;
      if (words[i][0] == '!')
      {
        importance = 0;
        forbidden.Add(Utils.Tokenizer(words[i].Substring(1)));
      }

      if (words[i][0] == '^')
      {
        needed.Add(Utils.Tokenizer(words[i].Substring(1)));
      }

      int j = 0;
      while (j < words[i].Length && words[i][j] == '*')
      {
        j++; importance++;
      }

      string word = Utils.Tokenizer(words[i]);
      if (word.Length == 0)
      {
        continue;
      }

      if (!QTF.ContainsKey(word)) QTF.Add(word, 0.0);
      QTF[word] = importance;
    }

    foreach (string term in QTF.Keys)
    {
      QTF[term] /= (double)words.Length;
    }

    List<(double, int)> items = new List<(double, int)>();
    for (int i = 0; i < AllDocuments.numberOfDocuments; i++)
    {
      double similarity = AllDocuments.ComputeRelevance(ref QTF, i);

      bool ok = true;

      // ignore forbidden words
      foreach (var term in forbidden)
      {
        if (AllDocuments.TF[i].ContainsKey(term))
        {
          ok = false;
        }
      }

      // consider needed words
      foreach (var term in needed)
      {
        if (AllDocuments.TF[i].ContainsKey(term) == false)
        {
          ok = false;
        }
      }

      if (similarity == 0 || !ok) continue;

      Console.WriteLine($"{i} {similarity}");
      items.Add((similarity, i));
    }

    items.Sort(); items.Reverse();

    return items;
  }

  public static SearchResult Query(string query)
  {
    var res = findItems(query);

    SearchItem[] items = new SearchItem[res.Count()];
    for (int i = 0; i < res.Count(); i++)
    {
      items[i] = new SearchItem(AllDocuments.documentTitle[res[i].Item2], "Falta por implementar", (float)res[i].Item1);
    }

    return new SearchResult(items, query);
  }
}
