using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CharacterControl : MonoBehaviour
{
	public Tilemap levelTilemap;
	
	public int movementRange;
	
	private List<Vector3Int> rangeOfAction;
	
    // Start is called before the first frame update
    void Start()
    {
        MoveToCell(new Vector3Int(0, 0, 0));
		
		rangeOfAction = new List<Vector3Int>();
    }
	
	void PaintTiles(List<Vector3Int> tilesToPaint, Color color)
	{
		foreach (Vector3Int tile in tilesToPaint)
		{
			levelTilemap.RemoveTileFlags(tile, TileFlags.LockColor);
			levelTilemap.SetColor(tile, color);
		}
	}
	
	void MoveToCell(Vector3Int cell)
	{
		transform.position = levelTilemap.GetCellCenterWorld(cell);
	}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("m"))
		{
			Vector3Int myCellPosition = levelTilemap.WorldToCell(transform.position);
			rangeOfAction.Clear();

			for (int i=movementRange; i>=0; i--)
			{
				for (int j=movementRange-i; j>=0; j--)
				{
					if (i != 0 || j != 0)
					{
						rangeOfAction.Add(myCellPosition + (new Vector3Int(i, j, 0)));
						if (i > 0)
						{
							rangeOfAction.Add(myCellPosition + (new Vector3Int(-i, j, 0)));
						}
						if (j > 0)
						{
							rangeOfAction.Add(myCellPosition + (new Vector3Int(i, -j, 0)));
						}
						if (i > 0 && j > 0)
						{
							rangeOfAction.Add(myCellPosition + (new Vector3Int(-i, -j, 0)));
						}
					}
				}
			}

			PaintTiles(rangeOfAction, Color.red);
		}
		
		if (Input.GetMouseButtonDown(0))
		{
			Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);			
			Vector3Int mouseCellPosition = levelTilemap.WorldToCell(mouseWorldPosition);
			
			if (rangeOfAction.Contains(mouseCellPosition))
			{
				MoveToCell(mouseCellPosition);
			}
		}
    }
}
