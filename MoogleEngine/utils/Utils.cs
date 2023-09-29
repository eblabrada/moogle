namespace MoogleEngine;

public static class Utils
{
  // given two string 'a' and 'b' returns the minimum number
  // of operations needed to make equal both strings using
  // three operations: insert a character, erase a character,
  // and change the character in some position. This problem
  // is commonly known as "Edit Distance problem". The time
  // complexity for this algorithm is O(nm).
  public static int EditDistance(string a, string b)
  {
    if (a.Length < b.Length) {
      (a, b) = (b, a); // swap
    }
    
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

  // given two strings 'a' and 'b' returns the longest common 
  // prefix of both strings. The time complexity of this algorithm
  // is O(min(n, m)).
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

  // given two strings 'a' and 'b' returns the similarity
  // of both strings. I used a heuristic for determinate the
  // similarity of both strings. The heuristic uses the cocient
  // between EditDistance and LongestCommonPrefix.
  public static double Distance(string a, string b)
  {
    int lcp = LongestCommonPrefix(a, b);
    if (lcp == 0) return EditDistance(a, b);
    return (double)EditDistance(a, b) / (double)LongestCommonPrefix(a, b);
  }

  // given two strings 'a' and 'b' returns true if both
  // string are considered similars (EditDistance results is <= 1).
  public static bool AreSimilar(string a, string b)
  {
    a = a.ToLower(); b = b.ToLower();
    if (b.Length > 1 && EditDistance(a, b) <= 1)
    {
      return true;
    }
    return false;
  }

  // given a string 'word' returns this string capitalized
  public static string Capitalize(string word)
  {
    if (word.Length == 0) return word;
    string res = word[0].ToString().ToUpper() + word.Substring(1);
    return res;
  }

  // given a string 'word' returns this string removing
  // all characters that are not letters or digits.  
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

  // given a text returns a list of the words of this text
  // all the words are returned tokenized. 
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

  // given a list of words returns the words that have '^' before.
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

  // given a list of words returns the words that have '!' before.
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

  // given a list of words returns the words that have '*' before,
  // the words are returned as a tuple (frequency, word), frequency
  // is the number of '*' before 'word'.
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

  // give a list of words returns the words are asociated with
  // '~' operator. 
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

  // given a vector returns the norm.
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
