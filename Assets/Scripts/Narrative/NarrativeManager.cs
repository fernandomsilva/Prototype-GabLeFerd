using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NarrativeManager : MonoBehaviour
{
	public GameObject dialogueBox;
	public Text dialogueBoxText;
	public GameObject dialogueBoxOption1;
	private Button buttonOption1;
	public Text dialogueBoxOption1Text;
	public GameObject dialogueBoxOption2;
	private Button buttonOption2;
	public Text dialogueBoxOption2Text;
	
	public bool narrativeActive;
	
	private int dialoguePhase;
	private Dictionary<string, string> currentDialogueTree;
	
	private Dictionary<string, CharacterStats> charactersInScene;
	
	private string activeCharacter;
	private Dictionary<float, CharacterStats> characterActionOrder;
	
	private Queue<CharacterStats> currentTurnOrder;
	private CharacterStats currentCharacterActing;
	
    // Start is called before the first frame update
    void Start()
    {
		buttonOption1 = dialogueBoxOption1.GetComponent<Button>();
		buttonOption1.onClick.AddListener(ChooseOption1);
		
		buttonOption2 = dialogueBoxOption2.GetComponent<Button>();
		buttonOption2.onClick.AddListener(ChooseOption2);
		
		narrativeActive = false;
		dialoguePhase = 0;
		currentDialogueTree = null;
		
        charactersInScene = new Dictionary<string, CharacterStats>();
		
		foreach (GameObject playerCharacter in GameObject.FindGameObjectsWithTag("Player"))
		{
			charactersInScene.Add(playerCharacter.GetComponent<CharacterStats>().charName, playerCharacter.GetComponent<CharacterStats>());
		}
		foreach (GameObject enemyCharacter in GameObject.FindGameObjectsWithTag("Enemy"))
		{
			charactersInScene.Add(enemyCharacter.GetComponent<CharacterStats>().charName, enemyCharacter.GetComponent<CharacterStats>());
		}
		
		characterActionOrder = new Dictionary<float, CharacterStats>();
		currentTurnOrder = new Queue<CharacterStats>();
		
		currentCharacterActing = null;

		NextTurn();
    }

	public CharacterStats GetCharacter(string name)
	{
		if (charactersInScene.ContainsKey(name))
		{
			return charactersInScene[name];
		}
		
		return null;
	}
	
	public List<string> GetListOfCharactersAlive()
	{
		List<string> result = new List<string>();
		
		foreach (string charName in charactersInScene.Keys)
		{
			if (charactersInScene[charName].currentHealth > 0)
			{
				result.Add(charName);
			}
		}
		
		return result;
	}
	
	public List<string> GetListOfCharactersAliveWithTag(string tag)
	{
		List<string> result = new List<string>();
		
		foreach (string charName in charactersInScene.Keys)
		{
			if (charactersInScene[charName].currentHealth > 0 && charactersInScene[charName].GetTag() == tag)
			{
				result.Add(charName);
			}
		}
		
		return result;
	}
	
	void OrderCharacterActions()
	{
		characterActionOrder.Clear();
		
		foreach (string characterName in charactersInScene.Keys)
		{
			characterActionOrder.Add(charactersInScene[characterName].TurnPriority(), charactersInScene[characterName]);
		}		
	}
	
	void CreateTurnOrder()
	{
		OrderCharacterActions();
		
		currentTurnOrder.Clear();
		
		List<float> keyList = new List<float>(characterActionOrder.Keys);
		keyList.Sort();
		keyList.Reverse();
		
		foreach (float key in keyList)
		{
			//Debug.Log("Q: " + characterActionOrder[key].charName + " / " + key);
			if (characterActionOrder[key].currentHealth > 0)
			{
				currentTurnOrder.Enqueue(characterActionOrder[key]);
			}
		}
	}
	
	void NextTurn()
	{
		CharacterStats nextCharacter = null;
		
		if (currentCharacterActing != null)
		{
			currentCharacterActing.EndTurn();
		}
		
		while (currentTurnOrder.Count >= 1)
		{
			nextCharacter = currentTurnOrder.Dequeue();
			
			if (nextCharacter.currentHealth > 0)
			{
				break;
			}
		}
		
		if (currentTurnOrder.Count == 0)
		{
			CreateTurnOrder();
		}
		
		if (nextCharacter == null || nextCharacter.currentHealth <= 0)
		{
			nextCharacter = currentTurnOrder.Dequeue();
		}
		
		currentCharacterActing = nextCharacter;
		currentCharacterActing.StartTurn();
	}

	void UpdateNarrativeStateToCharacters(bool state)
	{
		foreach (string charName in charactersInScene.Keys)
		{
			charactersInScene[charName].isNarrativeActive = state;
		}
	}

	public void TriggerNarrative(Dictionary<string, string> dialogueTree)
	{
		narrativeActive = true;
		UpdateNarrativeStateToCharacters(narrativeActive);
		
		currentDialogueTree = dialogueTree;
		
		dialoguePhase = 0;
		dialogueBoxText.text = currentDialogueTree["first sentence"];
		dialogueBox.SetActive(true);
	}

	void ChooseDialogueOption(int optionNumber)
	{
		buttonOption1.enabled = false;
		buttonOption2.enabled = false;
		
		dialoguePhase += optionNumber;
		
		dialogueBoxOption1.SetActive(false);
		dialogueBoxOption2.SetActive(false);
		dialogueBoxText.text = currentDialogueTree["response" + optionNumber];
	}

	void ChooseOption1()
	{
		ChooseDialogueOption(1);
	}

	void ChooseOption2()
	{
		ChooseDialogueOption(2);
	}
	
    // Update is called once per frame
    void Update()
    {
        if (currentCharacterActing.IsTurnDone() && !narrativeActive)
		{
			NextTurn();
		}
		else if (narrativeActive)
		{
			if (Input.GetMouseButtonDown(1))
			{
				if (dialoguePhase == 0)
				{
					dialoguePhase += 1;
					
					dialogueBoxOption1Text.text = currentDialogueTree["option1"];
					dialogueBoxOption2Text.text = currentDialogueTree["option2"];
					dialogueBoxText.text = "";
					buttonOption1.enabled = true;
					buttonOption2.enabled = true;
					dialogueBoxOption1.SetActive(true);
					dialogueBoxOption2.SetActive(true);
				}
				else if (dialoguePhase > 1)
				{
					dialogueBox.SetActive(false);
					int outcome_value = dialoguePhase - 1;
					Debug.Log(currentDialogueTree["outcome" + outcome_value]);
					
					narrativeActive = false;
					UpdateNarrativeStateToCharacters(narrativeActive);
				}
			}
		}
    }
}
