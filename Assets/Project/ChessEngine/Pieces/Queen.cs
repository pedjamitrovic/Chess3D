namespace Assets.Project.ChessEngine.Pieces
{
    public class Queen : Piece
    {
        public override char Label
        {
            get
            {
                if (Color == Color.White) return 'Q';
                else return 'q';
            }
        }

        public override int Index
        {
            get
            {
                if (Color == Color.White) return 5;
                else return 11;
            }
        }

        public Queen(Color color, Square square) : base(color, square, 1000)
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
            if (color == Color.White) return 'Q';
            else return 'q';
        }

        public new static int GetIndex(Color color)
        {
            if (color == Color.White) return 5;
            else return 11;
        }
    }
}
