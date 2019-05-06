using Assets.Project.Scripts.Pieces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Project.Scripts
{
    public class ChessUiEngine : MonoBehaviour
    {
        public enum PieceType { None, WhitePawn, WhiteRook, WhiteKnight, WhiteBishop, WhiteQueen, WhiteKing, BlackPawn, BlackRook, BlackKnight, BlackBishop, BlackQueen, BlackKing };
        public BoxCollider bounds;
        public List<Transform> piecePrefabs;
        public Piece[] Pieces { get; set; }
        public Piece SelectedPiece { get; set; }
        public bool isWhiteTurn;
        public List<Material> materials;

        void Start()
        {
            SetupPieces();
            FetchMaterials();
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
                    if (IsValidCell(cellNumber) && Pieces[cellNumber] == null)
                    {
                        MovePiece(cellNumber);
                    }
                }
            }
        }

        public void SelectPiece(int cellNumber)
        {
            if (SelectedPiece != null) SelectedPiece.SetOriginalMaterial();
            if (Pieces[cellNumber] == null) return;
            if (Pieces[cellNumber].isWhite != isWhiteTurn) return;
            SelectedPiece = Pieces[cellNumber];
            SelectedPiece.SetMaterial(materials[0]);
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
                SelectedPiece.SetOriginalMaterial();
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

            SpawnPiece(PieceType.WhiteRook, 0);
            SpawnPiece(PieceType.WhiteKnight, 1);
            SpawnPiece(PieceType.WhiteBishop, 2);
            SpawnPiece(PieceType.WhiteQueen, 3);
            SpawnPiece(PieceType.WhiteKing, 4);
            SpawnPiece(PieceType.WhiteBishop, 5);
            SpawnPiece(PieceType.WhiteKnight, 6);
            SpawnPiece(PieceType.WhiteRook, 7);
            for (int i = 0; i < 8; i++) SpawnPiece(PieceType.WhitePawn, i + 8);

            SpawnPiece(PieceType.BlackRook, 56);
            SpawnPiece(PieceType.BlackKnight, 57);
            SpawnPiece(PieceType.BlackBishop, 58);
            SpawnPiece(PieceType.BlackQueen, 59);
            SpawnPiece(PieceType.BlackKing, 60);
            SpawnPiece(PieceType.BlackBishop, 61);
            SpawnPiece(PieceType.BlackKnight, 62);
            SpawnPiece(PieceType.BlackRook, 63);
            for (int i = 0; i < 8; i++) SpawnPiece(PieceType.BlackPawn, i + 48);
        }

        public void SpawnPiece(PieceType type, int cellNumber)
        {
            if (Pieces[cellNumber] != null) throw new System.Exception("Spawning piece on non empty field.");
            Vector3 worldPoint = ToWorldPoint(cellNumber);
            Transform piece = GameObject.Instantiate(piecePrefabs[(int)type]);
            piece.position = new Vector3(worldPoint.x, piece.position.y, worldPoint.z);
            piece.parent = GameObject.Find("Pieces").transform;
            Pieces[cellNumber] = piece.gameObject.GetComponent<Piece>();
            piece.gameObject.GetComponent<Piece>().CellNumber = cellNumber;
        }

        public static string GetCellString(int cellNumber)
        {
            int j = cellNumber % 8;
            int i = cellNumber / 8;
            return char.ConvertFromUtf32(j + 65) + "" + (i + 1);
        }

        public static Vector3 ToWorldPoint(int cellNumber)
        {
            int j = cellNumber % 8;
            int i = cellNumber / 8;
            return new Vector3(i * -4 + 14, 1, j * 4 - 14);
        }

        public bool IsValidCell(int cellNumber)
        {
            return cellNumber >= 0 && cellNumber <= 63;
        }

        public void FetchMaterials()
        {
            Material gold = Resources.Load("Gold") as Material;
            materials.Add(gold);
            Material copper = Resources.Load("Copper") as Material;
            materials.Add(copper);
            Material steel = Resources.Load("Steel") as Material;
            materials.Add(steel);
        }
    }
}
