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
            //Crucial variables
            const String USER_FILE = "users.json";
            const String CWD_FOLDER = "Crosswords";

            // Crucial objects
            UserList userAccounts = new UserList();
            Menu mainMenu = new Menu(userAccounts, CWD_FOLDER);
            Validator validation = new Validator();

            validation.FirstLoadCheck(USER_FILE, CWD_FOLDER, userAccounts);
            mainMenu.DisplayMenu();
        }
    }
}
