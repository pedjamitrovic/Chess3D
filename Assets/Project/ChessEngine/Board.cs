using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Project.ChessEngine
{
    public class Board
    {
        public SquareContent[] Pieces { get; set; }
        public Bitboard[] Pawns { get; set; }
        public Square[] Kings { get; set; }
        public byte[] PieceCount { get; set; }
        public byte[] BigPiecesCount { get; set; }
        public byte[] MajorPiecesCount { get; set; }
        public byte[] MinorPiecesCount { get; set; }
        public byte[,] PieceList { get; set; }

        public Side OnTurn { get; set; }
        public byte FiftyMove { get; set; }
        public Square EnPassant { get; set; }
        public int CastlePerm { get; set; }

        public int HistoryPly { get; set; } // number of halfmoves in the game so far
        public int Ply { get; set; } // number of halfmoves in the move search so far
        public ulong StateKey { get; set; } // board state hash key

        public Board()
        {
            Pieces = new SquareContent[BoardSquaresNumber];
            for (int i = 0; i < BoardSquaresNumber; ++i) Pieces[i] = SquareContent.OffLimits;
            for (int i = 0; i < 64; ++i) Pieces[SqIndexes64To120[i]] = SquareContent.None;

            Kings = new Square[KingsRepresentationNumber];
            Kings[(byte)Side.White] = Kings[(byte)Side.Black] = Square.None;

            PieceCount = new byte[PieceTypesCount];
            for (int i = 0; i < PieceTypesCount; ++i)
            {
                PieceCount[i] = 0;
            }

            Pawns = new Bitboard[PawnsRepresentationNumber];
            BigPiecesCount = new byte[BigPiecesRepresentationNumber];
            MajorPiecesCount = new byte[MajorPiecesRepresentationNumber];
            MinorPiecesCount = new byte[MinorPiecesRepresentationNumber];
            for (int i = 0; i < 3; ++i)
            {
                Pawns[i] = new Bitboard();
                BigPiecesCount[i] = 0;
                MajorPiecesCount[i] = 0;
                MinorPiecesCount[i] = 0;
            }

            OnTurn = Side.Both;
            FiftyMove = 0;
            EnPassant = Square.None;
            CastlePerm = 0;

            HistoryPly = 0;
            Ply = 0;

            StateKey = 0;
        }

        public static readonly int BoardSquaresNumber = 120; // 120 square board representation style
        public static readonly int PawnsRepresentationNumber = 3; // keeping track of white, black and both pawn positions in 64bit array
        public static readonly int KingsRepresentationNumber = 2; // keeping track of both white and black king position
        public static readonly int PieceTypesCount = 13; // keeping track of both white and black piece type count
        public static readonly int BigPiecesRepresentationNumber = 3; // keeping track of both white and black piece count (excluding pawns)
        public static readonly int MajorPiecesRepresentationNumber = 3; // keeping track of both white and black queens and rooks count
        public static readonly int MinorPiecesRepresentationNumber = 3; // keeping track of both white and black knights and bishops count
        public static readonly int MaxCountOfPieceType = 10; // 8 pawns can upgrade to same figure, plus max 2 same figures on board

        public static readonly byte[] SqIndexes120To64;
        public static readonly byte[] SqIndexes64To120;

        static Board()
        {
            SqIndexes120To64 = new byte[120];
            SqIndexes64To120 = new byte[64];
            for (int i = 0; i < 120; i++) SqIndexes120To64[i] = (byte)SquareContent.OffLimits;
            for (int i = 0; i < 64; i++) SqIndexes64To120[i] = (byte)SquareContent.OffLimits;

            byte currSq, sq64 = 0;
            for (Rank r = Rank.Rank1; r <= Rank.Rank8; ++r)
            {
                for (File f = File.FileA; f <= File.FileH; ++f, ++sq64)
                {
                    currSq = ConvertToSq120(f, r);
                    SqIndexes64To120[sq64] = currSq;
                    SqIndexes120To64[currSq] = sq64;
                }
            }
        }

        public static byte ConvertToSq120(File f, Rank r)
        {
            return (byte)((int)r * 10 + (int)f + 21);
        }
    }
}
