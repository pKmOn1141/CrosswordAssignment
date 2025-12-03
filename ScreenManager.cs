using System.Runtime.InteropServices;
using UserManager;
using Validation;
using CrosswordManager;
using static System.Collections.Specialized.BitVector32;

namespace ScreenManager
{
    // Class for each subsection of the main menu
    class MenuSection
    {
        private String _name;
        private String[] _dropDown;
        // Char version of dropdown for validation
        private Char[] _dropShort;
        // 0 = new, load - 1 = login, register
        private int _dropType;
        // true = changes after login, false = no change
        private bool _change;
        private String[] _changedMenu;
        private Char[] _changedShort;
        private int _row_position;

        public MenuSection(String n, String[] l, Char[] ds, int dt, bool b, String[] c, Char[] cS,int r)
        {
            _name = n;
            _dropDown = l;
            _dropShort = ds;
            _dropType = dt;
            _change = b;
            _changedMenu = c;
            _changedShort = cS;
            _row_position = r;
        }

        public String name
        {
            get { return _name; }
        }

        public String[] dropDown
        {
            get { return _dropDown; }
        }

        public int row_position
        {
            get { return _row_position; }
        }

        public Char[] dropShort
        {
            get { return _dropShort; }
        }

        public int dropType
        {
            get { return _dropType; }
        }

        public bool change
        {
            get { return _change; }
        }

        public String[] changedMenu
        {
            get { return _changedMenu; }
        }

        public Char[] changedShort
        {
            get { return _changedShort; }
        }
    }

    // The main menu class
    class Menu
    {
        private UserList _userAccounts;
        // Check if user is logged in
        // -1 not logged in, 0 admin, 1 user
        public int loggedStatus = -1;
        
        private List<MenuSection> _sections = new List<MenuSection>();
        private const String HEADER = "Welcome to the Crossword Creator";
        private const String BAR = "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~";
        private int _screenWidth;
        private const int OPTION_LINE = 3;

        private Validator val = new Validator();
        private AccountMenu LoginPage;
        private CrosswordScreens _cwScreen;

        public Menu(UserList a, String cF)
        {
            _userAccounts = a;
            LoginPage = new AccountMenu(a);
            _cwScreen = new CrosswordScreens(cF);

            // Creating objects for each menu subsection
            _sections.Add(new MenuSection("Create", ["New", "Load"], ['n', 'l', 'b'], 0, false, [], [], 0));
            _sections.Add(new MenuSection("Solve", ["Load"], ['l', 'b'], 0, false, [], [], 12));
            _sections.Add(new MenuSection("User", ["Login", "Register"], ['l', 'r', 'b'], 1, true, ["Log out", "Change Role"], ['l', 'c', 'b'], 23));
            _sections.Add(new MenuSection("Quit", [""], [], 0, false, [], [], 33));
            _screenWidth = Console.WindowWidth;
        }

        // Displays the basic parts of the menu
        public void DisplayMenu()
        {
            // Writing the header to the console
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.SetCursorPosition((_screenWidth / 2) - (HEADER.Length / 2), 0);
            Console.WriteLine(HEADER);
            Console.SetCursorPosition((_screenWidth / 2) - (BAR.Length / 2), 1);
            Console.WriteLine(BAR);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("\n");

            Char[] availableInp = CreateMainMenu();

            Console.Write("\n\n\n\n\n\n\n\n\n");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Press the key that matches the first letter of the menu you want to open. Press 'b' to go back.");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Created by James Curzon, 2025\n");
            Console.SetCursorPosition(0, 4);
            MenuInteraction(availableInp);
        }

        // Creates the interactive menu parts
        public Char[] CreateMainMenu()
        {
            Char[] chars = [];

            switch(loggedStatus)
            {
                // Not logged in
                case -1:
                    for (int i = 0; i < 4; i++)
                    {
                        if (i < 2)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                        }
                        Console.SetCursorPosition(_sections[i].row_position, OPTION_LINE);
                        Console.Write(_sections[i].name);
                    }
                    chars = ['u', 'q'];
                    break;
                // admin
                case 0:
                    Console.ForegroundColor= ConsoleColor.Green;
                    foreach (MenuSection section in _sections)
                    {
                        Console.SetCursorPosition(section.row_position, OPTION_LINE);
                        Console.Write(section.name);
                    }
                    chars = ['c', 's', 'u', 'q'];
                    break;
                // player
                case 1:
                    for (int i = 0; i < 4; i++)
                    {
                        if (i < 1)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                        }
                        Console.SetCursorPosition(_sections[i].row_position, OPTION_LINE);
                        Console.Write(_sections[i].name);
                    }
                    chars = ['s', 'u', 'q'];
                    break;
            }

            Console.ForegroundColor = ConsoleColor.White;
            return chars;
        }

        // Used to clear the given console lines
        public void ClearConsoleLine()
        {
            for (int i = 0; i < 2; i++)
            {
                Console.SetCursorPosition(0, (OPTION_LINE+1) + i);
                Console.Write(new String(' ', Console.BufferWidth));
            }
        }

        // Display the drop down menus
        public void DisplaySub(int i)
        {
            int target_line = OPTION_LINE;
            Console.SetCursorPosition(_sections[i].row_position, target_line+1);
            // If available, show the changed menus
            if (loggedStatus != -1 && _sections[i].change)
            {
                foreach (String sub in _sections[i].changedMenu)
                {
                    Console.Write(sub);
                    target_line++;
                    Console.SetCursorPosition(_sections[i].row_position, target_line + 1);
                }
            }
            else
            {
                // Iterate each line, displaying the correct submenu item
                foreach (String sub in _sections[i].dropDown)
                {
                    Console.Write(sub);
                    target_line++;
                    Console.SetCursorPosition(_sections[i].row_position, target_line + 1);
                }
            }
        }

        // Allows interaction with the menus
        public void MenuInteraction(Char[] availableInps)
        {
            bool fin = false;
            while (!fin)
            {
                char menuInp = val.CharCheck(availableInps, true);

                // opening subsection based on user input
                switch (menuInp)
                {
                    case 'c':
                        DisplaySub(0);
                        SubChoice(0, 0);
                        fin = true;
                        DisplayMenu();
                        break;
                    case 's':
                        DisplaySub(1);
                        SubChoice(1, 1);
                        fin = true;
                        DisplayMenu();
                        break;
                    case 'u':
                        DisplaySub(2);
                        SubChoice(2, 2);
                        fin = true;
                        DisplayMenu();
                        break;
                    case 'q':
                        ClearConsoleLine();
                        Console.SetCursorPosition(0, OPTION_LINE + 3);
                        Console.WriteLine("Exiting...");
                        Environment.Exit(0);
                        break;
                    default:
                        break;
                }
            }
        }

        // Controls making choices when in a submenu
        public void SubChoice(int i, int section)
        {
            bool changeSection = _sections[i].change;
            char subInp;
            int subType = _sections[i].dropType;

            if (changeSection && loggedStatus != -1)
            {
                subInp = val.CharCheck(_sections[i].changedShort, true);
            }
            else
            {
                subInp = val.CharCheck(_sections[i].dropShort, true);
            }

            if (subInp == 'b')
            {
                ClearConsoleLine();
                return;
            }
            else
            {
                // Create or solve menus
                if (subType == 0)
                {
                    switch (subInp)
                    {
                        // new
                        case 'n':
                            _cwScreen.IniCreationMenu();
                            break;
                            // load
                        case 'l':
                            if (section == 0)
                            {
                                // Creation
                                _cwScreen.LoadCrosswordMenu(0);
                            }
                            else
                            {
                                // PLay
                                _cwScreen.LoadCrosswordMenu(1);
                            }
                            break;
                        default:
                            break;
                    }
                }
                // User menu
                else
                {
                    // Changed
                    if (changeSection && loggedStatus != -1)
                    {
                        switch (subInp)
                        {
                            // logout
                            case 'l':
                                Console.Clear();
                                loggedStatus = -1;
                                break;
                            // change role
                            case 'c':
                                LoginPage.ChangeRoleMenu(loggedStatus);
                                break;
                        }
                    }
                    // Not changed
                    else
                    {
                        switch (subInp)
                        {
                            // logij
                            case 'l':
                                Console.Clear();
                                loggedStatus = LoginPage.DisplayUserPage(1);
                                break;
                            // register
                            case 'r':
                                Console.Clear();
                                loggedStatus = LoginPage.DisplayUserPage(0);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
    }

    // Manages the login and account menu sections
    class AccountMenu
    {
        private UserList _userAccounts;
        private Validator _val = new Validator();

        private String[] _tabs = ["Username:   ", "Password:   "];
        private const String _LOGIN = "Enter Login Information";
        private const String _REGISTER = "Enter new account information";

        public AccountMenu(UserList a)
        {
            _userAccounts = a;
        }

        // Creates the login and registration page
        // Paramaters - 0 = registration, 1 = login
        public int DisplayUserPage(int type)
        {
            int loggedStatus = -1;
            Console.Clear();

            // Registration
            if (type == 0)
            {
                Console.SetCursorPosition((Console.WindowWidth / 2) - (_REGISTER.Length / 2), 1);
                Console.WriteLine(_REGISTER);
            }
            // Login
            else
            {
                Console.SetCursorPosition((Console.WindowWidth / 2) - (_LOGIN.Length / 2), 0);
                Console.WriteLine(_LOGIN);
            }

            int linePos = 4;
            foreach (String tab in _tabs)
            {
                Console.SetCursorPosition(Console.WindowWidth / 3, linePos);
                Console.Write(tab);
                linePos+= 2;
            }

            if (type == 0)
            {
                loggedStatus = RegisterMenu();
            }
            else
            {
                loggedStatus = LoginMenu();
            }

            return loggedStatus;
        }

        // Creates the menu for logging in
        public int LoginMenu()
        {
            int loggedStatus = -1;

            Console.SetCursorPosition((Console.WindowWidth / 3) + _tabs[0].Length, 4);
            String? userInp = Console.ReadLine();
            Console.SetCursorPosition((Console.WindowWidth / 3) + _tabs[1].Length, 6);
            String? passInp = Console.ReadLine();

            String success = "Successfully Logged In, press any key to continue";
            String unsuccess = "Unsuccessful login, please try again. Press any key to continue";

            // Fix: Ensure non-null arguments for UserVerification
            var results = _userAccounts.UserVerification(userInp ?? string.Empty, passInp ?? string.Empty);

            if (results.Item1 == true)
            {
                Console.SetCursorPosition((Console.WindowWidth / 2) - (success.Length / 2), 8);
                Console.Write(success);
                Console.ReadKey(true);
                Console.Clear();
                if (results.Item2 == true)
                {
                    // Makes the user an admin
                    loggedStatus = 0;
                }
                else
                {
                    loggedStatus = 1;
                }
                return loggedStatus;

            }
            else
            {
                Console.SetCursorPosition((Console.WindowWidth / 2) - (unsuccess.Length / 2), 8);
                Console.Write(unsuccess);
                Console.ReadKey(true);
                Console.Clear();

                return loggedStatus;
            }
        }

        // The menu for registering a new account
        public int RegisterMenu()
        {
            int loggedStatus = -1;

            Console.SetCursorPosition((Console.WindowWidth / 3) + _tabs[0].Length, 4);
            String? userInp = Console.ReadLine();
            Console.SetCursorPosition((Console.WindowWidth / 3) + _tabs[1].Length, 6);
            String? passInp = Console.ReadLine();

            String success = "Successfully registered, press any key to continue";
            String unsuccess = "Unsuccessful registration, please try again. Press any key to continue";

            if (userInp == null)
            {
                userInp = string.Empty;
            }
            if (passInp == null)
            {
                passInp = string.Empty;
            }

            // If name isnt taken
            if (_val.UsernameCheck(userInp, _userAccounts))
            {
                // If inputs arent blank
                if (!_val.BlankCheck([userInp, passInp]))
                {
                    _userAccounts.NewUser(userInp, passInp, false);
                    Console.SetCursorPosition((Console.WindowWidth / 2) - (success.Length / 2), 8);
                    Console.Write(success);
                    Console.ReadKey(true);
                    Console.Clear();
                    // Makes the user a player at default
                    loggedStatus = 1;
                    return loggedStatus;
                }
            }
            Console.SetCursorPosition((Console.WindowWidth / 2) - (unsuccess.Length / 2), 8);
            Console.Write(unsuccess);
            Console.ReadKey(true);
            Console.Clear();

            return loggedStatus;
        }

        // The menu for allowing changing of roles
        public void ChangeRoleMenu(int loggedStatus)
        {
            Console.Clear();
            const String INV = "You dont have permission to access this feature. Press any key to go back.";
            const String VAL = "Enter username to change. Enter '0' to make admin, enter '1' to make player.";
            const String ERR = "Unable to change user role, try again. Press any key to continue.";
            const String SCC = "Succesfully changed user role. Press any key to continue";
            String[] tabs = ["Username:    ", "New Role:    "];

            // Makes sure the user is an admin
            if (loggedStatus != 0)
            {
                // Error message for user
                Console.SetCursorPosition((Console.WindowWidth / 2) - (INV.Length / 2), 2);
                Console.Write(INV);
                Console.ReadKey(true);
                Console.Clear();
            }
            else
            {
                Console.SetCursorPosition((Console.WindowWidth / 2) - (VAL.Length / 2), 2);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(VAL);
                Console.ForegroundColor = ConsoleColor.White;

                // Prints the menu
                int line = 6;
                foreach (String section in tabs)
                {
                    Console.SetCursorPosition((Console.WindowWidth / 3) - (section.Length / 2), line);
                    Console.Write(section);
                    line += 3;
                }

                Console.SetCursorPosition((Console.WindowWidth / 3) + tabs[0].Length, 6);
                String? userInp = Console.ReadLine();
                Console.SetCursorPosition((Console.WindowWidth / 3) + tabs[1].Length, 9);
                // Checks the inputted int is valid
                int changeInp = _val.IntCheck();
                if (changeInp == -99)
                {
                    Console.Clear();
                    Console.SetCursorPosition((Console.WindowWidth / 2) - (ERR.Length / 2), 2);
                    Console.Write(ERR);
                    Console.ReadKey(true);
                    Console.Clear();
                }
                else
                {
                    Console.Clear();
                    bool changed = _userAccounts.ChangeUserRole(userInp ?? string.Empty, changeInp);
                    if (changed)
                    {
                        Console.SetCursorPosition((Console.WindowWidth / 2) - (SCC.Length / 2), 2);
                        Console.Write(SCC);
                        Console.ReadKey(true);
                        Console.Clear();
                    }
                    else
                    {
                        Console.SetCursorPosition((Console.WindowWidth / 2) - (ERR.Length / 2), 2);
                        Console.Write(ERR);
                        Console.ReadKey(true);
                        Console.Clear();
                    }
                }
            }
        }
    }

}