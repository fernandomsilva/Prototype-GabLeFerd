using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageCharacter : CharacterStats
{
    CharacterStats mageStats;

    // Start is called before the first frame update
    void Start()
    {
		currentHealth = maxHealth;
		currentMana = maxMana;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
