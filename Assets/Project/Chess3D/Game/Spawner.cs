using Assets.Project.Chess3D.Pieces;
using Assets.Project.ChessEngine;
using Assets.Project.ChessEngine.Pieces;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Project.Chess3D
{
    public class Spawner: MonoBehaviour
    {
        public List<Transform> piecePrefabs;
        public GameObject Pieces;
        public GameController gc;

        void Start()
        {
        }

        public void DoMove(Move move)
        {
            if (move.IsEnPassant)
            {
                if (gc.Board.OnTurn == ChessEngine.Color.White)
                {
                    DestroyPiece(gc.Board.Pieces[(int)move.ToSq - 10]);
                }
                else
                {
                    DestroyPiece(gc.Board.Pieces[(int)move.ToSq + 10]);
                }
            }
            else if (move.IsCastle)
            {
                switch (move.ToSq)
                {
                    case Square.C1:
                        MovePiece(gc.Board.Pieces[(int)Square.A1], Board.Sq64((int)Square.D1));
                        break;
                    case Square.C8:
                        MovePiece(gc.Board.Pieces[(int)Square.A8], Board.Sq64((int)Square.D8));
                        break;
                    case Square.G1:
                        MovePiece(gc.Board.Pieces[(int)Square.H1], Board.Sq64((int)Square.F1));
                        break;
                    case Square.G8:
                        MovePiece(gc.Board.Pieces[(int)Square.H8], Board.Sq64((int)Square.F8));
                        break;
                }
            }

            if (move.CapturedPiece != null)
            {
                DestroyPiece(gc.Board.Pieces[(int)move.ToSq]);
            }

            MovePiece(gc.Board.Pieces[(int)move.FromSq], Board.Sq64((int)move.ToSq));

            if (move.PromotedPiece.HasValue)
            {
                DestroyPiece(gc.Board.Pieces[(int)move.FromSq]);
            }
        }

        public PieceWrapper SpawnPiece(Piece piece)
        {
            Vector3 worldPoint = ToWorldPoint(Board.Sq64((int)piece.Square));
            Transform transform = Instantiate(piecePrefabs[piece.Index]);
            transform.position = new Vector3(worldPoint.x, transform.position.y, worldPoint.z);
            transform.parent = Pieces.transform;
            PieceWrapper wrapper = transform.GetComponent<PieceWrapper>();
            wrapper.Value = piece;
            return wrapper;
        }

        public void DestroyPiece(Piece piece)
        {
            PieceWrapper wrapper = FindPieceWrapper(piece);
            Destroy(wrapper.gameObject);
        }

        public void MovePiece(Piece piece, int sq64)
        {
            Vector3 worldPoint = ToWorldPoint(sq64);
            PieceWrapper wrapper = FindPieceWrapper(piece);
            wrapper.transform.position = new Vector3(worldPoint.x, wrapper.transform.position.y, worldPoint.z);
        }

        public PieceWrapper FindPieceWrapper(Piece piece)
        {
            foreach (Transform child in Pieces.transform)
            {
                PieceWrapper current = child.GetComponent<PieceWrapper>();
                if (current.Value.Square == piece.Square) return current;
            }
            return null;
        }

        private Vector3 ToWorldPoint(int cellNumber)
        {
            int j = cellNumber % 8;
            int i = cellNumber / 8;
            return new Vector3(i * -4 + 14, 1, j * 4 - 14);
        }
    }
}
