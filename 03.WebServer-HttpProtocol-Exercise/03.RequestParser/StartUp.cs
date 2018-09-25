namespace _03.RequestParser
{
    using System;
    using System.Collections.Generic;

    public class StartUp
    {
        private const string ErrorStatusCode = "404 Not Found";
        private const string ErrorResponseText = "NotFound";

        private const string OkStatusCode = "200 OK";
        private const string OkResponseText = "OK";

        public static void Main()
        {
            string input = Console.ReadLine();

            var requests = new Dictionary<string, List<string>>();

            while (input != "END")
            {
                string[] httpRequestTokens = input.Split('/', StringSplitOptions.RemoveEmptyEntries);

                string requestPath = httpRequestTokens[0];
                string requestMethod = httpRequestTokens[1];

                if (!requests.ContainsKey(requestPath))
                {
                    requests.Add(requestPath, new List<string>());
                }

                requests[requestPath].Add(requestMethod);

                input = Console.ReadLine();
            }

            string[] httpResponseTokens = Console.ReadLine().Split(new char[] { ' ', '/'}, StringSplitOptions.RemoveEmptyEntries);

            string responseMethod = httpResponseTokens[0];
            string responsePath = httpResponseTokens[1];
            string responseProtocol = httpResponseTokens[2];
            string responseProtocolVersion = httpResponseTokens[3];

            if (!requests.ContainsKey(responsePath))
            {
                PrintErrorResponse(responseProtocol, responseProtocolVersion);
                return;
            }

            if (!requests[responsePath].Contains(responseMethod.ToLower()))
            {
                PrintErrorResponse(responseProtocol, responseProtocolVersion);
                return;
            }

            Console.WriteLine($"{responseProtocol}/{responseProtocolVersion} {OkStatusCode}");
            Console.WriteLine($"Content-Length: {OkResponseText.Length}");
            Console.WriteLine($"Content-Type: text/plain");
            Console.WriteLine();
            Console.WriteLine(OkResponseText);
        }

        private static void PrintErrorResponse(string responseProtocol, string responseProtocolVersion)
        {
            Console.WriteLine($"{responseProtocol}/{responseProtocolVersion} {ErrorStatusCode}");
            Console.WriteLine($"Content-Length: {ErrorResponseText.Length}");
            Console.WriteLine($"Content-Type: text/plain");
            Console.WriteLine();
            Console.WriteLine(ErrorResponseText);
        }
    }
}
