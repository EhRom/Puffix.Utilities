using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace Puffix.Utilities
{
    /// <summary>
    /// Helper for objects.
    /// </summary>
    public static class ObjectUtilities
    {
        /// <summary>
        /// Clone one object.
        /// </summary>
        /// <typeparam name="ObjectT">Type of the object to clone.</typeparam>
        /// <param name="objectToClone">Object to clone.</param>
        /// <returns>Objet cloné.</returns>
        public static ObjectT DeepClone<ObjectT>(ObjectT objectToClone)
            where ObjectT : class
        {
            ObjectT clonedObject;

            using MemoryStream memoryStream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();

            // Serialisation de l'objet.
            formatter.Serialize(memoryStream, objectToClone);
            memoryStream.Flush();
            memoryStream.Seek(0, SeekOrigin.Begin);

            // Deserialisation de l'objet, dans une nouvelle instance.
            clonedObject = (ObjectT)formatter.Deserialize(memoryStream);

            return clonedObject;
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
            ObjectT clonedObject;

            using MemoryStream memoryStream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();

            // Serialize the object.
            formatter.Serialize(memoryStream, objectToClone);
            await memoryStream.FlushAsync();
            memoryStream.Seek(0, SeekOrigin.Begin);

            // Deserialize the object for the cloned instance.
            clonedObject = (ObjectT)formatter.Deserialize(memoryStream);

            return clonedObject;
        }
    }
}
