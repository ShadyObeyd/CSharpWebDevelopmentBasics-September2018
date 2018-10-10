namespace SIS.HTTP.Sessions
{
    using Contracts;

    using System;
    using System.Linq;
    using System.Collections.Generic;

    public class HttpSession : IHttpSession
    {
        private const string IdNullMessage = "Id cannot be null or empty!";
        private const string NameNullMessage = "Name cannot be null or empty!";
        private const string ParameterNullMessage = "Parameter cannot be null!";
        private const string SessionIsContainedMessage = "Cannot add existing session!";

        private readonly Dictionary<string, object> sessionParameters;

        public HttpSession(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException(IdNullMessage);
            }

            this.Id = id;
            this.sessionParameters = new Dictionary<string, object>();
        }

        public string Id { get; }

        public void AddParameter(string name, object parameter)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException(NameNullMessage);
            }

            if (parameter == null)
            {
                throw new ArgumentException(ParameterNullMessage);
            }

            if (!this.sessionParameters.ContainsKey(name))
            {
                this.sessionParameters.Add(name, parameter);
            }
        }

        public void ClearParameters()
        {
            this.sessionParameters.Clear();
        }

        public bool ContainsParameter(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException(NameNullMessage);
            }

            return this.sessionParameters.ContainsKey(name);
        }

        public object GetParameter(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException(NameNullMessage);
            }

            return this.sessionParameters.FirstOrDefault(s => s.Key == name).Value;
        }
    }
}
