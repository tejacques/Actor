using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility
{
    public struct Point
    {
        public int X;
        public int Y;
        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }

    public class GameScanner
    {
        MemoryAPI memoryReader;
        byte[] boardBytes;
        int mouseX;
        int mouseY;
        int cursorX;
        int cursorY;
        int boardX;
        int boardY;

        long mouseXPtr;
        long mouseYPtr;
        long boardStatePtr;
        long boardCoordsPtr;
        long cursorCoordsPtr;
        long menuRulesPtr;
        long boardRulesPtr;
        long targetPtr;


        public static readonly List<Point> BlueHandOffsets = new List<Point>
        {
            new Point(84, 282), new Point(192, 282), new Point(300, 282),
                    new Point(138, 420), new Point(246,420)
        };

        public static readonly List<Point> RedHandOffsets = new List<Point>
        {
            new Point(854, 282), new Point(962, 282), new Point(300, 282),
                    new Point(908, 420), new Point(1016,420)
        };

        public static readonly List<Point> BoardOffsets = new List<Point>
        {
            new Point(442, 144), new Point(570, 144), new Point(714, 144),
            new Point(442, 280), new Point(570, 280), new Point(714, 280),
            new Point(442, 416), new Point(570, 416), new Point(714, 416),
        };

        public GameScanner()
        {
            this.memoryReader = new MemoryAPI("ffxiv");
            this.boardBytes = new byte[(int)Offsets.BoardOffsets.Timer+2];
        }

        public MemoryAPI MemoryAPI()
        {
            return this.memoryReader;
        }

        public void RefreshPointers()
        {
            //Console.WriteLine("Getting Pointer for: Mouse X");
            this.mouseXPtr = this.memoryReader.Pointer(Offsets.MouseX);
            //Console.WriteLine("Getting Pointer for: Mouse Y");
            this.mouseYPtr = this.memoryReader.Pointer(Offsets.MouseY);

            //Console.WriteLine("Getting Pointer for: BoardState");
            this.boardStatePtr = this.memoryReader.Pointer(Offsets.BoardState);
            //Console.WriteLine("Getting Pointer for: Board Coords");
            this.boardCoordsPtr = this.memoryReader.Pointer(Offsets.BoardCoords);

            //Console.WriteLine("Getting Pointer for: Cursor Coords");
            this.cursorCoordsPtr = this.memoryReader.Pointer(Offsets.CursorCoords);

            //Console.WriteLine("Getting Pointer for: Menu Rules");
            this.menuRulesPtr = this.memoryReader.Pointer(Offsets.MenuRules);

            //Console.WriteLine("Getting Pointer for: Board Rules");
            this.boardRulesPtr = this.memoryReader.Pointer(Offsets.BoardRules);
        }

        public void RefreshBoardState()
        {
            int bytesRead;
            this.boardBytes = this.memoryReader.Read(
                this.boardStatePtr,
                (uint)Offsets.BoardOffsets.Turn + 2,
                out bytesRead);

            if (0 == this.boardCoordsPtr)
            {
                this.boardX = -1;
                this.boardY = -1;
            }
            else
            {
                this.boardX = this.memoryReader.ReadInt16(this.boardCoordsPtr + Offsets.BoardX);
                this.boardY = this.memoryReader.ReadInt16(this.boardCoordsPtr + Offsets.BoardY);
            }
        }

        public void RefreshMouseState()
        {
            this.mouseX = this.memoryReader.ReadInt16(this.mouseXPtr);
            this.mouseY = this.memoryReader.ReadInt16(this.mouseYPtr);
        }

        public void RefreshCursorState()
        {
            if (0 == this.cursorCoordsPtr)
            {
                this.cursorX = -1;
                this.cursorY = -1;
                return;
            }

            this.cursorX = this.memoryReader.ReadInt16(this.cursorCoordsPtr + Offsets.CursorX);
            this.cursorY = this.memoryReader.ReadInt16(this.cursorCoordsPtr + Offsets.CursorY);
        }

        public void Refresh()
        {
            RefreshPointers();
            RefreshMouseState();
            RefreshCursorState();
            RefreshBoardState();
        }

        public static int CreatePlayerPortion(int playerId)
        {
            return playerId << 7;
        }

        public static int CreatePlayerCard(byte cardId, byte playerId) {
            return cardId == 0xFF
                ? 0xFF
                : ((cardId & 0x7F) | CreatePlayerPortion(playerId));
        }

        public static bool HasPlayer(int boardCard) {
            return GetPlayer(boardCard) < 2;
        }

        public static int GetPlayer(int boardCard) {
            return boardCard >> 7;
        }

        public static int GetCardId(int boardCard) {
            return boardCard & 0x7F;
        }

        public int[] PlayerHand(int playerId)
        {
            if (playerId > 2 || playerId < 0)
            {
                throw new ArgumentOutOfRangeException("playerId");
            }

            var hand = new int[5];
            int offset = (int)(playerId == 0
                ? Offsets.BoardOffsets.Player0Hand
                : Offsets.BoardOffsets.Player1Hand);
            var cardIdOffset = (int)Offsets.BoardPositions.CardId;
            var boardOffset = (int)Offsets.BoardOffsets.BoardPositionOffset;
            var playerIdPortion = CreatePlayerPortion(playerId);
            for (var i = 0; i < hand.Length; i++)
            {
                var cur = boardOffset * i;
                hand[i] = playerIdPortion | this.boardBytes[offset + cardIdOffset + cur];
            }

            return hand;
        }

        public int[] Board()
        {
            var board = new int[9];
            int offset = (int)(Offsets.BoardOffsets.Board);
            var cardIdOffset = (int)Offsets.BoardPositions.CardId;
            var boardOffset = (int)Offsets.BoardOffsets.BoardPositionOffset;
            var playerIdOffset = (int)Offsets.BoardPositions.PlayerId;
            for (var i = 0; i < board.Length; i++)
            {
                var cur = boardOffset * i;
                var playerId = this.boardBytes[offset + playerIdOffset + cur];
                var playerIdPortion = CreatePlayerPortion(playerId);
                board[i] = playerIdPortion | this.boardBytes[offset + cardIdOffset + cur];
            }

            return board;
        }

        public int playerIdTurn()
        {
            return this.boardBytes[(int)Offsets.BoardOffsets.Turn];
        }

        public int MouseX()
        {
            return this.mouseX;
        }

        public int MouseY()
        {
            return this.mouseY;
        }

        public int CursorX()
        {
            return this.cursorX;
        }

        public int CursorY()
        {
            return this.cursorY;
        }

        public int BoardX()
        {
            return this.boardX;
        }

        public int BoardY()
        {
            return this.boardY;
        }

        public int GetTurnPlayerId()
        {
            if (0 == GetTimer())
            {
                return -1;
            }
            return this.boardBytes[(int)Offsets.BoardOffsets.Turn];
        }

        public int GetTimer()
        {
            return BitConverter.ToInt16(this.boardBytes, (int)Offsets.BoardOffsets.Timer);
        }

        public int GetBoardOffsetX()
        {
            if (BoardOpen())
            {
                return CursorX() - BoardX();
            }

            return -1;
        }

        public int GetBoardOffsetY()
        {
            if (BoardOpen())
            {
                return CursorY() - BoardY();
            }

            return -1;
        }

        public bool BoardOpen()
        {
            return (BoardX() > 0 && BoardY() > 0);
        }

    }
}
