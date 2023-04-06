namespace MoogleEngine;

public static class Utils
{
  public static string tokenizer(string word)
  {
    while (word.Length > 0 && Char.IsPunctuation(word[0]))
    {
      word = word.Substring(1);
    }

    while (word.Length > 0 && Char.IsPunctuation(word[word.Length - 1]))
    {
      word = word.Substring(0, word.Length - 1);
    }

    return word.ToLower();
  }

  public static double norm(Dictionary<string, double> vec)
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
public class tfidfAnalyzer
{
  public List<Dictionary<string, double>> TF = new List<Dictionary<string, double>>();
  public Dictionary<string, double> IDF = new Dictionary<string, double>();
  public List<Dictionary<string, double>> Relevance = new List<Dictionary<string, double>>();
  public Dictionary<string, List<int>> Voc = new Dictionary<string, List<int>>();
  public string path = "";
  public List<string> docs = new List<string>();
  public int numDocs = 0;

  public tfidfAnalyzer(string path)
  {
    // this.path = path;

    string[] docs1 = {
      "las instrucciones para hacer cafe son muy claras, tfidfAnalyzer>hola, pero hola es cubano; y tfidfAnalyzer no... aunque en CUBA hay mucho tfidfAnalyzer",
      "hay que aprender algoritmos para PROSPERAR en la vida, la Competencia es ConTIGo..... ....UH-IOIn't.... world final is near",
      "no os preocupeis, la inteligencia artificial Es eL futuro....",
      "por que debemos hacer cafe?",
      "esto no tiene nada de sentido pero aqui vamos",
      "perro casa... CaSA, string algorithms, estaba pensando en un Trie porque es muy genial...",
      "despues de que esto funcione debo cambiarle el nombre de tfidfAnalyzer a la clase, no puedo entregar con ese nombre tan RaNDoM",
      "habia una vez, en un lugar muy random, alguien estaba EsCribiendo cosas randoms, mientras prefiere tirar contests en CodeForces, pero no puede porque tiene que terminar el proyecto que tiene que entregar dentro de 4 semanas, no es dificil....... pero hay que dedicarle un poco de tiempo para programar... algo algo perro casa... en fin que con esto al menos voy a aprender cosas como TFIDF que me van a servir en el futuro en cosas de Machine Learning y eso... espero que no sea en vano todas estas pruebas",
      "estas son palabras randoms #1: segment tree, tree, rerooting, root, dp, dynamic programming, trie, knuth, morris, pratt, kmp, manacher, palindromic tree... esto no puede estar mejor",
      "estas son palabras randoms #2: no lean esto por favor",
      "no se que mas escribir"
    };

    string[] docs2 = {
      "the mouses played with the cat",
      "the quick brown fox jumped over the lazy dog",
      "dog 1 and dog 2 ate the hot dog"
    };

    for (int i = 0; i < docs1.Length; i++)
    {
      this.docs.Add(docs1[i]);
    }

    this.numDocs = docs.Count();

    for (int i = 0; i < numDocs; i++)
    {
      TF.Add(new Dictionary<string, double>());
      processDocuments(docs[i], i);
    }

    foreach (string term in Voc.Keys)
    {
      IDF[term] = Math.Log((double)numDocs / Voc[term].Count());
    }

    for (int i = 0; i < numDocs; i++)
    {
      Relevance.Add(new Dictionary<string, double>());
      foreach (string term in TF[i].Keys)
      {
        Relevance[i][term] = TF[i][term] * IDF[term];
      }
    }
  }
  private void processDocuments(string doc, int index)
  {
    string[] words = doc.Split();
    for (int i = 0; i < words.Length; i++)
    {
      string word = Utils.tokenizer(words[i]);
      if (word.Length == 0)
      {
        continue;
      }

      if (!TF[index].ContainsKey(word)) TF[index].Add(word, 0.0);
      if (!Voc.ContainsKey(word)) Voc.Add(word, new List<int>());

      TF[index][word] += 1.0;

      if (!Voc[word].Contains(index))
        Voc[word].Add(index);
    }

    foreach (string term in TF[index].Keys)
    {
      TF[index][term] /= (double)words.Length;
    }
  }

  public double computeRelevance(ref Dictionary<string, double> queryVec, int index)
  {
    double num = 0.0, den = 0.0;
    foreach (var word in queryVec.Keys)
    {
      double docWordScore = 0.0;
      if (Relevance[index].ContainsKey(word))
      {
        docWordScore = Relevance[index][word];
      }
      double queryWordScore = queryVec[word];
      num += docWordScore * queryWordScore;
    }

    den = Utils.norm(Relevance[index]) * Utils.norm(queryVec);

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
  public static tfidfAnalyzer allDocs = new tfidfAnalyzer("tfidfAnalyzer");

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
        forbidden.Add(Utils.tokenizer(words[i].Substring(1)));
      }

      if (words[i][0] == '^')
      {
        needed.Add(Utils.tokenizer(words[i].Substring(1)));
      }

      int j = 0;
      while (j < words[i].Length && words[i][j] == '*')
      {
        j++; importance++;
      }

      string word = Utils.tokenizer(words[i]);
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
    for (int i = 0; i < allDocs.numDocs; i++)
    {
      double similarity = allDocs.computeRelevance(ref QTF, i);

      bool ok = true;

      // ignore forbidden words
      foreach (var term in forbidden)
      {
        if (allDocs.TF[i].ContainsKey(term))
        {
          ok = false;
        }
      }

      // consider needed words
      foreach (var term in needed)
      {
        if (allDocs.TF[i].ContainsKey(term) == false)
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
      items[i] = new SearchItem($"Document {res[i].Item2}", "Falta por implementar", res[i].Item2);
    }

    return new SearchResult(items, query);
  }
}
