namespace Assets.Project.ChessEngine.Pieces
{
    public class Pawn : Piece
    {
        public override char Label
        {
            get
            {
                if (Color == Color.White) return 'P';
                else return 'p';
            }
        }

        public override int Index
        {
            get
            {
                if (Color == Color.White) return 1;
                else return 7;
            }
        }

        public Pawn(Color color, Square square) : base(color, square, 100)
        {

        }
        
        public override bool IsBig()
        {
            return false;
        }

        public override bool IsMajor()
        {
            return false;
        }

        public override bool IsMinor()
        {
            return false;
        }

        public new static char GetLabel(Color color)
        {
            if (color == Color.White) return 'P';
            else return 'p';
        }

        public new static int GetIndex(Color color)
        {
            if (color == Color.White) return 1;
            else return 7;
        }
    }
}
