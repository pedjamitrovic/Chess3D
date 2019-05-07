using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Project.Scripts.Pieces
{
    public abstract class Piece : MonoBehaviour
    {
        public int CellNumber;
        public bool isWhite;
        
        void Start()
        {
        }
        
        void FixedUpdate()
        {
        }

        public virtual bool PossibleMove(int cellNumber)
        {
            return true;
        }
    }
}
