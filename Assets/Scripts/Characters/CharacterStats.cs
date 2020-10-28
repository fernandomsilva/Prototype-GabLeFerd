using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public string name;
    public string job;


    // atributos
    public int currentHealth;
    public int maxHealth;
    public int currentMana;
    public int maxMana;
    public int moveSpeed;
    public int meleeAttack;
    public int rangedAttack;
    public int meleeDefense;
    public int rangedDefense;
    public int magicAttack;
    public int magicDefense;

    // Start is called before the first frame update
    void Start()
    {
     
        job = "Recruit";
      
}

    // Update is called once per frame
    void Update()
    {
      
      
    }
}
