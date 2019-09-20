using Assets.Project.ChessEngine.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Project.ChessEngine.Pieces
{
    public class PieceList 
    {
        private readonly int[] pieceCount;
        private readonly Piece[,] pieceList;

        public PieceList()
        {
            pieceCount = new int[Constants.PieceTypeCount];
            pieceList = new Piece[Constants.PieceTypeCount, Constants.MaxSamePieceCount];
        }
        public IEnumerable<Piece> GetList(int pieceIndex)
        {
            for (int i = 0; i < pieceCount[pieceIndex]; ++i)
            {
                yield return pieceList[pieceIndex, i];
            }
        }
        public void AddPiece(Piece piece)
        {
            pieceList[piece.Index, pieceCount[piece.Index]++] = piece;
        }
        public void RemovePiece(Piece piece)
        {
            int indexOfRemovedPiece = -1;
            for (int i = 0; i < pieceCount[piece.Index]; ++i)
            {
                if (pieceList[piece.Index, i] == piece)
                {
                    indexOfRemovedPiece = i;
                    break;
                }
            }

            --pieceCount[piece.Index];
            pieceList[piece.Index, indexOfRemovedPiece] = pieceList[piece.Index, pieceCount[piece.Index]];
        }
        public int GetPieceCount(int pieceIndex)
        {
            return pieceCount[pieceIndex];
        }
    }
}
