using ScreenManager;
using UserManager;
using FileManager;
using Validation;

namespace CrosswordAssignment
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Crucial objects
            Menu mainMenu = new Menu();
            UserList userAccounts = new UserList();
            Validator validation = new Validator();

            //Crucial variables
            const String USER_FILE = "users.json";

            validation.FirstLoadCheck(USER_FILE, userAccounts);
            mainMenu.DisplayMenu();
        }
    }
}
