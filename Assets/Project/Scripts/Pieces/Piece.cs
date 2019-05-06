using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Project.Scripts.Pieces
{
    public abstract class Piece : MonoBehaviour
    {
        public int CellNumber;
        public bool isWhite;
        public ChessUiEngine uiEngine;
        
        void Start()
        {
            uiEngine = GameObject.Find("GameController").GetComponent<GameController>().uiEngine;
        }
        
        void FixedUpdate()
        {
        }

        public virtual bool PossibleMove(int cellNumber)
        {
            return true;
        }

        public void SetMaterial(Material m)
        {
            GetComponent<Renderer>().material = m;
        }

        public void SetOriginalMaterial()
        {
            GetComponent<Renderer>().material = isWhite ? uiEngine.materials[1] : uiEngine.materials[2];
        }
    }
}
