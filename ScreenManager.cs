using System.Reflection.Metadata.Ecma335;
using Validation;

namespace ScreenManager;

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

    public MenuSection(String n, String[] l, int u, bool b, String[] c)
    {
        _name = n;
        _dropDown = l;
        _userRights = u;
        _change = b;    
        _changedMenu = c;
    }

    public String name 
    { 
        get { return _name; }
    }

    public String[] dropDown
    {
        get { return _dropDown; }
}

// The main menu class
class Menu
{
    private List<MenuSection> _sections = new List<MenuSection>();
    private const String HEADER = "Welcome to the Crossword Creator";
    private const String BAR = "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~";
    private int _screenWidth;

    private char menuInp;
    private Validator val = new Validator();

    public Menu()
    {
        // Creating objects for each menu subsection
        _sections.Add(new MenuSection("Create", ["Load"], 1, false, []));
        _sections.Add(new MenuSection("Solve", ["New", "Continue"], 0, false, []));
        _sections.Add(new MenuSection("User", ["Login", "Register"], -1,true, ["Log out", "Change Role"]));
        _sections.Add(new MenuSection("Quit", [""], -1, false, []));
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
        foreach (MenuSection section in _sections )
        {
            Console.Write(section.name + "      ");
        }
        Console.Write("\n\n\n\n\n\n\n\n\n");
        Console.Write("Created by James Curzon, 2025\n");
        Console.SetCursorPosition(0, 4);
    }

    public void DisplaySub()
    {

    }

    public void MenuInteraction()
    {
        Char[] chars = { 'c', 's', 'u', 'q' };
        menuInp = val.CharCheck(chars, true);

        // opening subsection based on user input
        
        switch (menuInp)
        {
            case 'c':
                String[] subMenu = _sections[0].dropDown;

                break;
            case 's':
                break;
            case 'u':
                break;
            case 'q':
                break;
            default:
                break;
        }    
    }
}