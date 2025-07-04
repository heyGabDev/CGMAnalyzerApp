namespace CGMAnalyzer.API.Modeles
{
    public class CGMResult
    {
        public string FileName { get; set; }
        public string BmpPath { get; set; }
        public List<string> Errors { get; set; }
    }
}
