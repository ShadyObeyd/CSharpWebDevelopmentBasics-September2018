namespace SIS.Framework.Models
{
    using System.Collections.Generic;

    public class Model
    {
        private bool? isValid;

        private IDictionary<string, string> modelErrors;

        public Model()
        {
            this.modelErrors = new Dictionary<string, string>();
        }

        public bool? IsValid
        {
            get => this.isValid;
            set => this.isValid = this.isValid ?? value;
        }
    }
}