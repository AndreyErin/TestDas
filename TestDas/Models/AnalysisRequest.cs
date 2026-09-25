namespace TestDas.Models
{
    public class AnalysisRequest
    {
        public string Selector { get; set; } = string.Empty;
        public string Attribute { get; set; } = string.Empty;
        public string Url_B64 { get; set; } = string.Empty;
        public string Encrypted_Text_Bytes_B64 { get; set; } = string.Empty;
        public string Key_Bytes_B64 { get; set; } = string.Empty;
        public string Page_B64 { get; set; } = string.Empty;
    }
}
