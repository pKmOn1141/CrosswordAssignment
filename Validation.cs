using System.Linq;

namespace Validation;

public class Validator
{
    public Validator()
    {
        
    }

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
}