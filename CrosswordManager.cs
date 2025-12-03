using FileManager;
using System.Data;
using System.Text.RegularExpressions;
using Validation;

namespace CrosswordManager
{
    class CrosswordScreens
    {
        private Validator _val = new Validator();
        private String _cwdFolder;
        private FileHandler _fileH = new FileHandler();

        public CrosswordScreens(String cF)
        {
            _cwdFolder = cF;
        }

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

        // Loading either for creation or playing
        // type = 0 loads creation, type = 1 loads play
        public void LoadCrosswordMenu(int menuType)
        {
            Console.Clear();

            String title = "Load Crossword";
            Console.SetCursorPosition((Console.WindowWidth / 2) - title.Length / 2, 1);
            Console.Write(title);

            int[] pos = { Console.WindowWidth / 3, 4 };
            Console.SetCursorPosition(pos[0], pos[1]);
            Console.Write("Title:");

            bool fail = true;
            string crossName = "";
            while (fail)
            {
                Console.SetCursorPosition(pos[0] + 10, pos[1]);
                crossName = Console.ReadLine();
                fail = _val.BlankCheck([crossName]);
            }

            // Try to find the crossword
            string fileName = Regex.Replace(crossName, @"[^a-zA-Z0-9]", "").ToLower();
            fileName = fileName + ".json";
            string filePath = Path.Combine(_cwdFolder, fileName);

            // If it does exist
            if (File.Exists(filePath))
            {
                (Crossword loadedCwd, bool success) = _fileH.DeSerialiseCwd(filePath);
                if (success)
                {
                    string succText = "Crossword found and loaded. Press any key to continue";
                    Console.SetCursorPosition((Console.WindowWidth / 2) - succText.Length / 2, 6);
                    Console.WriteLine(succText);
                    Console.ReadKey();

                    // Open the menu
                    if (menuType == 0)
                    {
                        CreationMenu(loadedCwd);
                        return;
                    }
                    else if (menuType == 1)
                    {
                        PlayMenu(loadedCwd);
                        return;
                    }
                }
            }

            // If failure somewhere
            string failText = "The crossword couldn't be found or loaded, try again. Press any key to continue.";
            Console.SetCursorPosition((Console.WindowWidth / 2) - failText.Length / 2, 6);
            Console.Write(failText);
            Console.ReadKey();
            Console.Clear();
            return;
        }

        public void PlayMenu(Crossword cwd)
        {
            Console.Clear();
            int[] crossPos = [Console.WindowWidth / 3, 1];

            // Get necessary information
            int wordAmount = cwd.wordAmount;
            int currentWord = 0;
            Clue currentClue = cwd.wordClue[currentWord];

            // Display
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.SetCursorPosition(0, 20);
            Console.Write("Use ARROW KEYS to move between clues, press ENTER then TYPE to enter word, press ENTER to submit guess, press ESC to exit.");
            Console.ForegroundColor = ConsoleColor.White;

            bool moveOn = false;
            bool changed = false;
            int[] empty = [0, 0];
            // Different val for what the user does: -2 = right arrow, -1 = left arrow,
            int userFunc = -99;
            PrintGrid(cwd, crossPos, 1);
            Console.SetCursorPosition(0, 16);
            Console.WriteLine("Word: " + (currentWord + 1).ToString() + " Clue: " + currentClue.clue);
            Console.Write("Enter word: ");
            while (!moveOn)
            {
                // If the screen needs to be redrawn
                if (changed)
                {
                    // Override the previous grid
                    PrintGrid(cwd, crossPos, 1);
                    ChangePlayGrid(cwd, crossPos, currentClue, 0);


                    Console.SetCursorPosition(0, 16);
                    Console.WriteLine("Word: " + (currentWord + 1).ToString() + " Clue: " + currentClue.clue);
                    Console.Write("Enter word: ");

                }

                // Retrieve the users input
                (empty, userFunc) = _val.ArrowCheck(empty, 0, 0, 1);
                
                switch (userFunc)
                {
                    // Right arrow
                    case -2:
                        currentWord += 1;
                        if (currentWord > wordAmount - 1)
                        {
                            currentWord = 0;
                        }
                        currentClue = cwd.wordClue[currentWord];
                        changed = true;
                        break;

                    // Left arrow
                    case -1:
                        currentWord -= 1;
                        if (currentWord < 0)
                        {
                            currentWord = wordAmount - 1;
                        }
                        currentClue = cwd.wordClue[currentWord];
                        changed = true;
                        break;

                    // Enter
                    case 1:
                        Console.SetCursorPosition(13, 17);
                        string guess = Console.ReadLine().ToUpper();
                        // If the guess isnt blank
                        if (!_val.BlankCheck([guess]))
                        {
                            // Checks if the answer is correct
                            string answer = currentClue.word;
                            if (guess == answer)
                            {
                                CorrectWord(cwd, currentClue);
                                changed = true;
                            }
                            else
                            {
                                Console.SetCursorPosition(0, 18);
                                Console.WriteLine("Incorrect, try again. Press any key.");
                                Console.ReadKey(true);
                                ClearCurrentLine([0, 18]);
                            }
                        }
                        ClearCurrentLine([0,17]);
                        break;

                    // Esc
                    case 2:
                        ClearCurrentLine([0, 16]);
                        ClearCurrentLine([0, 17]);
                        Console.SetCursorPosition(0, 16);
                        Console.Write("Exiting, press any key to exit.");
                        Console.ReadKey(true);
                        Console.Clear();
                        return;
                }
            }
        }

        public void ChangePlayGrid(Crossword cwd, int[] crossPos, Clue currentClue, int col)
        {
            // Find positions to start highlighting
            int startRow = currentClue.startPos[0];
            int startCol = currentClue.startPos[1];

            Console.ForegroundColor = ConsoleColor.Red;

            for (int i = 0; i < currentClue.word.Length; i++)
            {
                if (currentClue.direction == 'd')
                {
                    int[] screenPos = CalcCellPos(crossPos, startRow + i, startCol);
                    Console.SetCursorPosition(screenPos[0], screenPos[1]);
                    Console.Write(cwd.guessGrid[startRow + i, startCol]);
                }
                else if (currentClue.direction == 'a')
                {
                    int[] screenPos = CalcCellPos(crossPos, startRow, startCol + i);
                    Console.SetCursorPosition(screenPos[0], screenPos[1]);
                    Console.Write(cwd.guessGrid[startRow, startCol + i]);
                }
            }

            // Make it white when done
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void CorrectWord(Crossword cwd, Clue clue)
        {
            for (int i = 0; i < clue.word.Length; i++)
            {
                string letter = clue.word[i].ToString() + " ";
                if (clue.direction == 'd')
                {
                    // Set row and columns
                    int r = clue.startPos[0] + i;
                    int c = clue.startPos[1];
                    cwd.guessGrid[r, c] = letter;
                }
                else if (clue.direction == 'a')
                {
                    int r = clue.startPos[0];
                    int c = clue.startPos[1] + i;
                    cwd.guessGrid[r, c] = letter;
                }
            }
        }

        public void PrintTabs()
        {
            // Displaying the text onto the console
            Console.Clear();
            String header = "New Crossword";
            Console.SetCursorPosition(((Console.WindowWidth) / 2) - header.Length / 2, 1);
            Console.Write(header);
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
            String instruct = "Use Arrow Keys to select a cell to start a word, then press Enter. Press ESC to save and exit.";

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
            PrintGrid(cwd, cwdPos, 0);
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
        // vers = 0, creation - vers = 1, play
        public void PrintGrid(Crossword c, int[] linePos, int vers)
        {
            int rw = c.rows;
            int col = c.columns;
            string[,] grid;
            if (vers == 0)
            {
                grid = c.cells;
            }
            else
            {
                grid = c.guessGrid;
            }

            int line = linePos[1];
            Console.SetCursorPosition(linePos[0], linePos[1]);
            // Normal printgrid
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
            int[] previousCell = [0, 0];
            int altFunc = 0;
            bool moveOn = false;
            SelectedCell(originPos, currentCell, cwd);

            while (!moveOn)
            {
                previousCell = currentCell;
                (currentCell, altFunc) = _val.ArrowCheck(currentCell, cwd.rows, cwd.columns, 0);

                if (altFunc == 0)
                {
                    // If changed
                    if (previousCell[0] != currentCell[0] || previousCell[1] != currentCell[1])
                    {
                        ShowCurrentCellStat(currentCell);

                        int[] prevLoc = CalcCellPos(originPos, previousCell[0], previousCell[1]);
                        int[] currLoc = CalcCellPos(originPos, currentCell[0], currentCell[1]);

                        ResetCell(prevLoc, previousCell, cwd);
                        SelectedCell(currLoc, currentCell, cwd);
                    }
                }
                // If user wants to add a word
                else if (altFunc == 1)
                {
                    AddWord(cwd, currentCell);
                    PrintGrid(cwd, [Console.WindowWidth / 5, 3], 0);
                    Console.SetCursorPosition(originPos[0], originPos[1]);
                    currentCell = [0, 0];
                }
                // User wants to exit
                else if (altFunc == 2)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.SetCursorPosition(0, 18);
                    Console.WriteLine("Proceed with saving and exit? Press enter to continue, any other key to go back");
                    ConsoleKeyInfo input = Console.ReadKey(true);
                    if (input.Key == ConsoleKey.Enter)
                    {
                        moveOn = true;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Clear();

                        // Serialise the crossword to json
                        cwd.SaveCrossword(_cwdFolder);
                        return;
                    }
                    else
                    {
                        ClearCurrentLine([0, 18]);
                    }
                }
            }
        }

        // Calculuating the position of the cell based off the screen coords and the array position
        public int[] CalcCellPos(int[] originPos, int row, int col)
        {
            return new int[] { originPos[0] + (col * 2), originPos[1] + row };
        }

        // The user is adding a word
        public void AddWord(Crossword cwd, int[] currentCell)
        {
            Console.ForegroundColor = ConsoleColor.White;
            char direction;
            string word = "";
            string clue = ""; // Initialize clue

            Char[] validChoice = new Char[2];
            validChoice = ['d', 'a'];

            // Enter the word
            Console.SetCursorPosition(0, 18);
            Console.Write("Enter a word:");
            bool invalid = true;
            while(invalid)
            {
                Console.SetCursorPosition(0, 19);
                word = Console.ReadLine().ToUpper();
                invalid = _val.BlankCheck([word]);
            }

            // Clear the line
            ClearCurrentLine([0, 19]);

            // Enter the direction
            Console.SetCursorPosition(0, 18);
            Console.WriteLine("Which direction, DOWN (d) or ACROSS (a)");
            direction = _val.CharCheck(validChoice, true);
            Console.Write(direction);

            // If the word can fit
            if (LengthCheck(word, direction, cwd, currentCell))
            {
                ClearCurrentLine([0, 19]);
                Console.SetCursorPosition(0, 18);
                Console.WriteLine("Type the clue for the word, press enter when completed:");
                invalid = true;
                while(invalid)
                {
                    Console.SetCursorPosition(0, 19);
                    clue = Console.ReadLine();
                    invalid = _val.BlankCheck([clue]);
                }
                // Word is saved
                ClearCurrentLine([0, 19]);
                Console.SetCursorPosition(0, 18);

                // Add the word to the crossword object
                cwd.AddWordClue(word, clue, direction, currentCell);
                cwd.UpdateGrid(currentCell, word, direction);
                ClearCurrentLine([0, 18]);
                Console.WriteLine("Word is saved. Press ANY key to continue.");
                Console.ReadKey(true);
                ClearCurrentLine([0, 18]);
                return;
            }
            // If the word doesnt fit
            else
            {
                ClearCurrentLine([0, 18]);
                ClearCurrentLine([0, 19]);
                Console.SetCursorPosition(0, 18);
                Console.Write("Word cannot fit in the grid in this position. Press any key to continue.");
                Console.ReadKey(true);
                ClearCurrentLine([0, 18]);
                return;
            }

            
            
        }

        // Checks if the inputted word can fit in the grid
        public bool LengthCheck(string word, char direc, Crossword cwd, int[] currCell)
        {
            int wordLength = word.Length;
            int cell;
            switch (direc)
            {
                // Down
                case 'd':
                    cell = currCell[0];
                    if (cell + wordLength > cwd.rows)
                    {
                        return false;
                    }
                    break;
                // Across
                case 'a':
                    cell = currCell[1];
                    if (cell + wordLength > cwd.columns)
                    {
                        return false;
                    }
                    break;
            }
            return true;
        }

        // Shows the stats of the currently selected cell
        public void ShowCurrentCellStat(int[] currentCell)
        {
            // Clears the line
            for (int i = 7; i < 9; i++)
            {
                Console.SetCursorPosition((Console.WindowWidth / 2) + 25, i);
                Console.Write(new String(' ', Console.WindowWidth - ((Console.WindowWidth / 2) + 25)));
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition((Console.WindowWidth / 2) + 25, 7);
            Console.Write(currentCell[0]+1);
            Console.SetCursorPosition((Console.WindowWidth / 2) + 25, 8);
            Console.Write(currentCell[1]+1);
        }

        // Shows the currently highlighted cells
        public void SelectedCell(int[] cellPos, int[]currCell, Crossword cwd)
        {
            Console.SetCursorPosition(cellPos[0], cellPos[1]);
            Console.ForegroundColor = ConsoleColor.Red;
            string toWrite = cwd.cells[currCell[0], currCell[1]];
            Console.Write(toWrite);
            // So the little dash is under the selected cell
            Console.SetCursorPosition(cellPos[0], cellPos[1]);
        }

        // Makes the previous cell white again
        public void ResetCell(int[] cellPos, int[] currCell, Crossword cwd)
        {
            Console.SetCursorPosition(cellPos[0], cellPos[1]);
            Console.ForegroundColor = ConsoleColor.White;
            string toWrite = cwd.cells[currCell[0], currCell[1]];
            Console.Write(toWrite);
        }

        // Clears the inputted line
        public void ClearCurrentLine(int[] line)
        {
            (int one, int two) = Console.GetCursorPosition();
            Console.SetCursorPosition(line[0], line[1]);
            Console.Write(new String(' ', Console.BufferWidth));
            Console.SetCursorPosition(one, two);

        }
    }

    class Crossword
    {
        public String title;
        public int rows;
        public int columns;
        public string[,] cells;
        public string[,] guessGrid;
        public int wordAmount = 0;
        // Holds a number for each word, then the information for each as an object
        public Dictionary<int, Clue> wordClue = new Dictionary<int, Clue>();
        private FileHandler fileH = new FileHandler();

        public Crossword(String t, int r, int c)
        {
            title = t;
            rows = r;
            columns = c;
            cells = new string[rows, columns];
            guessGrid = new string[rows, columns];
            // Fill the grids
            cells = FillGrid(cells);
            guessGrid = FillGrid(guessGrid);

        }

        // Fill the grid with astericks
        public string[,] FillGrid(string[,] grid)
        {
            for (int rw = 0; rw < rows; rw++)
            {
                for (int cl = 0; cl < columns; cl++)
                {
                    grid[rw, cl] = "*" + " ";
                }
            }
            return grid;
        }

        //Checks if the grid is 'empty' or not
        public bool CheckEmptyGrid(int[] pos)
        {
            // The cell is 'empty'
            if (cells[pos[0], pos[1]] != "* ")
            {
                return true;
            }
            // Has a letter in it
            else
            {
                return false;
            }
        }

        // Add a word/clue to the dictionary
        public void AddWordClue(string word, string clue, char direction, int[] pos)
        {
            word = word.ToUpper();
            wordClue.Add(wordAmount, new Clue(word, clue, direction, pos));
            wordAmount += 1;
        }

        // Updates the grid for a new word
        public void UpdateGrid(int[] cellPos, string word, char dir)
        {
            string[] wordArray = word.Select(c => c.ToString()).ToArray();
            for (int i = 0; i<word.Length; i++)
            {
                if (dir == 'd')
                {
                    cells[cellPos[0] + i, cellPos[1]] = wordArray[i] + " ";
                }
                else if (dir == 'a')
                {
                    cells[cellPos[0], cellPos[1]+i] = wordArray[i] + " ";
                }
                
            }
        }

        public void SaveCrossword(String cwdFolder)
        {
            string fileName = Regex.Replace(title, @"[^a-zA-Z0-9]", "").ToLower();
            fileName = fileName + ".json";
            string filePath = Path.Combine(cwdFolder, fileName);

            fileH.SerialiseObj(filePath, this);
        }
    }

    // Holds the clue and direction
    class Clue
    {
        public string word;
        public string clue;
        // 0 = across, 1 = down
        public char direction;
        public int[] startPos;

        public Clue(string w, string c, char d, int[] sP)
        {
            word = w;
            clue = c;
            direction = d;
            startPos = sP;
        }
    }
}