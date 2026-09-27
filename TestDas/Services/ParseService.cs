using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using TestDas.Models;
using Element = TestDas.Models.Element;

namespace TestDas.Services
{
    public class ParseService : IParseService
    {
        private static readonly Regex EmailRegex = new(
            @"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}\b",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public async Task<ParseResult> ParseAsync(HtmlExtractionRequest request)
        {
            try
            {
                var url = GetStringFromB64(request.UrlB64);

                var page = GetStringFromB64(request.PageB64);

                var htmlCollection = await GetHtmlCollection(page, request.Selector);

                var elements = GetElements(htmlCollection, request.Attribute);

                var elementsValues = GetElementValues(elements);
  
                var decryptedText = GetStringFromEncryptedB64(request.EncryptedTextBytesB64 ,request.KeyBytesB64);

                var emailList = GetEmails(page);

                var analysisResponse = new HtmlExtractionResponse
                {
                    ElementsCount = htmlCollection.Count,
                    EmailsCount = emailList.Count,
                    Url = url,
                    DecryptedPlainText = decryptedText,
                    ElementsAttrList = elementsValues,
                    EmailsList = emailList
                };

                return new ParseResult
                {
                    HtmlExtractionResponse = analysisResponse,
                    Elements = elements
                };

            }
            catch (Exception exception)
            {
                return new ParseResult
                {
                    HtmlExtractionResponse = new HtmlExtractionResponse
                    {
                        IsError = 1,
                        ErrorCode = exception.GetType().Name,
                        ErrorMessage = exception.Message
                    },
                    Elements = []
                };
            }
        }

        private static List<Element> GetElements(IHtmlCollection<IElement> htmlCollection, string attribute)
        {
            return htmlCollection.Select(e => new Element
            {
                HtmlContent = e.OuterHtml,
                AttributeValue = e.GetAttribute(attribute) ?? string.Empty
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
            elements.Select(e => e.AttributeValue)
                           .ToList();
        
        private static List<string> GetEmails(string page) =>
            EmailRegex.Matches(page)
                .Select(e => e.Value)
                .ToList();
    }
}
