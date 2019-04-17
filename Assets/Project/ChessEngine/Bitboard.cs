using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Project.ChessEngine
{
    public class Bitboard
    {
        public ulong Value { get; set; }

        public Bitboard() { this.Value = 0; }
        public Bitboard(ulong value) { this.Value = value; }
        public void SetBit(int position)
        {
            if (position < 0 || position > 63) throw new IndexOutOfRangeException("Bitboard has indexes of range [0..63], provided: " + position);
            this.Value |= SetMask[position];
        }
        public void ClearBit(int position)
        {
            if (position < 0 || position > 63) throw new IndexOutOfRangeException("Bitboard has indexes of range [0..63], provided: " + position);
            this.Value &= ClearMask[position];
        }
        public int CountBit()
        {
            ulong val = this.Value;
            int count = 0;
            while(val > 0) { count++; val &= val - 1; }
            return count;
        }

        private static ulong[] SetMask { get; set; }
        private static ulong[] ClearMask { get; set; }

        static Bitboard()
        {
            SetMask = new ulong[64];
            ClearMask = new ulong[64];

            for (int i = 0; i < 64; i++)
            {
                SetMask[i] |= ((ulong)1 << i);
                ClearMask[i] = ~SetMask[i];
            }
    }
    }
}
