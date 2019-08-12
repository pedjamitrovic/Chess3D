using Assets.Project.ChessEngine.Pieces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Project.Scripts
{
    public enum PieceType { None, WhitePawn, WhiteRook, WhiteKnight, WhiteBishop, WhiteQueen, WhiteKing, BlackPawn, BlackRook, BlackKnight, BlackBishop, BlackQueen, BlackKing };
    public class GameController : MonoBehaviour
    {
        /*public BoxCollider bounds;
        public Piece[] Pieces { get; set; }
        public Piece SelectedPiece { get; set; }
        public bool isWhiteTurn;
        public Spawner spawner;
        public Visualizer visualizer;

        void Start()
        {
            SetupPieces();
            isWhiteTurn = true;
        }

        void FixedUpdate()
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                LayerMask mask = LayerMask.GetMask("Pieces");
                if (Physics.Raycast(ray, out hit, 100, mask))
                {
                    SelectPiece(hit.transform.gameObject.GetComponent<Piece>().CellNumber);
                    return;
                }
                else if (SelectedPiece != null)
                {
                    int cellNumber = RaycastCell(ray);
                    if (IsValidCell(cellNumber) && Pieces[cellNumber] == null) // DRUGI USLOV NE MOZE TEK TAKO
                    {
                        MovePiece(cellNumber);
                    }
                }
            }
        }

        public void SelectPiece(int cellNumber)
        {
            if (SelectedPiece != null) visualizer.RemoveHighlightFromPiece(SelectedPiece);
            if (Pieces[cellNumber] == null) return;
            if (Pieces[cellNumber].isWhite != isWhiteTurn) return;
            SelectedPiece = Pieces[cellNumber];
            visualizer.HighlightPiece(SelectedPiece);
        }

        public void MovePiece(int cellNumber)
        {
            if (SelectedPiece.PossibleMove(cellNumber))
            {
                Pieces[SelectedPiece.CellNumber] = null;
                Pieces[cellNumber] = SelectedPiece;
                Vector3 worldPoint = ToWorldPoint(cellNumber);
                SelectedPiece.transform.position = new Vector3(worldPoint.x, SelectedPiece.transform.position.y, worldPoint.z);
                SelectedPiece.CellNumber = cellNumber;
                visualizer.RemoveHighlightFromPiece(SelectedPiece);
                isWhiteTurn = !isWhiteTurn;
            }
            SelectedPiece = null;
        }

        public int RaycastCell(Ray ray)
        {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100))
            {
                Vector3 point = hit.point + new Vector3(-16, 0, 16);
                int i = (int)-point.x / 4;
                int j = (int)point.z / 4;
                return i * 8 + j;
            }
            return -1;
        }

        public void SetupPieces()
        {
            Pieces = new Piece[64];

            Pieces[0] = spawner.SpawnPiece(PieceType.WhiteRook, 0);
            Pieces[1] = spawner.SpawnPiece(PieceType.WhiteKnight, 1);
            Pieces[2] = spawner.SpawnPiece(PieceType.WhiteBishop, 2);
            Pieces[3] = spawner.SpawnPiece(PieceType.WhiteQueen, 3);
            Pieces[4] = spawner.SpawnPiece(PieceType.WhiteKing, 4);
            Pieces[5] = spawner.SpawnPiece(PieceType.WhiteBishop, 5);
            Pieces[6] = spawner.SpawnPiece(PieceType.WhiteKnight, 6);
            Pieces[7] = spawner.SpawnPiece(PieceType.WhiteRook, 7);
            for (int i = 0; i < 8; i++) Pieces[i+8] = spawner.SpawnPiece(PieceType.WhitePawn, i + 8);

            Pieces[56] = spawner.SpawnPiece(PieceType.BlackRook, 56);
            Pieces[57] = spawner.SpawnPiece(PieceType.BlackKnight, 57);
            Pieces[58] = spawner.SpawnPiece(PieceType.BlackBishop, 58);
            Pieces[59] = spawner.SpawnPiece(PieceType.BlackQueen, 59);
            Pieces[60] = spawner.SpawnPiece(PieceType.BlackKing, 60);
            Pieces[61] = spawner.SpawnPiece(PieceType.BlackBishop, 61);
            Pieces[62] = spawner.SpawnPiece(PieceType.BlackKnight, 62);
            Pieces[63] = spawner.SpawnPiece(PieceType.BlackRook, 63);
            for (int i = 0; i < 8; i++) Pieces[i+48] = spawner.SpawnPiece(PieceType.BlackPawn, i + 48);
        }
        
        public static string GetCellString(int cellNumber)
        {
            int j = cellNumber % 8;
            int i = cellNumber / 8;
            return char.ConvertFromUtf32(j + 65) + "" + (i + 1);
        }

        private Vector3 ToWorldPoint(int cellNumber)
        {
            int j = cellNumber % 8;
            int i = cellNumber / 8;
            return new Vector3(i * -4 + 14, 1, j * 4 - 14);
        }

        public bool IsValidCell(int cellNumber)
        {
            return cellNumber >= 0 && cellNumber <= 63;
        }
        */
    }
}
