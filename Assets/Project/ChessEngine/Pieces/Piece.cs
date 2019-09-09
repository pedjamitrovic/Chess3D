
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

        public Piece(Color color, Square square, int value)
        {
            Color = color;
            Square = square;
            Value = value;
        }
        public abstract bool IsBig();
        public abstract bool IsMajor();
        public abstract bool IsMinor();
        public abstract char GetLabel();

        private static readonly Dictionary<char, int> pieceLabelToValueMap = new Dictionary<char, int>()
        {
            { Pawn.GetLabel(Color.White), 1 },
            { Knight.GetLabel(Color.White), 2 },
            { Bishop.GetLabel(Color.White), 3 },
            { Rook.GetLabel(Color.White), 4 },
            { Queen.GetLabel(Color.White), 5 },
            { King.GetLabel(Color.White), 6 },
            { Pawn.GetLabel(Color.Black), 7 },
            { Knight.GetLabel(Color.Black), 8 },
            { Bishop.GetLabel(Color.Black), 9 },
            { Rook.GetLabel(Color.Black), 10 },
            { Queen.GetLabel(Color.Black), 11 },
            { King.GetLabel(Color.Black), 12 }
        };
        private static readonly Dictionary<int, char> pieceValueToLabelMap = new Dictionary<int, char>()
        {
            { 1, Pawn.GetLabel(Color.White) },
            { 2, Knight.GetLabel(Color.White) },
            { 3, Bishop.GetLabel(Color.White) },
            { 4, Rook.GetLabel(Color.White) },
            { 5, Queen.GetLabel(Color.White) },
            { 6, King.GetLabel(Color.White) },
            { 7, Pawn.GetLabel(Color.Black) },
            { 8, Knight.GetLabel(Color.Black) },
            { 9, Bishop.GetLabel(Color.Black) },
            { 10, Rook.GetLabel(Color.Black) },
            { 11, Queen.GetLabel(Color.Black) },
            { 12, King.GetLabel(Color.Black) }
        };

        public static char GetLabel(Color color) { return '-'; }
        public static Piece CreatePiece(Type pieceType, Color color, Square square)
        {
            if (pieceType.Equals(typeof(Pawn)))
            {
                return new Pawn(color, square);
            }
            if (pieceType.Equals(typeof(Knight)))
            {
                return new Knight(color, square);
            }
            if (pieceType.Equals(typeof(Bishop)))
            {
                return new Bishop(color, square);
            }
            if (pieceType.Equals(typeof(Rook)))
            {
                return new Rook(color, square);
            }
            if (pieceType.Equals(typeof(King)))
            {
                return new King(color, square);
            }
            if (pieceType.Equals(typeof(Queen)))
            {
                return new Queen(color, square);
            }
            throw new IllegalArgumentException("Invalid PieceType provided.");
        }
        public static Piece CreatePiece(char pieceLabel, Square square)
        {
            Type pieceType = GetTypeFromPieceLabel(pieceLabel);
            Color color = GetColorFromPieceLabel(pieceLabel);
            return CreatePiece(pieceType, color, square);
        }
        public static Color GetColorFromPieceLabel(char pieceLabel)
        {
            if (char.IsUpper(pieceLabel)) return Color.White;
            else return Color.Black;
        }
        public static Type GetTypeFromPieceLabel(char pieceLabel)
        {
            switch (pieceLabel)
            {
                case 'p': case 'P': return typeof(Pawn);
                case 'n': case 'N': return typeof(Knight);
                case 'b': case 'B': return typeof(Bishop);
                case 'r': case 'R': return typeof(Rook);
                case 'k': case 'K': return typeof(King);
                case 'q': case 'Q': return typeof(Queen);
                default: throw new IllegalArgumentException();
            }
        }
        public static int GetValueFromPieceLabel(char pieceLabel)
        {
            return pieceLabelToValueMap[pieceLabel];
        }
        public static char GetPieceLabelFromValue(int pieceValue)
        {
            return pieceValueToLabelMap[pieceValue];
        }
    }
}
