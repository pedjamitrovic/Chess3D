using Assets.Project.ChessEngine.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Project.ChessEngine
{
    /*
        0000 0000 0000 0000 0000 0111 1111 -> FromSq
        0000 0000 0000 0011 1111 1000 0000 -> ToSq
        0000 0000 0011 1100 0000 0000 0000 -> CapturedPiece
        0000 0000 0100 0000 0000 0000 0000 -> IsEnPassant
        0000 0000 1000 0000 0000 0000 0000 -> IsPawnStart
        0000 1111 0000 0000 0000 0000 0000 -> PromotedPiece
        0001 0000 0000 0000 0000 0000 0000 -> IsCastle
    */
    public class Move
    {
        private static readonly int fromSqMask = 0x7F;
        private static readonly int toSqMask = 0x7F;
        private static readonly int capturedPieceMask = 0xF;
        private static readonly int isEnPassantMask = 0x40000;
        private static readonly int isPawnStartMask = 0x80000;
        private static readonly int promotedPieceMask = 0xF;
        private static readonly int isCastleMask = 0x1000000;
        
        public int Value { get; private set; } = 0;
        public int Score { get; set; } = 0;

        public Square FromSq
        {
            get { return (Square)(Value & fromSqMask); }
            set
            {
                if ((int)value < 0 || (int)value > 127) throw new IllegalArgumentException("Invalid argument provided for FromSq property. Expected [0-127].");
                Value |= (int)value;
            }
        }

        public Square ToSq
        {
            get { return (Square)((Value >> 7) & toSqMask); }
            set
            {
                if ((int)value < 0 || (int)value > 127) throw new IllegalArgumentException("Invalid argument provided for ToSq property. Expected [0-127].");
                Value |= ((int)value << 7);
            }
        }

        public PieceType CapturedPiece
        {
            get { return (PieceType)((Value >> 14) & capturedPieceMask); }
            set
            {
                if ((int)value < 0 || (int)value > 15) throw new IllegalArgumentException("Invalid argument provided for CapturedPiece property. Expected [0-15].");
                Value |= ((int)value << 14);
            }
        }

        public PieceType PromotedPiece
        {
            get { return (PieceType)((Value >> 20) & promotedPieceMask); }
            set
            {
                if ((int)value < 0 || (int)value > 15) throw new IllegalArgumentException("Invalid argument provided for PromotedPiece property. Expected [0-15].");
                Value |= ((int)value << 20);
            }
        }

        public bool IsEnPassant
        {
            get { return (Value & isEnPassantMask) != 0; }
            set
            {
                if (value)
                {
                    Value |= isEnPassantMask;
                }
                else
                {
                    Value &= ~(1 << 19);
                }
            }
        }

        public bool IsPawnStart
        {
            get { return (Value & isPawnStartMask) != 0; }
            set
            {
                if (value)
                {
                    Value |= isPawnStartMask;
                }
                else
                {
                    Value &= ~(1 << 20);
                }
            }
        }

        public bool IsCastle
        {
            get { return (Value & isCastleMask) != 0; }
            set
            {
                if (value)
                {
                    Value |= isCastleMask;
                }
                else
                {
                    Value &= ~(1 << 25);
                }
            }
        }

        public Move()
        {

        }

        public Move(Square fromSq, Square toSq)
        {
            FromSq = fromSq;
            ToSq = toSq;
        }

        public Move(Square fromSq, Square toSq, PieceType capturedPiece)
        {
            FromSq = fromSq;
            ToSq = toSq;
            CapturedPiece = capturedPiece;
        }

        public Move(Square fromSq, Square toSq, PieceType capturedPiece, PieceType promotedPiece)
        {
            FromSq = fromSq;
            ToSq = toSq;
            CapturedPiece = capturedPiece;
            PromotedPiece = promotedPiece;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            string fromSq = FromSq.GetLabel();
            string toSq = ToSq.GetLabel();
            string promotedPiece = "";
            if (PromotedPiece != 0)
            {
                promotedPiece = PromotedPiece.GetLabel().ToLower();
            }
            sb.Append(fromSq + toSq + promotedPiece);
            return sb.ToString();
        }
    }
}
