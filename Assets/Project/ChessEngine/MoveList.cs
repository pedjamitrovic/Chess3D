using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Project.ChessEngine
{
    public class MoveList : List<Move>
    {
        public void AddQuietMove(Move move)
        {
            move.Score = 0;
            Add(move);
        }
        public void AddCaptureMove(Move move)
        {
            move.Score = 0;
            Add(move);
        }
        public void AddEnPassantMove(Move move)
        {
            move.Score = 0;
            Add(move);
        }
        public void AddPawnCaptureMove(Color onTurn, Square fromSq, Square toSq, PieceType capturedPiece)
        {
            if (onTurn == Color.White)
            {
                if (Board.GetRank(fromSq) == Rank.Rank7)
                {
                    AddCaptureMove(new Move
                    {
                        FromSq = fromSq,
                        ToSq = toSq,
                        CapturedPiece = capturedPiece,
                        PromotedPiece = PieceType.WhiteQueen
                    });
                    AddCaptureMove(new Move
                    {
                        FromSq = fromSq,
                        ToSq = toSq,
                        CapturedPiece = capturedPiece,
                        PromotedPiece = PieceType.WhiteRook
                    });
                    AddCaptureMove(new Move
                    {
                        FromSq = fromSq,
                        ToSq = toSq,
                        CapturedPiece = capturedPiece,
                        PromotedPiece = PieceType.WhiteBishop
                    });
                    AddCaptureMove(new Move
                    {
                        FromSq = fromSq,
                        ToSq = toSq,
                        CapturedPiece = capturedPiece,
                        PromotedPiece = PieceType.WhiteKnight
                    });
                }
                else
                {
                    AddCaptureMove(new Move
                    {
                        FromSq = fromSq,
                        ToSq = toSq,
                        CapturedPiece = capturedPiece
                    });
                }
            }
            else
            {
                if (Board.GetRank(fromSq) == Rank.Rank2)
                {
                    AddCaptureMove(new Move
                    {
                        FromSq = fromSq,
                        ToSq = toSq,
                        CapturedPiece = capturedPiece,
                        PromotedPiece = PieceType.BlackQueen
                    });
                    AddCaptureMove(new Move
                    {
                        FromSq = fromSq,
                        ToSq = toSq,
                        CapturedPiece = capturedPiece,
                        PromotedPiece = PieceType.BlackRook
                    });
                    AddCaptureMove(new Move
                    {
                        FromSq = fromSq,
                        ToSq = toSq,
                        CapturedPiece = capturedPiece,
                        PromotedPiece = PieceType.BlackBishop
                    });
                    AddCaptureMove(new Move
                    {
                        FromSq = fromSq,
                        ToSq = toSq,
                        CapturedPiece = capturedPiece,
                        PromotedPiece = PieceType.BlackKnight
                    });
                }
                else
                {
                    AddCaptureMove(new Move
                    {
                        FromSq = fromSq,
                        ToSq = toSq,
                        CapturedPiece = capturedPiece
                    });
                }
            }
        }
        public void AddPawnQuietMove(Color onTurn, Square fromSq, Square toSq)
        {
            if (onTurn == Color.White)
            {
                if (Board.GetRank(fromSq) == Rank.Rank7)
                {
                    AddQuietMove(new Move
                    {
                        FromSq = fromSq,
                        ToSq = toSq,
                        PromotedPiece = PieceType.WhiteQueen
                    });
                    AddQuietMove(new Move
                    {
                        FromSq = fromSq,
                        ToSq = toSq,
                        PromotedPiece = PieceType.WhiteRook
                    });
                    AddQuietMove(new Move
                    {
                        FromSq = fromSq,
                        ToSq = toSq,
                        PromotedPiece = PieceType.WhiteBishop
                    });
                    AddQuietMove(new Move
                    {
                        FromSq = fromSq,
                        ToSq = toSq,
                        PromotedPiece = PieceType.WhiteKnight
                    });
                }
                else
                {
                    AddQuietMove(new Move
                    {
                        FromSq = fromSq,
                        ToSq = toSq
                    });
                }
            }
            else
            {
                if (Board.GetRank(fromSq) == Rank.Rank2)
                {
                    AddQuietMove(new Move
                    {
                        FromSq = fromSq,
                        ToSq = toSq,
                        PromotedPiece = PieceType.BlackQueen
                    });
                    AddQuietMove(new Move
                    {
                        FromSq = fromSq,
                        ToSq = toSq,
                        PromotedPiece = PieceType.BlackRook
                    });
                    AddQuietMove(new Move
                    {
                        FromSq = fromSq,
                        ToSq = toSq,
                        PromotedPiece = PieceType.BlackBishop
                    });
                    AddQuietMove(new Move
                    {
                        FromSq = fromSq,
                        ToSq = toSq,
                        PromotedPiece = PieceType.BlackKnight
                    });
                }
                else
                {
                    AddQuietMove(new Move
                    {
                        FromSq = fromSq,
                        ToSq = toSq
                    });
                }
            }
        }
    }
}
