namespace SIS.Framework.Attributes.Methods
{
    public class HttpPostAttribute : HttpMethodAttribute
    {
        private const string MethodName = "POST";

        public override bool IsValid(string requestMethod)
        {
            return requestMethod.ToUpper() == MethodName;
        }
    }
}
