using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace GridWorld
{
    /// <summary>
    /// My own representation of the Infectious board. 
    /// It is possible to change this board, e.g. to DoMove() and UndoMove()
    /// </summary>
    class Board
    {
        /// <summary>
        /// Get the estimated score of the board (i.e. a forecast of who will win) from the point of view of player id.
        /// Positive values indicate Player id is ahead, negative values indicate he is behind.
        /// The size of the value indicates by how much the player is winning/losing. 
        /// Writing this function is the most important part of your coursework.
        /// </summary>
        internal double GetEstimatedResult(int id)
        {
            
            return (GetSquareCount(Active(id)) * 100.0) +
                (GetSquareCount(Passive(id)) * 1.0) +
                (GetSquareCount(Active(Opp(id))) * -100.0) +
                (GetSquareCount(Passive(Opp(id))) * -1.0) +
                (GetFreeMoves(Active(id)).Count * 10.0) +
                (GetFreeMoves(Active(Opp(id))).Count * -10.0) + // * rnd.Next(-10, 10);
                (Get3x3Blocks(Passive(id)) * 10000) +
                (Get3x3Blocks(Passive(Opp(id))) * -10000);
        }


        // int values that define square contents on my board
        public const int Empty = 0;
        public const int Impassable = -1;

        // Dimensions of the board
        public int Width, Height;

        private Random rnd = new Random();

        /// <summary>
        /// Get the square contents corresponding to an active colony for this player
        /// </summary>
        public int Active(int player)
        {
            return player;
        }

        /// <summary>
        /// Get the square contents corresponding to a passive colony for this player
        /// </summary>
        public int Passive(int player)
        {
            return player + 100;
        }

        /// <summary>
        /// Get the ID for the opponent. Assumes 2 players
        /// </summary>
        public int Opp(int player)
        {
            return 3 - player;
        }

        /// <summary>
        /// The contents of the board. 
        /// If B[x,y] = Active(1) then a player 1 active colony ('1') is in square (x,y)
        /// If B[x,y] = Active(2) then a player 2 active colony ('2') is in square (x,y)
        /// If B[x,y] = Passive(1) then a player 1 passive colony ('!') is in square (x,y)
        /// If B[x,y] = Passive(2) then a player 2 passive colony ('"') is in square (x,y)
        /// If B[x,y] = Impassable then square (x,y) is impassable
        /// If B[x,y] = Empty then square (x,y) is empty
        /// </summary>
        internal int[,] B;

        /// <summary>
        /// Copy the PlayerWorldState pws into Board data format.
        /// </summary>
        public Board(PlayerWorldState pws)
        {
            Width = pws.GridWidthInSquares;
            Height = pws.GridHeightInSquares;
            B = new int[Width, Height];

            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                {
                    if (pws[x, y].Contents == GridSquare.Empty)
                        B[x, y] = Empty;
                    else if (pws[x, y].Contents == GridSquare.Impassable)
                        B[x, y] = Impassable;
                    else if (pws[x, y].Contents == GridSquare.ActiveColony)
                            B[x, y] = Active(pws[x,y].Player);
                    else if (pws[x, y].Contents == GridSquare.PassiveColony)
                        B[x, y] = Passive(pws[x, y].Player);
                    else
                        throw (new Exception("Illegal GridSquare Contents in Board()"));
                }
        }

        public int Get3x3Blocks(int type)
        {
            int blocks = 0;

            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    if (B[x, y] == type)
                        if (IsOnBoard(x + 1, y) && B[x + 1, y] == type &&
                            IsOnBoard(x + 2, y) && B[x + 2, y] == type &&
                            IsOnBoard(x, y + 1) && B[x, y + 1] == type &&
                            IsOnBoard(x + 1, y + 1) && B[x + 1, y + 1] == type &&
                            IsOnBoard(x + 2, y + 1) && B[x + 2, y + 1] == type &&
                            IsOnBoard(x, y + 2) && B[x, y + 2] == type &&
                            IsOnBoard(x + 1, y + 2) && B[x + 1, y + 2] == type &&
                            IsOnBoard(x + 2, y + 2) && B[x + 2, y + 2] == type)
                            blocks++;
                        else continue;

            return blocks;
        }

        /// <summary>
        /// Loop through squares, and find my active colonies.
        /// For each active colony, return moves to all adjacent empty squares.
        /// </summary>
        public List<Move> GetMoves(int id)
        {
            List<Move> moves = new List<Move>();

            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                {
                    if (B[x, y] == Active(id)) // player id's acive colony
                    {
                        Square mysq = new Square(x,y);
                        List<Square> emplist = GetAdjacentSquares(x, y, Empty);
                        foreach (Square s in emplist)
                            moves.Add(new Move(mysq,s.X,s.Y,this,id));
                    }
                }

            return moves;
        }

        /// <summary>
        /// Get squares adjacent to (x,y) of the given type.
        /// </summary>
        private List<Square> GetAdjacentSquares(int x, int y, int type)
        {
            List<Square> sqlist = new List<Square>();

            for (int i = x - 1; i <= x + 1; i++)
                for (int j = y - 1; j <= y + 1; j++)
                {
                    if (i == x && j == y)
                        continue;
                    else if (IsOnBoard(i, j) && B[i, j] == type)
                        sqlist.Add(new Square(i, j));
                }

            return sqlist;
        }

        private List<Square> GetFreeMoves(int type)
        {
            List<Square> sqlist = new List<Square>();

            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    if (B[x, y] == type)
                        sqlist = GetAdjacentSquares(x, y, Empty);
                        // for (int i = x - 1; i <= x + 1; i++)
                        //     for (int j = y - 1; j <= y + 1; j++)
                        //         if (IsOnBoard(i, j) && B[i, j] == Empty)
                        //             sqlist.Add(new Square(i, j));
                        else continue;


            return sqlist;
        }

        /// <summary>
        /// Is (x,y) on the board?
        /// </summary>
        private bool IsOnBoard(int x, int y)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Get the number of squares of the given type
        /// </summary>
        private int GetSquareCount(int type)
        {
            int count = 0;
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    if (B[x, y] == type)
                        count++;

            return count;
        }

        /// <summary>
        /// Do Move m and update the Board and ActiveColonySquares[]
        /// </summary>
        public void DoMove(Move m)
        {
            if (m.TheCommand != null)
            {
                foreach (SquareChange sc in m.Changes)
                {
                    Debug.Assert(B[sc.X, sc.Y] == sc.OldContents); // check the old contents are as expected
                    B[sc.X, sc.Y] = sc.NewContents;
                }
            }
        }

        /// <summary>
        /// Undo Move m and update the Board and ActiveColonySquares[]
        /// </summary>
        public void UndoMove(Move m)
        {
            if (m.TheCommand != null)
            {
                foreach (SquareChange sc in m.Changes)
                {
                    Debug.Assert(B[sc.X, sc.Y] == sc.NewContents); // check the old contents are as expected
                    B[sc.X, sc.Y] = sc.OldContents;
                }
            }
        }

        /// <summary>
        /// Output a string representation of the Board
        /// </summary>
        public override string ToString()
        {
            return IndentedToString(0);
        }

        /// <summary>
        /// Return a string representation of the board, indented to level indentlevel to make the search tree clearer, where we have:
        /// '1' - active colony player 1
        /// '!' - passive colony player 1 (think [Shift] 1)
        /// '2' - active colony player 2
        /// '"' - passive colony player 2 (think [Shift] 2)
        /// '.' - empty square
        /// '#' - impassable square
        /// </summary>
        public string IndentedToString(int indentlevel)
        {
            string indentstr = GetIndentString(indentlevel);

            string outstr = indentstr;

            for (int y = Height - 1; y >= 0; y--)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (B[x, y] == Empty)
                        outstr += ".";
                    else if (B[x, y] == Impassable)
                        outstr += "#";
                    else if (B[x, y] == Active(1))
                        outstr += "1"; 
                    else if (B[x, y] == Passive(1))
                        outstr += "!"; 
                    else if (B[x, y] == Active(2))
                        outstr += "2"; 
                    else if (B[x, y] == Passive(2))
                        outstr += "\""; 
                    else
                        throw (new Exception("Illegal square contents in Board:ToString()"));
                }
                outstr += "\r\n" + indentstr;
            }

            return outstr;
        }



        /// <summary>
        /// Get the Indent string used to indent boards etc. at this depth in the tree.
        /// </summary>
        public static string GetIndentString(int indentlevel)
        {
            string indentstr = ""; // the string used to indent each row
            for (int i = 0; i < indentlevel; i++)
                indentstr += "|  "; // 3 spaces per indent level
            return indentstr;
        }

        /// <summary>
        /// The GameOverResult of the board from the point of view of player id.
        /// Note that this function assumes that the game is over and will return the
        /// wrong result if not - it only counts colonies on the current board.
        /// Value is 1001 * (player colonies - opponent colonies). This means that
        /// 1. a guaranteed win is better than any result returned by GetEstimatedScore
        /// 2. a guaranteed loss is worse than any result returned by GetEstimatedScore
        /// 3. big wins are favoured over small wins
        /// 4. small losses are favoured over big losses
        /// </summary>
        internal double GameOverResult(int playerid)
        {
            return 1001.0 * GetNumSquaresAdvantage(playerid);
        }

        /// <summary>
        /// Return (player id squares) - (opponent squares)
        /// </summary>
        internal int GetNumSquaresAdvantage(int id)
        {
            return GetSquareCount(Active(id)) + GetSquareCount(Passive(id))
                     - GetSquareCount(Active(Opp(id))) - GetSquareCount(Passive(Opp(id)));
        }
    }
}

