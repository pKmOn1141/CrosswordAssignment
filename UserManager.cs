using FileManager;
using Newtonsoft.Json;

namespace UserManager
{
    
    // Holds every user account
    class UserList
    {
        public List<User> accounts = new List<User>();
        private FileHandler fileHandler = new FileHandler();

        public void SerialiseList(String path)
        {
            fileHandler.SerialiseObj(path, accounts);
        }

        public void NewUser(String name, String pwd, bool role)
        {
            accounts.Add(new User(name, pwd, role));
        }
        
        public void NewUser(User newUser)
        {
            accounts.Add(newUser);
        }
    }
    
    // For each individual user
    class User
    {
        public String user;
        public String pwd;
        /// <summary>
        /// True if admin, false is user
        /// </summary>
        public bool role;
        public User(String u, String p, bool r)
        {
            user = u;
            pwd = p;
            role = r;
        }
    }
}