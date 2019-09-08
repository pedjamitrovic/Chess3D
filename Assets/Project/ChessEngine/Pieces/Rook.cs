namespace Assets.Project.ChessEngine.Pieces
{
    public class Rook : Piece
    {
        public Rook(Color color, Square square) : base(color, square, 550)
        {

        }

        public override char GetLabel()
        {
            if (Color == Color.White) return 'R';
            else return 'r';
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

        public new static char GetLabel(Color color)
        {
            if (color == Color.White) return 'R';
            else return 'r';
        }
    }
}
