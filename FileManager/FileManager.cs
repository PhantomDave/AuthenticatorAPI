using Newtonsoft.Json;
using System.Text;

namespace Project8.FileManager
{
    public class FileManager<T>
    {
        public delegate void SaveToFileDelegate(string filename, T toAdd);


        private static readonly string _path = "../FileManager/Files/";
        private static byte retry = 0;

        public async Task<T> LoadFromFile(string filename)
        {
            string path = _path + filename;
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
                T val = JsonConvert.DeserializeObject<T>(fileString);
                reader.Dispose();
                fs.Dispose();
                retry = 0;
                return val;
            }
            catch (Exception ex)
            {
                if (retry >= 3)
                {
                    Console.WriteLine($"Exception to write file after 3 retries, aborting execution {ex.Message}");
                    return default(T);
                }
                Console.WriteLine($"There was an error reading from file, trying again, retry number {retry}");
                await Task.Delay(1000);
                retry++;
                return await LoadFromFile(filename);

            }
        }

        public async void SaveToFile(string filename, T newlist)
        {
            string path = _path + filename;

            try
            {
                //CreateFile(path);
                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
                {
                    fs.SetLength(0);
                    byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(newlist, Formatting.Indented));
                    await fs.WriteAsync(bytes);
                    await fs.FlushAsync();

                    Console.WriteLine($"{path} Saved");
                    fs.Dispose();
                    retry = 0;
                }
            }
            catch (Exception ex)
            {
                if (retry >= 3)
                {
                    Console.WriteLine($"Exception to write file after 3 retries, aborting execution {ex.Message}");
                    return;
                }
                Console.WriteLine($"There was an error reading from file, trying again, retry number {retry}");
                await Task.Delay(1000);
                SaveToFile(filename, newlist);
                retry++;

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