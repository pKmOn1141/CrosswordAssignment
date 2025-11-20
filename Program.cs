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
            UserList userAccounts = new UserList();
            Menu mainMenu = new Menu(userAccounts);
            Validator validation = new Validator();

            //Crucial variables
            const String USER_FILE = "users.json";

            validation.FirstLoadCheck(USER_FILE, userAccounts);
            mainMenu.DisplayMenu();
        }
    }
}
