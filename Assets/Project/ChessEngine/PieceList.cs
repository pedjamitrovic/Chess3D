using Assets.Project.ChessEngine.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Project.ChessEngine.Pieces
{
    public class PieceList : Dictionary<char, List<Piece>>
    {
        public PieceList()
        {
            Add(Pawn.GetLabel(Color.White), new List<Piece>());
            Add(Knight.GetLabel(Color.White), new List<Piece>());
            Add(Bishop.GetLabel(Color.White), new List<Piece>());
            Add(Rook.GetLabel(Color.White), new List<Piece>());
            Add(Queen.GetLabel(Color.White), new List<Piece>());
            Add(King.GetLabel(Color.White), new List<Piece>());
            Add(Pawn.GetLabel(Color.Black), new List<Piece>());
            Add(Knight.GetLabel(Color.Black), new List<Piece>());
            Add(Bishop.GetLabel(Color.Black), new List<Piece>());
            Add(Rook.GetLabel(Color.Black), new List<Piece>());
            Add(Queen.GetLabel(Color.Black), new List<Piece>());
            Add(King.GetLabel(Color.Black), new List<Piece>());
        }
        public List<Piece> GetList(char pieceLabel)
        {
            if (TryGetValue(pieceLabel, out var list))
            {
                return list;
            }
            else throw new IllegalArgumentException();
        }
        public void AddPiece(Piece piece)
        {
            if (TryGetValue(piece.GetLabel(), out var list))
            {
                list.Add(piece);
            }
            else throw new IllegalArgumentException();
        }
        public void RemovePiece(Piece piece)
        {
            if (TryGetValue(piece.GetLabel(), out var list))
            {
                list.Remove(piece);
            }
            else throw new IllegalArgumentException();
        }
        public int GetPieceCount(char pieceLabel)
        {
            if (TryGetValue(pieceLabel, out var list))
            {
                return list.Count;
            }
            else throw new IllegalArgumentException();
        }
    }
}
