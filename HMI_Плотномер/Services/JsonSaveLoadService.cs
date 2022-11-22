using IDensity.Models;
using Microsoft.Win32;
using System;
using System.IO;
using System.Runtime.Serialization.Json;

namespace IDensity.Core.Services
{
    public static class JsonSaveLoadService
    {
        public static void Save(object writeObject, Type type)
        {
            string path = string.Empty;
            OpenFileDialog fileDialog = new OpenFileDialog()
            {
                CheckFileExists = false,
                CheckPathExists = true,
                Multiselect = true,
                Title = "Выберите файл"
            };
            fileDialog.Filter = "  Текстовые файлы (*.json)|*.json";
            Nullable<bool> dialogOK = fileDialog.ShowDialog();


            if (dialogOK == true)
            {
                path = fileDialog.FileName;

            }
            var jsonFormatter = new DataContractJsonSerializer(type);
            using (var file = new FileStream(path, FileMode.OpenOrCreate))
            {
                jsonFormatter.WriteObject(file, writeObject);
            }
        }


        

    }
}
