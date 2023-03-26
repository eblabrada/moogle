using System.IO;

namespace MoogleEngine {
  public class DocInfo {
    string title = "";
    public Dictionary<string, List<int>> index = new Dictionary<string, List<int>>();
    public Dictionary<string, double> tfidf = new Dictionary<string, double>();
  
    public DocInfo() {}  
    public DocInfo(string file) {
      string contents = File.ReadAllText(file);
      title = new FileInfo(file).Name;
    }
    
    public bool contains(string word) {
      return index.ContainsKey(word);
    }
    
    public void insert(string word, int ind) {
      if (!this.contains(word)) {
        this.index[word] = new List<int>();
      }
      this.index[word].Add(ind);
    }
  
    public void delete(string word, int ind) {
      if (!this.contains(word)) {
        return;
      }
      this.index[word].Remove(ind);
    }
    
    public void calc(Dictionary<string, double> idfs) {
      foreach (var x in idfs) {
        if (!this.contains(x.Key)) {
          this.tfidf[x.Key] = 0;
        } else {
          double tf = index[x.Key].Count() / index.Count();
          double idf = x.Value;
          tfidf.Add(x.Key, tf * idf);
        }
      }
    }   
  }
}