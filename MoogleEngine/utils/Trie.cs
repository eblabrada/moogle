namespace MoogleEngine;

public class Trie
{
  const int MaxLen = 100000, AlphaLen = 250;
  private char[] alphabet = new char[MaxLen + 1];
  private List<int>[] child = new List<int>[MaxLen + 1];
  private int[] parent = new int[MaxLen + 1];
  private int[] nodeCnt = new int[MaxLen + 1];
  private int curNode = 0;

  private void CreateNode(char c, int par)
  {
    this.alphabet[curNode] = c;
    this.parent[curNode] = par;
    this.child[curNode] = new List<int>(new int[AlphaLen]);
    this.curNode++;
  }

  public Trie()
  {
    CreateNode('$', -1);
  }

  public void Insert(string word)
  {
    int cur = 0;
    for (int i = 0; i < word.Length; i++)
    {
      if (!Char.IsAscii(word[i])) continue;
      int alphaNum = (int)word[i];
      if (child[cur][alphaNum] == 0)
      {
        child[cur][alphaNum] = curNode;
        CreateNode(word[i], cur);
      }
      cur = child[cur][alphaNum];
      nodeCnt[cur]++;
    }
  }

  public int MaxLCP(string word)
  {
    int cur = 0, lcp = 0;
    for (int i = 0; i < word.Length; i++)
    {
      if (!Char.IsAscii(word[i])) continue;
      int alphaNum = (int)word[i];
      if (child[cur][alphaNum] == 0)
      {
        break;
      }
      cur = child[cur][alphaNum];
      lcp++;
    }

    return lcp;
  }

  public double PrefixRelevance(string word, double percent = 80.0)
  {
    double res = 0.0;
    int expectedLen = (int)(word.Length * percent / 100.0);
    int cur = 0, lcp = 0;
    for (int i = 0; i < word.Length; i++)
    {
      if (!Char.IsAscii(word[i])) continue;
      int alphaNum = (int)word[i];
      if (child[cur][alphaNum] == 0)
      {
        break;
      }
      cur = child[cur][alphaNum];
      lcp++;

      if (lcp >= expectedLen)
      {
        res += (double)lcp / expectedLen * Math.Log(nodeCnt[cur] + 1);
      }
    }

    return res;
  }
}
