using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageCharacter : MonoBehaviour
{
    CharacterStats mageStats;


    // Start is called before the first frame update
    void Start()
    {
        mageStats = GetComponent<CharacterStats>();
        mageStats.maxHealth = 50;
        //mageStats = new CharacterStats();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
