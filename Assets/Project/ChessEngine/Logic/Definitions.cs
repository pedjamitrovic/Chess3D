namespace Assets.Project.ChessEngine
{
    public enum File { FileA, FileB, FileC, FileD, FileE, FileF, FileG, FileH };
    public enum Rank { Rank1, Rank2, Rank3, Rank4, Rank5, Rank6, Rank7, Rank8 };
    public enum Color { White, Black, Both };
    public enum Square
    {
        A1 = 21, B1, C1, D1, E1, F1, G1, H1,
        A2 = 31, B2, C2, D2, E2, F2, G2, H2,
        A3 = 41, B3, C3, D3, E3, F3, G3, H3,
        A4 = 51, B4, C4, D4, E4, F4, G4, H4,
        A5 = 61, B5, C5, D5, E5, F5, G5, H5,
        A6 = 71, B6, C6, D6, E6, F6, G6, H6,
        A7 = 81, B7, C7, D7, E7, F7, G7, H7,
        A8 = 91, B8, C8, D8, E8, F8, G8, H8, None = 99,
    };
    public enum CastlingPermit { WhiteKingCastling = 1, WhiteQueenCastling = 2, BlackKingCastling = 4, BlackQueenCastling = 8 };
    public static class Constants
    {
        public static readonly int BoardSquareCount = 120; // 120 square board representation style
        public static readonly int ColorWBB = 3; // keeping track of white, black and both
        public static readonly int ColorWB = 2; // keeping track of both white and black
        public static readonly int PieceTypeCount = 13; // 1..12 PieceType (6*W, 6*B)
        public static readonly int MaxSamePieceCount = 10; // 8 pawns can upgrade to same figure, plus max 2 same figures on board

        public static readonly int Infinity = 100000;
        public static readonly int MaxSearchDepth = 128;
        public static readonly int IsMate = Infinity - MaxSearchDepth;
    }

    public static class FileMethods
    {
        public static string FileLabels = "abcdefgh";

        public static string GetLabel(this File f) { return FileLabels[(int)f].ToString(); }
    }

    public static class RankMethods
    {
        public static string RankLabels = "12345678";

        public static string GetLabel(this Rank r) { return RankLabels[(int)r].ToString(); }
    }

    public static class SquareMethods
    {
        public static string FileLabels = "abcdefgh";
        public static string RankLabels = "12345678";

        public static string GetLabel(this Square sq)
        {
            int val = (int)sq;
            if (val == 99) return "-";
            else
            {
                int file = (val - 21) % 10;
                int rank = (val - 21) / 10;
                return "" + FileLabels[file] + RankLabels[rank];
            }
        }
    }

    public static class ColorMethods
    {
        public static string ColorLabels = "wb-";

        public static string GetLabel(this Color s) { return ColorLabels[(int)s].ToString(); }
    }
}