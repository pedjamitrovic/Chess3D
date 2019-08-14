﻿using Assets.Project.ChessEngine.Exceptions;
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
        public int Score { get; private set; } = 0;

        public int FromSq
        {
            get { return Value & fromSqMask; }
            set
            {
                if (value < 0 || value > 63) throw new IllegalArgumentException("Invalid argument provided for FromSq property. Expected [0-63].");
                Value |= value;
            }
        }

        public int ToSq
        {
            get { return (Value >> 7) & toSqMask; }
            set
            {
                if (value < 0 || value > 63) throw new IllegalArgumentException("Invalid argument provided for ToSq property. Expected [0-63].");
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
                    Value |= (1 << isEnPassantMask);
                }
                else
                {
                    Value &= ~(1 << isEnPassantMask);
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
                    Value |= (1 << isPawnStartMask);
                }
                else
                {
                    Value &= ~(1 << isPawnStartMask);
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
                    Value |= (1 << isCastleMask);
                }
                else
                {
                    Value &= ~(1 << isCastleMask);
                }
            }
        }
    }
}
