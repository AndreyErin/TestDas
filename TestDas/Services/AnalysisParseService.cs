using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using AngleSharp.Html.Parser;
using TestDas.Models;

namespace TestDas.Services
{
    public class AnalysisParseService : IParseService
    {
        private static readonly Regex EmailRegex = new(
            @"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}\b",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public async Task<AnalysisResponse> ParseAsync(AnalysisRequest request)
        {
            try
            {
                var url = GetStringFromB64(request.Url_B64);
                var page = GetStringFromB64(request.Page_B64);

                
                var selectorResult = await GetSelectorResult(page, request.Selector, request.Attribute);

                var decryptedText = GetStringFromEncryptedB64(request.Encrypted_Text_Bytes_B64 ,request.Key_Bytes_B64);   

                var emailList = EmailRegex.Matches(page)
                    .Select(e => e.Value)
                    .ToList();

                return new AnalysisResponse
                {
                    Elements_Count = selectorResult.Count,
                    Emails_Count = emailList.Count,
                    Url = url,
                    Decrypted_Plain_Text = decryptedText,
                    Elements_Attr_List = selectorResult.ElementValues,
                    Emails_List = emailList
                };

            }
            catch (Exception exception)
            {
                return new AnalysisResponse
                {
                    Is_Error = 1,
                    Error_Code = exception.GetType().Name,
                    Error_Message = exception.Message
                };
            }
        }

        private static async Task<SelectorResult> GetSelectorResult(string htmlText, string selector, string attribute)
        {
            var parser = new HtmlParser();
            var document = await parser.ParseDocumentAsync(htmlText);
            var elements = document.QuerySelectorAll(selector);
            var elementValues = elements.Select(e => e.GetAttribute(attribute))
                                                    .Where(v => v != null)
                                                    .ToList();

            return new SelectorResult
            {
                Count = elementValues.Count,
                ElementValues = elementValues
            };
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
    }
}
