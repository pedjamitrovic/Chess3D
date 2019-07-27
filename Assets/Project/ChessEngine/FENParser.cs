using Assets.Project.ChessEngine.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Project.ChessEngine
{
    public static class FENParser
    {
        public static Board ParseFEN(string fen)
        {
            int count;
            File file = File.FileA;
            Rank rank = Rank.Rank8;
            int i = 0;
            SquareContent piece = 0;

            Board board = new Board();

            for (; rank >= Rank.Rank1; ++i)
            {
                count = 1;
                switch (fen[i])
                {
                    case 'p': piece = SquareContent.BlackPawn; break;
                    case 'n': piece = SquareContent.BlackKnight; break;
                    case 'b': piece = SquareContent.BlackBishop; break;
                    case 'r': piece = SquareContent.BlackRook; break;
                    case 'k': piece = SquareContent.BlackKing; break;
                    case 'q': piece = SquareContent.BlackQueen; break;
                    case 'P': piece = SquareContent.WhitePawn; break;
                    case 'N': piece = SquareContent.WhiteKnight; break;
                    case 'B': piece = SquareContent.WhiteBishop; break;
                    case 'R': piece = SquareContent.WhiteRook; break;
                    case 'K': piece = SquareContent.WhiteKing; break;
                    case 'Q': piece = SquareContent.WhiteQueen; break;

                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                        piece = SquareContent.None;
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
                    sq120 = Board.Sq120(sq64);
                    if (piece != SquareContent.None)
                    {
                        board.Pieces[sq120] = piece;
                    }
                    file++;
                }
            }

            board.OnTurn = (fen[i] == 'w') ? Side.White : Side.Black;
            i += 2;

            for (int j = 0; fen[i] != ' ' && j < 4; ++j, ++i)
            {
                switch (fen[i])
                {
                    case 'k': board.CastlePerm |= (int)CastlingPermit.BlackKingCastling; break;
                    case 'q': board.CastlePerm |= (int)CastlingPermit.BlackQueenCastling; break;
                    case 'K': board.CastlePerm |= (int)CastlingPermit.WhiteKingCastling; break;
                    case 'Q': board.CastlePerm |= (int)CastlingPermit.WhiteQueenCastling; break;
                    default: break;
                }
            }
            i++;

            if (fen[i] != '-')
            {
                file = (File)(fen[i] - 'a');
                rank = (Rank)(fen[i+1] - '1');

                board.EnPassant = (Square)Board.ConvertToSq120(file, rank);
            }

            board.StateKey = HashGenerator.CalculateStateKey(board);

            board.UpdateListsAndMaterial();

            return board;
        }
    }
}
