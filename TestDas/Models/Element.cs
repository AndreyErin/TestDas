using System.Text.Json.Serialization;

namespace TestDas.Models
{
    public class Element
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("html_content")]
        public string HtmlContent { get; set; }

        [JsonPropertyName("attribute_value")]
        public string AttributeValue { get; set; }
    }
}
