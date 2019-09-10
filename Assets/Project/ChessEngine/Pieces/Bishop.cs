namespace Assets.Project.ChessEngine.Pieces
{
    public class Bishop : Piece
    {
        public override char Label
        {
            get
            {
                if (Color == Color.White) return 'B';
                else return 'b';
            }
        }

        public override int Index
        {
            get
            {
                if (Color == Color.White) return 3;
                else return 9;
            }
        }

        public Bishop(Color color, Square square) : base(color, square, 325)
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
            if (color == Color.White) return 'B';
            else return 'b';
        }

        public new static int GetIndex(Color color)
        {
            if (color == Color.White) return 3;
            else return 9;
        }
    }
}