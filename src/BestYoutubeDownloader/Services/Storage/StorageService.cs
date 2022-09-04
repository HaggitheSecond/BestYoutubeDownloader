using BestYoutubeDownloader.Services.ExceptionHandling;
using Caliburn.Micro;
using System;
using System.IO;
using System.Xml.Serialization;

namespace BestYoutubeDownloader.Services.Storage
{
    public class StorageService : IStorageService
    {
        public bool Save<T>(T input, string? fileName = null)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(T));

                using (var writer = new StreamWriter(this.BuildPath(typeof(T).Name, fileName)))
                {
                    serializer.Serialize(writer, input);
                }
            }
            catch (Exception e)
            {
                IoC.Get<IExceptionHandler>().Handle(e);
                return false;
            }

            return true;
        }

        public T? Load<T>(string? fileName = null)
        {
            try
            {
                var deserializer = new XmlSerializer(typeof(T));

                var reader = new StreamReader(this.BuildPath(typeof(T).Name, fileName));

                var obj = deserializer.Deserialize(reader);

                if (obj is null || obj is not T t)
                    return default;

                return t;
            }
            catch (Exception e)
            {
                IoC.Get<IExceptionHandler>().Handle(e);
                return default;
            }
        }
        
        private string BuildPath(string typeName, string? fileName = null)
        {
            var path = this.GetDirectoryPath();

            if (Directory.Exists(path) == false)
                Directory.CreateDirectory(path);

            path += string.IsNullOrWhiteSpace(fileName) switch
            {
                false => fileName,
                _ => typeName,
            };

            path += ".xml";

            return path;
        }

        private string GetDirectoryPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\BestYoutubeDownloader\";
        }
    }
}