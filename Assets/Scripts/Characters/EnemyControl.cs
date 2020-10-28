using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyControl : MonoBehaviour
{
	public Tilemap levelTilemap;
	
	private OccupiedCells blockedCells;
	
    // Start is called before the first frame update
    void Start()
    {
		blockedCells = levelTilemap.GetComponent<OccupiedCells>();
		
		Vector3Int myPosition = levelTilemap.WorldToCell(transform.position);
		//Debug.Log(myPosition);
		blockedCells.AddOccupiedCell(myPosition);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
