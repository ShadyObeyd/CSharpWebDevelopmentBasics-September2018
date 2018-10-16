namespace SIS.Framework.Attributes.Methods
{
    public class HttpPutAttribute : HttpMethodAttribute
    {
        private const string MethodName = "PUT";

        public override bool IsValid(string requestMethod)
        {
            return requestMethod.ToUpper() == MethodName;
        }
    }
}
