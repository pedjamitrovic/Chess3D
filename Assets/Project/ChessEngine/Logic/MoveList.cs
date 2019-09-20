using Assets.Project.ChessEngine.Pieces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Project.ChessEngine
{
    public class MoveList : IEnumerable
    {
        private readonly int maxMoves = 256;
        public Move[] Moves { get; set; }
        public int Count { get; set; }
        public Move this[int index]
        {
            get
            {
                return Moves[index];
            }

            set
            {
                Moves[index] = value;
            }
        }

        public MoveList()
        {
            Moves = new Move[maxMoves];
            Count = 0;
        }
        public Move PickNextMove(int moveIndex)
        {
            int bestScore = 0, bestIndex = moveIndex;
            for (int i = moveIndex; i < Count; ++i)
            {
                if (Moves[i].Score > bestScore)
                {
                    bestScore = Moves[i].Score;
                    bestIndex = i;
                }
            }
            Swap(moveIndex, bestIndex);
            return Moves[moveIndex];
        }
        public void AddQuietMove(Move move)
        {
            move.Score = 0;
            Add(move);
        }
        public void AddCaptureMove(Board board, Move move)
        {
            int vic = move.CapturedPiece.Value;
            int att = board.Pieces[(int)move.FromSq].Index;
            move.Score = Board.MvvLvaScores[vic, att];
            Add(move);
        }
        public void AddEnPassantMove(Move move)
        {
            move.Score = 105;
            Add(move);
        }
        public void AddPawnCaptureMove(Board board, Color onTurn, Square fromSq, Square toSq, int capturedPiece)
        {
            if (onTurn == Color.White)
            {
                if (Board.GetRank(fromSq) == Rank.Rank7)
                {
                    AddCaptureMove(board, new Move
                    {
                        FromSq = fromSq,
                        ToSq = toSq,
                        CapturedPiece = capturedPiece,
                        PromotedPiece = Queen.GetIndex(Color.White)
                    });
                    AddCaptureMove(board, new Move
                    {
                        FromSq = fromSq,
                        ToSq = toSq,
                        CapturedPiece = capturedPiece,
                        PromotedPiece = Rook.GetIndex(Color.White)
                    });
                    AddCaptureMove(board, new Move
                    {
                        FromSq = fromSq,
                        ToSq = toSq,
                        CapturedPiece = capturedPiece,
                        PromotedPiece = Bishop.GetIndex(Color.White)
                    });
                    AddCaptureMove(board, new Move
                    {
                        FromSq = fromSq,
                        ToSq = toSq,
                        CapturedPiece = capturedPiece,
                        PromotedPiece = Knight.GetIndex(Color.White)
                    });
                }
                else
                {
                    AddCaptureMove(board, new Move
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
                    AddCaptureMove(board, new Move
                    {
                        FromSq = fromSq,
                        ToSq = toSq,
                        CapturedPiece = capturedPiece,
                        PromotedPiece = Queen.GetIndex(Color.Black)
                    });
                    AddCaptureMove(board, new Move
                    {
                        FromSq = fromSq,
                        ToSq = toSq,
                        CapturedPiece = capturedPiece,
                        PromotedPiece = Rook.GetIndex(Color.Black)
                    });
                    AddCaptureMove(board, new Move
                    {
                        FromSq = fromSq,
                        ToSq = toSq,
                        CapturedPiece = capturedPiece,
                        PromotedPiece = Bishop.GetIndex(Color.Black)
                    });
                    AddCaptureMove(board, new Move
                    {
                        FromSq = fromSq,
                        ToSq = toSq,
                        CapturedPiece = capturedPiece,
                        PromotedPiece = Knight.GetIndex(Color.Black)
                    });
                }
                else
                {
                    AddCaptureMove(board, new Move
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
                        PromotedPiece = Queen.GetIndex(Color.White)
                    });
                    AddQuietMove(new Move
                    {
                        FromSq = fromSq,
                        ToSq = toSq,
                        PromotedPiece = Rook.GetIndex(Color.White)
                    });
                    AddQuietMove(new Move
                    {
                        FromSq = fromSq,
                        ToSq = toSq,
                        PromotedPiece = Bishop.GetIndex(Color.White)
                    });
                    AddQuietMove(new Move
                    {
                        FromSq = fromSq,
                        ToSq = toSq,
                        PromotedPiece = Knight.GetIndex(Color.White)
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
                        PromotedPiece = Queen.GetIndex(Color.Black)
                    });
                    AddQuietMove(new Move
                    {
                        FromSq = fromSq,
                        ToSq = toSq,
                        PromotedPiece = Rook.GetIndex(Color.Black)
                    });
                    AddQuietMove(new Move
                    {
                        FromSq = fromSq,
                        ToSq = toSq,
                        PromotedPiece = Bishop.GetIndex(Color.Black)
                    });
                    AddQuietMove(new Move
                    {
                        FromSq = fromSq,
                        ToSq = toSq,
                        PromotedPiece = Knight.GetIndex(Color.Black)
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
        
        private void Swap(int i, int j)
        {
            Move temp = Moves[i];
            Moves[i] = Moves[j];
            Moves[j] = temp;
        }
        private IEnumerable<Move> GetMoves()
        {
            for (int i = 0; i < Count; ++i) yield return Moves[i];
        }
        public IEnumerator<Move> GetEnumerator()
        {
            return GetMoves().GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        private void Add(Move move)
        {
            Moves[Count++] = move;
        }
    }
}
