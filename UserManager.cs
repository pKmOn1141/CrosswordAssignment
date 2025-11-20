using FileManager;
using Newtonsoft.Json;
using System.Runtime.InteropServices;

namespace UserManager
{
    
    // Holds every user account
    class UserList
    {
        public List<User> accounts = new List<User>();
        private FileHandler fileHandler = new FileHandler();
        private const String USER_FILE = "users.json";

        public void SerialiseList(String path)
        {
            fileHandler.SerialiseObj(path, accounts);
        }

        public void NewUser(String name, String pwd, bool role)
        {
            accounts.Add(new User(name, pwd, role));
            SerialiseList(USER_FILE);
        }
        
        public void NewUser(User newUser)
        {
            accounts.Add(newUser);
        }

        // Checks if the inputted information contains a user account, and if its correct
        public (bool, bool) UserVerification(String name, String pwd)
        {
            bool verified = false;
            bool admin = false;

            foreach (User account in accounts)
            {
                if (account.user == name)
                {
                    if(account.pwd == pwd)
                    {
                        verified = true;
                        // Checks if is an admin
                        if (account.role == true)
                        {
                            admin = true;
                        }
                        return (verified, admin);
                    }
                }
            }

            return (verified, admin);
        }

        public bool ChangeUserRole(String name, int role)
        {
            bool changed = false;
            // Checks that the inputted role is valid
            if (role == 0 || role == 1)
            {
                foreach (User account in accounts)
                {
                    // Checks the name exists
                    if (name == account.user)
                    {
                        switch (role)
                        {
                            // Make admin
                            case 0:
                                account.ChangeRole(true);
                                break;
                            // Make player
                            case 1:
                                account.ChangeRole(false);
                                break;
                        }
                        // Reserialise the user list to save changes
                        SerialiseList(USER_FILE);
                        changed = true;
                        return changed;
                    }
                }
            }
            // For whatever reason, unable to change
            return changed;
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

        public void ChangeRole(bool newRole)
        {
            this.role = newRole;
        }
    }

}