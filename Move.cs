using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace GridWorld
{
    /// <summary>
    /// A move on an Infectious board (i.e. an active colony moving to an adjacent empty square 
    /// and associated colony flips). Keeps track of all squares changed so that we can Undo
    /// a move (and well as Do).
    /// </summary>
    public class Move
    {
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
        /// The Command that causes this move (i.e. this move involves moving an active colony from
        /// (XFrom,YFrom) to (XTo,YTo).
        /// </summary>
        public Command TheCommand;

        /// <summary>
        /// The SquareChanges associated with this move (i.e. the change from active to passive for (XFrom,YFrom),
        /// the change from empty to active colony for (XTo,YTo) and any opponents' colonies flipped which were
        /// adjacent to (XTo,YTo).
        /// </summary>
        public List<SquareChange> Changes;

        /// <summary>
        /// Create an empty move (with a null TheCommand)
        /// </summary>
        public Move()
        {
            TheCommand = null; // pass command
            Changes = new List<SquareChange>(); // no changes
        }

        /// <summary>
        /// A move from (from.X, from.Y) to (toX, toY). 
        /// Considers colony flips for opposing colonies adjacent to (toX, toY). 
        /// Checks that parameters are sensible using Debug.Assert, so that these are checked in the
        /// Debug version but will no longer be checked in the Release version.
        /// </summary>
        internal Move(Square from, int toX, int toY, Board b, int id)
        {
            // opponent id
            int opp = 3 - id;

            // check that id is legal
            Debug.Assert(id == 1 || id == 2);

            // check that the target square is on the board
            Debug.Assert(toX >= 0 && toX < b.Width && toY >= 0 && toY < b.Height);

            // check that the from Square contains an active colony belonging to Player id
            Debug.Assert(b.B[from.X, from.Y] == id);

            // check that the move is exactly one square.
            Debug.Assert(Math.Abs(from.X - toX) <= 1 && Math.Abs(from.Y - toY) <= 1 &&
                Math.Abs(from.X - toX) + Math.Abs(from.Y - toY) > 0);

            // check the square I am moving to is empty.
            Debug.Assert(b.B[toX, toY] == Board.Empty);

            // save the command to execute this move
            TheCommand = new Command(from.X, from.Y, toX, toY);

            // find all colony flips for colonies adjacent to (toX,toY)
            Changes = new List<SquareChange>();
            for (int x = toX - 1; x <= toX + 1; x++)
                for (int y = toY - 1; y <= toY + 1; y++)
                    if (x >= 0 && x < b.Width && y >= 0 && y < b.Height)
                    {
                        if (b.B[x, y] == Active(opp))
                            Changes.Add(new SquareChange(x, y, Active(opp), Active(id))); // capture active colony at (x,y)
                        if (b.B[x, y] == Passive(opp))
                            Changes.Add(new SquareChange(x, y, Passive(opp), Passive(id))); // capture passive colony at (x,y)
                    }

            Changes.Add(new SquareChange(from.X, from.Y, Active(id), Passive(id))); // change active to passive colony            
            Changes.Add(new SquareChange(toX, toY, Board.Empty, Active(id))); // change empty to active colony            
        }

        /// <summary>
        /// Output the Move as [(XFrom,YFrom) -> (XTo, YTo): Changes ***SquareChanges***]
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string str = "(" + TheCommand.XFrom + "," + TheCommand.YFrom + ") -> (" + TheCommand.XTo + "," + TheCommand.YTo + "): Changes:";
            foreach (SquareChange sc in Changes)
                str += " " + sc.ToString();

            return str;
        }
    }
}
