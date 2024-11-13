using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Puffix.Utilities
{
    /// <summary>
    /// Helper for objects.
    /// </summary>
    public static class ObjectUtilities
    {
        private readonly static JsonSerializerOptions options = new JsonSerializerOptions
        {
            IncludeFields = true,
        };

        /// <summary>
        /// Clone one object.
        /// </summary>
        /// <typeparam name="ObjectT">Type of the object to clone.</typeparam>
        /// <param name="objectToClone">Object to clone.</param>
        /// <returns>Objet cloné.</returns>
        public static ObjectT DeepClone<ObjectT>(ObjectT objectToClone)
            where ObjectT : class
        {
            string serializedObject = JsonSerializer.Serialize(objectToClone, options);

            return JsonSerializer.Deserialize<ObjectT>(serializedObject, options);
        }

        /// <summary>
        /// Clone one object.
        /// </summary>
        /// <typeparam name="ObjectT">Type of the object to clone.</typeparam>
        /// <param name="objectToClone">Object to clone.</param>
        /// <returns>Objet cloné.</returns>
        public async static Task<ObjectT> DeepCloneAsync<ObjectT>(ObjectT objectToClone)
            where ObjectT : class
        {
            using MemoryStream memoryStream = new MemoryStream();

            await JsonSerializer.SerializeAsync(memoryStream, objectToClone, options);

            return await JsonSerializer.DeserializeAsync<ObjectT>(memoryStream, options);
        }
    }
}
