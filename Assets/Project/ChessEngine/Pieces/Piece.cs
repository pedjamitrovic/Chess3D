
using Assets.Project.ChessEngine.Exceptions;
using System;
using System.Collections.Generic;

namespace Assets.Project.ChessEngine.Pieces
{
    public abstract class Piece
    {
        public Color Color { get; set; }
        public Square Square { get; set; }
        public int Value { get; set; }
        public abstract char Label { get; }
        public abstract int Index { get; }

        public Piece(Color color, Square square, int value)
        {
            Color = color;
            Square = square;
            Value = value;
        }
        public abstract bool IsBig();
        public abstract bool IsMajor();
        public abstract bool IsMinor();

        public static char GetLabel(Color color) { return '-'; }
        public static int GetIndex(Color color) { return -1; }
        private static Piece CreatePiece(Type pieceType, Color color, Square square)
        {
            return (Piece)Activator.CreateInstance(pieceType, new object[] { color, square });
        }
        public static Piece CreatePiece(int pieceIndex, Square square)
        {
            Type pieceType = GetTypeFromPieceIndex(pieceIndex);
            Color color = GetColorFromPieceIndex(pieceIndex);
            return CreatePiece(pieceType, color, square);
        }
        public static Color GetColorFromPieceIndex(int pieceIndex)
        {
            if (pieceIndex >= 1 && pieceIndex <= 6) return Color.White;
            else if (pieceIndex >= 7 && pieceIndex <= 12) return Color.Black;
            throw new IllegalArgumentException("Illegal PieceIndex provided = " + pieceIndex);
        }
        public static Type GetTypeFromPieceIndex(int pieceIndex)
        {
            switch (pieceIndex)
            {
                case 1: case 7: return typeof(Pawn);
                case 2: case 8: return typeof(Knight);
                case 3: case 9: return typeof(Bishop);
                case 4: case 10: return typeof(Rook);
                case 5: case 11: return typeof(Queen);
                case 6: case 12: return typeof(King);
                default: throw new IllegalArgumentException();
            }
        }
        public static char GetLabelFromPieceIndex(int pieceIndex)
        {
            switch (pieceIndex)
            {
                case 1: return 'P';
                case 2: return 'N';
                case 3: return 'B';
                case 4: return 'R';
                case 5: return 'Q';
                case 6: return 'K';
                case 7: return 'p';
                case 8: return 'n';
                case 9: return 'b';
                case 10: return 'r';
                case 11: return 'q';
                case 12: return 'k';
                default: throw new IllegalArgumentException();
            }
        }
        public static int GetIndexFromPieceLabel(char pieceLabel)
        {
            switch (pieceLabel)
            {
                case 'P': return 1;
                case 'N': return 2;
                case 'B': return 3;
                case 'R': return 4;
                case 'Q': return 5;
                case 'K': return 6;
                case 'p': return 7;
                case 'n': return 8;
                case 'b': return 9;
                case 'r': return 10;
                case 'q': return 11;
                case 'k': return 12;
                default: throw new IllegalArgumentException();
            }
        }
    }
}
