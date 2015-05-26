using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Utility
{
    public class GameInput
    {
        private Process _process;
        private GameScanner _gs;

        public GameInput()
        {
            this._gs = new GameScanner();
            this._process = this._gs.MemoryAPI().Process;

        }

        public bool SendKey(Keys key)
        {
            return WindowHook.SendKeystroke(this._process.MainWindowHandle, (ushort)key);
        }

        public bool StartGame(int deck)
        {
            this._gs.Refresh();

            // Game already started
            if (this._gs.BoardOpen())
            {
                return false;
            }

            // Target Person
            Console.WriteLine("Hitting 0 to target nearest thing");
            HitConfirm();
            Thread.Sleep(400);

            // Interact
            Console.WriteLine("Hitting 0 to interact");
            HitConfirm();

            this._gs.Refresh();
            while(this._gs.ActiveMenuSelection() != 0)
            {
                Thread.Sleep(50);
                this._gs.Refresh();
            }

            // Select First thing (Triple Triad Match)
            Console.WriteLine("Hitting 0 to select play Triple Triad");
            HitConfirm();
            Thread.Sleep(700);

            Console.WriteLine("Hitting 0 to select Advance Chat Menu");
            // Talk Menu
            HitConfirm();
            Thread.Sleep(1600);


            // Register For Match
            RegisterForMatch(deck);


            return true;
        }

        public bool RegisterForMatch(int deck)
        {
            // Accept Match
            Console.WriteLine("Hitting 0 to accept match");
            HitConfirm();
            Thread.Sleep(1000);

            // Select Deck
            SelectDeck(deck);

            return true;
        }

        private bool SelectDeck(int deck)
        {
            for(int i = 0; i < deck; i++)
            {
                Console.WriteLine("Hitting down");
                MoveDown();
                Thread.Sleep(80);
            }

            Console.WriteLine("Hitting 0 to confirm deck");
            HitConfirm();
            Thread.Sleep(2000);

            return true;
        }

        public bool Rematch(int deck)
        {
            HitConfirm();
            Thread.Sleep(500);

            RegisterForMatch(deck);

            return true;
        }

        public bool PlayCard(int handIndex, int boardIndex)
        {
            this._gs.Refresh();

            if(!this._gs.BoardOpen())
            {
                return false;
            }

            if (0 == this._gs.GetTimer())
            {
                return false;
            }

            if (handIndex >= GameScanner.BlueHandOffsets.Count
                || boardIndex >= GameScanner.BoardOffsets.Count)
            {
                return false;
            }

            var playerId = this._gs.GetTurnPlayerId();
            if (!MoveToHand(playerId, handIndex))
            {

            }
            else if (!PickUpCard())
            {

            }
            else if (!MoveToBoard(boardIndex))
            {

            }
            else if (!PlaceCard())
            {

            }

            return true;
        }

        public bool PickUpCard()
        {
            // TODO: Make sure card is not held before, and is held after
            HitConfirm();

            return true;
        }

        public bool PlaceCard()
        {
            // TODO: Make sure card is being held before, and not after
            HitConfirm();

            return true;
        }

        public bool MoveToHand(int playerId, int handIndex)
        {
            var offsets = playerId == 0
                ? GameScanner.BlueHandOffsets
                : GameScanner.RedHandOffsets;

            var targetOffset = offsets[handIndex];

            return MoveToBoardPosition(targetOffset);
        }


        public bool MoveToBoard(int boardIndex)
        {
            var offsets = GameScanner.BoardOffsets;

            var targetOffset = offsets[boardIndex];

            return MoveToBoardPosition(targetOffset);
        }

        private bool MoveToBoardPosition(Point targetOffset)
        {
            Console.WriteLine("Target: X=" + targetOffset.X + ", Y=" + targetOffset.Y);
            this._gs.Refresh();
            while (targetOffset.X != this._gs.GetBoardOffsetX()
                || targetOffset.Y != this._gs.GetBoardOffsetY())
            {
                if (!this._gs.BoardOpen())
                {
                    return false;
                }

                if (0 == this._gs.GetTimer())
                {
                    return false;
                }

                var x = this._gs.GetBoardOffsetX();
                var y = this._gs.GetBoardOffsetY();

                var xDelta = targetOffset.X - x;
                var yDelta = targetOffset.Y - y;

                Console.WriteLine("Target: X=" + targetOffset.X + ", Y=" + targetOffset.Y);
                Console.WriteLine("Current: X=" + x + ", Y=" + y);
                Console.WriteLine("Delta: X=" + xDelta + ", Y=" + yDelta + "\n");

                // Move the one that's further away closer
                if (Math.Abs(xDelta) > Math.Abs(yDelta))
                {
                    if (xDelta > 0)
                    {
                        MoveRight();
                    }
                    else
                    {
                        MoveLeft();
                    }
                }
                else
                {
                    if (yDelta > 0)
                    {
                        MoveDown();
                    }
                    else
                    {
                        MoveUp();
                    }
                }

                Thread.Sleep(30);
                this._gs.Refresh();
            }

            Console.WriteLine("Current: X=" + this._gs.GetBoardOffsetX()
                + ", Y=" + this._gs.GetBoardOffsetY());

            return true;
        }


        public bool MoveUp()
        {
            return SendKey(Keys.NumPad8);
        }

        public bool MoveDown()
        {
            return SendKey(Keys.NumPad2);
        }

        public bool MoveLeft()
        {
            return SendKey(Keys.NumPad4);
        }

        public bool MoveRight()
        {
            return SendKey(Keys.NumPad6);
        }

        public bool HitConfirm()
        {
            return SendKey(Keys.NumPad0);
        }
    }

    public class WindowHook
    {
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll")]
        public static extern IntPtr PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        const uint WM_KEYDOWN = 0x100;
        const uint WM_KEYUP = 0x101;
        const uint WM_SYSCOMMAND = 0x018;
        const uint SC_CLOSE = 0x053;

        public static bool SendKeystroke(IntPtr window, ushort k)
        {
            var success = SendMessage(window, WM_KEYDOWN, ((IntPtr)k), (IntPtr)0);

            //if (!success)
            //{
            //    return success;
            //}

            Thread.Sleep(20);

            success &= SendMessage(window, WM_KEYUP, ((IntPtr)k), (IntPtr)0);

            Thread.Sleep(20);

            return true;
        }
    }
}
