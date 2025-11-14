using Newtonsoft.Json;
using UserManager;

namespace FileManager
{
    class FileHandler
    {
        public void SerialiseObj(String filePath, List<User> objs)
        {
            try
            {
                // Create serialiser and open text file to write
                JsonSerializer serializer = new JsonSerializer();
                using (StreamWriter sw = new StreamWriter(filePath))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    // Iterate through and serialise each object to the file
                    foreach (User targetObj in objs)
                    {
                        serializer.Serialize(writer, targetObj);
                        sw.Write("\n");
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("Error to serialize");
            }
        }

        public void DeSerialiseUser(String filePath, UserList accounts)
        {
            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    String? line = sr.ReadLine();
                    while (line != null)
                    {
                        User user = JsonConvert.DeserializeObject<User>(line);
                        accounts.NewUser(user);
                        line = sr.ReadLine();
                    }
                }
            }
            catch(IOException)
            {
                Console.WriteLine("Error to deserialise");
            }
        }
    }
}