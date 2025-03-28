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
        Console.Clear();
        Console.WriteLine("=== Welcome to Console Games ===\n");
        Console.WriteLine("Use UP/DOWN arrows to select and ENTER to start.\n");

        string[] options = { "1 Card Solitaire", "3 Card Solitaire", "Exit" };
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
            { "1 Card Solitaire", () => StartSolitaire(false) },
            { "3 Card Solitaire", () => StartSolitaire(true) },
            { "Exit", ExitGame }
        };

        actions[options[selectedIndex]].Invoke();

        static void StartSolitaire(bool drawThree)
        {
            SolitaireGame game = new SolitaireGame(drawThree);
            game.Run();
        }

        static void ExitGame()
        {
            Console.WriteLine("\nThanks for playing!");
            Environment.Exit(0);
        }
    }
}
