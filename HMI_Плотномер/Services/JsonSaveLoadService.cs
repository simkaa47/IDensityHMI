using System;
using System.IO;
using System.Runtime.Serialization.Json;

namespace IDensity.Core.Services
{
    public static class JsonSaveLoadService
    {
        public static void Save(object writeObject, Type type, string path)
        {
            var jsonFormatter = new DataContractJsonSerializer(type);
            using (var file = new FileStream(path, FileMode.Truncate))
            {
                
                jsonFormatter.WriteObject(file, writeObject);
            }
        }

        public static object LoadFromJson<T>(string path)
        {
            var jsonFormatter = new DataContractJsonSerializer(typeof(T));
            object fromFile;
            using (var file = new FileStream(path, FileMode.OpenOrCreate))
            {
                fromFile = jsonFormatter.ReadObject(file);

            }
            return fromFile;
        }




    }
}
