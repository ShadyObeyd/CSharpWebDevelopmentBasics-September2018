namespace SIS.Framework.Attributes.Methods
{
    public class HttpDeleteAttribute : HttpMethodAttribute
    {
        private const string MethodName = "DELETE";

        public override bool IsValid(string requestMethod)
        {
            return requestMethod.ToUpper() == MethodName;
        }
    }
}
