using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OccupiedCells : MonoBehaviour
{
	private List<Vector3Int> occupiedCells = new List<Vector3Int>();
	
    // Start is called before the first frame update
    void Start()
    {
        //occupiedCells = new List<Vector3Int>();
    }
	
	public void AddOccupiedCell(Vector3Int cell)
	{
		occupiedCells.Add(cell);
	}
	
	public void RemoveOccupiedCell(Vector3Int cell)
	{
		if (IsCellOccupied(cell))
		{
			occupiedCells.Remove(cell);
		}
	}
	
	public bool IsCellOccupied(Vector3Int cell)
	{
		return occupiedCells.Contains(cell);
	}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
		{
			foreach (Vector3Int cell in occupiedCells)
			{
				Debug.Log(cell);
			}
		}
    }
}
