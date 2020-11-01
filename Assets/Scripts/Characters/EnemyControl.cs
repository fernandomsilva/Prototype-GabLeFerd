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
	
	Dictionary<float, CharacterStats> DistanceToPlayerCharacters()
	{
		Dictionary<float, CharacterStats> distanceToPlayerCharacters = new Dictionary<float, CharacterStats>();
		
		foreach (CharacterStats playerCharacter in playerCharactersInBattle)
		{
			if (playerCharacter.currentHealth > 0)
			{
				Vector3Int playerCharacterCellPosition = levelTilemap.WorldToCell(playerCharacter.GetWorldPosition());
				Vector3Int myCellPosition = levelTilemap.WorldToCell(transform.position);
				
				distanceToPlayerCharacters.Add(Vector3Int.Distance(playerCharacterCellPosition, myCellPosition), playerCharacter);
			}
		}
		
		return distanceToPlayerCharacters;
	}

    // Update is called once per frame
    void Update()
    {
		if (myStats.isThisMyTurn && !myStats.isNarrativeActive)
		{
			Dictionary<float, CharacterStats> distanceDict = DistanceToPlayerCharacters();
			
			foreach (float k in distanceDict.Keys)
			{
				Debug.Log(distanceDict[k].charName + " : " + k);
			}
			
			myStats.isThisMyTurn = false;
		}
    }
}
