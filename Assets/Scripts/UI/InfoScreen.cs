using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoScreen : MonoBehaviour
{
	private GameObject[] portraits;
	private GameObject[] healthInfo;
	
	private Dictionary<string, CharacterStats> charInfo;
	
    // Start is called before the first frame update
    void Start()
    {
        portraits = GameObject.FindGameObjectsWithTag("Portrait Info");
		healthInfo = GameObject.FindGameObjectsWithTag("Health Info");
		
		foreach (GameObject obj in portraits)
		{
			obj.SetActive(false);
		}
		foreach (GameObject obj in healthInfo)
		{
			obj.SetActive(false);
		}
		
		charInfo = new Dictionary<string, CharacterStats>();
		
		foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Player"))
		{
			CharacterStats charStats = obj.GetComponent<CharacterStats>();
			
			charInfo.Add(charStats.charName + " Info", charStats); 
		}

		foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Enemy"))
		{
			CharacterStats charStats = obj.GetComponent<CharacterStats>();
			
			charInfo.Add(charStats.charName + " Info", charStats);
		}
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Tab))
		{
			foreach (string key in charInfo.Keys)
			{
				foreach (GameObject obj in healthInfo)
				{
					if (obj.name == key)
					{
						obj.GetComponent<Text>().text = "" + charInfo[key].currentHealth;
						break;
					}
				}
			}
			
			foreach (GameObject obj in portraits)
			{
				if (!obj.activeSelf)
				{
					obj.SetActive(true);
				}
			}
			foreach (GameObject obj in healthInfo)
			{
				if (!obj.activeSelf)
				{
					obj.SetActive(true);
				}
			}

		}
		if (Input.GetKeyUp(KeyCode.Tab))
		{
			foreach (GameObject obj in portraits)
			{
				obj.SetActive(false);
			}
			foreach (GameObject obj in healthInfo)
			{
				obj.SetActive(false);
			}
		}
    }
}
