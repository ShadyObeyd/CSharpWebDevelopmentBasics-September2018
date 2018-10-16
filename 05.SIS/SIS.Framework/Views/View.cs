namespace SIS.Framework.Views
{
    using ActionResults.Contracts;

    using System.IO;

    public class View : IRenderable
    {
        private readonly string fullyQualifiedTemplateName;

        public View(string fullyQualifiedTemplateName)
        {
            this.fullyQualifiedTemplateName = fullyQualifiedTemplateName;
        }

        public string Render()
        {
            string fullHtml = this.ReadFile(this.fullyQualifiedTemplateName);

            return fullHtml;
        }

        private string ReadFile(string fullyQualifiedTemplateName)
        {
            string result = File.ReadAllText(fullyQualifiedTemplateName);

            if (string.IsNullOrEmpty(result))
            {
                throw new FileNotFoundException();
            }

            return result;
        }
    }
}
