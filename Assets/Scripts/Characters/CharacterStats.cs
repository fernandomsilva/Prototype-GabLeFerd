using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public string charName;
    public string job;

    // atributos
    public int currentHealth;
    public int maxHealth;
    public int currentMana;
    public int maxMana;
    public int moveSpeed;
    public int meleeAttack;
    public int rangedAttack;
	public int meleeAttackRange;
	public int rangedAttackRange;
    public int meleeDefense;
    public int rangedDefense;
    public int magicAttack;
    public int magicDefense;
	
	public bool isThisMyTurn;
	public bool movedThisTurn;
	public bool attackedThisTurn;
	public bool isNarrativeActive;
	
	private float randomInitiativeValue;
	

    // Start is called before the first frame update
    void Start()
    {
	}
	
	void Awake()
	{
		currentHealth = maxHealth;
		currentMana = maxMana;
		
		isThisMyTurn = false;
		isNarrativeActive = false;
		
		randomInitiativeValue = Random.Range(0.000010f, 0.000099f);
	}
	
	public Vector3 GetWorldPosition()
	{
		return transform.position;
	}
	
	public void MoveToWorldPosition(Vector3 position)
	{
		transform.position = position;
	}
	
	public string GetTag()
	{
		return tag;
	}
	
	public void Buff(string attribute, float intensity)
	{
		if (attribute == "maxhp")
		{
			int amountOfExtraHealth = maxHealth;
			
			maxHealth = ((int) Mathf.Ceil(intensity * ((float) maxHealth)));
			amountOfExtraHealth = maxHealth - amountOfExtraHealth;
			
			currentHealth += amountOfExtraHealth;
		}
		else if (attribute == "attack")
		{
			meleeAttack = ((int) Mathf.Ceil(intensity * ((float) meleeAttack)));
			rangedAttack = ((int) Mathf.Ceil(intensity * ((float) rangedAttack)));
		}
		else if (attribute == "defense")
		{
			meleeDefense = ((int) Mathf.Ceil(intensity * ((float) meleeDefense)));
			rangedDefense = ((int) Mathf.Ceil(intensity * ((float) rangedDefense)));
		}
		else if (attribute == "movement")
		{
			moveSpeed = moveSpeed + ((int) Mathf.Floor(intensity));
		}
		else if (attribute == "magicattack")
		{
			magicAttack = ((int) Mathf.Ceil(intensity * ((float) magicAttack)));
		}
		else if (attribute == "magicdefense")
		{
			magicDefense = ((int) Mathf.Ceil(intensity * ((float) magicDefense)));
		}
	}
	
	public void StartTurn()
	{
		isThisMyTurn = true;
		movedThisTurn = false;
		attackedThisTurn = false;
	}
	
	public bool IsTurnDone()
	{
		return movedThisTurn && attackedThisTurn;
	}
	
	public void EndTurn()
	{
		isThisMyTurn = false;
	}

	public float TurnPriority()
	{
		return ((float) moveSpeed) + (((float) maxHealth) / 1000.0f) + randomInitiativeValue;
	}

	public float PercentOfHealthLeft()
	{
		return (float) currentHealth / (float) maxHealth;
	}
	
	public void Heal(int amountToHeal)
	{
		currentHealth += amountToHeal;
	}

	public void Ressucitate()
	{
		currentHealth = maxHealth;
		gameObject.SetActive(true);
	}

	public void RecoverMana(int amountToRecover)
	{
		currentMana += amountToRecover;
	}
	
	public void RecoverManaToMax()
	{
		currentMana = maxMana;
	}

	public void GotHit(int opponentAttack, string type)
	{
		if (type == "melee")
		{
			currentHealth -= Mathf.Max(opponentAttack - meleeDefense, 1);
		}
		else if (type == "ranged")
		{
			currentHealth -= Mathf.Max(opponentAttack - rangedDefense, 1);
		}
		else if (type == "magical")
		{
			currentHealth -= Mathf.Max(opponentAttack - magicDefense, 1);
		}
		
		if (currentHealth <= 0)
		{
			gameObject.SetActive(false);
		}
	}
	
	public void EscapeFromBattlefield()
	{
		currentHealth = 0;
		gameObject.SetActive(false);
	}

    // Update is called once per frame
    void Update()
    {
    
    }
}
