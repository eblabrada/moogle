namespace MoogleEngine;

using System.Text;
using System.Text.Json;

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
  public List<Trie> documentTrie = new List<Trie>();

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

      Console.WriteLine("Adding information to the Trie...\n");

      for (int i = 0; i < numberOfDocuments; i++)
      {
        documentTrie.Add(new Trie());
      }

      for (int i = 0; i < fdocuments.Count(); i++)
      {
        foreach (var word in fdocuments[i])
        {
          documentTrie[i].Insert(word);
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
    if (Directory.GetFiles(database, "*.json").Length != 9)
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

      Console.WriteLine("\n");

      return true;
    }
    return false;
  }

  public void SaveInfo(string database = "../Database")
  {
    File.WriteAllText(database + "/TF.json", JsonSerializer.Serialize(TF));
    File.WriteAllText(database + "/IDF.json", JsonSerializer.Serialize(IDF));
    File.WriteAllText(database + "/relevance.json", JsonSerializer.Serialize(relevance));
    File.WriteAllText(database + "/frequency.json", JsonSerializer.Serialize(frequency));
    File.WriteAllText(database + "/vocabulary.json", JsonSerializer.Serialize(vocabulary));
    File.WriteAllText(database + "/documents.json", JsonSerializer.Serialize(documents));
    File.WriteAllText(database + "/fdocuments.json", JsonSerializer.Serialize(fdocuments));
    File.WriteAllText(database + "/documentTitle.json", JsonSerializer.Serialize(documentTitle));
    File.WriteAllText(database + "/documentTrie", JsonSerializer.Serialize(documentTrie));
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
    jsonString = File.ReadAllText(database + "/documentTrie.json");
    this.documentTrie = JsonSerializer.Deserialize<List<Trie>>(jsonString)!;
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
    File.Delete(database + "/documentTrie.json");
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

    Dictionary<string, bool> marked = new Dictionary<string, bool>();
    foreach (var x in forb)
    {
      marked[x] = true;
    }

    double trieRes = 0.0;
    foreach (var word in queryVec.Keys)
    {
      if (marked.ContainsKey(word)) continue;

      trieRes += documentTrie[index].PrefixRelevance(word);
    }

    res += Math.Min(0.5, trieRes);

    res *= OperatorIn(need, index);
    res *= OperatorNotIn(forb, index);
    res *= OperatorMore(more, index);
    res *= OperatorNear(near, index);


    return res;
  }
}