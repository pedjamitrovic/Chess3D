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
            while (val > 0) { count++; val &= val - 1; }
            return count;
        }
        public bool IsSet(int position)
        {
            if (position < 0 || position > 63) throw new IndexOutOfRangeException("Bitboard has indexes of range [0..63], provided: " + position);
            return (this.Value & SetMask[position]) > 0;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(Environment.NewLine + "Bitboard: " + Environment.NewLine + Environment.NewLine);
            ulong mask = 1;

            int sq120 = 0;
            int sq64 = 0;

            for (Rank r = Rank.Rank8; r >= Rank.Rank1; --r)
            {
                for (File f = File.FileA; f <= File.FileH; ++f)
                {
                    sq120 = Board.ConvertToSq120(f, r);
                    sq64 = Board.Sq64(sq120);

                    if (((mask << sq64) & Value) != 0) sb.Append("X");
                    else sb.Append("-");
                }
                sb.Append(Environment.NewLine);
            }
            sb.Append(Environment.NewLine + Environment.NewLine);

            return sb.ToString();
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
