
using Assets.Project.ChessEngine.Exceptions;
using System;

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
    }
}
