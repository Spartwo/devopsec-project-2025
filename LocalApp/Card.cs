using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerminalSolitaire
{
    class Card
    {
        // Card Data Model, holds the Suit(spades, hearts, clubs, diamonds) and Rank(1-10 etc)
        public string Rank { get; }
        public string Suit { get; }

        // Set is declared on initialisation and never again
        public Card(string rank, string suit)
        {
            Rank = rank;
            Suit = suit;
        }

        // Retrieve the cards value in a visually nice way
        public override string ToString()
        {
            // Unicode symbols for suits
            string suitSymbol = Suit switch
            {
                "Hearts" => "♥",
                "Diamonds" => "♦",
                "Clubs" => "♣",
                "Spades" => "♠",
                _ => "?"
            };

            string cardString = $"[{Rank}{suitSymbol}]";
            Console.ResetColor();

            return cardString;
        }

        // Prints the card in color
        public void PrintCard()
        {
            // Set the color for red suits
            if (Suit == "Hearts" || Suit == "Diamonds")
                Console.ForegroundColor = ConsoleColor.Red;
            else
                Console.ForegroundColor = ConsoleColor.White;

            Console.Write(ToString() + " "); // Print the card
            Console.ResetColor(); // Reset the console color after printing
        }
    }
}
