namespace MoogleEngine;

public class Nescafe
{
  public List<Dictionary<string, double>> TF = new List<Dictionary<string, double>>();
  public Dictionary<string, double> IDF = new Dictionary<string, double>();
  public List<Dictionary<string, double>> Relevance = new List<Dictionary<string, double>>();
  public Dictionary<string, List<int>> Voc = new Dictionary<string, List<int>>();
  public string path = "";

  public Nescafe(string path)
  {
    // this.path = path;

    string[] docs1 = {
      "las instrucciones para hacer cafe son muy claras, nescafe>hola, pero hola es cubano; y nescafe no... aunque en CUBA hay mucho NEScAFE",
      "hay que aprender algoritmos para PROSPERAR en la vida, la Competencia es ConTIGo..... ....UH-IOIn't.... world final is near",
      "no os preocupeis, la inteligencia artificial Es eL futuro....",
      "por que debemos hacer cafe?",
      "esto no tiene nada de sentido pero aqui vamos",
      "perro casa... CaSA, string algorithms, estaba pensando en un Trie porque es muy genial...",
      "despues de que esto funcione debo cambiarle el nombre de Nescafe a la clase, no puedo entregar con ese nombre tan RaNDoM",
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


    for (int i = 0; i < docs2.Length; i++)
    {
      TF.Add(new Dictionary<string, double>());
      processDocuments(docs2[i], i);
    }

    foreach (string term in Voc.Keys)
    {
      IDF[term] = Math.Log((double)docs2.Length / Voc[term].Count());
    }

    for (int i = 0; i < docs2.Length; i++)
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
      string word = tokenizer(words[i]);
      if (word.Length == 0)
      {
        continue;
      }

      if (!TF[index].ContainsKey(word)) TF[index].Add(word, 0);
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

  private string tokenizer(string word)
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

}

public static class Moogle
{
  public static SearchResult Query(string query)
  {
    // Modifique este método para responder a la búsqueda

    SearchItem[] items = new SearchItem[3] {
      new SearchItem("Hello World", "Lorem ipsum dolor sit amet", 0.9f),
      new SearchItem("Hello World", "Lorem ipsum dolor sit amet", 0.5f),
      new SearchItem("Hello World", "Lorem ipsum dolor sit amet", 0.1f),
    };

    return new SearchResult(items, query);
  }
}
