using Newtonsoft.Json;
using System.Text;

namespace Project8.FileManager
{
    public class FileManager<T>
    {
        public delegate void SaveToFileDelegate(string filename, T toAdd);


        private static readonly string Path = "../FileManager/Files/";
        private static byte _retry = 0;

        public async Task<T> LoadFromFile(string filename)
        {
            string path = Path + filename;
            Console.WriteLine(path);
            try
            {
                Console.WriteLine($"Loading: {path}");
                CreateFile(path);
                FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite);
                StreamReader reader = new StreamReader(fs);
                string fileString = await reader.ReadToEndAsync();
                Console.WriteLine(fileString);
                if (fileString.Length == 0)
                {
                    return default;
                }
                T val = JsonConvert.DeserializeObject<T>(fileString, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });
                reader.Dispose();
                fs.Dispose();
                _retry = 0;
                return val;
            }
            catch (Exception ex)
            {
                if (_retry >= 3)
                {
                    Console.WriteLine($"Exception to write file after 3 retries, aborting execution {ex.Message}");
                    return default(T);
                }
                Console.WriteLine($"There was an error reading from file, trying again, retry number {_retry}");
                await Task.Delay(1000);
                _retry++;
                return await LoadFromFile(filename);

            }
        }

        public async void SaveToFile(string filename, T newlist)
        {
            string path = Path + filename;

            try
            {
                //CreateFile(path);
                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
                {
                    fs.SetLength(0);
                    byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(newlist, new JsonSerializerSettings
                    {
                        Formatting = Formatting.Indented,
                        TypeNameHandling = TypeNameHandling.All,
                    }));
                    await fs.WriteAsync(bytes);
                    await fs.FlushAsync();

                    Console.WriteLine($"{path} Saved");
                    fs.Dispose();
                    _retry = 0;
                }
            }
            catch (Exception ex)
            {
                if (_retry >= 3)
                {
                    Console.WriteLine($"Exception to write file after 3 retries, aborting execution {ex.Message}");
                    return;
                }
                Console.WriteLine($"There was an error reading from file, trying again, retry number {_retry}");
                await Task.Delay(1000);
                SaveToFile(filename, newlist);
                _retry++;

            }
        }

        private void CreateFile(string path)
        {

            if (!File.Exists(path))
            {
                Console.WriteLine(path);
                File.Create(path);
            }
        }
    }
}