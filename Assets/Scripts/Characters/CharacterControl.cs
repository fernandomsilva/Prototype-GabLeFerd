using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CharacterControl : MonoBehaviour
{
	public Tilemap levelTilemap;
	
	public Vector3Int startCell;
	
	private CharacterStats myStats;
	
	private OccupiedCells blockedCells;
	private List<Vector3Int> rangeOfAction;
	
	private Color originalTileColor;
	
	private string currentAction;
	
	private NarrativeTriggers storyTriggers;
	
    // Start is called before the first frame update
    void Start()
    {
		myStats = GetComponent<CharacterStats>();
		
		blockedCells = levelTilemap.GetComponent<OccupiedCells>();

        MoveToCell(startCell);
		originalTileColor = levelTilemap.GetColor(new Vector3Int(0, 0, 0));
		
		rangeOfAction = new List<Vector3Int>();
		
		currentAction = "";
		
		storyTriggers = GameObject.Find("Narrative Director").GetComponent<NarrativeTriggers>();
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
		Vector3Int previousPosition = levelTilemap.WorldToCell(transform.position);
		
		transform.position = levelTilemap.GetCellCenterWorld(cell);
		
		blockedCells.RemoveOccupiedCell(previousPosition);
		blockedCells.AddOccupiedCell(cell);
	}

	void CalculateRange(int rangeLength, bool isAreaRange, bool movement, List<Vector3Int> range)
	{
		Vector3Int myCellPosition = levelTilemap.WorldToCell(transform.position);
		range.Clear();

		range.Add(myCellPosition);

		for (int i=rangeLength; i>=0; i--)
		{
			if (isAreaRange)
			{
				for (int j=rangeLength-i; j>=0; j--)
				{
					Vector3Int cell = myCellPosition + (new Vector3Int(i, j, 0));
					range.Add(cell);
					
					if (i > 0)
					{
						cell = myCellPosition + (new Vector3Int(-i, j, 0));
						range.Add(cell);
					}
					
					if (j > 0)
					{
						cell = myCellPosition + (new Vector3Int(i, -j, 0));
						range.Add(cell);
					}
					
					if (i > 0)
					{
						cell = myCellPosition + (new Vector3Int(-i, -j, 0));
						range.Add(cell);
					}
				}
			}
			else
			{
				Vector3Int cell = myCellPosition + (new Vector3Int(i, 0, 0));
				range.Add(cell);
				cell = myCellPosition + (new Vector3Int(-i, 0, 0));
				range.Add(cell);
				cell = myCellPosition + (new Vector3Int(0, i, 0));
				range.Add(cell);
				cell = myCellPosition + (new Vector3Int(0, -i, 0));
				range.Add(cell);
			}
		}
	}
	
	GameObject ReturnEnemyInCell(Vector3Int cell)
	{
		GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
		
		foreach (GameObject enemy in enemies)
		{
			if (cell == levelTilemap.WorldToCell(enemy.transform.position))
			{
				return enemy;
			}
		}
		
		return null;
	}

    // Update is called once per frame
    void Update()
    {
		if (myStats.isThisMyTurn && !myStats.isNarrativeActive)
		{
			if (Input.GetKeyDown("m") && !myStats.movedThisTurn)
			{
				CalculateRange(myStats.moveSpeed, true, true, rangeOfAction);

				PaintTiles(rangeOfAction, Color.red);
				
				currentAction = "move";
			}
			if (Input.GetKeyDown("a") && !myStats.attackedThisTurn)
			{
				CalculateRange(myStats.meleeAttackRange, false, false, rangeOfAction);

				PaintTiles(rangeOfAction, Color.red);
				
				currentAction = "meleeAttack";
			}
			
			if (Input.GetMouseButtonDown(0))
			{
				Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);			
				Vector3Int mouseCellPosition = levelTilemap.WorldToCell(mouseWorldPosition);
				
				if (rangeOfAction.Contains(mouseCellPosition))
				{
					if (currentAction == "move")
					{
						if (!blockedCells.IsCellOccupied(mouseCellPosition))
						{
							Vector3Int myPreviousCell = levelTilemap.WorldToCell(transform.position);
							string myPreviousPosition = "(" + myPreviousCell.x + "," + myPreviousCell.y + "," + myPreviousCell.z + ")";
							
							MoveToCell(mouseCellPosition);
							myStats.movedThisTurn = true;
							
							Vector3Int myCurrentCellPosition = levelTilemap.WorldToCell(transform.position);
							string myCurrentPosition = "(" + myCurrentCellPosition.x + "," + myCurrentCellPosition.y + "," + myCurrentCellPosition.z + ")";
							
							string[] eventParameters = {myStats.charName, "move", myPreviousPosition, myCurrentPosition};
							storyTriggers.EventPast(eventParameters);
						}
					}
					else
					{
						GameObject enemy = ReturnEnemyInCell(mouseCellPosition);
						if (enemy != null)
						{
							CharacterStats enemyStats = enemy.GetComponent<CharacterStats>();
							
							enemyStats.GotHit(myStats.meleeAttack, "melee");
							myStats.attackedThisTurn = true;
							
							string[] eventParameters = {myStats.charName, "melee", enemyStats.charName};
							storyTriggers.EventPast(eventParameters);
						}
					}
					
					currentAction = "";

					PaintTiles(rangeOfAction, originalTileColor);
					rangeOfAction.Clear();
				}
			}
			else if (Input.GetMouseButtonDown(1))
			{
				if (currentAction != "")
				{
					currentAction = "";
					PaintTiles(rangeOfAction, originalTileColor);
					rangeOfAction.Clear();
				}
			}
		}
    }
}
