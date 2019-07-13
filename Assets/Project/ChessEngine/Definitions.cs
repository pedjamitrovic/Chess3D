namespace Assets.Project.ChessEngine
{
    public enum SquareContent { None, WhitePawn, WhiteRook, WhiteKnight, WhiteBishop, WhiteQueen, WhiteKing, BlackPawn, BlackRook, BlackKnight, BlackBishop, BlackQueen, BlackKing, OffLimits };
    public enum File { FileA, FileB, FileC, FileD, FileE, FileF, FileG, FileH };
    public enum Rank { Rank1, Rank2, Rank3, Rank4, Rank5, Rank6, Rank7, Rank8 };
    public enum Color { White, Black, Both };
    public enum Square
    {
        None = 0, A1 = 21, B1, C1, D1, E1, F1, G1, H1,
                  A2 = 31, B2, C2, D2, E2, F2, G2, H2,
                  A3 = 41, B3, C3, D3, E3, F3, G3, H3,
                  A4 = 51, B4, C4, D4, E4, F4, G4, H4,
                  A5 = 61, B5, C5, D5, E5, F5, G5, H5,
                  A6 = 71, B6, C6, D6, E6, F6, G6, H6,
                  A7 = 81, B7, C7, D7, E7, F7, G7, H7,
                  A8 = 91, B8, C8, D8, E8, F8, G8, H8
    };
    public enum CastlingPermit { WhiteKingCastling = 1, WhiteQueenCastling = 2, BlackKingCastling = 4, BlackQueenCastling = 8 };
    public enum Side { White, Black, Both };
}