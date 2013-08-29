using System;
using System.Collections.Generic;
using System.Text;

namespace GridWorld
{
    /// <summary>
    /// Coordinates of a square.
    /// </summary>
    public class Square
    {
        public int X;
        public int Y;

        public Square(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            string outstr = "(" + X + "," + Y + ")";
            return outstr;
        }
    }
}
