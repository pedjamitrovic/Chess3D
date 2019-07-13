using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Project.ChessEngine
{
    public static class HashGenerator
    {
        private static ulong[,] pieceKeys; // 13x120
        private static ulong sideKey;
        private static ulong[] castleKeys; // 16 (4 bit representation 0-15 values)
        private static Random rnd = new Random();

        static HashGenerator()
        {
            pieceKeys = new ulong[Board.PieceTypesCount, Board.BoardSquaresNumber];
            castleKeys = new ulong[16];
            for (int i = 0; i < Board.PieceTypesCount; ++i)
            {
                for (int j = 0; j < 120; ++j)
                {
                    pieceKeys[i, j] = Get64BitRandom();
                }
            }
            sideKey = Get64BitRandom();
            for (int i = 0; i < 16; ++i)
            {
                castleKeys[i] = Get64BitRandom();
            }
        }

        public static ulong CalculateStateKey(Board board)
        {
            ulong finalKey = 0;
            SquareContent piece;

            for (int sq = 0; sq < Board.BoardSquaresNumber; ++sq)
            {
                piece = board.Pieces[sq];
                if (piece != SquareContent.None && piece != SquareContent.OffLimits)
                {
                    finalKey ^= pieceKeys[(byte)piece, sq];
                }
            }

            if (board.OnTurn == Side.White) finalKey ^= sideKey;

            if (board.EnPassant != Square.None) finalKey ^= pieceKeys[0, (byte)board.EnPassant];

            finalKey ^= castleKeys[board.CastlePerm];

            return finalKey;
        }

        private static ulong Get64BitRandom(ulong minValue = ulong.MinValue, ulong maxValue = ulong.MaxValue)
        {
            byte[] buffer = new byte[sizeof(ulong)];
            rnd.NextBytes(buffer);
            return BitConverter.ToUInt64(buffer, 0) % (maxValue - minValue + 1) + minValue;
        }
    }
}
