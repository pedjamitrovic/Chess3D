﻿using Assets.Project.Chess3D.Pieces;
using Assets.Project.ChessEngine.Pieces;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Project.Chess3D
{
    public class Visualizer : MonoBehaviour
    {
        public enum Materials { Gold, White, Black };
        public Spawner spawner;
        public List<Material> materials;

        void Start()
        {
        }

        public void HighlightPiece(Piece piece)
        {
            PieceWrapper wrapper = spawner.FindPieceWrapper(piece);
            try
            {
                if (wrapper != null)
                {
                    var mat = materials[(int)Materials.Gold];
                    var ren = wrapper.GetComponent<Renderer>();
                    ren.material = mat;
                }
            }
            catch(Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }
        public void RemoveHighlightFromPiece(Piece piece)
        {
            PieceWrapper wrapper = spawner.FindPieceWrapper(piece);
            try
            {
                if (wrapper != null)
                {
                    var mat = (wrapper.Value.Color == ChessEngine.Color.White ? materials[(int)Materials.White] : materials[(int)Materials.Black]);
                    var ren = wrapper.GetComponent<Renderer>();
                    ren.material = mat;
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }
    }
}
