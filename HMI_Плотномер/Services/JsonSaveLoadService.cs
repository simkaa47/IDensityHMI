using IDensity.Models;
using System;
using System.IO;
using System.Runtime.Serialization.Json;

namespace IDensity.Core.Services
{
    public static class JsonSaveLoadService
    {
        public static void Save(object writeObject, Type type)
        {
            var jsonFormatter = new DataContractJsonSerializer(type);
            using (var file = new FileStream($"mainModel.json", FileMode.OpenOrCreate))
            {
                jsonFormatter.WriteObject(file, writeObject);
            }
        }


        

    }
}
