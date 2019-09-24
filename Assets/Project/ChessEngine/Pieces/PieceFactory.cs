using Assets.Project.ChessEngine.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Project.ChessEngine.Pieces
{
    public class PieceFactory
    {
        private Queue<Piece>[] pieces;
        
        public PieceFactory()
        {
            pieces = new Queue<Piece>[Constants.PieceTypeCount];
            for(int i = 0; i < Constants.PieceTypeCount; ++i)
            {
                pieces[i] = new Queue<Piece>();
            }
        }

        public Piece CreatePiece(int pieceIndex, Square square)
        {
            Piece piece;
            Queue<Piece> queue = pieces[pieceIndex];

            if (queue.Count > 0)
            {
                piece = queue.Dequeue();
            }
            else
            {
                piece = Piece.CreatePiece(pieceIndex, square);
            }
            piece.Square = square;
            return piece;
        }

        public void DeletePiece(Piece piece)
        {
            if (piece != null)
            {
                Queue<Piece> queue = pieces[piece.Index];
                queue.Enqueue(piece);
            }
        }
    }
}
