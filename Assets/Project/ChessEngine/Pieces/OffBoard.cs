namespace Assets.Project.ChessEngine.Pieces
{
    /* Dummy piece for out of bounds check */
    public sealed class OffLimits : Piece
    {
        public static OffLimits Instance { get; private set; }

        static OffLimits()
        {
            Instance = new OffLimits();
        }

        public override char Label
        {
            get
            {
                return 'X';
            }
        }

        public override int Index
        {
            get
            {
                return -1;
            }
        }

        private OffLimits() : base(Color.Both, Square.None, 0)
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

        public static new char GetLabel(Color color)
        {
            return 'X';
        }

        public static new int GetIndex(Color color)
        {
            return -1;
        }
    }
}
