using Assets.Project.ChessEngine.Exceptions;
using Assets.Project.ChessEngine.Pieces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Project.ChessEngine
{
    public class Board
    {
        #region PublicFields
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

        public LinkedList<UndoMove> History { get; set; } // list with data needed to undo past moves
        #endregion
        #region PublicMethods
        /* Default ctor. */
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

            History = new LinkedList<UndoMove>();
        }
        /* Parses FEN string in order and inits Board with parsed values. */
        public Board(string fen) : this()
        {
            int count;
            File file = File.FileA;
            Rank rank = Rank.Rank8;
            int i = 0;
            PieceType pieceType;

            for (; rank >= Rank.Rank1; ++i)
            {
                count = 1;
                switch (fen[i])
                {
                    case 'p': pieceType = PieceType.BlackPawn; break;
                    case 'n': pieceType = PieceType.BlackKnight; break;
                    case 'b': pieceType = PieceType.BlackBishop; break;
                    case 'r': pieceType = PieceType.BlackRook; break;
                    case 'k': pieceType = PieceType.BlackKing; break;
                    case 'q': pieceType = PieceType.BlackQueen; break;
                    case 'P': pieceType = PieceType.WhitePawn; break;
                    case 'N': pieceType = PieceType.WhiteKnight; break;
                    case 'B': pieceType = PieceType.WhiteBishop; break;
                    case 'R': pieceType = PieceType.WhiteRook; break;
                    case 'K': pieceType = PieceType.WhiteKing; break;
                    case 'Q': pieceType = PieceType.WhiteQueen; break;

                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                        pieceType = PieceType.None;
                        count = fen[i] - '0';
                        break;

                    case '/':
                    case ' ':
                        rank--;
                        file = File.FileA;
                        continue;

                    default:
                        throw new FENFormatException("FEN parsing error");
                }

                int sq64, sq120;
                for (int j = 0; j < count; ++j)
                {
                    sq64 = (int)rank * 8 + (int)file;
                    sq120 = Sq120(sq64);
                    if (pieceType != PieceType.None)
                    {
                        Pieces[sq120] = Piece.CreatePiece(pieceType, (Square)sq120);
                    }
                    file++;
                }
            }

            OnTurn = (fen[i] == 'w') ? Color.White : Color.Black;
            i += 2;

            for (int j = 0; fen[i] != ' ' && j < 4; ++j, ++i)
            {
                switch (fen[i])
                {
                    case 'k': CastlePerm |= (int)CastlingPermit.BlackKingCastling; break;
                    case 'q': CastlePerm |= (int)CastlingPermit.BlackQueenCastling; break;
                    case 'K': CastlePerm |= (int)CastlingPermit.WhiteKingCastling; break;
                    case 'Q': CastlePerm |= (int)CastlingPermit.WhiteQueenCastling; break;
                    default: break;
                }
            }
            i++;

            if (fen[i] != '-')
            {
                file = (File)(fen[i] - 'a');
                rank = (Rank)(fen[i + 1] - '1');

                EnPassant = (Square)ConvertToSq120(file, rank);
            }

            StateKey = CalculateStateKey();

            UpdateListsAndMaterial();
        }
        /* Updates tracked data of importance based on current board pieces. It should be called after any board state change is made. */
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
                        Pawns[(int)piece.Color].SetBit((Square)i);
                        Pawns[(int)Color.Both].SetBit((Square)i);
                    }

                    Material[(int)piece.Color] += piece.Value;
                }
            }
        }
        /* Checks if tracked data of importance is valid based on current board pieces. It's used for integrity testing. */
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

            if (CalculateStateKey() != StateKey)
            {
                throw new BoardIntegrityException("Expected state key " + CalculateStateKey() + ", found " + StateKey);
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
        /* Checks if provided square is attacked by provided side. */
        public bool IsSquareAttacked(Square sq, Color side)
        {
            //CheckIntegrity(); 

            // pawns
            if (side == Color.White)
            {
                Piece piece = Pieces[(int)sq - 11];
                if (piece is Pawn && piece.Color == Color.White) return true;
                piece = Pieces[(int)sq - 9];
                if (piece is Pawn && piece.Color == Color.White) return true;
            }
            else
            {
                Piece piece = Pieces[(int)sq + 11];
                if (piece is Pawn && piece.Color == Color.Black) return true;
                piece = Pieces[(int)sq + 9];
                if (piece is Pawn && piece.Color == Color.Black) return true;
            }

            // knights
            for (int i = 0; i < 8; ++i)
            {
                Piece piece = Pieces[(int)sq + KnDirection[i]];
                if (piece is Knight && piece.Color == side)
                {
                    return true;
                }
            }

            // rooks, queens
            for (int i = 0; i < 4; ++i)
            {
                int direction = RkDirection[i];
                int tempSq = (int)sq + direction;
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
                int tempSq = (int)sq + direction;
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
                Piece piece = Pieces[(int)sq + KiDirection[i]];
                if (piece is King && piece.Color == side)
                {
                    return true;
                }
            }

            return false;
        }
        /* Returns string in human readable format. */
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

        #endregion
        #region StaticFields
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

        public static readonly int[] KnDirection = { -8, -19, -21, -12, 8, 19, 21, 12 };
        public static readonly int[] RkDirection = { -1, -10, 1, 10 };
        public static readonly int[] BiDirection = { -9, -11, 11, 9 };
        public static readonly int[] KiDirection = { -1, -10, 1, 10, -9, -11, 11, 9 };
        #endregion
        #region StaticMethods
        static Board()
        {
            SqIndexes120To64 = new int[120];
            SqIndexes64To120 = new int[64];
            InitSqIndexes();

            FileBoard = new int[BoardSquaresNumber];
            RankBoard = new int[BoardSquaresNumber];
            InitFileRankBoards();
            InitHashGenerator();
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

        private static void InitHashGenerator()
        {
            pieceKeys = new ulong[PieceTypesCount, BoardSquaresNumber];
            castleKeys = new ulong[16];
            for (int i = 0; i < PieceTypesCount; ++i)
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

        public static Rank GetRank(Square square)
        {
            return (Rank)RankBoard[(int)square];
        }
        #endregion
        #region HashGenerator
        private static ulong[,] pieceKeys; // 13x120
        private static ulong sideKey;
        private static ulong[] castleKeys; // 16 (4 bit representation 0-15 values)
        private static Random rnd = new Random();

        public ulong CalculateStateKey()
        {
            ulong finalKey = 0;
            Piece piece;

            for (int sq = 0; sq < BoardSquaresNumber; ++sq)
            {
                piece = Pieces[sq];
                if (piece != null && !(piece is OffLimits))
                {
                    finalKey ^= pieceKeys[(int)piece.GetPieceType(), sq];
                }
            }

            if (OnTurn == Color.White) finalKey ^= sideKey;

            if (EnPassant != Square.None) finalKey ^= pieceKeys[0, (byte)EnPassant];

            finalKey ^= castleKeys[CastlePerm];

            return finalKey;
        }

        private void HashPiece(Piece piece, Square square)
        {
            StateKey ^= pieceKeys[(int)piece.GetPieceType(), (int)square];
        }

        private void HashCastle()
        {
            StateKey ^= castleKeys[CastlePerm];
        }

        private void HashSide()
        {
            StateKey ^= sideKey;
        }

        private void HashEnPassant()
        {
            StateKey ^= pieceKeys[(int)PieceType.None, (int)EnPassant];
        }

        private static ulong Get64BitRandom(ulong minValue = ulong.MinValue, ulong maxValue = ulong.MaxValue)
        {
            byte[] buf = new byte[8];
            rnd.NextBytes(buf);
            ulong longRand = BitConverter.ToUInt64(buf, 0);
            return longRand % (maxValue - minValue) + minValue;
        }
        #endregion
        #region MoveGenerator
        private static readonly PieceType[] LoopSlidePieces =
        {
            PieceType.WhiteBishop, PieceType.WhiteRook, PieceType.WhiteQueen, PieceType.None,
            PieceType.BlackBishop, PieceType.BlackRook, PieceType.BlackQueen, PieceType.None
        };

        private static readonly PieceType[] LoopNonSlidePieces =
        {
            PieceType.WhiteKnight, PieceType.WhiteKing, PieceType.None,
            PieceType.BlackKnight, PieceType.BlackKing, PieceType.None
        };

        private static readonly int[] StartLoopSlideIndex = { 0, 4 };
        private static readonly int[] StartLoopNonSlideIndex = { 0, 3 };

        private static readonly int[,] PieceDirection = {
            { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { -8, -19,  -21, -12, 8, 19, 21, 12, 0 },
            { -9, -11, 11, 9, 0, 0, 0, 0, 0 },
            { -1, -10,  1, 10, 0, 0, 0, 0, 0 },
            { -1, -10,  1, 10, -9, -11, 11, 9, 0 },
            { -1, -10,  1, 10, -9, -11, 11, 9, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { -8, -19,  -21, -12, 8, 19, 21, 12, 0 },
            { -9, -11, 11, 9, 0, 0, 0, 0, 0 },
            { -1, -10,  1, 10, 0, 0, 0, 0, 0 },
            { -1, -10,  1, 10, -9, -11, 11, 9, 0 },
            { -1, -10,  1, 10, -9, -11, 11, 9, 0 }
        };

        public MoveList GenerateAllMoves()
        {
            //CheckIntegrity();

            MoveList moveList = new MoveList();
            Move move;

            if (OnTurn == Color.White)
            {
                for (int i = 0; i < PieceCount[(int)PieceType.WhitePawn]; ++i)
                {
                    Square square = PieceList[(int)PieceType.WhitePawn, i];

                    if (Pieces[(int)square + 10] == null)
                    {
                        moveList.AddPawnQuietMove(OnTurn, square, square + 10);
                        if (GetRank(square) == Rank.Rank2 && Pieces[(int)square + 20] == null)
                        {
                            move = new Move
                            {
                                FromSq = square,
                                ToSq = square + 20,
                                IsPawnStart = true
                            };
                            moveList.AddQuietMove(move);
                        }
                    }

                    if (Pieces[(int)square + 9]?.Color == Color.Black)
                    {
                        moveList.AddPawnCaptureMove(OnTurn, square, square + 9, Pieces[(int)square + 9].GetPieceType());
                    }
                    if (Pieces[(int)square + 11]?.Color == Color.Black)
                    {
                        moveList.AddPawnCaptureMove(OnTurn, square, square + 11, Pieces[(int)square + 11].GetPieceType());
                    }

                    if (EnPassant != Square.None)
                    {
                        if (square + 9 == EnPassant)
                        {
                            move = new Move
                            {
                                FromSq = square,
                                ToSq = square + 9,
                                IsEnPassant = true
                            };
                            moveList.AddEnPassantMove(move);
                        }
                        if (square + 11 == EnPassant)
                        {
                            move = new Move
                            {
                                FromSq = square,
                                ToSq = square + 11,
                                IsEnPassant = true
                            };
                            moveList.AddEnPassantMove(move);
                        }
                    }
                }

                if ((CastlePerm & (int)CastlingPermit.WhiteKingCastling) > 0)
                {
                    if (Pieces[(int)Square.F1] == null && Pieces[(int)Square.G1] == null)
                    {
                        if (!IsSquareAttacked(Square.E1, Color.Black) && !IsSquareAttacked(Square.F1, Color.Black))
                        {
                            move = new Move
                            {
                                FromSq = Square.E1,
                                ToSq = Square.G1,
                                IsCastle = true
                            };
                            moveList.AddQuietMove(move);
                        }
                    }
                }

                if ((CastlePerm & (int)CastlingPermit.WhiteQueenCastling) > 0)
                {
                    if (Pieces[(int)Square.D1] == null && Pieces[(int)Square.C1] == null && Pieces[(int)Square.B1] == null)
                    {
                        if (!IsSquareAttacked(Square.E1, Color.Black) && !IsSquareAttacked(Square.D1, Color.Black))
                        {
                            move = new Move
                            {
                                FromSq = Square.E1,
                                ToSq = Square.C1,
                                IsCastle = true
                            };
                            moveList.AddQuietMove(move);
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < PieceCount[(int)PieceType.BlackPawn]; ++i)
                {
                    Square square = PieceList[(int)PieceType.BlackPawn, i];

                    if (Pieces[(int)square - 10] == null)
                    {
                        moveList.AddPawnQuietMove(OnTurn, square, square - 10);
                        if (GetRank(square) == Rank.Rank7 && Pieces[(int)square - 20] == null)
                        {
                            move = new Move
                            {
                                FromSq = square,
                                ToSq = square - 20,
                                IsPawnStart = true
                            };
                            moveList.AddQuietMove(move);
                        }
                    }

                    if (Pieces[(int)square - 9]?.Color == Color.White)
                    {
                        moveList.AddPawnCaptureMove(OnTurn, square, square - 9, Pieces[(int)square - 9].GetPieceType());
                    }
                    if (Pieces[(int)square - 11]?.Color == Color.White)
                    {
                        moveList.AddPawnCaptureMove(OnTurn, square, square - 11, Pieces[(int)square - 11].GetPieceType());
                    }

                    if (EnPassant != Square.None)
                    {
                        if (square - 9 == EnPassant)
                        {
                            move = new Move
                            {
                                FromSq = square,
                                ToSq = square - 9,
                                IsEnPassant = true
                            };
                            moveList.AddEnPassantMove(move);
                        }
                        if (square - 11 == EnPassant)
                        {
                            move = new Move
                            {
                                FromSq = square,
                                ToSq = square - 11,
                                IsEnPassant = true
                            };
                            moveList.AddEnPassantMove(move);
                        }
                    }
                }

                if ((CastlePerm & (int)CastlingPermit.BlackKingCastling) > 0)
                {
                    if (Pieces[(int)Square.F8] == null && Pieces[(int)Square.G8] == null)
                    {
                        if (!IsSquareAttacked(Square.E8, Color.White) && !IsSquareAttacked(Square.F8, Color.White))
                        {
                            move = new Move
                            {
                                FromSq = Square.E8,
                                ToSq = Square.G8,
                                IsCastle = true
                            };
                            moveList.AddQuietMove(move);
                        }
                    }
                }

                if ((CastlePerm & (int)CastlingPermit.BlackQueenCastling) > 0)
                {
                    if (Pieces[(int)Square.D8] == null && Pieces[(int)Square.C8] == null && Pieces[(int)Square.B8] == null)
                    {
                        if (!IsSquareAttacked(Square.E8, Color.White) && !IsSquareAttacked(Square.D8, Color.White))
                        {
                            move = new Move
                            {
                                FromSq = Square.E8,
                                ToSq = Square.C8,
                                IsCastle = true
                            };
                            moveList.AddQuietMove(move);
                        }
                    }
                }
            }

            int pieceIndex = StartLoopSlideIndex[(int)OnTurn];
            PieceType pieceType = LoopSlidePieces[pieceIndex++];
            while (pieceType != PieceType.None)
            {
                for (int i = 0; i < PieceCount[(int)pieceType]; ++i)
                {
                    Square square = PieceList[(int)pieceType, i];

                    int index = 0;
                    int direction = PieceDirection[(int)pieceType, index++];
                    while (direction != 0)
                    {
                        Square tempSq = square + direction;
                        Piece piece = Pieces[(int)tempSq];

                        while (!(piece is OffLimits))
                        {
                            if (piece != null)
                            {
                                if (piece.Color == (Color)((int)OnTurn ^ 1)) // xor operation gives opposite side as result
                                {
                                    move = new Move
                                    {
                                        FromSq = square,
                                        ToSq = tempSq,
                                        CapturedPiece = piece.GetPieceType()
                                    };
                                    moveList.AddCaptureMove(move);
                                }
                                break;
                            }
                            move = new Move
                            {
                                FromSq = square,
                                ToSq = tempSq
                            };
                            moveList.AddQuietMove(move);
                            tempSq += direction;
                            piece = Pieces[(int)tempSq];
                        }
                        direction = PieceDirection[(int)pieceType, index++];
                    }
                }
                pieceType = LoopSlidePieces[pieceIndex++];
            }

            /* Loop for non slide */
            pieceIndex = StartLoopNonSlideIndex[(int)OnTurn];
            pieceType = LoopNonSlidePieces[pieceIndex++];

            while (pieceType != PieceType.None)
            {
                for (int i = 0; i < PieceCount[(int)pieceType]; ++i)
                {
                    Square square = PieceList[(int)pieceType, i];

                    int index = 0;
                    int direction = PieceDirection[(int)pieceType, index++];
                    while (direction != 0)
                    {
                        Square tempSq = square + direction;
                        Piece piece = Pieces[(int)tempSq];

                        if (piece is OffLimits)
                        {
                            direction = PieceDirection[(int)pieceType, index++];
                            continue;
                        }

                        if (piece != null)
                        {
                            if (piece.Color == (Color)((int)OnTurn ^ 1)) // xor operation gives opposite side as result
                            {
                                move = new Move
                                {
                                    FromSq = square,
                                    ToSq = tempSq,
                                    CapturedPiece = piece.GetPieceType()
                                };
                                moveList.AddCaptureMove(move);
                            }
                            direction = PieceDirection[(int)pieceType, index++];
                            continue;
                        }
                        move = new Move
                        {
                            FromSq = square,
                            ToSq = tempSq
                        };
                        moveList.AddQuietMove(move);
                        direction = PieceDirection[(int)pieceType, index++];
                    }
                }
                pieceType = LoopNonSlidePieces[pieceIndex++];
            }

            return moveList;
        }
        #endregion
        #region MoveHandler
        private readonly int[] castlePermXorValues = {
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15, 13, 15, 15, 15, 12, 15, 15, 14, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15,  7, 15, 15, 15,  3, 15, 15, 11, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15
        };

        private void AddPiece(PieceType pieceType, Square square)
        {
            Piece piece = Piece.CreatePiece(pieceType, square);

            HashPiece(piece, square);

            Pieces[(int)square] = piece;

            if (piece.IsBig())
            {
                ++BigPiecesCount[(int)piece.Color];
                if (piece.IsMajor())
                {
                    ++MajorPiecesCount[(int)piece.Color];
                }
                else
                {
                    ++MinorPiecesCount[(int)piece.Color];
                }
            }
            else
            {
                Pawns[(int)piece.Color].SetBit(square);
                Pawns[(int)Color.Both].SetBit(square);
            }

            Material[(int)piece.Color] += piece.Value;
            PieceList[(int)piece.GetPieceType(), PieceCount[(int)piece.GetPieceType()]++] = square;

        }

        private void RemovePiece(Square square)
        {
            Piece piece = Pieces[(int)square];
            
            HashPiece(piece, square);

            Pieces[(int)square] = null;
            Material[(int)piece.Color] -= piece.Value;

            if (piece.IsBig())
            {
                --BigPiecesCount[(int)piece.Color];
                if (piece.IsMajor())
                {
                    --MajorPiecesCount[(int)piece.Color];
                }
                else
                {
                    --MinorPiecesCount[(int)piece.Color];
                }
            }
            else
            {
                Pawns[(int)piece.Color].ClearBit(square);
                Pawns[(int)Color.Both].ClearBit(square);
            }
            
            int indexOfRemovedPiece = -1;
            for (int i = 0; i < PieceCount[(int)piece.GetPieceType()]; ++i)
            {
                if (PieceList[(int)piece.GetPieceType(), i] == square)
                {
                    indexOfRemovedPiece = i;
                    break;
                }
            }

            /* Situation: remove white pawn at square 62
             * 
             * WhitePawn PieceList contains (0) -> 61, (1) -> 62, (2) -> 63, (3) -> 64
             * WhitePawn PieceCount is 4
             * 
             * Decrement PieceCount
             * WhitePawn PieceCount is 3
             * Set (1) -> 64
             * 
             * WhitePawn PieceList contains (0) -> 61, (1) -> 64, (2) -> 63
             */
            --PieceCount[(int)piece.GetPieceType()];
            PieceList[(int)piece.GetPieceType(), indexOfRemovedPiece] = PieceList[(int)piece.GetPieceType(), PieceCount[(int)piece.GetPieceType()]];
        }

        private void MovePiece(Square from, Square to)
        {
            Piece piece = Pieces[(int)from];

            HashPiece(piece, from);
            Pieces[(int)from] = null;

            HashPiece(piece, to);
            Pieces[(int)to] = piece;

            if (!piece.IsBig())
            {
                Pawns[(int)piece.Color].ClearBit(from);
                Pawns[(int)Color.Both].ClearBit(from);
                Pawns[(int)piece.Color].SetBit(to);
                Pawns[(int)Color.Both].SetBit(to);
            }

            for (int i = 0; i < PieceCount[(int)piece.GetPieceType()]; ++i)
            {
                if (PieceList[(int)piece.GetPieceType(), i] == from)
                {
                    PieceList[(int)piece.GetPieceType(), i] = to;
                    break;
                }
            }
        }

        public bool DoMove(Move move)
        {
            //CheckIntegrity();

            UndoMove undoMoveData = new UndoMove();

            History.AddLast(undoMoveData);

            undoMoveData.StateKey = StateKey;

            if (move.IsEnPassant)
            {
                if (OnTurn == Color.White)
                {
                    RemovePiece(move.ToSq - 10);
                }
                else
                {
                    RemovePiece(move.ToSq + 10);
                }
            }
            else if (move.IsCastle)
            {
                switch (move.ToSq)
                {
                    case Square.C1:
                        MovePiece(Square.A1, Square.D1);
                        break;
                    case Square.C8:
                        MovePiece(Square.A8, Square.D8);
                        break;
                    case Square.G1:
                        MovePiece(Square.H1, Square.F1);
                        break;
                    case Square.G8:
                        MovePiece(Square.H8, Square.F8);
                        break;
                    default:
                        throw new IllegalArgumentException("Castling ToSq must have one of values C1, C8, G1, G8.");
                }
            }

            if (EnPassant != Square.None) HashEnPassant();
            HashCastle();

            undoMoveData.Move = move;
            undoMoveData.FiftyMove = FiftyMove;
            undoMoveData.EnPassant = EnPassant;
            undoMoveData.CastlePerm = CastlePerm;

            CastlePerm &= castlePermXorValues[(int)move.FromSq];
            CastlePerm &= castlePermXorValues[(int)move.ToSq];
            EnPassant = Square.None;

            HashCastle();

            FiftyMove++;

            if (move.CapturedPiece != (int)PieceType.None)
            {
                RemovePiece(move.ToSq);
                FiftyMove = 0;
            }

            ++HistoryPly;
            ++Ply;

            if (Pieces[(int)move.FromSq].GetPieceType().IsPawn())
            {
                FiftyMove = 0;
                if (move.IsPawnStart)
                {
                    if (OnTurn == Color.White)
                    {
                        EnPassant = move.FromSq + 10;
                    }
                    else
                    {
                        EnPassant = move.FromSq - 10;
                    }
                    HashEnPassant();
                }
            }

            MovePiece(move.FromSq, move.ToSq);
            
            if (move.PromotedPiece != PieceType.None)
            {
                RemovePiece(move.ToSq);
                AddPiece(move.PromotedPiece, move.ToSq);
            }

            if (Pieces[(int)move.ToSq].GetPieceType().IsKing())
            {
                Kings[(int)OnTurn] = move.ToSq;
            }

            OnTurn = (Color)((int)OnTurn ^ 1);
            HashSide();

            if (IsSquareAttacked(Kings[(int)OnTurn ^ 1], OnTurn))
            {
                UndoMove();
                return false;
            }

            return true;
        }

        public void UndoMove()
        {
            --HistoryPly;
            --Ply;

            UndoMove undoMoveData = History.Last.Value;
            History.RemoveLast();

            if (EnPassant != Square.None) HashEnPassant();
            HashCastle();

            CastlePerm = undoMoveData.CastlePerm;
            FiftyMove = undoMoveData.FiftyMove;
            EnPassant = undoMoveData.EnPassant;

            if (EnPassant != Square.None) HashEnPassant();
            HashCastle();

            OnTurn = (Color)((int)OnTurn ^ 1);
            HashSide();

            if (undoMoveData.Move.IsEnPassant)
            {
                if (OnTurn == (int)Color.White)
                {
                    AddPiece(PieceType.BlackPawn, undoMoveData.Move.ToSq - 10);
                }
                else
                {
                    AddPiece(PieceType.WhitePawn, undoMoveData.Move.ToSq + 10);
                }
            }
            else if (undoMoveData.Move.IsCastle)
            {
                switch (undoMoveData.Move.ToSq)
                {
                    case Square.C1:
                        MovePiece(Square.D1, Square.A1);
                        break;
                    case Square.C8:
                        MovePiece(Square.D8, Square.A8);
                        break;
                    case Square.G1:
                        MovePiece(Square.F1, Square.H1);
                        break;
                    case Square.G8:
                        MovePiece(Square.F8, Square.H8);
                        break;
                    default: throw new IllegalArgumentException("UndoMove Castling Error.");
                }
            }

            MovePiece(undoMoveData.Move.ToSq, undoMoveData.Move.FromSq);

            if (Pieces[(int)undoMoveData.Move.FromSq].GetPieceType().IsKing())
            {
                Kings[(int)OnTurn] = undoMoveData.Move.FromSq;
            }
            
            if (undoMoveData.Move.CapturedPiece != PieceType.None)
            {
                AddPiece(undoMoveData.Move.CapturedPiece, undoMoveData.Move.ToSq);
            }

            if (undoMoveData.Move.PromotedPiece != PieceType.None)
            {
                RemovePiece(undoMoveData.Move.FromSq);
                AddPiece(undoMoveData.Move.PromotedPiece.GetColor() == Color.White ? PieceType.WhitePawn : PieceType.BlackPawn, undoMoveData.Move.FromSq);
            }
            
            //CheckIntegrity();
        }
        #endregion
    }
}
