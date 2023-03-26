
namespace MoogleEngine {
  public class AllDocs {
    public Dictionary<string, DocInfo> info = new Dictionary<string, DocInfo>();
    public Dictionary<string, double> idfs = new Dictionary<string, double>();
    
    public Dictionary<string, int> numDocs = new Dictionary<string, int>(); 
    public string docsPath = "";
    
    public AllDocs(string path) {
      this.docsPath = path;
      initialize();
    }
    
    private void initialize() {
      var docs = Directory.GetFiles(docsPath, ".txt");
      int n = docs.Length;
      for (int i = 0; i < n; i++) {
        processDocument(docs[i], i);
      }
    }
    
    private string deletePunctuation(string s) {
      while (s.Length > 0 && char.IsPunctuation(s[0]))
        s = s.Substring(1);
      while (s.Length > 0 && char.IsPunctuation(s[s.Length - 1]))
        s = s.Substring(0, s.Length - 1);    
      return s;
    }
    
    private void processDocument(string doc, int ind) {
      DocInfo info = new DocInfo(doc);
      string text = File.ReadAllText(Directory.GetFiles(docsPath, ".txt")[ind]).ToLower();
      
      string[] words = text.Split(' ');
      for (int i = 0; i < words.Length; i++) {
        string cur = deletePunctuation(words[i]);
        if (cur.Length == 0) continue;
      }     
    }
    
    private void addWord(string word, DocInfo info) {
      // if (!info.contains(word)) {
      //   info.insert(word);
        
      // }
    }
  }
}