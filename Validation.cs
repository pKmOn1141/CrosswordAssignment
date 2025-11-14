using System.Linq;
using UserManager;
using FileManager;

namespace Validation
{
    class Validator
    {
        /// <summary>
        /// Asks for a char, then checks it is valid based of the charArray
        /// </summary>
        /// <param name="b"> Used to check if the returned char should be lowered or not</param>
        public char CharCheck(Char[] charArray, bool b)
        {
            char input;

            // Iterates forever unti there is a valid input
            while (true)
            {
                input = Console.ReadKey(true).KeyChar;

                // If char is valid
                if (charArray.Contains(char.ToLower(input)))
                {
                    if (b)
                    {
                        return char.ToLower(input);
                    }
                    else
                    {
                        return input;
                    }
                }
            }

        }

        /// <summary>
        /// Creates files on first run, and checks they still exist
        /// </summary>
        public void FirstLoadCheck(String filePath, UserList userAccounts)
        {
            if (!File.Exists(filePath))
            {
                userAccounts.NewUser("admin", "password", true);
                userAccounts.SerialiseList(filePath);
                Console.WriteLine("File NOT loaded. Login with 'admin' and 'password' or REGISTER an account. Press any key to continue.");
                Console.ReadKey(true);
                Console.Clear();
            }
        }
    }
}