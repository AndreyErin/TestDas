namespace TestDas.Models
{
    public class AnalysisResponse
    {
        public int Is_Error { get; set; }
        public string? Error_Code { get; set; }
        public string? Error_Message { get; set; }
        public int Elements_Count { get; set; }
        public int Emails_Count { get; set; }
        public string? Url { get; set; }
        public string? Decrypted_Plain_Text { get; set; }
        public List<string> Elements_Attr_List { get; set; } = new();
        public List<string> Emails_List { get; set; } = new();
    }
}
