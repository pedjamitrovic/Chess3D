namespace Assets.Project.ChessEngine.Pieces
{
    public class Bishop : Piece
    {
        public Bishop(Color color, Square square) : base(color, square, 325)
        {

        }

        public override string GetLabel()
        {
            if (Color == Color.White) return "B";
            else return "b";
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
            if (Color == Color.White) return PieceType.WhiteBishop;
            else return PieceType.BlackBishop;
        }
    }
}