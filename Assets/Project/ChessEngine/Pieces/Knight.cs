namespace Assets.Project.ChessEngine.Pieces
{
    public class Knight : Piece
    {
        public Knight(Color color, Square square) : base(color, square, 325)
        {

        }

        public override string GetLabel()
        {
            if (Color == Color.White) return "N";
            else return "n";
        }

        public override bool IsBig()
        {
            return true;
        }

        public override bool IsMajor()
        {
            return false;
        }

        public override bool IsMinor()
        {
            return true;
        }

        public override PieceType GetPieceType()
        {
            if (Color == Color.White) return PieceType.WhiteKnight;
            else return PieceType.BlackKnight;
        }
    }
}
