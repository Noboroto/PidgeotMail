using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Sheets.v4;
using Google.Apis.Util.Store;
using System.IO;
using System.Threading;

namespace PidgeotMailMVVM.Lib
{
    public class GoogleService
    {
        private static UserCredential credential;
        private static readonly string TextCredential = "{\"installed\":{\"client_id\":\"155023122563-9qnq9022jb9189377rjt98k91s393pen.apps.googleusercontent.com\",\"project_id\":\"PidgeotMail\",\"auth_uri\":\"https://accounts.google.com/o/oauth2/auth\",\"token_uri\":\"https://oauth2.googleapis.com/token\",\"auth_provider_x509_cert_url\":\"https://www.googleapis.com/oauth2/v1/certs\",\"client_secret\":\"ztuBZE_qYRBEJbYSAgbPimu4\",\"redirect_uris\":[\"urn:ietf:wg:oauth:2.0:oob\",\"http://localhost\"]}}";
        public static UserCredential Credential => credential;

        private static string[] Scopes = { GmailService.Scope.GmailReadonly, SheetsService.Scope.Spreadsheets, GmailService.Scope.GmailSend };

        private static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        static public void Init ()
        {
            using (var stream = GenerateStreamFromString(TextCredential))
            {
                string path = "4xR24anAtrw2ajpqW45SVB56saAfas";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None, new FileDataStore(path, true)).Result;
                File.Delete("credentials.json");
            }
        }
    }
}
