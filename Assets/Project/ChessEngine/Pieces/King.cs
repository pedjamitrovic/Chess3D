namespace Assets.Project.ChessEngine.Pieces
{
    public class King : Piece
    {
        public King(Color color, Square square) : base(color, square, 50000)
        {

        }

        public override char GetLabel()
        {
            if (Color == Color.White) return 'K';
            else return 'k';
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
            if (color == Color.White) return 'K';
            else return 'k';
        }
    }
}
