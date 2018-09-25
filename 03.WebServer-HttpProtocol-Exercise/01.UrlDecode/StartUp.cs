namespace _01.UrlDecode
{
    using System;
    using System.Net;

    public class StartUp
    {
        public static void Main()
        {
            string input = Console.ReadLine();

            string decoded = WebUtility.UrlDecode(input);

            Console.WriteLine(decoded);
        }
    }
}