using Assets.Project.ChessEngine.Exceptions;
using Assets.Project.ChessEngine.Pieces;

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
            PieceType pieceType;

            Board board = new Board();

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
                    sq120 = Board.Sq120(sq64);
                    if (pieceType != PieceType.None)
                    {
                        board.Pieces[sq120] = Piece.CreatePiece(pieceType, (Square)sq120);
                    }
                    file++;
                }
            }

            board.OnTurn = (fen[i] == 'w') ? Color.White : Color.Black;
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
