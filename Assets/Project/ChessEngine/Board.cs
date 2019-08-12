using Assets.Project.ChessEngine.Exceptions;
using Assets.Project.ChessEngine.Pieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Project.ChessEngine
{
    public class Board
    {
        public Piece[] Pieces { get; set; }
        public Bitboard[] Pawns { get; set; }
        public Square[] Kings { get; set; }
        public int[] PieceCount { get; set; }
        public int[] BigPiecesCount { get; set; }
        public int[] MajorPiecesCount { get; set; }
        public int[] MinorPiecesCount { get; set; }
        public int[] Material { get; set; }
        public Square[,] PieceList { get; set; }

        public Color OnTurn { get; set; }
        public int FiftyMove { get; set; }
        public Square EnPassant { get; set; }
        public int CastlePerm { get; set; }

        public int HistoryPly { get; set; } // number of halfmoves in the game so far
        public int Ply { get; set; } // number of halfmoves in the move search so far
        public ulong StateKey { get; set; } // board state hash key

        public Board()
        {
            Pieces = new Piece[BoardSquaresNumber];
            for (int i = 0; i < BoardSquaresNumber; ++i) Pieces[i] = OffLimits.Instance;
            for (int i = 0; i < 64; ++i) Pieces[SqIndexes64To120[i]] = null;

            Pawns = new Bitboard[PawnsRepresentationNumber];
            for (int i = 0; i < PawnsRepresentationNumber; ++i) Pawns[i] = new Bitboard();

            Kings = new Square[KingsRepresentationNumber];
            Kings[(int)Color.White] = Kings[(int)Color.Black] = Square.None;

            PieceCount = new int[PieceTypesCount];

            BigPiecesCount = new int[BigPiecesRepresentationNumber];
            MajorPiecesCount = new int[MajorPiecesRepresentationNumber];
            MinorPiecesCount = new int[MinorPiecesRepresentationNumber];
            Material = new int[2];
            PieceList = new Square[PieceTypesCount, MaxCountOfPieceType];

            OnTurn = Color.Both;
            FiftyMove = 0;
            EnPassant = Square.None;
            CastlePerm = 0;

            HistoryPly = 0;
            Ply = 0;

            StateKey = 0;
        }

        public void UpdateListsAndMaterial()
        {
            for (int i = 0; i < BoardSquaresNumber; ++i)
            {
                Piece piece = Pieces[i];
                if (piece != null && !(piece is OffLimits))
                {
                    if (piece.IsBig()) BigPiecesCount[(int)piece.Color]++;
                    if (piece.IsMajor()) MajorPiecesCount[(int)piece.Color]++;
                    if (piece.IsMinor()) MinorPiecesCount[(int)piece.Color]++;

                    PieceList[(int)piece.GetPieceType(), PieceCount[(int)piece.GetPieceType()]] = (Square)i;
                    ++PieceCount[(int)piece.GetPieceType()];

                    if (piece is King) Kings[(int)piece.Color] = (Square)i;
                    else if (piece is Pawn)
                    {
                        Pawns[(int)piece.Color].SetBit(Sq64(i));
                        Pawns[(int)Color.Both].SetBit(Sq64(i));
                    }

                    Material[(int)piece.Color] += piece.Value;
                }
            }
        }

        public void CheckIntegrity()
        {
            int[] TempPieceCount = new int[PieceTypesCount];
            int[] TempBigPiecesCount = new int[BigPiecesRepresentationNumber];
            int[] TempMajorPiecesCount = new int[MajorPiecesRepresentationNumber];
            int[] TempMinorPiecesCount = new int[MinorPiecesRepresentationNumber];
            int[] TempMaterial = new int[2];

            for (PieceType pieceType = PieceType.WhitePawn; pieceType <= PieceType.BlackKing; ++pieceType)
            {
                for (int i = 0; i < PieceCount[(int)pieceType]; ++i)
                {
                    Square sq120 = PieceList[(int)pieceType, i];
                    if (Pieces[(int)sq120].GetPieceType() != pieceType) throw new BoardIntegrityException("Piece type found on square " + (int)sq120
                        + " was " + Pieces[(int)sq120] + ", expected " + pieceType);
                }
            }

            for (int sq64 = 0; sq64 < 64; ++sq64)
            {
                int sq120 = Sq120(sq64);
                Piece piece = Pieces[sq120];
                if (piece == null) continue;

                ++TempPieceCount[(int)piece.GetPieceType()];
                if (piece.IsBig()) ++TempBigPiecesCount[(int)piece.Color];
                if (piece.IsMajor()) ++TempMajorPiecesCount[(int)piece.Color];
                if (piece.IsMinor()) ++TempMinorPiecesCount[(int)piece.Color];

                TempMaterial[(int)piece.Color] += piece.Value;
            }

            for (PieceType pieceType = PieceType.WhitePawn; pieceType <= PieceType.BlackKing; ++pieceType)
            {
                if (TempPieceCount[(int)pieceType] != PieceCount[(int)pieceType]) throw new BoardIntegrityException("Piece type " + (int)pieceType + " count found was "
                    + PieceCount[(int)pieceType] + ", expected " + TempPieceCount[(int)pieceType]);
            }

            if (Pawns[(int)Color.White].CountBit() != PieceCount[(int)PieceType.WhitePawn]
                ||
                Pawns[(int)Color.Black].CountBit() != PieceCount[(int)PieceType.BlackPawn]
                ||
                Pawns[(int)Color.Both].CountBit() != (PieceCount[(int)PieceType.WhitePawn] + PieceCount[(int)PieceType.BlackPawn]))
            {
                throw new BoardIntegrityException("Bitboard pawn count inequality");
            }

            for (int sq64 = 0; sq64 < 64; ++sq64)
            {
                if (Pawns[(int)Color.White].IsSet(sq64) && Pieces[Sq120(sq64)].GetPieceType() != PieceType.WhitePawn)
                    throw new BoardIntegrityException("Bitboard bit set on square " + sq64 + ", unexpected square content " + Pieces[Sq120(sq64)].GetPieceType() + " found");
                if (Pawns[(int)Color.Black].IsSet(sq64) && Pieces[Sq120(sq64)].GetPieceType() != PieceType.BlackPawn)
                    throw new BoardIntegrityException("Bitboard bit set on square " + sq64 + ", unexpected square content " + Pieces[Sq120(sq64)].GetPieceType() + " found");
                if (Pawns[(int)Color.Both].IsSet(sq64) && (Pieces[Sq120(sq64)].GetPieceType() != PieceType.WhitePawn && Pieces[Sq120(sq64)].GetPieceType() != PieceType.BlackPawn))
                    throw new BoardIntegrityException("Bitboard bit set on square " + sq64 + ", unexpected square content " + Pieces[Sq120(sq64)].GetPieceType() + " found");
            }

            if (TempMaterial[(int)Color.White] != Material[(int)Color.White]
                ||
                TempMaterial[(int)Color.Black] != Material[(int)Color.Black])
            {
                throw new BoardIntegrityException("Material inequality");
            }

            if (TempBigPiecesCount[(int)Color.White] != BigPiecesCount[(int)Color.White]
                ||
                TempBigPiecesCount[(int)Color.Black] != BigPiecesCount[(int)Color.Black])
            {
                throw new BoardIntegrityException("Big pieces count inequality");
            }

            if (TempMajorPiecesCount[(int)Color.White] != MajorPiecesCount[(int)Color.White]
                ||
                TempMajorPiecesCount[(int)Color.Black] != MajorPiecesCount[(int)Color.Black])
            {
                throw new BoardIntegrityException("Major pieces count inequality");
            }

            if (TempMinorPiecesCount[(int)Color.White] != MinorPiecesCount[(int)Color.White]
                ||
                TempMinorPiecesCount[(int)Color.Black] != MinorPiecesCount[(int)Color.Black])
            {
                throw new BoardIntegrityException("Minor pieces count inequality");
            }

            if (OnTurn != Color.White && OnTurn != Color.Black)
            {
                throw new BoardIntegrityException("Player on turn can be either white or black, found " + (int)OnTurn);
            }

            if (HashGenerator.CalculateStateKey(this) != StateKey)
            {
                throw new BoardIntegrityException("Expected state key " + HashGenerator.CalculateStateKey(this) + ", found " + StateKey);
            }

            if (EnPassant != Square.None &&
                ((RankBoard[(int)EnPassant] != (int)Rank.Rank6 && OnTurn == Color.White) ||
                 (RankBoard[(int)EnPassant] != (int)Rank.Rank3 && OnTurn == Color.Black))
                )
            {

            }

            if (Pieces[(int)Kings[(int)Color.White]].GetPieceType() != PieceType.WhiteKing || Pieces[(int)Kings[(int)Color.Black]].GetPieceType() != PieceType.BlackKing)
            {
                throw new BoardIntegrityException("King position inconsistency");
            }
        }

        public static readonly int[] KnDirection = { -8, -19, -21, -12, 8, 19, 21, 12 };
        public static readonly int[] RkDirection = { -1, -10, 1, 10 };
        public static readonly int[] BiDirection = { -9, -11, 11, 9 };
        public static readonly int[] KiDirection = { -1, -10, 1, 10, -9, -11, 11, 9 };
        
        public bool IsSquareAttacked(int sq, Color side)
        {
            CheckIntegrity(); // TODO: remove CheckIntegrity for perfomance boost

            // pawns
            if (side == Color.White)
            {
                Piece piece = Pieces[sq - 11];
                if (piece is Pawn && piece.Color == Color.White) return true;
                piece = Pieces[sq - 9];
                if (piece is Pawn && piece.Color == Color.White) return true;
            }
            else
            {
                Piece piece = Pieces[sq + 11];
                if (piece is Pawn && piece.Color == Color.Black) return true;
                piece = Pieces[sq + 9];
                if (piece is Pawn && piece.Color == Color.Black) return true;
            }

            // knights
            for (int i = 0; i < 8; ++i)
            {
                Piece piece = Pieces[sq + KnDirection[i]];
                if (piece is Knight && piece.Color == side)
                {
                    return true;
                }
            }

            // rooks, queens
            for (int i = 0; i < 4; ++i)
            {
                int direction = RkDirection[i];
                int tempSq = sq + direction;
                Piece piece = Pieces[tempSq];
                while (!(piece is OffLimits))
                {
                    if (piece != null)
                    {
                        if ((piece is Rook || piece is Queen) && piece.Color == side)
                        {
                            return true;
                        }
                        break;
                    }
                    tempSq += direction;
                    piece = Pieces[tempSq];
                }
            }

            // bishops, queens
            for (int i = 0; i < 4; ++i)
            {
                int direction = BiDirection[i];
                int tempSq = sq + direction;
                Piece piece = Pieces[tempSq];
                while (!(piece is OffLimits))
                {
                    if (piece != null)
                    {
                        if ((piece is Bishop || piece is Queen) && piece.Color == side)
                        {
                            return true;
                        }
                        break;
                    }
                    tempSq += direction;
                    piece = Pieces[tempSq];
                }
            }

            // kings
            for (int i = 0; i < 8; ++i)
            {
                Piece piece = Pieces[sq + KiDirection[i]];
                if (piece is King && piece.Color == side)
                {
                    return true;
                }
            }

            return false;
        }

        public override string ToString()
        {
            int square;
            Piece piece;
            StringBuilder sb = new StringBuilder(Environment.NewLine + "Board: " + Environment.NewLine + Environment.NewLine);
            for (Rank rank = Rank.Rank8; rank >= Rank.Rank1; --rank)
            {
                sb.Append(rank.GetLabel() + "  ");
                for (File file = File.FileA; file <= File.FileH; ++file)
                {
                    square = ConvertToSq120(file, rank);
                    piece = Pieces[square];
                    sb.Append(piece.GetLabel().AlignCenter(3));
                }
                sb.Append(Environment.NewLine);
            }
            sb.Append(Environment.NewLine + "    ");
            for (File file = File.FileA; file <= File.FileH; ++file)
            {
                sb.Append(file.GetLabel().AlignCenter(3));
            }
            sb.Append(Environment.NewLine + Environment.NewLine);
            sb.Append("OnTurn: " + OnTurn.GetLabel() + Environment.NewLine);
            sb.Append("EnPassant: " + EnPassant.GetLabel() + Environment.NewLine);
            sb.AppendFormat("CastlePerm: {0}{1}{2}{3}{4}",
                    (CastlePerm & (int)CastlingPermit.WhiteKingCastling) != 0 ? 'K' : '-',
                    (CastlePerm & (int)CastlingPermit.WhiteQueenCastling) != 0 ? 'Q' : '-',
                    (CastlePerm & (int)CastlingPermit.BlackKingCastling) != 0 ? 'k' : '-',
                    (CastlePerm & (int)CastlingPermit.BlackQueenCastling) != 0 ? 'q' : '-',
                    Environment.NewLine
                    );

            sb.Append("StateKey: " + StateKey.ToString("X") + Environment.NewLine);
            return sb.ToString();
        }

        public static readonly int BoardSquaresNumber = 120; // 120 square board representation style
        public static readonly int PawnsRepresentationNumber = 3; // keeping track of white, black and both pawn positions in 64bit array
        public static readonly int KingsRepresentationNumber = 2; // keeping track of both white and black king position
        public static readonly int PieceTypesCount = 13; // keeping track of both white and black piece type count
        public static readonly int BigPiecesRepresentationNumber = 3; // keeping track of both white and black piece count (excluding pawns)
        public static readonly int MajorPiecesRepresentationNumber = 3; // keeping track of both white and black queens and rooks count
        public static readonly int MinorPiecesRepresentationNumber = 3; // keeping track of both white and black knights and bishops count
        public static readonly int MaxCountOfPieceType = 10; // 8 pawns can upgrade to same figure, plus max 2 same figures on board

        private static readonly int[] SqIndexes120To64;
        private static readonly int[] SqIndexes64To120;
        private static readonly int[] FileBoard;
        private static readonly int[] RankBoard;

        static Board()
        {
            SqIndexes120To64 = new int[120];
            SqIndexes64To120 = new int[64];
            InitSqIndexes();

            FileBoard = new int[BoardSquaresNumber];
            RankBoard = new int[BoardSquaresNumber];
            InitFileRankBoards();
        }

        private static void InitSqIndexes()
        {
            for (int i = 0; i < 120; i++) SqIndexes120To64[i] = 64;
            for (int i = 0; i < 64; i++) SqIndexes64To120[i] = 0;

            int currSq, sq64 = 0;
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

        private static void InitFileRankBoards()
        {
            for (int i = 0; i < BoardSquaresNumber; ++i)
            {
                FileBoard[i] = (int)Square.None;
                RankBoard[i] = (int)Square.None;
            }

            for (Rank r = Rank.Rank1; r <= Rank.Rank8; ++r)
            {
                for (File f = File.FileA; f <= File.FileH; ++f)
                {
                    int sq = ConvertToSq120(f, r);
                    FileBoard[sq] = (int)f;
                    RankBoard[sq] = (int)r;
                }
            }
        }

        public static int ConvertToSq120(File f, Rank r)
        {
            return ((int)r * 10 + (int)f + 21);
        }

        public static int Sq120(int sq64)
        {
            return SqIndexes64To120[sq64];
        }

        public static int Sq64(int sq120)
        {
            return SqIndexes120To64[sq120];
        }
    }
}
