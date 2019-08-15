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
                    AddCaptureMove(new Move(fromSq, toSq, capturedPiece, PieceType.WhiteQueen));
                    AddCaptureMove(new Move(fromSq, toSq, capturedPiece, PieceType.WhiteRook));
                    AddCaptureMove(new Move(fromSq, toSq, capturedPiece, PieceType.WhiteBishop));
                    AddCaptureMove(new Move(fromSq, toSq, capturedPiece, PieceType.WhiteKnight));
                }
                else
                {
                    AddCaptureMove(new Move(fromSq, toSq, capturedPiece));
                }
            }
            else
            {
                if (Board.GetRank(fromSq) == Rank.Rank2)
                {
                    AddCaptureMove(new Move(fromSq, toSq, capturedPiece, PieceType.BlackQueen));
                    AddCaptureMove(new Move(fromSq, toSq, capturedPiece, PieceType.BlackRook));
                    AddCaptureMove(new Move(fromSq, toSq, capturedPiece, PieceType.BlackBishop));
                    AddCaptureMove(new Move(fromSq, toSq, capturedPiece, PieceType.BlackKnight));
                }
                else
                {
                    AddCaptureMove(new Move(fromSq, toSq, capturedPiece));
                }
            }
        }
        public void AddPawnQuietMove(Color onTurn, Square fromSq, Square toSq)
        {
            if (onTurn == Color.White)
            {
                if (Board.GetRank(fromSq) == Rank.Rank7)
                {
                    AddQuietMove(new Move(fromSq, toSq, PieceType.WhiteQueen));
                    AddQuietMove(new Move(fromSq, toSq, PieceType.WhiteRook));
                    AddQuietMove(new Move(fromSq, toSq, PieceType.WhiteBishop));
                    AddQuietMove(new Move(fromSq, toSq, PieceType.WhiteKnight));
                }
                else
                {
                    AddQuietMove(new Move(fromSq, toSq));
                }
            }
            else
            {
                if (Board.GetRank(fromSq) == Rank.Rank2)
                {
                    AddQuietMove(new Move(fromSq, toSq, PieceType.BlackQueen));
                    AddQuietMove(new Move(fromSq, toSq, PieceType.BlackRook));
                    AddQuietMove(new Move(fromSq, toSq, PieceType.BlackBishop));
                    AddQuietMove(new Move(fromSq, toSq, PieceType.BlackKnight));
                }
                else
                {
                    AddQuietMove(new Move(fromSq, toSq));
                }
            }
        }
    }
}
