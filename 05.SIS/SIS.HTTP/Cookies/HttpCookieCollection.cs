namespace SIS.HTTP.Cookies
{
    using Contracts;

    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class HttpCookieCollection : IHttpCookieCollection
    {
        private const string CookieNullMessage = "Cookie cannot be null!";
        private const string CookieIsContainedMessage = "Cannot add existing cookie!";
        private const string CookieParametersNullMessage = "Cookie key and value cannot be null or empty!";
        private const string CookieKeyNullMessage = "Given key cannot be null or empty!";

        private readonly Dictionary<string, HttpCookie> cookies;

        public HttpCookieCollection()
        {
            this.cookies = new Dictionary<string, HttpCookie>();
        }

        public void Add(HttpCookie cookie)
        {
            if (cookie == null)
            {
                throw new ArgumentException(CookieNullMessage);
            }

            if (string.IsNullOrEmpty(cookie.Key) || string.IsNullOrEmpty(cookie.Value))
            {
                throw new ArgumentException(CookieParametersNullMessage);
            }

            if (cookies.ContainsKey(cookie.Key))
            {
                throw new InvalidOperationException(CookieIsContainedMessage);
            }

            this.cookies.Add(cookie.Key, cookie);
        }

        public bool ContainsCookie(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException(CookieKeyNullMessage);
            }

            return this.cookies.ContainsKey(key);
        }

        public HttpCookie GetCookie(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException(CookieKeyNullMessage);
            }

            return this.cookies.FirstOrDefault(c => c.Key == key).Value;
        }

        public bool HasCookies()
        {
            return this.cookies.Any();
        }

        public override string ToString()
        {
            return string.Join("; ", this.cookies.Values);
        }
    }
}
