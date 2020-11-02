using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyControl : MonoBehaviour
{
	private Tilemap levelTilemap;
	private CharacterStats myStats;
	private OccupiedCells blockedCells;
	
	private List<CharacterStats> playerCharactersInBattle;
	
    // Start is called before the first frame update
    void Start()
    {
		levelTilemap = GameObject.Find("Level Tilemap").GetComponent<Tilemap>();

		myStats = GetComponent<CharacterStats>();	
	
		blockedCells = levelTilemap.GetComponent<OccupiedCells>();
		
		Vector3Int myPosition = levelTilemap.WorldToCell(transform.position);

		blockedCells.AddOccupiedCell(myPosition);
		
		playerCharactersInBattle = new List<CharacterStats>();
		FindAllCharactersInBattle();
    }

	void FindAllCharactersInBattle()
	{
		GameObject[] allPlayerCharacters = GameObject.FindGameObjectsWithTag("Player");
		
		foreach (GameObject playerCharacter in allPlayerCharacters)
		{
			playerCharactersInBattle.Add(playerCharacter.GetComponent<CharacterStats>());
		}
	}
	
	void MoveToCell(Vector3Int cell)
	{
		Vector3Int previousPosition = levelTilemap.WorldToCell(transform.position);
		
		transform.position = levelTilemap.GetCellCenterWorld(cell);
		
		blockedCells.RemoveOccupiedCell(previousPosition);
		blockedCells.AddOccupiedCell(cell);
	}
	
	Dictionary<float, Vector3Int> DistanceToPlayerCharacters(Vector3Int myCellPosition)
	{
		Dictionary<float, Vector3Int> distanceToPlayerCharacters = new Dictionary<float, Vector3Int>();
		
		foreach (CharacterStats playerCharacter in playerCharactersInBattle)
		{
			if (playerCharacter.currentHealth > 0)
			{
				Vector3Int playerCharacterCellPosition = levelTilemap.WorldToCell(playerCharacter.GetWorldPosition());
				//Vector3Int myCellPosition = levelTilemap.WorldToCell(transform.position);
				
				//float distance = Vector3Int.Distance(playerCharacterCellPosition, myCellPosition);
				
				//distanceToPlayerCharacters.Add(distance, playerCharacter);
				float distance = Mathf.Abs(playerCharacterCellPosition.x - myCellPosition.x) + Mathf.Abs(playerCharacterCellPosition.y - myCellPosition.y);
				
				if (playerCharacterCellPosition.x == myCellPosition.x || playerCharacterCellPosition.y == myCellPosition.y)
				{
					distanceToPlayerCharacters.Add(distance, myCellPosition);
				}
			}
		}
		
		return distanceToPlayerCharacters;
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

    // Update is called once per frame
    void Update()
    {
		if (myStats.isThisMyTurn && !myStats.isNarrativeActive)
		{
			List<Vector3Int> rangeOfAction = new List<Vector3Int>();
			
			CalculateRange(myStats.moveSpeed, true, true, rangeOfAction);
			
			Vector3Int myCellPosition = levelTilemap.WorldToCell(myStats.GetWorldPosition());

			float shortestDistance = 0;
			Vector3Int cellToMoveTo = new Vector3Int(-1000, -1000, 0);
			
			foreach (Vector3Int cell in rangeOfAction)
			{
				if (!blockedCells.IsCellOccupied(cell))
				{
					Dictionary<float, Vector3Int> distanceDict = DistanceToPlayerCharacters(cell);
					
					if (distanceDict.Count > 0)
					{
						List<float> orderedDistance = new List<float>(distanceDict.Keys);
						orderedDistance.Sort();
						
						if (shortestDistance == 0 || shortestDistance > orderedDistance[0])
						{
							shortestDistance = orderedDistance[0];
							cellToMoveTo = cell;
							
							if (shortestDistance == 1)
							{
								break;
							}
						}
					}
					/*foreach (float k in orderedDistance)
					{
						Debug.Log(myStats.moveSpeed >= k);
						Debug.Log(distanceDict[k].charName + " : " + k);
					}*/
				}
			}
			
			if (cellToMoveTo.magnitude < 100)
			{
				MoveToCell(cellToMoveTo);
			}
			else
			{
				int index = Random.Range(0, rangeOfAction.Count-1);
				while (blockedCells.IsCellOccupied(rangeOfAction[index]))
				{
					index = Random.Range(0, rangeOfAction.Count-1);
				}
				
				MoveToCell(rangeOfAction[index]);
			}
			
			/*Dictionary<float, CharacterStats> distanceDict = DistanceToPlayerCharacters();
			
			List<float> orderedDistance = new List<float>(distanceDict.Keys);
			orderedDistance.Sort();
			
			foreach (float k in orderedDistance)
			{
				Debug.Log(myStats.moveSpeed >= k);
				Debug.Log(distanceDict[k].charName + " : " + k);
			}*/
			
			myStats.isThisMyTurn = false;
		}
    }
}
