using System;
using System.IO;
using System.Xml.Serialization;

namespace BestYoutubeDownloader.Services.Storage
{
    public class StorageService : IStorageService
    {
        public bool Save<T>(T input, string fileName = null)
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
                // add exception handling here
                return false;
            }

            return true;
        }

        public T Load<T>(string fileName = null)
        {
            var deserializer = new XmlSerializer(typeof(T));
            var output = default(T);
            
            try
            {
                var reader = new StreamReader(this.BuildPath(typeof(T).Name, fileName));

                var obj = deserializer.Deserialize(reader);

                output = (T) obj;
            }
            catch (Exception e)
            {
                // add exception handling here

                return default(T);
            }

            return output;
        }
        
        private string BuildPath(string typeName, string fileName = null)
        {
            var path = this.GetDirectoryPath();

            if (Directory.Exists(path) == false)
                Directory.CreateDirectory(path);

            if (fileName != null)
            {
                path += fileName;
            }
            else
            {
                path += typeName;
            }

            path += ".xml";

            return path;
        }

        private string GetDirectoryPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\BestYoutubeDownloader\";
        }
    }
}