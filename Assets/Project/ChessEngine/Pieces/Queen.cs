namespace Assets.Project.ChessEngine.Pieces
{
    public class Queen : Piece
    {
        public Queen(Color color, Square square) : base(color, square, 1000)
        {

        }

        public override string GetLabel()
        {
            if (Color == Color.White) return "Q";
            else return "q";
        }

        public override bool IsBig()
        {
            return true;
        }

        public override bool IsMajor()
        {
            return true;
        }

        public override bool IsMinor()
        {
            return false;
        }

        public override PieceType GetPieceType()
        {
            if (Color == Color.White) return PieceType.WhiteQueen;
            else return PieceType.BlackQueen;
        }
    }
}
