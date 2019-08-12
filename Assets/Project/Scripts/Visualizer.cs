using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Project.Scripts
{
    public class Visualizer: MonoBehaviour
    {
        public enum Materials { Gold, White, Black };
        public Spawner spawner;
        public List<Material> materials;

        void Start()
        {
        }

        /*public void HighlightFields(List<int> cellNumbers)
        {
            foreach (var cellNumber in cellNumbers)
            {
                spawner.HighlightField(cellNumber);
            }
        }
        public void RemoveHighlightedFields()
        {
            spawner.DestroyHighlightedFields();
        }
        public void HighlightPiece(Piece piece)
        {
            piece.GetComponent<Renderer>().material = materials[(int)Materials.Gold];
        }
        public void RemoveHighlightFromPiece(Piece piece)
        {
            piece.GetComponent<Renderer>().material = piece.isWhite ? materials[(int)Materials.White] : materials[(int)Materials.Black];
        }*/
    }
}
