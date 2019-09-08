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
        private static readonly Dictionary<char, int> pieceLabelToValueMap =  new Dictionary<char, int>()
        {
            { Pawn.GetLabel(Color.White), 1 },
            { Knight.GetLabel(Color.White), 2 },
            { Bishop.GetLabel(Color.White), 3 },
            { Rook.GetLabel(Color.White), 4 },
            { Queen.GetLabel(Color.White), 5 },
            { King.GetLabel(Color.White), 6 },
            { Pawn.GetLabel(Color.Black), 7 },
            { Knight.GetLabel(Color.Black), 8 },
            { Bishop.GetLabel(Color.Black), 9 },
            { Rook.GetLabel(Color.Black), 10 },
            { Queen.GetLabel(Color.Black), 11 },
            { King.GetLabel(Color.Black), 12 }
        };
        private static readonly Dictionary<int, char> pieceValueToLabelMap = new Dictionary<int, char>()
        {
            { 1, Pawn.GetLabel(Color.White) },
            { 2, Knight.GetLabel(Color.White) },
            { 3, Bishop.GetLabel(Color.White) },
            { 4, Rook.GetLabel(Color.White) },
            { 5, Queen.GetLabel(Color.White) },
            { 6, King.GetLabel(Color.White) },
            { 7, Pawn.GetLabel(Color.Black) },
            { 8, Knight.GetLabel(Color.Black) },
            { 9, Bishop.GetLabel(Color.Black) },
            { 10, Rook.GetLabel(Color.Black) },
            { 11, Queen.GetLabel(Color.Black) },
            { 12, King.GetLabel(Color.Black) }
        };

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

        public char? CapturedPiece
        {
            get
            {
                if (pieceValueToLabelMap.TryGetValue(((Value >> 14) & capturedPieceMask), out char retVal))
                {
                    return retVal;
                }
                return null;
            }
            set
            {
                if (value.HasValue)
                {
                    if (pieceLabelToValueMap.TryGetValue(value.Value, out int val))
                    {
                        Value &= (~(capturedPieceMask << 14));
                        Value |= (val << 14);
                    }
                    else throw new IllegalArgumentException("Invalid argument provided for CapturedPiece property.");
                }
                else Value &= (~(capturedPieceMask << 14)); // set null
            }
        }

        public char? PromotedPiece
        {
            get
            {
                if (pieceValueToLabelMap.TryGetValue(((Value >> 20) & promotedPieceMask), out char retVal))
                {
                    return retVal;
                }
                return null;
            }
            set
            {
                if (value.HasValue)
                {
                    if (pieceLabelToValueMap.TryGetValue(value.Value, out int val))
                    {
                        Value &= (~(capturedPieceMask << 20));
                        Value |= (val << 20);
                    }
                    else throw new IllegalArgumentException("Invalid argument provided for PromotedPiece property.");
                }
                else Value &= (~(capturedPieceMask << 20)); // set null
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
                return (FromSq == other.FromSq) && (ToSq == other.ToSq)
                    && (CapturedPiece == other.CapturedPiece) && (PromotedPiece == other.PromotedPiece)
                    && (IsEnPassant == other.IsEnPassant) && (IsPawnStart == other.IsPawnStart)
                    && (IsCastle == other.IsCastle);
            }
        }

        public override int GetHashCode()
        {
            var hashCode = 1200616606;
            hashCode = hashCode * -1521134295 + Value.GetHashCode();
            hashCode = hashCode * -1521134295 + Score.GetHashCode();
            hashCode = hashCode * -1521134295 + FromSq.GetHashCode();
            hashCode = hashCode * -1521134295 + ToSq.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<char?>.Default.GetHashCode(CapturedPiece);
            hashCode = hashCode * -1521134295 + EqualityComparer<char?>.Default.GetHashCode(PromotedPiece);
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
                sb.Append(char.ToLower(PromotedPiece.Value));
            }
            return sb.ToString();
        }
    }
}
