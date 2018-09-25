namespace _02.ValidateUrl
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Text;

    public class StartUp
    {
        private const int httpPort = 80;
        private const int httpsPort = 443;

        public static void Main()
        {
            string input = Console.ReadLine();

            string decodedUrl = WebUtility.UrlDecode(input);

            Uri uriResult;

            bool isUrl = Uri.TryCreate(decodedUrl, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            if (!isUrl)
            {
                Console.WriteLine("Invalid URL");
                return;
            }

            bool urlContainsElements = uriResult.Scheme != null && uriResult.Host != null;

            bool portsValid = (uriResult.Scheme == Uri.UriSchemeHttp && uriResult.Port == httpPort) ||
                              (uriResult.Scheme == Uri.UriSchemeHttps && uriResult.Port == httpsPort);

            if (!urlContainsElements || !portsValid)
            {
                Console.WriteLine("Invalid URL");
                return;
            }

            Console.WriteLine($"Protocol: {uriResult.Scheme}");
            Console.WriteLine($"Host: {uriResult.Host}");
            Console.WriteLine($"Port: {uriResult.Port}");
            Console.WriteLine($"Path: {uriResult.AbsolutePath}");

            if (uriResult.Query != null && uriResult.Query != "")
            {
                string querry = RemoveStringFirstElement(uriResult.Query);
                Console.WriteLine($"Querry: {querry}");
            }

            if (uriResult.Fragment != null && uriResult.Fragment != "")
            {
                string fragment = RemoveStringFirstElement(uriResult.Fragment);
                Console.WriteLine($"Fragment: {fragment}");
            }
        }

        private static string RemoveStringFirstElement(string str)
        {
            char[] strTokens = str.Skip(1).ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (char ch in strTokens)
            {
                sb.Append(ch);
            }

            return sb.ToString();
        }
    }
}