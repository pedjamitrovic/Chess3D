﻿namespace Assets.Project.ChessEngine.Pieces
{
    /* Dummy piece for out of bounds check */
    public sealed class OffLimits : Piece
    {
        public static OffLimits Instance { get; private set; } = new OffLimits();

        static OffLimits()
        {

        }
        
        private OffLimits() : base(Color.Both, Square.None, 0)
        {

        }

        public override char GetLabel()
        {
            return 'X';
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
    }
}
