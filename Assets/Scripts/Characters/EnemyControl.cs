using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyControl : MonoBehaviour
{
	private Tilemap levelTilemap;

	private CharacterStats myStats;
	
	private OccupiedCells blockedCells;
	
    // Start is called before the first frame update
    void Start()
    {
		levelTilemap = GameObject.Find("Level Tilemap").GetComponent<Tilemap>();

		myStats = GetComponent<CharacterStats>();	
	
		blockedCells = levelTilemap.GetComponent<OccupiedCells>();
		
		Vector3Int myPosition = levelTilemap.WorldToCell(transform.position);

		blockedCells.AddOccupiedCell(myPosition);
    }

    // Update is called once per frame
    void Update()
    {
		if (myStats.isThisMyTurn && !myStats.isNarrativeActive)
		{
			
		}
    }
}
