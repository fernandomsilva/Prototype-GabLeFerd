using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NarrativeTriggers : MonoBehaviour
{
	public Tilemap levelTilemap;
	
	private Stack<string[]> pastEvents;
	private DialogueDatabase dialogueDB;
	private NarrativeManager manager;
	
	private int enemyFullPartySize;
	
    // Start is called before the first frame update
    void Start()
    {		
		pastEvents = new Stack<string[]>();
		
		dialogueDB = GetComponent<DialogueDatabase>();
		
		manager = GetComponent<NarrativeManager>();
		
		enemyFullPartySize = -1;
    }
	
	public void EventPast(string[] eventThatJustHappened)
	{
		pastEvents.Push(eventThatJustHappened);
		
		if (enemyFullPartySize == -1)
		{
			enemyFullPartySize = manager.GetListOfCharactersAliveWithTag("Enemy").Count;
		}
		
		CheckForTriggers();
	}
	
	HashSet<string> GetAdjacentCharacters(CharacterStats character, string tagRequiredForAdjacentCharacters)
	{
		Vector3Int characterCellPosition = levelTilemap.WorldToCell(character.GetWorldPosition());

		List<string> charactersAlive = manager.GetListOfCharactersAlive();
		HashSet<string> adjacentChars = new HashSet<string>();
		
		foreach (string charName in charactersAlive)
		{
			CharacterStats otherCharacter = manager.GetCharacter(charName);
			
			if (otherCharacter.GetTag() == tagRequiredForAdjacentCharacters)
			{
				Vector3Int otherCharacterCellPosition = levelTilemap.WorldToCell(otherCharacter.GetWorldPosition());
				
				if (Vector3Int.Distance(otherCharacterCellPosition, characterCellPosition) == 1)
				{
					adjacentChars.Add(otherCharacter.charName);
				}
			}
		}
		
		return adjacentChars;
	}
	
	public void CheckForTriggers()
	{
		string[] latestEvent = pastEvents.Peek();
				
		string playerCharacterName = latestEvent[0];
		CharacterStats playerCharacter = manager.GetCharacter(playerCharacterName);
		
		if (latestEvent[1] == "move")
		{
			HashSet<string> adjacentEnemies = GetAdjacentCharacters(playerCharacter, "Enemy");
			
			foreach (string enemy in adjacentEnemies)
			{
				HashSet<string> playerCharactersAdjacentToThisEnemy = GetAdjacentCharacters(manager.GetCharacter(enemy), "Player");

				if (playerCharactersAdjacentToThisEnemy.Count > 1)
				{
					string dialogueReference = "surrounded,any," + enemy;
					
					if (dialogueDB.IsConditionInDialogueOptions(dialogueReference))
					{
						manager.TriggerNarrative(dialogueDB.GetDialogueAttributes(dialogueReference));
						dialogueDB.RemoveDialogueCondition(dialogueReference);
						return;
					}
				}
			}
		}
		else if (latestEvent[1] == "melee")
		{
			string enemyCharacterName = latestEvent[2];
			string dialogueReference = "firstpartymembertodie,any," + enemyCharacterName;
			
			int currentEnemyPartySize = manager.GetListOfCharactersAliveWithTag("Enemy").Count;
			
			if (currentEnemyPartySize == enemyFullPartySize - 1)
			{
				if (dialogueDB.IsConditionInDialogueOptions(dialogueReference))
				{
					manager.TriggerNarrative(dialogueDB.GetDialogueAttributes(dialogueReference));
					dialogueDB.RemoveDialogueCondition(dialogueReference);
					return;
				}
			}

			dialogueReference = "friendsfighting," + playerCharacterName + "," + enemyCharacterName;
			if (dialogueDB.IsConditionInDialogueOptions(dialogueReference))
			{
				manager.TriggerNarrative(dialogueDB.GetDialogueAttributes(dialogueReference));
				dialogueDB.RemoveDialogueCondition(dialogueReference);
				return;
			}

			dialogueReference = "firstfight,any," + enemyCharacterName;
			
			if (dialogueDB.IsConditionInDialogueOptions(dialogueReference))
			{
				manager.TriggerNarrative(dialogueDB.GetDialogueAttributes(dialogueReference));
				dialogueDB.RemoveDialogueCondition(dialogueReference);
				return;
			}

			dialogueReference = "lowhealth,any," + enemyCharacterName;
			CharacterStats enemy = manager.GetCharacter(enemyCharacterName);

			if (enemy.PercentOfHealthLeft() <= 0.3 && dialogueDB.IsConditionInDialogueOptions(dialogueReference))
			{
				manager.TriggerNarrative(dialogueDB.GetDialogueAttributes(dialogueReference));
				dialogueDB.RemoveDialogueCondition(dialogueReference);
				return;
			}
			
			List<string> enemiesAlive = manager.GetListOfCharactersAliveWithTag("Enemy");
			if (enemiesAlive.Count == 1)
			{
				dialogueReference = "lastmanstanding,any," + enemiesAlive[0];
			
				if (dialogueDB.IsConditionInDialogueOptions(dialogueReference))
				{
					manager.TriggerNarrative(dialogueDB.GetDialogueAttributes(dialogueReference));
					dialogueDB.RemoveDialogueCondition(dialogueReference);
					return;
				}
			}		
		}
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
