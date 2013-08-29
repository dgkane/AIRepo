//#define OUTPUT_TREE // leave this line commented or you will output the (very large) game tree
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics; // for Debug

namespace GridWorld
{
    /// <summary>
    /// This infectious player does a tree search using the alpha beta negamax search algorithm.
    /// </summary>
    public class DavesAI2 : BasePlayer
    {
        /// <summary>
        /// The maximum depth that we will search the game tree.
        /// Play with this parameter!
        /// </summary>
        int MaxDepth = 7;

        /// <summary>
        /// The board data in GridWorld format.
        /// </summary>
        PlayerWorldState myWorldState;

        /// <summary>
        /// The number of boards evaluated in the search so far this turn.
        /// </summary>
        int BoardsEvaluated = 0;

        /// <summary>
        /// The best move found from the original board position in the search.
        /// </summary>
        Move BestMove;

        /// <summary>
        /// This is the temporary board on which moves are tried out during search.
        /// </summary>
        Board TheBoard;

        /// <summary>
        /// The constuctor function is only called once (when the AIPlayer is first created).
        /// Initialise any data structures you need in here and set your AI player's name.
        /// Note that you do not have access to the board position in myWorldState when this function is called.
        /// </summary>
        public DavesAI2() : base()
        {
            this.Name = "DavesAI2";
        }

        /// <summary>
        /// Return the commands for this turn
        /// </summary>
        public override ICommand GetTurnCommands(IPlayerWorldState igrid)
        {
            myWorldState = (PlayerWorldState)igrid;

            // initialise my own copy of Board
            TheBoard = new Board(myWorldState);

            // reset the counter of boards evaluated
            BoardsEvaluated = 0; 

            // No BestMove known yet - BestMove == null corresponds to passing the turn
            BestMove = new Move();

            // Search for the best move
            double alpha = GetGameResultDepthLimitedAlphaBeta(this.ID, 0, -1000.0, 1000.0);

            // Display the best move so far for debugging purposes
            if (BestMove.TheCommand == null)
                WriteTrace("Best Move - PASS - after " + BoardsEvaluated + " boards");
            else
                WriteTrace("Best Move - " + BestMove + " - after " + BoardsEvaluated + " boards");

            // return the move
            return BestMove.TheCommand;
        }

        /// <summary>
        /// Find game result using negamax search with alphabeta pruning.
        /// When the search reaches MaxDepth down the tree, an estimated score is returned.
        /// The best move from the root position (depth zero) is kept in BestMove
        /// </summary>
        double GetGameResultDepthLimitedAlphaBeta(int playerid, int depth, double alpha, double beta)
        {
            BoardsEvaluated++; // record that we are looking at a new board
            
            int opponentid = 3 - playerid; // opponent ID

            List<Move> moves = TheBoard.GetMoves(playerid); // get all moves from this board

#if OUTPUT_TREE	// Output the search tree if the first line of this source file says "#define OUTPUT_TREE"
			if (moves.Count == 0 && TheBoard.GetMoves(opponentid).Count == 0)
				Debug.WriteLine("Boards: " + BoardsEvaluated + " Player: " + playerid + " Depth: " + depth + " Alpha: " + alpha + " Beta: " + beta + " GORes: " + TheBoard.GameOverResult(playerid)); // game over position
			else if (depth < MaxDepth)
				Debug.WriteLine("Boards: " + BoardsEvaluated + " Player: " + playerid + " Depth: " + depth + " Alpha: " + alpha + " Beta: " + beta); // intermediate position
			else
				Debug.WriteLine("Boards: " + BoardsEvaluated + " Player: " + playerid + " Depth: " + depth + " Alpha: " + alpha + " Beta: " + beta + " EstRes: " + TheBoard.GetEstimatedResult(playerid)); // max depth position
			Debug.WriteLine(TheBoard.IndentedToString(depth)); // use newlines so that board is easy to read
#endif //OUTPUT_TREE

            if (moves.Count == 0 && TheBoard.GetMoves(opponentid).Count == 0) // only pass moves are left
                return TheBoard.GameOverResult(playerid); // no more moves so return game over result

            // If we have reached the maximum search depth, return our best "guess" as to the game result.
            if (depth == MaxDepth)
                return TheBoard.GetEstimatedResult(playerid);

            if (moves.Count == 0)
                moves.Add(new Move()); // add a null move.

            foreach (Move m in moves)
            {
                if (alpha >= beta)
                {
#if OUTPUT_TREE // Output the search tree if the first line of this source file says "#define OUTPUT_TREE"		
                    Debug.WriteLine("*** Search cutoff at depth " + depth + ". Alpha = " + alpha + ", Beta = " + beta);
                    Debug.WriteLine("");
                    Debug.WriteLine("");
#endif //OUTPUT_TREE
                    return alpha; // cut off search here.
                }

                TheBoard.DoMove(m);
                double gr = -GetGameResultDepthLimitedAlphaBeta(opponentid, depth + 1, -beta, -alpha);
                if (gr > alpha)
                {
                    alpha = gr;
                    if (depth == 0)
                        BestMove = m;  // save the best move from the root position.
                }
                TheBoard.UndoMove(m);
            }

            return alpha;
        }
    }
}
