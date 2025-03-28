using System;
using System.Collections.Generic;

namespace TerminalSolitaire
{
    public class SolitaireGame
    {
        // https://defbnszqe1hwm.cloudfront.net/images/Solitaire-play-are-set-up-2.png
        private List<Stack<Card>> tableau;
        private Stack<Card> stockpile;
        private List<Stack<Card>> foundations;

        // Which gametype it is
        private bool drawThree;

        // Track Position in interface
        private int selectedColumn = 0;
        private GameSection selectedSection = GameSection.Tableau; // Start at the tableau

        public SolitaireGame(bool drawThree)
        {
            this.drawThree = drawThree;
            InitializeGame();
        }

        // TODO: Actual Initialisation logic

        private void InitializeGame()
        {
            tableau = new List<Stack<Card>> { new Stack<Card>(), new Stack<Card>(), new Stack<Card>(), new Stack<Card>(), new Stack<Card>(), new Stack<Card>(), new Stack<Card>() }; // Example setup
            stockpile = new Stack<Card>();
            foundations = new List<Stack<Card>> { new Stack<Card>(), new Stack<Card>(), new Stack<Card>(), new Stack<Card>() };

            // TODO: Establish actual selection
            tableau[0].Push(new Card("7", "Hearts"));
            tableau[1].Push(new Card("5", "Spades"));
            tableau[2].Push(new Card("K", "Diamonds"));
        }

        public void Run()
        {
            ConsoleKeyInfo key;
            do
            {
                RenderGame();
                key = Console.ReadKey(true);
                HandleInput(key);
            } while (key.Key != ConsoleKey.Escape);
        }

        private void HandleInput(ConsoleKeyInfo key)
        {
            switch (key.Key)
            {
                case ConsoleKey.LeftArrow:
                    if (selectedSection == GameSection.Tableau)
                    {
                        selectedColumn = (selectedColumn == 0) ? tableau.Count - 1 : selectedColumn - 1;
                    }
                    else if (selectedSection == GameSection.Foundation)
                    {
                        selectedColumn = (selectedColumn == 0) ? foundations.Count - 1 : selectedColumn - 1;
                    }
                    // There is only one Stockpile card, we don't need to move laterally.
                    break;
                case ConsoleKey.RightArrow:
                    if (selectedSection == GameSection.Tableau)
                    {
                        selectedColumn = (selectedColumn == tableau.Count - 1) ? 0 : selectedColumn + 1;
                    }
                    else if (selectedSection == GameSection.Foundation)
                    {
                        selectedColumn = (selectedColumn == foundations.Count - 1) ? 0 : selectedColumn + 1;
                    }
                    // There is only one Stockpile card, we don't need to move laterally.
                    break;
                case ConsoleKey.UpArrow:
                    // Move UP: Tableau -> Stockpile -> Foundation -> (Loop back to Tableau)
                    if (selectedSection == GameSection.Tableau)
                    {
                        selectedSection = GameSection.Stockpile;
                    }
                    else if (selectedSection == GameSection.Stockpile)
                    {
                        selectedSection = GameSection.Foundation;
                        selectedColumn = 0; // Reset to first foundation pile
                    }
                    else
                    {
                        selectedSection = GameSection.Tableau;
                        selectedColumn = 0; // Reset to first tableau column
                    }
                    break;
                case ConsoleKey.DownArrow:// Move DOWN: Tableau -> Foundation -> Stockpile -> (Loop back to Tableau)
                    if (selectedSection == GameSection.Tableau)
                    {
                        selectedSection = GameSection.Foundation;
                        selectedColumn = 0; // Reset to first foundation pile
                    }
                    else if (selectedSection == GameSection.Foundation)
                    {
                        selectedSection = GameSection.Stockpile;
                    }
                    else
                    {
                        selectedSection = GameSection.Tableau;
                        selectedColumn = 0; // Reset to first tableau column
                    }
                    break;
                // TODO: up and down should jump between the tableau, deck, and foundation
                // TODO: Loop input selection, when column is max then overflow to 1
                case ConsoleKey.Enter:
                    AttemptAutoMove();
                    break;
            }
        }

        private void AttemptAutoMove()
        {
            if (tableau[selectedColumn].Count == 0) return;
            Card selectedCard = tableau[selectedColumn].Peek();

            foreach (var column in tableau)
            {
                if (column != tableau[selectedColumn] && CanMoveToTableau(selectedCard, column))
                {
                    column.Push(tableau[selectedColumn].Pop());
                    return;
                }
            }

            foreach (var foundation in foundations)
            {
                if (CanMoveToFoundation(selectedCard, foundation))
                {
                    foundation.Push(tableau[selectedColumn].Pop());
                    return;
                }
            }
        }

        private bool CanMoveToTableau(Card card, Stack<Card> column)
        {
            if (column.Count == 0) return card.Rank == "K"; // Only Kings start empty columns
            Card topCard = column.Peek();
            return IsOppositeColor(card, topCard) && GetCardValue(card.Rank) == GetCardValue(topCard.Rank) - 1;
        }

        private bool CanMoveToFoundation(Card card, Stack<Card> foundation)
        {
            if (foundation.Count == 0) return card.Rank == "A";
            Card topCard = foundation.Peek();
            return card.Suit == topCard.Suit && GetCardValue(card.Rank) == GetCardValue(topCard.Rank) + 1;
        }

        // Display the game itself
        // TODO: Actually display the deck and whatnot
        private void RenderGame()
        {
            Console.Clear();
            Console.WriteLine("--- Solitaire ---");
            for (int i = 0; i < tableau.Count; i++)
            {
                Console.Write(i == selectedColumn ? "> " : "  ");
                if (tableau[i].Count > 0)
                {
                    tableau[i].Peek().PrintCard();
                }
                else
                {
                    Console.Write("[Empty]"); // Print the card
                }
                //Console.WriteLine(tableau[i].Count > 0 ? tableau[i].Peek().ToString() : "[Empty]");
            }
            Console.WriteLine("Press Left/Right to change choice, Enter to select a card, Esc to exit.");
        }

        // Assigns a numerical value to non-numerical cards for sorting purposes
        private static int GetCardValue(string rank)
        {
            return rank switch { "A" => 1, "J" => 11, "Q" => 12, "K" => 13, _ => int.Parse(rank) };
        }

        // Ensures that only alternating colours are valid in the deck
        private static bool IsOppositeColor(Card c1, Card c2)
        {
            bool isRed = c1.Suit == "Hearts" || c1.Suit == "Diamonds";
            bool isBlack = c2.Suit == "Clubs" || c2.Suit == "Spades";
            return isRed != isBlack;
        }
    }

    // Enum to define the sections
    public enum GameSection
    {
        Tableau,
        Stockpile,
        Foundation
    }
}