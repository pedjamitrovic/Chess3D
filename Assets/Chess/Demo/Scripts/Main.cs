using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour {
	public ChessUiEngine uiEngine;
	public Text cell;
	public Transform brightSquare;
	// Use this for initialization
	void Start () {
		uiEngine.SetupPieces ();
	}

	void FixedUpdate () {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); 
		int cellNumber = uiEngine.RaycastCell (ray);
		if (!IsValidCell(cellNumber)) {
			return;
		}
		cell.text = ChessUiEngine.GetCellString(cellNumber);
		PlaceBrightSquare (cellNumber);
	}

	void PlaceBrightSquare (int cellNumber)
	{
		brightSquare.position = ChessUiEngine.ToWorldPoint (cellNumber);
	}

	bool IsValidCell (int cellNumber)
	{
		return cellNumber >= 0 && cellNumber < 64;
	}
}
