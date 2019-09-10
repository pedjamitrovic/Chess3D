namespace Assets.Project.ChessEngine.Pieces
{
    public class Knight : Piece
    {
        public override char Label
        {
            get
            {
                if (Color == Color.White) return 'N';
                else return 'n';
            }
        }

        public override int Index
        {
            get
            {
                if (Color == Color.White) return 2;
                else return 8;
            }
        }

        public Knight(Color color, Square square) : base(color, square, 325)
        {

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

        public new static char GetLabel(Color color)
        {
            if (color == Color.White) return 'N';
            else return 'n';
        }

        public new static int GetIndex(Color color)
        {
            if (color == Color.White) return 2;
            else return 8;
        }
    }
}
