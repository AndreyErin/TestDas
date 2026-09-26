using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using TestDas.Models;
using Element = TestDas.Models.Element;

namespace TestDas.Services
{
    public class AnalysisParseService : IParseService
    {
        private static readonly Regex EmailRegex = new(
            @"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}\b",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public async Task<ParseResult> ParseAsync(AnalysisRequest request)
        {
            try
            {
                var url = GetStringFromB64(request.Url_B64);
                var page = GetStringFromB64(request.Page_B64);

                
                var htmlCollection = await GetHtmlCollection(page, request.Selector);

                var elements = GetElements(htmlCollection, request.Attribute);

                var elementsValues = GetElementValues(elements);
  
                var decryptedText = GetStringFromEncryptedB64(request.Encrypted_Text_Bytes_B64 ,request.Key_Bytes_B64);

                var emailList = GetEmails(page);

                var analysisResponse = new AnalysisResponse
                {
                    Elements_Count = htmlCollection.Count,
                    Emails_Count = emailList.Count,
                    Url = url,
                    Decrypted_Plain_Text = decryptedText,
                    Elements_Attr_List = elementsValues,
                    Emails_List = emailList
                };

                return new ParseResult
                {
                    Analysis = analysisResponse,
                    Elements = elements
                };

            }
            catch (Exception exception)
            {
                return new ParseResult
                {
                    Analysis = new AnalysisResponse
                    {
                        Is_Error = 1,
                        Error_Code = exception.GetType().Name,
                        Error_Message = exception.Message
                    },
                    Elements = []
                };
            }
        }

        private static List<Element> GetElements(IHtmlCollection<IElement> htmlCollection, string attribute)
        {
            return htmlCollection.Select(e => new Element
            {
                HtmlText = e.OuterHtml,
                ValueAttribute = e.GetAttribute(attribute) ?? string.Empty
            }).ToList();
        }

        private static async Task<IHtmlCollection<IElement>> GetHtmlCollection(string htmlText, string selector)
        {
            var parser = new HtmlParser();
            var document = await parser.ParseDocumentAsync(htmlText);

            return document.QuerySelectorAll(selector);
        }

        private static string GetStringFromEncryptedB64(string encryptedTextBytesB64, string keyBytesB64)
        {
            var cipherBytes = Convert.FromBase64String(encryptedTextBytesB64);
            var key = Convert.FromBase64String(keyBytesB64);

            using var aes = Aes.Create();
            aes.Key = key;
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.None;

            using var decryptor = aes.CreateDecryptor();
            var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

            return Encoding.UTF8.GetString(plainBytes);
        }

        private static string GetStringFromB64(string stringB64) =>
            Encoding.UTF8.GetString(Convert.FromBase64String(stringB64));

        private static List<string> GetElementValues(List<Element> elements) =>
            elements.Select(e => e.ValueAttribute)
                           .ToList();
        
        private static List<string> GetEmails(string page) =>
            EmailRegex.Matches(page)
                .Select(e => e.Value)
                .ToList();
    }
}
