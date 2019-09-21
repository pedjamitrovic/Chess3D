using Assets.Project.Chess3D;
using Assets.Project.ChessEngine;
using Assets.Project.ChessEngine.Pieces;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public GameController gc;

    void Start()
    {
        gc = GameObject.Find("GameController").transform.GetComponent<GameController>();
    }

    void FixedUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            LayerMask mask = LayerMask.GetMask("Pieces");
            int sq64, sq120;
            if (Physics.Raycast(ray, out hit, 100, mask) && gc.SelectedPiece == null)
            {
                sq64 = RaycastCell(ray);
                sq120 = Board.Sq120(sq64);
                gc.SelectPiece(sq120);
                return;
            }
            else if (gc.SelectedPiece != null)
            {
                sq64 = RaycastCell(ray);
                if (gc.IsValidCell(sq64))
                {
                    sq120 = Board.Sq120(sq64);
                    gc.DoMove(sq120);
                }
            }
        }
    }

    private int RaycastCell(Ray ray)
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
}
