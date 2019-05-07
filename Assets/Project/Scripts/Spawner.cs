using Assets.Project.Scripts.Pieces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Project.Scripts
{
    public class Spawner: MonoBehaviour
    {
        public Transform highlightedFields;
        public List<Transform> piecePrefabs;
        public Transform highlightedFieldPrefab;

        void Start()
        {
            highlightedFields = GameObject.Find("HighlightedFields").transform;
        }

        public void HighlightField(int cellNumber)
        {
            Vector3 worldPoint = ToWorldPoint(cellNumber);
            Transform highlightedField = Instantiate(highlightedFieldPrefab);
            highlightedField.position = new Vector3(worldPoint.x, highlightedField.position.y, worldPoint.z);
            highlightedField.parent = highlightedFields;
            highlightedField.gameObject.GetComponent<Piece>().CellNumber = cellNumber;
        }

        public void DestroyHighlightedFields()
        {
            foreach (Transform child in highlightedFields)
            {
                Destroy(child.gameObject);
            }
        }

        public Piece SpawnPiece(PieceType type, int cellNumber)
        {
            Vector3 worldPoint = ToWorldPoint(cellNumber);
            Transform piece = Instantiate(piecePrefabs[(int)type]);
            piece.position = new Vector3(worldPoint.x, piece.position.y, worldPoint.z);
            piece.parent = GameObject.Find("Pieces").transform;
            piece.gameObject.GetComponent<Piece>().CellNumber = cellNumber;
            return piece.gameObject.GetComponent<Piece>();
        }

        private Vector3 ToWorldPoint(int cellNumber)
        {
            int j = cellNumber % 8;
            int i = cellNumber / 8;
            return new Vector3(i * -4 + 14, 1, j * 4 - 14);
        }
    }
}
