using Assets.Project.ChessEngine.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Project.ChessEngine
{
    public class Perft
    {
        private long leafNodes;
        public long CountLeafNodes(Board board, int depth)
        {
            if (depth < 1) throw new IllegalArgumentException("Perft testing depth should be at least 1. Provided: " + depth);
            
            //board.CheckIntegrity();

            leafNodes = 0;

            MoveList moveList = board.GenerateAllMoves();
            foreach (Move move in moveList)
            {
                if (!board.DoMove(move)) continue;
                RecursivePerft(board, depth - 1);
                board.UndoMove();
            }
            return leafNodes;
        }

        private void RecursivePerft(Board board, int depth)
        {
            //board.CheckIntegrity();

            if (depth == 0)
            {
                ++leafNodes;
                return;
            }

            MoveList moveList = board.GenerateAllMoves();
            foreach (Move move in moveList)
            {
                if (!board.DoMove(move)) continue;
                RecursivePerft(board, depth - 1);
                board.UndoMove();
            }
        }
    }
}
