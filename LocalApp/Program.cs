using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerminalSolitaire;

class Program
{
    // Initialisation Class, starts the game. 
    // In future handles selection if other terminal games are added
    static void Main()
    {
        RenderMenu();
    }

    static void RenderMenu()
    {
        Console.Clear();
        // Get the width of the console window
        int consoleWidth = 60;

        // Define the text for the header
        string headerText = "Welcome to Console Games";

        // Calculate the number of spaces to prepend for centering
        int padding = (consoleWidth - headerText.Length) / 2;

        // Print the centered header
        Console.WriteLine($"{new string('=', padding)}{headerText}{new string('=', padding)}");
        Console.WriteLine("Menu Controls:");
        Console.WriteLine("  ▲/▼\t Move between options");
        Console.WriteLine("  ENTER\t Select a game");
        // Print the final line (separator)
        Console.WriteLine(new string('=', consoleWidth - 1));

        string[] options = { "One-Card Solitaire", "Three-Card Solitaire", "Exit" };
        int selectedIndex = 0;

        while (true)
        {
            for (int i = 0; i < options.Length; i++)
            {
                if (i == selectedIndex)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine($"> {options[i]}");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine($"  {options[i]}");
                }
            }

            ConsoleKeyInfo key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.UpArrow)
                selectedIndex = (selectedIndex == 0) ? options.Length - 1 : selectedIndex - 1;
            else if (key.Key == ConsoleKey.DownArrow)
                selectedIndex = (selectedIndex == options.Length - 1) ? 0 : selectedIndex + 1;
            else if (key.Key == ConsoleKey.Enter)
                break;

            Console.SetCursorPosition(0, Console.CursorTop - options.Length); // Reset cursor for redraw
        }

        Console.Clear();

        // Map menu selections to corresponding actions
        var actions = new Dictionary<string, Action>
        {
            { "One-Card Solitaire", () => StartSolitaire(false) },
            { "Three-Card Solitaire", () => StartSolitaire(true) },
            { "Exit", ExitGame }
        };

        actions[options[selectedIndex]].Invoke();

    }

    static void StartSolitaire(bool drawThree)
    {
        SolitaireGame game = new SolitaireGame(drawThree);
        game.Run();
        RenderMenu();
    }

    static void ExitGame()
    {
        Console.WriteLine("\nThanks for playing!");
        //Environment.Exit(0);
    }
}
