using Assets.Project.ChessEngine.Exceptions;
using Assets.Project.ChessEngine.Pieces;
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

        public int Value { get; private set; }
        public int Score { get; set; }

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

        public int? CapturedPiece
        {
            get
            {
                int val = (Value >> 14) & capturedPieceMask;
                if (val > 0) return val;
                return null;
            }
            set
            {
                if (value.HasValue)
                {
                    Value &= (~(capturedPieceMask << 14));
                    Value |= (value.Value << 14);
                }
                else Value &= (~(capturedPieceMask << 14)); // set null
            }
        }

        public int? PromotedPiece
        {
            get
            {
                int val = (Value >> 20) & promotedPieceMask;
                if (val > 0) return val;
                return null;
            }
            set
            {
                if (value.HasValue)
                {
                    Value &= (~(promotedPieceMask << 20));
                    Value |= (value.Value << 20);
                }
                else Value &= (~(promotedPieceMask << 20)); // set null
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

        public override bool Equals(Object obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || !GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                Move other = (Move)obj;
                return Value == other.Value;
            }
        }

        public override int GetHashCode()
        {
            var hashCode = 1200616606;
            hashCode = hashCode * -1521134295 + Value.GetHashCode();
            hashCode = hashCode * -1521134295 + Score.GetHashCode();
            hashCode = hashCode * -1521134295 + FromSq.GetHashCode();
            hashCode = hashCode * -1521134295 + ToSq.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<int?>.Default.GetHashCode(CapturedPiece);
            hashCode = hashCode * -1521134295 + EqualityComparer<int?>.Default.GetHashCode(PromotedPiece);
            hashCode = hashCode * -1521134295 + IsEnPassant.GetHashCode();
            hashCode = hashCode * -1521134295 + IsPawnStart.GetHashCode();
            hashCode = hashCode * -1521134295 + IsCastle.GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            string fromSq = FromSq.GetLabel();
            string toSq = ToSq.GetLabel();
            sb.Append(fromSq + toSq);
            if (PromotedPiece.HasValue)
            {
                sb.Append(char.ToLower(Piece.GetLabelFromPieceIndex(PromotedPiece.Value)));
            }
            return sb.ToString();
        }
    }
}
