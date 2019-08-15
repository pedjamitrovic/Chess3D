using Assets.Project.ChessEngine.Pieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Project.ChessEngine
{
    public static class MoveGenerator
    {
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
	        { -8, -19,	-21, -12, 8, 19, 21, 12, 0 },
	        { -9, -11, 11, 9, 0, 0, 0, 0, 0 },
	        { -1, -10,	1, 10, 0, 0, 0, 0, 0 },
	        { -1, -10,	1, 10, -9, -11, 11, 9, 0 },
	        { -1, -10,	1, 10, -9, -11, 11, 9, 0 },
	        { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
	        { -8, -19,	-21, -12, 8, 19, 21, 12, 0 },
	        { -9, -11, 11, 9, 0, 0, 0, 0, 0 },
	        { -1, -10,	1, 10, 0, 0, 0, 0, 0 },
	        { -1, -10,	1, 10, -9, -11, 11, 9, 0 },
	        { -1, -10,	1, 10, -9, -11, 11, 9, 0 }
        };

        public static MoveList GenerateAllMoves(Board board)
        {
            board.CheckIntegrity(); // TODO: remove CheckIntegrity for perfomance boost

            MoveList moveList = new MoveList();
            Move move;

            if (board.OnTurn == Color.White)
            {
                for (int i = 0; i < board.PieceCount[(int)PieceType.WhitePawn]; ++i)
                {
                    Square square = board.PieceList[(int)PieceType.WhitePawn, i];

                    if (board.Pieces[(int)square + 10] == null)
                    {
                        moveList.AddPawnQuietMove(board.OnTurn, square, square + 10);
                        if (Board.GetRank(square) == Rank.Rank2 && board.Pieces[(int)square + 20] == null)
                        {
                            move = new Move(square, square + 20)
                            {
                                IsPawnStart = true
                            };
                            moveList.AddQuietMove(move);
                        }
                    }

                    if (board.Pieces[(int)square + 9]?.Color == Color.Black)
                    {
                        moveList.AddPawnCaptureMove(board.OnTurn, square, square + 9, board.Pieces[(int)square + 9].GetPieceType());
                    }
                    if (board.Pieces[(int)square + 11]?.Color == Color.Black)
                    {
                        moveList.AddPawnCaptureMove(board.OnTurn, square, square + 11, board.Pieces[(int)square + 11].GetPieceType());
                    }

                    if (board.EnPassant != Square.None)
                    {
                        if (square + 9 == board.EnPassant)
                        {
                            move = new Move(square, square + 9)
                            {
                                IsEnPassant = true
                            };
                            moveList.AddEnPassantMove(move);
                        }
                        if (square + 11 == board.EnPassant)
                        {
                            move = new Move(square, square + 11)
                            {
                                IsEnPassant = true
                            };
                            moveList.AddEnPassantMove(move);
                        }
                    }
                }

                if ((board.CastlePerm & (int)CastlingPermit.WhiteKingCastling) > 0)
                {
                    if (board.Pieces[(int)Square.F1] == null && board.Pieces[(int)Square.G1] == null)
                    {
                        if (!board.IsSquareAttacked((int)Square.E1, Color.Black) && !board.IsSquareAttacked((int)Square.F1, Color.Black))
                        {
                            move = new Move(Square.E1, Square.G1)
                            {
                                IsCastle = true
                            };
                            moveList.AddQuietMove(move);
                        }
                    }
                }

                if ((board.CastlePerm & (int)CastlingPermit.WhiteQueenCastling) > 0)
                {
                    if (board.Pieces[(int)Square.D1] == null && board.Pieces[(int)Square.C1] == null && board.Pieces[(int)Square.B1] == null)
                    {
                        if (!board.IsSquareAttacked((int)Square.E1, Color.Black) && !board.IsSquareAttacked((int)Square.D1, Color.Black))
                        {
                            move = new Move(Square.E1, Square.C1)
                            {
                                IsCastle = true
                            };
                            moveList.AddQuietMove(move);
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < board.PieceCount[(int)PieceType.BlackPawn]; ++i)
                {
                    Square square = board.PieceList[(int)PieceType.BlackPawn, i];

                    if (board.Pieces[(int)square - 10] == null)
                    {
                        moveList.AddPawnQuietMove(board.OnTurn, square, square - 10);
                        if (Board.GetRank(square) == Rank.Rank7 && board.Pieces[(int)square - 20] == null)
                        {
                            move = new Move(square, square - 20)
                            {
                                IsPawnStart = true
                            };
                            moveList.AddQuietMove(move);
                        }
                    }

                    if (board.Pieces[(int)square - 9]?.Color == Color.White)
                    {
                        moveList.AddPawnCaptureMove(board.OnTurn, square, square - 9, board.Pieces[(int)square - 9].GetPieceType());
                    }
                    if (board.Pieces[(int)square - 11]?.Color == Color.White)
                    {
                        moveList.AddPawnCaptureMove(board.OnTurn, square, square - 11, board.Pieces[(int)square - 11].GetPieceType());
                    }

                    if (board.EnPassant != Square.None)
                    {
                        if (square - 9 == board.EnPassant)
                        {
                            move = new Move(square, square - 9)
                            {
                                IsEnPassant = true
                            };
                            moveList.AddEnPassantMove(move);
                        }
                        if (square - 11 == board.EnPassant)
                        {
                            move = new Move(square, square - 11)
                            {
                                IsEnPassant = true
                            };
                            moveList.AddEnPassantMove(move);
                        }
                    }
                }

                if ((board.CastlePerm & (int)CastlingPermit.BlackKingCastling) > 0)
                {
                    if (board.Pieces[(int)Square.F8] == null && board.Pieces[(int)Square.G8] == null)
                    {
                        if (!board.IsSquareAttacked((int)Square.E8, Color.White) && !board.IsSquareAttacked((int)Square.F8, Color.White))
                        {
                            move = new Move(Square.E8, Square.G8)
                            {
                                IsCastle = true
                            };
                            moveList.AddQuietMove(move);
                        }
                    }
                }

                if ((board.CastlePerm & (int)CastlingPermit.BlackQueenCastling) > 0)
                {
                    if (board.Pieces[(int)Square.D8] == null && board.Pieces[(int)Square.C8] == null && board.Pieces[(int)Square.B8] == null)
                    {
                        if (!board.IsSquareAttacked((int)Square.E8, Color.White) && !board.IsSquareAttacked((int)Square.D8, Color.White))
                        {
                            move = new Move(Square.E8, Square.C8)
                            {
                                IsCastle = true
                            };
                            moveList.AddQuietMove(move);
                        }
                    }
                }
            }
            
            int pieceIndex = StartLoopSlideIndex[(int)board.OnTurn];
            PieceType pieceType = LoopSlidePieces[pieceIndex++];
            while (pieceType != PieceType.None)
            {
                for (int i = 0; i < board.PieceCount[(int)pieceType]; ++i)
                {
                    Square square = board.PieceList[(int)pieceType, i];

                    int index = 0;
                    int direction = PieceDirection[(int)pieceType, index++];
                    while (direction != 0)
                    {
                        Square tempSq = square + direction;
                        Piece piece = board.Pieces[(int)tempSq];

                        while (!(piece is OffLimits))
                        {
                            if (piece != null)
                            {
                                if (piece.Color == (Color)((int)board.OnTurn ^ 1)) // xor operation gives opposite side as result
                                {
                                    move = new Move(square, tempSq, piece.GetPieceType());
                                    moveList.AddCaptureMove(move);
                                }
                                break;
                            }
                            move = new Move(square, tempSq);
                            moveList.AddQuietMove(move);
                            tempSq += direction;
                            piece = board.Pieces[(int)tempSq];
                        }
                        direction = PieceDirection[(int)pieceType, index++];
                    }
                }
                pieceType = LoopSlidePieces[pieceIndex++];
            }

            /* Loop for non slide */
            pieceIndex = StartLoopNonSlideIndex[(int)board.OnTurn];
            pieceType = LoopNonSlidePieces[pieceIndex++];

            while (pieceType != PieceType.None)
            {
                for (int i = 0; i < board.PieceCount[(int)pieceType]; ++i)
                {
                    Square square = board.PieceList[(int)pieceType, i];

                    int index = 0;
                    int direction = PieceDirection[(int)pieceType, index++];
                    while (direction != 0)
                    {
                        Square tempSq = square + direction;
                        Piece piece = board.Pieces[(int)tempSq];

                        if (piece is OffLimits)
                        {
                            direction = PieceDirection[(int)pieceType, index++];
                            continue;
                        }
                        
                        if (piece != null)
                        {
                            if (piece.Color == (Color)((int)board.OnTurn ^ 1)) // xor operation gives opposite side as result
                            {
                                move = new Move(square, tempSq, piece.GetPieceType());
                                moveList.AddCaptureMove(move);
                            }
                            direction = PieceDirection[(int)pieceType, index++];
                            continue;
                        }
                        move = new Move(square, tempSq);
                        moveList.AddQuietMove(move);
                        direction = PieceDirection[(int)pieceType, index++];
                    }
                }
                pieceType = LoopNonSlidePieces[pieceIndex++];
            }

            return moveList;
        }
    }
}
