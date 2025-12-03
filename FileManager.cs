using Newtonsoft.Json;
using UserManager;
using CrosswordManager;

namespace FileManager
{
    class FileHandler
    {
        // For if the obj is a user
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
            catch (IOException)
            {
                Console.WriteLine("Error to serialize");
            }
        }

        // For if the obj is a crossword
        public void SerialiseObj(String filePath, Crossword cwd)
        {
            try
            {
                // Create serialiser and open text file to write
                JsonSerializer serializer = new JsonSerializer();
                using (StreamWriter sw = new StreamWriter(filePath))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, cwd);
                    sw.Write("\n");
                }
            }
            catch (IOException)
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
                        User? user = JsonConvert.DeserializeObject<User>(line);
                        if (user != null)
                        {
                            accounts.NewUser(user);
                        }
                        line = sr.ReadLine();
                    }
                }
            }
            catch(IOException)
            {
                Console.WriteLine("Error to deserialise");
            }
        }

        public (Crossword, bool) DeSerialiseCwd(String filePath)
        {
            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    String? line = sr.ReadLine();
                    if (line != null)
                    {
                        Crossword? newCwd = JsonConvert.DeserializeObject<Crossword>(line);
                        if (newCwd != null)
                        {
                            return (newCwd, true);
                        }
                    }
                }
            }
            catch (IOException)
            {
                Console.WriteLine("Error to deserialise");
            }
            // Return empty
            return (new Crossword("", 0, 0), false);
        }
    }
}