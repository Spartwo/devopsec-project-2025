using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class HighScores
{
    public HighScores()
    {

    }
    public void Run()
    {
        ConsoleKeyInfo key;
        do
        {
            RenderScores();
            key = Console.ReadKey(true);
        } while (key.Key != ConsoleKey.Escape);
    }

    private void RenderScores()
    {

    }
}
