using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace AzureQuest.Api.Extensions
{
    public static class ValidationExtensions
    {
        public static T DeepClone<T>(this T source) where T : class
        {
            using (Stream cloneStream = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();

                formatter.Serialize(cloneStream, source);
                cloneStream.Position = 0;
                T clone = (T)formatter.Deserialize(cloneStream);

                return clone;
            }
        }

        public static ObjectValidationResult Validate<T>(this T obj) where T : class, new()
        {
            if (obj == null) { throw new System.MissingMemberException($"Cannot validate null object {typeof(T).Name} on current context"); }
            var context = new ValidationContext(obj, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            var validation = Validator.TryValidateObject(obj, context, results, validateAllProperties: true);
            return new ObjectValidationResult() { IsValid = validation, Results = results };
        }

        public static string ToHtml(this IEnumerable<string> list)
        {
            var message = new StringBuilder();
            foreach (var result in list)
            {
                message
                    .Append("<li>")
                    .Append(result)
                    .Append("</li>");
            }
            return message.ToString();
        }
                
    }

    public class ObjectValidationResult
    {
        public bool IsValid { get; set; }
        public IList<ValidationResult> Results { get; set; }
        public string ResultsToHtml()
        {
            var message = new StringBuilder();
            foreach (var result in Results)
            {
                message
                    .Append("<li>")
                    .Append(result.ErrorMessage)
                    .Append("</li>");
            }
            return message.ToString();
        }
    }
}

