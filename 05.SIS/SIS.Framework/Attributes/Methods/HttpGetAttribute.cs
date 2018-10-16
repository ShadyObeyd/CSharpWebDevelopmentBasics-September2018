namespace SIS.Framework.Attributes.Methods
{
    public class HttpGetAttribute : HttpMethodAttribute
    {
        private const string MethodName = "GET";

        public override bool IsValid(string requestMethod)
        {
            return requestMethod.ToUpper() == MethodName;
        }
    }
}
