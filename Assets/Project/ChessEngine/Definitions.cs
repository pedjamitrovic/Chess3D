using System.Text;

namespace Assets.Project.ChessEngine
{
    public enum SquareContent { None, WhitePawn, WhiteKnight, WhiteBishop, WhiteRook, WhiteQueen, WhiteKing, BlackPawn, BlackKnight, BlackBishop, BlackRook, BlackQueen, BlackKing, OffLimits };
    public enum File { FileA, FileB, FileC, FileD, FileE, FileF, FileG, FileH };
    public enum Rank { Rank1, Rank2, Rank3, Rank4, Rank5, Rank6, Rank7, Rank8 };
    public enum Color { White, Black, Both };
    public enum Square
    {
        B1 = 21, C1, D1, E1, F1, G1, H1,
        A2 = 31, B2, C2, D2, E2, F2, G2, H2,
        A3 = 41, B3, C3, D3, E3, F3, G3, H3,
        A4 = 51, B4, C4, D4, E4, F4, G4, H4,
        A5 = 61, B5, C5, D5, E5, F5, G5, H5,
        A6 = 71, B6, C6, D6, E6, F6, G6, H6,
        A7 = 81, B7, C7, D7, E7, F7, G7, H7,
        A8 = 91, B8, C8, D8, E8, F8, G8, H8, None = 99,
    };
    public enum CastlingPermit { WhiteKingCastling = 1, WhiteQueenCastling = 2, BlackKingCastling = 4, BlackQueenCastling = 8 };
    public enum Side { White, Black, Both };

    public static class SquareContentMethods
    {
        public static Color[] PieceColor = { Color.Both, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.Black, Color.Black, Color.Black, Color.Black, Color.Black, Color.Black };
        public static int[] PieceValue = { 0, 100, 325, 325, 550, 1000, 50000, 100, 325, 325, 550, 1000, 50000 };
        public static bool[] PieceBig = { false, false, true, true, true, true, true, false, true, true, true, true, true };
        public static bool[] PieceMaj = { false, false, false, false, true, true, true, false, false, false, true, true, true };
        public static bool[] PieceMin = { false, false, true, true, false, false, false, false, true, true, false, false, false };
        public static string PieceLabels = "-PNBRQKpnbrqkX";

        public static Color GetColor(this SquareContent sq) { return PieceColor[(int)sq]; }
        public static int GetValue(this SquareContent sq) { return PieceValue[(int)sq]; }
        public static bool IsBig(this SquareContent sq) { return PieceBig[(int)sq]; }
        public static bool IsMajor(this SquareContent sq) { return PieceMaj[(int)sq]; }
        public static bool IsMinor(this SquareContent sq) { return PieceMin[(int)sq]; }
        public static string GetLabel(this SquareContent sq) { return PieceLabels[(int)sq].ToString(); }
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
            if (val == 0) return "-";
            else
            {
                int file = (val - 21) % 10;
                int rank = (val - 21) / 10;
                return "" + FileLabels[file] + RankLabels[rank];
            }
        }
    }

    public static class SideMethods
    {
        public static string SideLabels = "wb-";

        public static string GetLabel(this Side s) { return SideLabels[(int)s].ToString(); }
    }

    public static class StringExtensions
    {
        public static string AlignCenter(this string s, int width)
        {
            if (s.Length >= width) return s;

            int leftPadding = (width - s.Length) / 2;
            int rightPadding = width - s.Length - leftPadding;

            return new string(' ', leftPadding) + s + new string(' ', rightPadding);
        }
    }
}