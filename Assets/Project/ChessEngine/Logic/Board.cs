using Assets.Project.ChessEngine.Exceptions;
using Assets.Project.ChessEngine.Pieces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Assets.Project.ChessEngine
{
    public class Board
    {
        #region Properties
        private PieceFactory pieceFactory;
        public Piece[] Pieces { get; set; } // 120 board rep
        public Bitboard[] Pawns { get; set; }
        public Square[] Kings { get; set; }
        public PieceList PieceList { get; set; }
        public int[] Material { get; set; }

        public Color OnTurn { get; set; }
        public int FiftyMove { get; set; }
        public Square EnPassant { get; set; }
        public int CastlePerm { get; set; }

        public int HistoryPly { get; set; } // number of halfmoves in the game so far
        public int Ply { get; set; } // number of halfmoves in the move search so far
        public ulong StateKey { get; set; } // board state hash key

        public LinkedList<UndoMove> History { get; set; } // list with data needed to undo past moves
        public PvTable PvTable { get; set; }
        public List<Move> PvMoves { get; set; }
        #endregion

        #region Methods
        /* Default ctor. */
        public Board()
        {
            pieceFactory = new PieceFactory();

            Pieces = new Piece[Constants.BoardSquareCount];
            for (int i = 0; i < Constants.BoardSquareCount; ++i) Pieces[i] = OffLimits.Instance;
            for (int i = 0; i < 64; ++i) Pieces[SqIndexes64To120[i]] = null;

            Pawns = new Bitboard[Constants.ColorWBB];
            for (int i = 0; i < Constants.ColorWBB; ++i) Pawns[i] = new Bitboard();

            Kings = new Square[Constants.ColorWB];
            Kings[(int)Color.White] = Kings[(int)Color.Black] = Square.None;

            Material = new int[2];
            PieceList = new PieceList();

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
            try
            {
                int count;
                File file = File.FileA;
                Rank rank = Rank.Rank8;
                int i = 0;

                char? pieceLabel = null;

                for (; rank >= Rank.Rank1; ++i)
                {
                    count = 1;
                    switch (fen[i])
                    {
                        case 'p':
                        case 'n':
                        case 'b':
                        case 'r':
                        case 'k':
                        case 'q':
                        case 'P':
                        case 'N':
                        case 'B':
                        case 'R':
                        case 'K':
                        case 'Q':
                            pieceLabel = fen[i];
                            break;

                        case '1':
                        case '2':
                        case '3':
                        case '4':
                        case '5':
                        case '6':
                        case '7':
                        case '8':
                            pieceLabel = null;
                            count = fen[i] - '0';
                            break;

                        case '/':
                        case ' ':
                            rank--;
                            file = File.FileA;
                            continue;

                        default:
                            throw new FENFormatException("FEN parsing error.");
                    }

                    int sq64, sq120;
                    for (int j = 0; j < count; ++j)
                    {
                        sq64 = (int)rank * 8 + (int)file;
                        sq120 = Sq120(sq64);
                        if (pieceLabel.HasValue)
                        {
                            int pieceIndex = Piece.GetIndexFromPieceLabel(pieceLabel.Value);
                            Pieces[sq120] = pieceFactory.CreatePiece(pieceIndex, (Square)sq120);
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
            catch (Exception)
            {
                throw new FENFormatException("FEN parsing error. Try again.");
            }
        }
        /* Updates tracked data of importance based on current board pieces. It should be called after any board state change is made. */
        public void UpdateListsAndMaterial()
        {
            for (int i = 0; i < Constants.BoardSquareCount; ++i)
            {
                Piece piece = Pieces[i];
                if (piece != null && !(piece is OffLimits))
                {
                    PieceList.AddPiece(piece);
                    piece.Square = (Square)i;

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
            PieceList tempPieceList = new PieceList();
            int[] TempMaterial = new int[2];

            for (int i = 1; i < Constants.PieceTypeCount; ++i)
            {
                foreach (Piece piece in PieceList.GetList(i))
                {
                    if (!piece.Index.Equals(i)) throw new BoardIntegrityException();
                    Piece tablePiece = Pieces[(int)piece.Square];
                    if (!tablePiece.Index.Equals(i)) throw new BoardIntegrityException();
                    if (tablePiece.Color != piece.Color) throw new BoardIntegrityException();
                }
            }

            for (int sq64 = 0; sq64 < 64; ++sq64)
            {
                int sq120 = Sq120(sq64);
                Piece piece = Pieces[sq120];
                if (piece == null || piece is OffLimits) continue;

                tempPieceList.AddPiece(piece);

                TempMaterial[(int)piece.Color] += piece.Value;
            }

            for (int i = 1; i < Constants.PieceTypeCount; ++i)
            {
                if (tempPieceList.GetPieceCount(i) != PieceList.GetPieceCount(i)) throw new BoardIntegrityException();
            }

            if (Pawns[(int)Color.White].CountBit() != PieceList.GetPieceCount(Pawn.GetIndex(Color.White))
                ||
                Pawns[(int)Color.Black].CountBit() != PieceList.GetPieceCount(Pawn.GetIndex(Color.Black))
                ||
                Pawns[(int)Color.Both].CountBit() != (PieceList.GetPieceCount(Pawn.GetIndex(Color.White)) + PieceList.GetPieceCount(Pawn.GetIndex(Color.Black))))
            {
                throw new BoardIntegrityException("Bitboard pawn count inequality");
            }

            for (int sq64 = 0; sq64 < 64; ++sq64)
            {
                Piece piece = Pieces[Sq120(sq64)];
                if (Pawns[(int)Color.White].IsSet(sq64) && (piece.Color != Color.White || !(piece is Pawn)))
                    throw new BoardIntegrityException("Bitboard bit set on square " + sq64 + ", unexpected square content " + piece.GetType() + " found");
                if (Pawns[(int)Color.Black].IsSet(sq64) && (piece.Color != Color.Black || !(piece is Pawn)))
                    throw new BoardIntegrityException("Bitboard bit set on square " + sq64 + ", unexpected square content " + piece.GetType() + " found");
                if (Pawns[(int)Color.Both].IsSet(sq64) && (!(piece is Pawn)))
                    throw new BoardIntegrityException("Bitboard bit set on square " + sq64 + ", unexpected square content " + piece.GetType() + " found");
            }

            if (TempMaterial[(int)Color.White] != Material[(int)Color.White]
                ||
                TempMaterial[(int)Color.Black] != Material[(int)Color.Black])
            {
                throw new BoardIntegrityException("Material inequality");
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
                throw new BoardIntegrityException("EnPassant square illegal.");
            }

            Piece whiteKing = Pieces[(int)Kings[(int)Color.White]];
            Piece blackKing = Pieces[(int)Kings[(int)Color.Black]];
            if (whiteKing.Color != Color.White || !(whiteKing is King) || blackKing.Color != Color.Black || !(blackKing is King))
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
                    sb.Append(piece != null ? piece.Label : '-');
                }
                sb.Append(Environment.NewLine);
            }
            sb.Append(Environment.NewLine + "    ");
            for (File file = File.FileA; file <= File.FileH; ++file)
            {
                sb.Append(file.GetLabel());
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

        #region Static
        private static readonly int[] SqIndexes120To64;
        private static readonly int[] SqIndexes64To120;
        private static readonly int[] FileBoard;
        private static readonly int[] RankBoard;

        private static readonly int[] KnDirection = { -8, -19, -21, -12, 8, 19, 21, 12 };
        private static readonly int[] RkDirection = { -1, -10, 1, 10 };
        private static readonly int[] BiDirection = { -9, -11, 11, 9 };
        private static readonly int[] KiDirection = { -1, -10, 1, 10, -9, -11, 11, 9 };

        static Board()
        {
            SqIndexes120To64 = new int[120];
            SqIndexes64To120 = new int[64];
            InitSqIndexes();

            FileBoard = new int[Constants.BoardSquareCount];
            RankBoard = new int[Constants.BoardSquareCount];
            InitFileRankBoards();
            InitHashGenerator();
            InitMvvLva();
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
            for (int i = 0; i < Constants.BoardSquareCount; ++i)
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

        public static Rank GetRank(Square square)
        {
            return (Rank)RankBoard[(int)square];
        }
        #endregion

        #region HashGenerator
        private static ulong[,] pieceKeys; // 13x120 12 + 1 en passant
        private static ulong sideKey; // black or white
        private static ulong[] castleKeys; // 16 (4 bit representation 0-15 values)
        private static Random rnd = new Random();

        private static void InitHashGenerator()
        {
            pieceKeys = new ulong[Constants.PieceTypeCount, Constants.BoardSquareCount];
            for (int i = 0; i < Constants.PieceTypeCount; ++i) // 0 index for en passant
            {
                for (int j = 0; j < Constants.BoardSquareCount; ++j)
                {
                    pieceKeys[i, j] = Get64BitRandom();
                }
            }

            sideKey = Get64BitRandom();
            castleKeys = new ulong[16];

            for (int i = 0; i < 16; ++i)
            {
                castleKeys[i] = Get64BitRandom();
            }
        }
        public ulong CalculateStateKey()
        {
            ulong finalKey = 0;
            Piece piece;

            for (int sq = 0; sq < Constants.BoardSquareCount; ++sq)
            {
                piece = Pieces[sq];
                if (piece != null && !(piece is OffLimits))
                {
                    finalKey ^= pieceKeys[piece.Index, sq];
                }
            }

            if (OnTurn == Color.White) finalKey ^= sideKey;

            if (EnPassant != Square.None)
            {
                finalKey ^= pieceKeys[0, (int)EnPassant]; // 0 index for en passant
            }

            finalKey ^= castleKeys[CastlePerm];

            return finalKey;
        }
        private void HashPiece(Piece piece, Square square)
        {
            StateKey ^= pieceKeys[piece.Index, (int)square];
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
            StateKey ^= pieceKeys[0, (int)EnPassant];
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
        private static readonly int[] StartLoopSlideIndex = { 0, 4 };
        private static readonly int[] StartLoopNonSlideIndex = { 0, 3 };
        private static readonly int?[] LoopSlidePieces =
        {
            Bishop.GetIndex(Color.White), Rook.GetIndex(Color.White), Queen.GetIndex(Color.White), null,
            Bishop.GetIndex(Color.Black), Rook.GetIndex(Color.Black), Queen.GetIndex(Color.Black), null,
        };
        private static readonly int?[] LoopNonSlidePieces =
        {
            Knight.GetIndex(Color.White), King.GetIndex(Color.White), null,
            Knight.GetIndex(Color.Black), King.GetIndex(Color.Black), null,
        };
        private static readonly int[,] PieceDirection = {
            { -9, -11, 11, 9, 0, 0, 0, 0, 0 },
            { -1, -10,  1, 10, 0, 0, 0, 0, 0 },
            { -1, -10,  1, 10, -9, -11, 11, 9, 0 },
            { -8, -19,  -21, -12, 8, 19, 21, 12, 0 },
            { -1, -10,  1, 10, -9, -11, 11, 9, 0 },
        };

        public MoveList GenerateAllMoves()
        {
            //CheckIntegrity();

            MoveList moveList = new MoveList();
            Move move;

            #region Pawns
            if (OnTurn == Color.White)
            {
                foreach (Piece pawn in PieceList.GetList(Pawn.GetIndex(Color.White)))
                {
                    Square square = pawn.Square;

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

                    if (Pieces[(int)square + 9] != null && Pieces[(int)square + 9].Color == Color.Black)
                    {
                        moveList.AddPawnCaptureMove(this, OnTurn, square, square + 9, Pieces[(int)square + 9].Index);
                    }
                    if (Pieces[(int)square + 11] != null && Pieces[(int)square + 11].Color == Color.Black)
                    {
                        moveList.AddPawnCaptureMove(this, OnTurn, square, square + 11, Pieces[(int)square + 11].Index);
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
                foreach (Piece pawn in PieceList.GetList(Pawn.GetIndex(Color.Black)))
                {
                    Square square = pawn.Square;

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

                    if (Pieces[(int)square - 9] != null && Pieces[(int)square - 9].Color == Color.White)
                    {
                        moveList.AddPawnCaptureMove(this, OnTurn, square, square - 9, Pieces[(int)square - 9].Index);
                    }
                    if (Pieces[(int)square - 11] != null && Pieces[(int)square - 11].Color == Color.White)
                    {
                        moveList.AddPawnCaptureMove(this, OnTurn, square, square - 11, Pieces[(int)square - 11].Index);
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
            #endregion

            #region Slide
            int pieceIndex = StartLoopSlideIndex[(int)OnTurn];
            int? pieceListIndex = LoopSlidePieces[pieceIndex++];
            while (pieceListIndex.HasValue)
            {
                foreach (Piece currentPiece in PieceList.GetList(pieceListIndex.Value))
                {
                    Square square = currentPiece.Square;

                    int index = 0;
                    int direction = PieceDirection[pieceIndex - 1 - StartLoopSlideIndex[(int)OnTurn], index++];
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
                                        CapturedPiece = piece.Index
                                    };
                                    moveList.AddCaptureMove(this, move);
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
                        direction = PieceDirection[pieceIndex - 1 - StartLoopSlideIndex[(int)OnTurn], index++];
                    }
                }
                pieceListIndex = LoopSlidePieces[pieceIndex++];
            }
            #endregion

            #region NonSlide
            pieceIndex = StartLoopNonSlideIndex[(int)OnTurn];
            pieceListIndex = LoopNonSlidePieces[pieceIndex++];

            while (pieceListIndex.HasValue)
            {
                foreach (Piece currentPiece in PieceList.GetList(pieceListIndex.Value))
                {
                    Square square = currentPiece.Square;

                    int index = 0;
                    int direction = PieceDirection[pieceIndex + 2 - StartLoopNonSlideIndex[(int)OnTurn], index++];
                    while (direction != 0)
                    {
                        Square tempSq = square + direction;
                        Piece piece = Pieces[(int)tempSq];

                        if (piece is OffLimits)
                        {
                            direction = PieceDirection[pieceIndex + 2 - StartLoopNonSlideIndex[(int)OnTurn], index++];
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
                                    CapturedPiece = piece.Index
                                };
                                moveList.AddCaptureMove(this, move);
                            }
                            direction = PieceDirection[pieceIndex + 2 - StartLoopNonSlideIndex[(int)OnTurn], index++];
                            continue;
                        }
                        move = new Move
                        {
                            FromSq = square,
                            ToSq = tempSq
                        };
                        moveList.AddQuietMove(move);
                        direction = PieceDirection[pieceIndex + 2 - StartLoopNonSlideIndex[(int)OnTurn], index++];
                    }
                }
                pieceListIndex = LoopNonSlidePieces[pieceIndex++];
            }
            #endregion

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

        private void AddPiece(int pieceIndex, Square square)
        {
            Piece piece = pieceFactory.CreatePiece(pieceIndex, square);

            HashPiece(piece, square);

            Pieces[(int)square] = piece;
            PieceList.AddPiece(piece);

            if (piece is Pawn)
            {
                Pawns[(int)piece.Color].SetBit(square);
                Pawns[(int)Color.Both].SetBit(square);
            }

            Material[(int)piece.Color] += piece.Value;
        }
        private void RemovePiece(Square square)
        {
            Piece piece = Pieces[(int)square];

            HashPiece(piece, square);

            Pieces[(int)square] = null;
            PieceList.RemovePiece(piece);
            pieceFactory.DeletePiece(piece);

            if (piece is Pawn)
            {
                Pawns[(int)piece.Color].ClearBit(square);
                Pawns[(int)Color.Both].ClearBit(square);
            }

            Material[(int)piece.Color] -= piece.Value;
        }
        private void MovePiece(Square from, Square to)
        {
            Piece piece = Pieces[(int)from];

            HashPiece(piece, from);
            Pieces[(int)from] = null;

            piece.Square = to;

            HashPiece(piece, to);
            Pieces[(int)to] = piece;

            if (piece is Pawn)
            {
                Pawns[(int)piece.Color].ClearBit(from);
                Pawns[(int)Color.Both].ClearBit(from);
                Pawns[(int)piece.Color].SetBit(to);
                Pawns[(int)Color.Both].SetBit(to);
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

            if (move.CapturedPiece != null)
            {
                RemovePiece(move.ToSq);
                FiftyMove = 0;
            }

            ++HistoryPly;
            ++Ply;

            if (Pieces[(int)move.FromSq] is Pawn)
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

            if (move.PromotedPiece.HasValue)
            {
                RemovePiece(move.ToSq);
                AddPiece(move.PromotedPiece.Value, move.ToSq);
            }

            if (Pieces[(int)move.ToSq] is King)
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
                    AddPiece(Pawn.GetIndex(Color.Black), undoMoveData.Move.ToSq - 10);
                }
                else
                {
                    AddPiece(Pawn.GetIndex(Color.White), undoMoveData.Move.ToSq + 10);
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

            if (Pieces[(int)undoMoveData.Move.FromSq] is King)
            {
                Kings[(int)OnTurn] = undoMoveData.Move.FromSq;
            }

            if (undoMoveData.Move.CapturedPiece.HasValue)
            {
                AddPiece(undoMoveData.Move.CapturedPiece.Value, undoMoveData.Move.ToSq);
            }

            if (undoMoveData.Move.PromotedPiece.HasValue)
            {
                RemovePiece(undoMoveData.Move.FromSq);
                Color color = Piece.GetColorFromPieceIndex(undoMoveData.Move.PromotedPiece.Value);
                AddPiece(Pawn.GetIndex(color), undoMoveData.Move.FromSq);
            }

            //CheckIntegrity();
        }

        public bool MoveExists(Move move)
        {
            if (move == null) return false;

            MoveList moveList = GenerateAllMoves();

            foreach (Move currentMove in moveList)
            {
                if (currentMove.Equals(move)) continue;
                if (!DoMove(move))
                {
                    return false;
                }
                else
                {
                    UndoMove();
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region PrincipalVariation
        private Move ProbePvMove()
        {
            PvTableValue value;
            if (PvTable.TryGetValue(StateKey, out value))
            {
                return value.Move;
            }
            else return null;
        }
        private void GetPvLine(int depth)
        {
            Move move = ProbePvMove();
            int count = 0;

            PvMoves = new List<Move>();

            while (move != null && count < depth)
            {
                if (MoveExists(move))
                {
                    DoMove(move);
                    PvMoves.Add(move);
                    count++;
                }
                else break;
                move = ProbePvMove();
            }

            while (count-- > 0)
            {
                UndoMove();
            }
        }
        #endregion

        #region Evaluation
        private static readonly int[] PawnPositionValue = {
        0   ,   0   ,   0   ,   0   ,   0   ,   0   ,   0   ,   0   ,
        10  ,   10  ,   0   ,   -10 ,   -10 ,   0   ,   10  ,   10  ,
        5   ,   0   ,   0   ,   5   ,   5   ,   0   ,   0   ,   5   ,
        0   ,   0   ,   10  ,   20  ,   20  ,   10  ,   0   ,   0   ,
        5   ,   5   ,   5   ,   10  ,   10  ,   5   ,   5   ,   5   ,
        10  ,   10  ,   10  ,   20  ,   20  ,   10  ,   10  ,   10  ,
        20  ,   20  ,   20  ,   30  ,   30  ,   20  ,   20  ,   20  ,
        0   ,   0   ,   0   ,   0   ,   0   ,   0   ,   0   ,   0
        };
        private static readonly int[] KnightPositionValue = {
        0   ,   -10 ,   0   ,   0   ,   0   ,   0   ,   -10 ,   0   ,
        0   ,   0   ,   0   ,   5   ,   5   ,   0   ,   0   ,   0   ,
        0   ,   0   ,   10  ,   10  ,   10  ,   10  ,   0   ,   0   ,
        0   ,   0   ,   10  ,   20  ,   20  ,   10  ,   5   ,   0   ,
        5   ,   10  ,   15  ,   20  ,   20  ,   15  ,   10  ,   5   ,
        5   ,   10  ,   10  ,   20  ,   20  ,   10  ,   10  ,   5   ,
        0   ,   0   ,   5   ,   10  ,   10  ,   5   ,   0   ,   0   ,
        0   ,   0   ,   0   ,   0   ,   0   ,   0   ,   0   ,   0
        };
        private static readonly int[] BishopPositionValue = {
        0   ,   0   ,   -10 ,   0   ,   0   ,   -10 ,   0   ,   0   ,
        0   ,   0   ,   0   ,   10  ,   10  ,   0   ,   0   ,   0   ,
        0   ,   0   ,   10  ,   15  ,   15  ,   10  ,   0   ,   0   ,
        0   ,   10  ,   15  ,   20  ,   20  ,   15  ,   10  ,   0   ,
        0   ,   10  ,   15  ,   20  ,   20  ,   15  ,   10  ,   0   ,
        0   ,   0   ,   10  ,   15  ,   15  ,   10  ,   0   ,   0   ,
        0   ,   0   ,   0   ,   10  ,   10  ,   0   ,   0   ,   0   ,
        0   ,   0   ,   0   ,   0   ,   0   ,   0   ,   0   ,   0
        };
        private static readonly int[] RookPositionValue = {
        0   ,   0   ,   5   ,   10  ,   10  ,   5   ,   0   ,   0   ,
        0   ,   0   ,   5   ,   10  ,   10  ,   5   ,   0   ,   0   ,
        0   ,   0   ,   5   ,   10  ,   10  ,   5   ,   0   ,   0   ,
        0   ,   0   ,   5   ,   10  ,   10  ,   5   ,   0   ,   0   ,
        0   ,   0   ,   5   ,   10  ,   10  ,   5   ,   0   ,   0   ,
        0   ,   0   ,   5   ,   10  ,   10  ,   5   ,   0   ,   0   ,
        25  ,   25  ,   25  ,   25  ,   25  ,   25  ,   25  ,   25  ,
        0   ,   0   ,   5   ,   10  ,   10  ,   5   ,   0   ,   0
        };
        private static readonly int[] Mirror64 = {
        56  ,   57  ,   58  ,   59  ,   60  ,   61  ,   62  ,   63  ,
        48  ,   49  ,   50  ,   51  ,   52  ,   53  ,   54  ,   55  ,
        40  ,   41  ,   42  ,   43  ,   44  ,   45  ,   46  ,   47  ,
        32  ,   33  ,   34  ,   35  ,   36  ,   37  ,   38  ,   39  ,
        24  ,   25  ,   26  ,   27  ,   28  ,   29  ,   30  ,   31  ,
        16  ,   17  ,   18  ,   19  ,   20  ,   21  ,   22  ,   23  ,
        8   ,   9   ,   10  ,   11  ,   12  ,   13  ,   14  ,   15  ,
        0   ,   1   ,   2   ,   3   ,   4   ,   5   ,   6   ,   7
        };

        private int Mirror(int sq64)
        {
            return Mirror64[sq64];
        }
        public int EvaluatePosition()
        {
            int score = Material[(int)Color.White] - Material[(int)Color.Black];

            int pieceIndex;

            //pawns
            pieceIndex = Pawn.GetIndex(Color.White);
            foreach (Piece piece in PieceList.GetList(pieceIndex))
            {
                Square square = piece.Square;
                score += PawnPositionValue[Sq64((int)square)];
            }
            pieceIndex = Pawn.GetIndex(Color.Black);
            foreach (Piece piece in PieceList.GetList(pieceIndex))
            {
                Square square = piece.Square;
                score -= PawnPositionValue[Mirror(Sq64((int)square))];
            }

            //knights
            pieceIndex = Knight.GetIndex(Color.White);
            foreach (Piece piece in PieceList.GetList(pieceIndex))
            {
                Square square = piece.Square;
                score += KnightPositionValue[Sq64((int)square)];
            }
            pieceIndex = Knight.GetIndex(Color.Black);
            foreach (Piece piece in PieceList.GetList(pieceIndex))
            {
                Square square = piece.Square;
                score -= KnightPositionValue[Mirror(Sq64((int)square))];
            }

            //bishops
            pieceIndex = Bishop.GetIndex(Color.White);
            foreach (Piece piece in PieceList.GetList(pieceIndex))
            {
                Square square = piece.Square;
                score += BishopPositionValue[Sq64((int)square)];
            }
            pieceIndex = Bishop.GetIndex(Color.Black);
            foreach (Piece piece in PieceList.GetList(pieceIndex))
            {
                Square square = piece.Square;
                score -= BishopPositionValue[Mirror(Sq64((int)square))];
            }

            //rooks
            pieceIndex = Rook.GetIndex(Color.White);
            foreach (Piece piece in PieceList.GetList(pieceIndex))
            {
                Square square = piece.Square;
                score += RookPositionValue[Sq64((int)square)];
            }
            pieceIndex = Rook.GetIndex(Color.Black);
            foreach (Piece piece in PieceList.GetList(pieceIndex))
            {
                Square square = piece.Square;
                score -= RookPositionValue[Mirror(Sq64((int)square))];
            }

            if (OnTurn == Color.White) return score;
            else return -score;
        }

        private static readonly int[] VictimScore =
        {
            0, 100, 200, 300, 400, 500, 600, 100, 200, 300, 400, 500, 600
        };
        public static readonly int[,] MvvLvaScores = new int[Constants.PieceTypeCount, Constants.PieceTypeCount];

        private static void InitMvvLva()
        {
            for (int vic = 1; vic < Constants.PieceTypeCount; ++vic)
            {
                for (int att = 1; att < Constants.PieceTypeCount; ++att)
                {
                    MvvLvaScores[vic, att] = VictimScore[vic] + 6 - (VictimScore[att] / 100);
                }
            }
            /*for (int vic = 1; vic < Constants.PieceTypeCount; ++vic)
            {
                for (int att = 1; att < Constants.PieceTypeCount; ++att)
                {
                    Console.WriteLine("Attacker: {0} -> Victim {1} = Score {2}", Piece.GetLabelFromPieceIndex(att), Piece.GetLabelFromPieceIndex(vic), MvvLvaScores[vic, att]);
                }
            }*/
        }
        #endregion

        #region Search
        private void CheckUp()
        {
            // .. check if time up, or interrupt from gui
        }
        public bool IsRepetition()
        {
            LinkedListNode<UndoMove> current = History.Last;
            for (int i = FiftyMove; i > 0; --i)
            {
                if (current.Value.StateKey == StateKey) return true;
                current = current.Previous;
            }
            return false;
        }
        private void PrepareForSearch()
        {
            PvTable = new PvTable();
            Ply = 0;
            stopwatch.Reset();
        }

        private Stopwatch stopwatch = new Stopwatch();
        public string SearchPosition(SearchInfo info)
        {
            PrepareForSearch();

            Move bestMove = null;
            int bestScore = -Constants.Infinity;
            
            StringBuilder sb = new StringBuilder();

            stopwatch.Start();
            for (int currentDepth = 1; currentDepth <= info.DepthLimit; ++currentDepth) // iterative deepening
            {
                bestScore = AlphaBeta(-Constants.Infinity, Constants.Infinity, currentDepth, info);
                GetPvLine(currentDepth);
                bestMove = PvMoves.Count > 0 ? PvMoves[0] : null;

                if (currentDepth == info.DepthLimit)
                {
                    stopwatch.Stop();
                    Decimal timeSpent = new Decimal(stopwatch.ElapsedMilliseconds);
                    sb.AppendFormat("Depth: {0} Score: {1} Best move: {2}, Visited {3} nodes in {4}s ", currentDepth, bestScore, bestMove, info.NodesVisited, decimal.Round(timeSpent / 1000, 3));
                    sb.Append("Pv:");
                    foreach (Move move in PvMoves)
                    {
                        sb.AppendFormat(" {0}", move);
                    }
                    sb.Append(Environment.NewLine);
                }
            }
            return sb.ToString();
        }
        private int AlphaBeta(int alpha, int beta, int depth, SearchInfo info)
        {
            ++info.NodesVisited;

            if (depth == 0) return EvaluatePosition();

            if (IsRepetition() || FiftyMove >= 100) return 0; // draw

            if (Ply >= Constants.MaxSearchDepth) return EvaluatePosition();

            int legalMovesCount = 0;
            int oldAlpha = alpha;
            Move bestMove = null;
            int score = -Constants.Infinity;

            MoveList moveList = GenerateAllMoves();

            for (int i = 0; i < moveList.Count; ++i)
            {
                Move move = moveList.PickNextMove(i);
                if (!DoMove(move)) continue;

                ++legalMovesCount;
                score = -AlphaBeta(-beta, -alpha, depth - 1, info);
                UndoMove();

                if (score > alpha)
                {
                    if (score >= beta)
                    {
                        return beta;
                    }
                    alpha = score;
                    bestMove = move;
                }
            }

            if (legalMovesCount == 0)
            {
                if (IsSquareAttacked(Kings[(int)OnTurn], (Color)((int)OnTurn ^ 1)))
                {
                    return -Constants.IsMate + Ply;
                }
                else return 0;
            }

            if (alpha != oldAlpha) { PvTable.StoreHashEntry(this, bestMove, score, depth); }

            return alpha;
        }
        private int Quiescence(int alpha, int beta, SearchInfo info)
        {
            return 0;
        }
        #endregion
    }
}
