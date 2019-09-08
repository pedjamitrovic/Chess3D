using Assets.Project.ChessEngine.Pieces;
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
        public void AddPawnCaptureMove(Color onTurn, Square fromSq, Square toSq, char capturedPiece)
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
                        PromotedPiece = Queen.GetLabel(Color.White)
                    });
                    AddCaptureMove(new Move
                    {
                        FromSq = fromSq,
                        ToSq = toSq,
                        CapturedPiece = capturedPiece,
                        PromotedPiece = Rook.GetLabel(Color.White)
                    });
                    AddCaptureMove(new Move
                    {
                        FromSq = fromSq,
                        ToSq = toSq,
                        CapturedPiece = capturedPiece,
                        PromotedPiece = Bishop.GetLabel(Color.White)
                    });
                    AddCaptureMove(new Move
                    {
                        FromSq = fromSq,
                        ToSq = toSq,
                        CapturedPiece = capturedPiece,
                        PromotedPiece = Knight.GetLabel(Color.White)
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
                        PromotedPiece = Queen.GetLabel(Color.Black)
                    });
                    AddCaptureMove(new Move
                    {
                        FromSq = fromSq,
                        ToSq = toSq,
                        CapturedPiece = capturedPiece,
                        PromotedPiece = Rook.GetLabel(Color.Black)
                    });
                    AddCaptureMove(new Move
                    {
                        FromSq = fromSq,
                        ToSq = toSq,
                        CapturedPiece = capturedPiece,
                        PromotedPiece = Bishop.GetLabel(Color.Black)
                    });
                    AddCaptureMove(new Move
                    {
                        FromSq = fromSq,
                        ToSq = toSq,
                        CapturedPiece = capturedPiece,
                        PromotedPiece = Knight.GetLabel(Color.Black)
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
                        PromotedPiece = Queen.GetLabel(Color.White)
                    });
                    AddQuietMove(new Move
                    {
                        FromSq = fromSq,
                        ToSq = toSq,
                        PromotedPiece = Rook.GetLabel(Color.White)
                    });
                    AddQuietMove(new Move
                    {
                        FromSq = fromSq,
                        ToSq = toSq,
                        PromotedPiece = Bishop.GetLabel(Color.White)
                    });
                    AddQuietMove(new Move
                    {
                        FromSq = fromSq,
                        ToSq = toSq,
                        PromotedPiece = Knight.GetLabel(Color.White)
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
                        PromotedPiece = Queen.GetLabel(Color.Black)
                    });
                    AddQuietMove(new Move
                    {
                        FromSq = fromSq,
                        ToSq = toSq,
                        PromotedPiece = Rook.GetLabel(Color.Black)
                    });
                    AddQuietMove(new Move
                    {
                        FromSq = fromSq,
                        ToSq = toSq,
                        PromotedPiece = Bishop.GetLabel(Color.Black)
                    });
                    AddQuietMove(new Move
                    {
                        FromSq = fromSq,
                        ToSq = toSq,
                        PromotedPiece = Knight.GetLabel(Color.Black)
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
