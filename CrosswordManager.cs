using System.Data;
using Validation;

namespace CrosswordManager
{
    class CrosswordScreens
    {
        private Validator _val = new Validator();

        // Start of the crossword creation
        public void IniCreationMenu()
        {
            PrintTabs();

            // Taking in the users inputs
            int[] errorPos = [Console.WindowWidth / 3, 11];
            int line = 4;
            bool moveOn = false;
            // Title
            String title = "";
            while (!moveOn)
            {
                Console.SetCursorPosition(((Console.WindowWidth) / 3)+20, line);
                title = Console.ReadLine();
                if (!_val.BlankCheck([title]))
                {
                    moveOn = true;
                    line += 2;
                }
                else
                {
                    PrintTabs();
                    Console.SetCursorPosition(errorPos[0], errorPos[1]);
                    Console.Write("Enter a valid string");
                }
            }
            int linePos = Console.WindowWidth / 3 +20;
            // Rows
            int rows = IntInput(moveOn, 20, line, errorPos);
            line += 2;
            // Columns
            int columns = IntInput(moveOn, 20, line, errorPos);

            Crossword createdCwd = new Crossword(title, rows, columns);
            CreationMenu(createdCwd);

        }

        public void PrintTabs()
        {
            // Displaying the text onto the console
            Console.Clear();
            String header = "New Crossword";
            Console.SetCursorPosition(((Console.WindowWidth) / 2) - header.Length / 2, 2);
            String[] menuTabs = ["Title:", "Num of Rows:", "Num of Columns:"];
            int line = 4;
            foreach (String tabs in menuTabs)
            {
                Console.SetCursorPosition(Console.WindowWidth / 3, line);
                Console.Write(tabs);
                line += 2;
            }
        }

        public int IntInput(bool moveOn, int winPos, int line, int[] errorPos)
        {
            int input = 0;
            moveOn = false;
            while (!moveOn)
            {
                Console.SetCursorPosition(((Console.WindowWidth) / 3) + winPos, line);
                input = _val.IntCheck();
                // If pass validation
                if (input != -99)
                {
                    moveOn = true;
                    line += 2;
                }
                else
                {
                    PrintTabs();
                    Console.SetCursorPosition(errorPos[0], errorPos[1]);
                    Console.Write("Enter a valid number");
                }
            }
            return input;
        }

        // Second part, creates the display and sections
        public void CreationMenu(Crossword cwd)
        {
            Console.Clear();
            int[] labelPos = [(Console.WindowWidth / 2)+10, 3];
            String[] labels = ["Title:  ", "No. of Rows:  ", "No. of Cols:  ", "Current Row:  ",
                "Current Col:  ", "Dir:  ", "Word:  ", "Clue:  "];
            String instruct = "Use Arrow Keys to select a cell to start a word, then press Enter.";

            // Print the labels
            foreach (String label in labels)
            {
                Console.SetCursorPosition(labelPos[0], labelPos[1]);
                Console.Write(label);
                if (labelPos[1] == 3 || labelPos[1] == 8)
                {
                    labelPos[1] += 2;
                }
                else
                {
                    labelPos[1]++;
                }
            }
            Console.SetCursorPosition(0, 23);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(instruct);
            Console.ForegroundColor = ConsoleColor.White;
            PrintStats(cwd);

            // Print the crossword
            int[] cwdPos = [Console.WindowWidth / 5, 3];
            PrintGrid(cwd, cwdPos);
            CrosswordInteraction(cwdPos, cwd);
        }

        // Prints the crossword stats
        public void PrintStats(Crossword cwd)
        {
            Console.SetCursorPosition((Console.WindowWidth / 2) + 25, 3);
            Console.Write(cwd.title);
            Console.SetCursorPosition((Console.WindowWidth / 2) + 25, 5);
            Console.Write(cwd.rows);
            Console.SetCursorPosition((Console.WindowWidth / 2) + 25, 6);
            Console.Write(cwd.columns);
        }

        // Draws the crossword grid
        public void PrintGrid(Crossword c, int[] linePos)
        {
            int rw = c.rows;
            int col = c.columns;
            string[,] grid = c.cells;
            int line = linePos[1];
            Console.SetCursorPosition(linePos[0], linePos[1]);
            for (int r = 0; r < rw; r++)
            {
                for (int cl = 0; cl < col; cl++)
                {
                    Console.Write(grid[r, cl]);
                }
                Console.WriteLine();
                line++;
                Console.SetCursorPosition(linePos[0], line);
            }
        }

        public void CrosswordInteraction(int[] originPos, Crossword cwd)
        {
            int[] currentCell = [0, 0];
            int[] previousCell;
            bool enterPressed = false;
            bool moveOn = false;
            SelectedCell(originPos);

            while (!moveOn)
            {
                previousCell = currentCell;
                (currentCell, enterPressed) = _val.ArrowCheck(currentCell, cwd.rows, cwd.columns);

                if (!enterPressed)
                {
                    if (previousCell != currentCell)
                    {
                        ShowCurrentCell(currentCell);
                        ResetCell([originPos[0] + (previousCell[1] * 2), originPos[1] + previousCell[0]]);
                        SelectedCell([originPos[0] + (currentCell[1] * 2), originPos[1] + currentCell[0]]);
                    }
                }
                // If user wants to add a word
                else
                {
                    AddWord(cwd, currentCell);
                }
                
                
            }
        }

        // The user is adding a word
        public void AddWord(Crossword cwd, int[] currentCell)
        {
            char direction;
            string word = "";
            string clue;

            Char[] validChoice = new Char[2];
            validChoice = ['d', 'a'];

            // Enter the word
            Console.SetCursorPosition(0, 18);
            Console.Write("Enter a word:");
            Console.SetCursorPosition(0, 19);
            bool valid = false;
            while(!valid)
            {
                word = Console.ReadLine();
                valid = _val.BlankCheck([word]);
            }

            // Direction
            Console.SetCursorPosition(0, 18);
            Console.Write("Which direction, DOWN (d) or ACROSS (a)");
            direction = _val.CharCheck(validChoice, true);
            // Check that the word can fit in the direction
            int wordLength = word.Length;
            if (direction == 'd')
            {
                int cell = currentCell[0];
                if (cell + wordLength > cwd.rows)
                {
                    Console.Write("Word is too long");
                }
            }
        }

        // Shows the stats of the currently selected cell
        public void ShowCurrentCell(int[] currentCell)
        {
            // Clears the line
            for (int i = 7; i < 9; i++)
            {
                Console.SetCursorPosition((Console.WindowWidth / 2) + 25, i);
                Console.Write(new String(' ', Console.WindowWidth - ((Console.WindowWidth / 2) + 25)));
            }

            Console.SetCursorPosition((Console.WindowWidth / 2) + 25, 7);
            Console.Write(currentCell[0]+1);
            Console.SetCursorPosition((Console.WindowWidth / 2) + 25, 8);
            Console.Write(currentCell[1]+1);
        }

        // Shows the currently highlighted cells
        public void SelectedCell(int[] cellPos)
        {
            Console.SetCursorPosition(cellPos[0], cellPos[1]);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("*"+" ");
            // So the little dash is under the selected cell
            Console.SetCursorPosition(cellPos[0], cellPos[1]);
        }

        // Makes the previous cell white again
        public void ResetCell(int[] cellPos)
        {
            Console.SetCursorPosition(cellPos[0], cellPos[1]);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("*" + " ");
        }
    }

    class Crossword
    {
        public String title;
        public int rows;
        public int columns;
        public string[,] cells;

        public Crossword(String t, int r, int c)
        {
            title = t;
            rows = r;
            columns = c;
            cells = new string[rows, columns];
            // Fill the grid with astericks
            for (int rw=0; rw < rows; rw++)
            {
                for (int cl=0; cl < columns; cl++)
                {
                    cells[rw, cl] = "*"+" ";
                }
            }

        }
    }
}