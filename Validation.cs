using System.Linq;
using UserManager;
using FileManager;

namespace Validation
{
    class Validator
    {
        private FileHandler files = new FileHandler();

        /// <summary>
        /// Asks for a char, then checks it is valid based of the charArray
        /// </summary>
        /// <param name="b"> Used to check if the returned char should be lowered or not</param>
        /// e, char to return if its not a match
        public char CharCheck(Char[] charArray, bool b)
        {
            // Iterate until a good char has been entered
            while (true)
            {
                char input = Console.ReadKey(true).KeyChar;

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
        public void FirstLoadCheck(String filePath, String folderPath, UserList userAccounts)
        {
            // If user file doesnt exist
            if (!File.Exists(filePath))
            {
                userAccounts.NewUser("admin", "password", true);
                userAccounts.SerialiseList(filePath);
                Console.WriteLine("File NOT loaded. Login with 'admin' and 'password' or REGISTER an account. Press any key to continue.");
                Console.ReadKey(true);
                Console.Clear();
            }
            // If it does
            else
            {
                files.DeSerialiseUser(filePath, userAccounts);
            }

            // Ensure the crossword folder exists
            Directory.CreateDirectory(folderPath);
        }

        // Checks to see if a username is already taken
        public bool UsernameCheck(String name, UserList accountManager)
        {
            foreach (User account in accountManager.accounts)
            {
                if (name == account.user)
                {
                    // Username is taken
                    return false;
                }
            }
            // Username is not taken
            return true;
        }

        // Checks all the strings in the given array to make sure they arent blank
        public bool BlankCheck(String[] check)
        {
            foreach (String item in check)
            {
                if (string.IsNullOrWhiteSpace(item))
                {
                    return true;
                }
            }
            return false;
        }

        // Checks a valid integer has been inputted
        public int IntCheck()
        {
            try
            {
                int input = Convert.ToInt32(Console.ReadLine());
                return input;
            }
            catch (Exception)
            {
                return -99;
            }
        }

        // Repeats until an arrow key is pressed
        public (int[], int) ArrowCheck(int[] currCell, int nRow, int nCol)
        {
            int altFunc = 0;
            int row = currCell[0];
            int col = currCell[1];
            ConsoleKeyInfo input = Console.ReadKey(true);
            switch (input.Key)
            {
                case ConsoleKey.UpArrow:
                    if (!PosCheck(--row, nRow))
                    {
                        row++;
                    }
                    break;
                case ConsoleKey.DownArrow:
                    if (!PosCheck(++row, nRow))
                    {
                        row--;
                    }
                    break;
                case ConsoleKey.LeftArrow:
                    if (!PosCheck(--col, nCol))
                    {
                        col++;
                    }
                    break;
                case ConsoleKey.RightArrow:
                    if (!PosCheck(++col, nCol))
                    {
                        col--;
                    }
                    break;
                case ConsoleKey.Enter:
                    altFunc = 1;
                    break;
                case ConsoleKey.Escape:
                    altFunc = 2;
                    break;
            }
            return ([row, col], altFunc);
        }

        // Checks that the current cell position can allow movement
        public bool PosCheck(int pos1, int poslim)
        {
            // If the location will be invalid
            if (pos1 < 0 || pos1 > poslim-1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

    }
}