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
        // Fixed width for each column
        const int columnWidth = 2; 
        const int cardWidth = 3;

        // Track Position in interface
        private int selectedColumn = 0;
        private GameSection selectedSection = GameSection.Tableau; // Start at the tableau

        public SolitaireGame(bool drawThree)
        {
            this.drawThree = drawThree;
            InitializeGame();
        }

        private void InitializeGame()
        {
            tableau = new List<Stack<Card>> {};
            stockpile = new Stack<Card>();
            // Foundation starts empty
            foundations = new List<Stack<Card>> { new Stack<Card>(), new Stack<Card>(), new Stack<Card>(), new Stack<Card>() };

            // Get a shuffled deck
            Stack<Card> deck = GenerateShuffledDeck();
            
            // Initialize tableau with 7 columns
            tableau = new List<Stack<Card>>();
            for (int i = 0; i < 7; i++)
                tableau.Add(new Stack<Card>());

            // Distribute cards with only the top one face-up
            for (int col = 0; col < 7; col++)
            {
                for (int row = 0; row <= col; row++)
                {
                    Card card = deck.Pop();
                    if (row == col)
                        card.IsFaceUp = true; // Only top card is face-up
                    tableau[col].Push(card);
                }
            }

            // Remaining cards go to the stockpile, they can all be revealed
            foreach (Card card in deck)
            {
                card.IsFaceUp = true;
            }
            stockpile = deck;

        }
        private Stack<Card> GenerateShuffledDeck()
        {
            string[] suits = { "Hearts", "Diamonds", "Clubs", "Spades" };
            string[] ranks = { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };

            List<Card> deck = new List<Card>();

            foreach (string suit in suits)
                foreach (string rank in ranks)
                    deck.Add(new Card(rank, suit));

            // Shuffle deck
            Random rng = new Random();
            deck = deck.OrderBy(c => rng.Next()).ToList();

            return new Stack<Card>(deck);
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
                    if (selectedSection == GameSection.Tableau)
                    {
                        selectedSection = GameSection.Stockpile;
                        selectedColumn = 0; // Reset to first tableau column
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
                case ConsoleKey.DownArrow: 
                    if (selectedSection == GameSection.Tableau)
                    {
                        selectedSection = GameSection.Foundation;
                        selectedColumn = 0; // Reset to first foundation pile
                    }
                    else if (selectedSection == GameSection.Foundation)
                    {
                        selectedSection = GameSection.Stockpile;
                        selectedColumn = 0; // Reset to first tableau column
                    }
                    else
                    {
                        selectedSection = GameSection.Tableau;
                        selectedColumn = 0; // Reset to first tableau column
                    }
                    break;
                case ConsoleKey.Spacebar:
                    CycleStockPile();
                    break;
                case ConsoleKey.Enter:
                    AttemptAutoMove();
                    break;
            }
        }

        #region Movement Handlers
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

        private void CycleStockPile()
        {
            int cardsToDraw = drawThree ? 3 : 1; // Determine how many cards to cycle

            if (stockpile.Count == 0) return; // No cards to cycle

            List<Card> movedCards = new List<Card>();

            // Move cards from the top to a temporary list
            for (int i = 0; i < cardsToDraw && stockpile.Count > 0; i++)
            {
                movedCards.Add(stockpile.Pop());
            }

            // Reinsert moved cards at the bottom in the same order
            movedCards.Reverse();
            foreach (Card card in movedCards)
            {
                stockpile = new Stack<Card>(stockpile.Reverse().Append(card));
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
        #endregion

        #region Display
        // Display the game itself
        private void RenderGame()
        {
            Console.Clear();
            // Get the width of the console window
            int consoleWidth = 60;

            // Define the text for the header
            string headerText = $"{(drawThree ? "Three-Card-Draw Solitaire" : "One-Card-Draw Solitaire")}";

            // Calculate the number of spaces to prepend for centering
            int padding = (consoleWidth - headerText.Length) / 2;

            // Print the centered header
            Console.WriteLine($"{new string('=', padding)}{headerText}{new string('=', padding)}");
            // Print the control instructions
            Console.WriteLine("Game Controls:\n"
                + "  ▲/▼\t Move between columns, foundations, and stockpile\n"
                + "  ◄/►\t Move left or right within the selected section\n"
                + "  ENTER\t Select a card to move it to another pile\n"
                + "  SPACE\t Cycle the stockpile\n"
                + "  ESC\t Return to the main menu");
            // Print the final line (separator)
            Console.WriteLine(new string('=', consoleWidth - 1));

            RenderDeck();
            Console.WriteLine();
            RenderTableau();
        }

        private void RenderTableau()
        {
            int maxHeight = tableau.Max(stack => stack.Count); // Find the tallest stack

            for (int row = 0; row < maxHeight; row++)
            {
                for (int col = 0; col < tableau.Count; col++)
                {
                    if (row < tableau[col].Count) // Only print if this row exists
                    {
                        Card[] columnCards = tableau[col].Reverse().ToArray(); // Reverse to get bottom-to-top order
                        Card card = columnCards[row];

                        Console.Write(col == selectedColumn && row == tableau[col].Count - 1 ? "> " : "  ");
                        PrintCard(card);
                    }
                    else
                    {
                        PrintEmpty();
                    }
                    PrintSpace();
                }
                Console.WriteLine();
            }
        }

        private void RenderDeck()
        {
            RenderFoundation();
            RenderStockpile();

            // Break Line for Tableau
            Console.Write("\n");
        }

        private void RenderFoundation()
        {
            // Print Foundations
            for (int i = 0; i < foundations.Count; i++)
            {
                Console.Write(i == selectedColumn ? "> " : "  ");
                if (foundations[i].Count > 0)
                {
                    PrintCard(foundations[i].Peek()); // Show top card of foundation
                }
                else
                {
                    PrintEmpty();
                }
                PrintSpace();
            }
        }

        private void RenderStockpile()
        {
            int cardsToShow = drawThree ? Math.Min(3, stockpile.Count) : 1;
            Stack<Card> tempStack = new Stack<Card>();

            // Extract top X cards for display
            for (int i = 0; i < cardsToShow; i++)
            {
                Card card = stockpile.Pop();
                tempStack.Push(card);
            }

            // Print empty spaces for missing cards to maintain alignment
            for (int i = 0; i < 3 - cardsToShow; i++)
            {
                PrintEmpty();
                PrintSpace();
            }

            // Print extracted cards (right-aligned)
            while (tempStack.Count > 0)
            {
                PrintCard(tempStack.Pop());
                PrintSpace();
            }

            // Push the extracted cards back in the same order
            while (tempStack.Count > 0)
            {
                stockpile.Push(tempStack.Pop());
            }
        }
        #endregion


        #region Card Handling

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

        // Prints the card in color
        private static void PrintCard(Card c)
        {
            string cardValue = c.ToString();
            if (c.IsFaceUp)
            {
                // Set the color for red suits
                if (c.Suit == "Hearts" || c.Suit == "Diamonds")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.BackgroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.White;
                }
            } 
            else
            {
                Console.ForegroundColor = ConsoleColor.Gray; //Grey
                Console.BackgroundColor = ConsoleColor.White;
            }

            Console.Write((c.ToString()).PadRight(cardWidth));
            Console.ResetColor(); // Reset the console color after printing
        }

        // Insert a designated spacer instead of tab
        private static void PrintEmpty()
        {
            Console.BackgroundColor = ConsoleColor.DarkGray;
            //Console.Write(("").PadRight(cardWidth));
            Console.Write(new string(' ', cardWidth)); // Ensures uniform spacing
            Console.ResetColor(); // Reset the console color after printing
        }

        private static void PrintSpace()
        {
            Console.Write(new string(' ', columnWidth));
        }
        #endregion
    }

    // Enum to define the sections
    public enum GameSection
    {
        Tableau,
        Stockpile,
        Foundation
    }
}