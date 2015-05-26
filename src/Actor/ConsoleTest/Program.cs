using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utility;
using System.Windows.Forms;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var gs = new GameScanner();

            while (false)
            {
                gs.Refresh();
                //var p0Hand = gs.PlayerHand(0);
                //Console.WriteLine(string.Join(", ", p0Hand.Select(pc => GameScanner.GetCardId(pc))));

                //var p1Hand = gs.PlayerHand(1);
                //Console.WriteLine(string.Join(", ", p1Hand.Select(pc => GameScanner.GetCardId(pc))));

                //var board = gs.Board();
                //Console.WriteLine(string.Join(", ", board.Select(pc => string.Join(" ",
                //    GameScanner.GetPlayer(pc),
                //    GameScanner.GetCardId(pc)))));

                Console.WriteLine("Cursor X: " + gs.CursorX());
                Console.WriteLine("Cursor Y: " + gs.CursorY());

                Console.WriteLine("Board X: " + gs.BoardX());
                Console.WriteLine("Board Y: " + gs.BoardY());

                Console.WriteLine("Board offset X: " + gs.GetBoardOffsetX());
                Console.WriteLine("Board offset Y: " + gs.GetBoardOffsetY());

                Console.WriteLine("Player {0} Turn", gs.GetTurnPlayerId());
                Console.WriteLine("Timer: " + gs.GetTimer());

                Console.ReadLine();
            }

            var gi = new GameInput();
            gs.Refresh();
            //gi.StartGame(4);
            Console.WriteLine(gi.PlayCard(0, 7));
            Console.WriteLine("Board offset X: " + gs.GetBoardOffsetX());
            Console.WriteLine("Board offset Y: " + gs.GetBoardOffsetY());
            //Console.ReadLine();
        }
    }
}
