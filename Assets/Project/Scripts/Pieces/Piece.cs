using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Project.Scripts.Pieces
{
    public abstract class Piece : MonoBehaviour
    {
        public int CellNumber;
        public bool isWhite;

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }

        public virtual bool PossibleMove(int cellNumber)
        {
            return true;
        }
    }
}
