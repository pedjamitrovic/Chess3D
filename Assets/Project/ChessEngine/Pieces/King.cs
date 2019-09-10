namespace Assets.Project.ChessEngine.Pieces
{
    public class King : Piece
    {
        public override char Label
        {
            get
            {
                if (Color == Color.White) return 'K';
                else return 'k';
            }
        }

        public override int Index
        {
            get
            {
                if (Color == Color.White) return 6;
                else return 12;
            }
        }

        public King(Color color, Square square) : base(color, square, 50000)
        {

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

        public new static int GetIndex(Color color)
        {
            if (color == Color.White) return 6;
            else return 12;
        }
    }
}
