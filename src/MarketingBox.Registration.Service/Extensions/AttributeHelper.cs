using System.ComponentModel;

namespace MarketingBox.Registration.Service.Extensions
{
    public static class AttributeHelper
    {
        public static string DescriptionAttr<T>(this T source)
        {
            var fieldInfo = source.GetType().GetField(source.ToString() ?? string.Empty);

            var attributes = (DescriptionAttribute[])fieldInfo?.GetCustomAttributes(
                typeof(DescriptionAttribute), false);

            return attributes != null && attributes.Length > 0 
                ? attributes[0].Description 
                : source.ToString();
        }
    }
}