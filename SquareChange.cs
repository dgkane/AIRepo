using System;
using System.Collections.Generic;
using System.Text;

namespace GridWorld
{
    /// <summary>
    /// Records a change to a square in TheBoard. 
    /// It also records the old contents of the square so that we can undo the change.
    /// </summary>
    public class SquareChange
    {
        public int X, Y; // coordinates of the square

        public int OldContents, NewContents; // contents of the square before and after the change

        public SquareChange(int x, int y, int oldcontents, int newcontents)
        {
            X = x;
            Y = y;
            OldContents = oldcontents;
            NewContents = newcontents;
        }

        public override string ToString()
        {
            string str = "[(" + X + "," + Y + ")" + OldContents + "->" + NewContents + "]";
            return str;
        }
    }
}
