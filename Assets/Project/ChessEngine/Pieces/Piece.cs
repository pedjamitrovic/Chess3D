
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
        public abstract string GetLabel();
        public abstract PieceType GetPieceType();
        public static Piece CreatePiece(PieceType type, Square square)
        {
            switch (type)
            {
                case PieceType.BlackPawn: return new Pawn(Color.Black, square);
                case PieceType.BlackKnight: return new Knight(Color.Black, square);
                case PieceType.BlackBishop: return new Bishop(Color.Black, square);
                case PieceType.BlackRook: return new Rook(Color.Black, square);
                case PieceType.BlackKing: return new King(Color.Black, square);
                case PieceType.BlackQueen: return new Queen(Color.Black, square);
                case PieceType.WhitePawn: return new Pawn(Color.White, square);
                case PieceType.WhiteKnight: return new Knight(Color.White, square);
                case PieceType.WhiteBishop: return new Bishop(Color.White, square);
                case PieceType.WhiteRook: return new Rook(Color.White, square);
                case PieceType.WhiteKing: return new King(Color.White, square);
                case PieceType.WhiteQueen: return new Queen(Color.White, square);
                default: throw new System.Exception("Invalid PieceType provided.");
            }
        }
    }
}
