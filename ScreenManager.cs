using Validation;

namespace ScreenManager
{
    // Class for each subsection of the main menu
    class MenuSection
    {
        private String _name;
        private String[] _dropDown;
        // -1 = not logged in, 0 = user, 1 = admin+user
        private int _userRights;
        // true = changes after login, false = no change
        private bool _change;
        private String[] _changedMenu;
        private int _row_position;

        public MenuSection(String n, String[] l, int u, bool b, String[] c, int r)
        {
            _name = n;
            _dropDown = l;
            _userRights = u;
            _change = b;
            _changedMenu = c;
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
    }

    // The main menu class
    class Menu
    {
        private List<MenuSection> _sections = new List<MenuSection>();
        private const String HEADER = "Welcome to the Crossword Creator";
        private const String BAR = "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~";
        private int _screenWidth;
        private const int OPTION_LINE = 3;

        private char menuInp;
        private Validator val = new Validator();

        public Menu()
        {
            // Creating objects for each menu subsection
            _sections.Add(new MenuSection("Create", ["New", "Load"], 1, false, [], 0));
            _sections.Add(new MenuSection("Solve", ["New", "Load"], 0, false, [], 12));
            _sections.Add(new MenuSection("User", ["Login", "Register"], -1, true, ["Log out", "Change Role"], 23));
            _sections.Add(new MenuSection("Quit", [""], -1, false, [], 33));
            _screenWidth = Console.WindowWidth;
        }

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

            // Writing out the menu options
            foreach (MenuSection section in _sections)
            {
                Console.SetCursorPosition(section.row_position, OPTION_LINE);
                Console.Write(section.name);
            }
            Console.Write("\n\n\n\n\n\n\n\n\n");
            Console.Write("Created by James Curzon, 2025\n");
            Console.SetCursorPosition(0, 4);
            MenuInteraction();
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
        public void DisplaySub(String[] a, int p)
        {
            ClearConsoleLine();
            int target_line = OPTION_LINE;
            Console.SetCursorPosition(p, target_line+1);
            // Iterate each line, displaying the correct submenu item
            foreach (String sub in a)
            {
                Console.Write(sub);
                target_line ++;
                Console.SetCursorPosition(p, target_line + 1);
            }
        }

        public void MenuInteraction()
        {
            bool moveOn = false;
            while (!moveOn)
            {
                Char[] chars = { 'c', 's', 'u', 'q' };
                menuInp = val.CharCheck(chars, true);

                // opening subsection based on user input
                String[] subMenu;
                int row_pos;
                switch (menuInp)
                {
                    case 'c':
                        subMenu = _sections[0].dropDown;
                        row_pos = _sections[0].row_position;
                        DisplaySub(subMenu, row_pos);
                        break;
                    case 's':
                        subMenu = _sections[1].dropDown;
                        row_pos = _sections[1].row_position;
                        DisplaySub(subMenu, row_pos);
                        break;
                    case 'u':
                        subMenu = _sections[2].dropDown;
                        row_pos = _sections[2].row_position;
                        DisplaySub(subMenu, row_pos);
                        break;
                    case 'q':
                        Console.WriteLine("Exiting...");
                        Environment.Exit(0);
                        break;
                    default:
                        moveOn = true;
                        break;
                }
            }
        }
    }
}