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

        public int FromSq
        {
            get { return Value & fromSqMask; }
            set
            {
                if (value < 0 || value > 127) throw new IllegalArgumentException("Invalid argument provided for FromSq property. Expected [0-127].");
                Value |= value;
            }
        }

        public int ToSq
        {
            get { return (Value >> 7) & toSqMask; }
            set
            {
                if (value < 0 || value > 127) throw new IllegalArgumentException("Invalid argument provided for ToSq property. Expected [0-127].");
                Value |= (value << 7);
            }
        }

        public int CapturedPiece
        {
            get { return (Value >> 14) & capturedPieceMask; }
            set
            {
                if (value < 0 || value > 15) throw new IllegalArgumentException("Invalid argument provided for CapturedPiece property. Expected [0-15].");
                Value |= (value << 14);
            }
        }

        public int PromotedPiece
        {
            get { return (Value >> 20) & promotedPieceMask; }
            set
            {
                if (value < 0 || value > 15) throw new IllegalArgumentException("Invalid argument provided for PromotedPiece property. Expected [0-15].");
                Value |= (value << 20);
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
            FromSq = (int)fromSq;
            ToSq = (int)toSq;
        }

        public Move(Square fromSq, Square toSq, PieceType capturedPiece)
        {
            FromSq = (int)fromSq;
            ToSq = (int)toSq;
            CapturedPiece = (int)capturedPiece;
        }

        public Move(Square fromSq, Square toSq, PieceType capturedPiece, PieceType promotedPiece)
        {
            FromSq = (int)fromSq;
            ToSq = (int)toSq;
            CapturedPiece = (int)capturedPiece;
            PromotedPiece = (int)promotedPiece;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            string fromSq = ((Square)FromSq).GetLabel();
            string toSq = ((Square)ToSq).GetLabel();
            string promotedPiece = "";
            if (PromotedPiece != 0)
            {
                promotedPiece = ((PieceType)PromotedPiece).GetLabel().ToLower();
            }
            sb.Append(fromSq + toSq + promotedPiece);
            return sb.ToString();
        }
    }
}
