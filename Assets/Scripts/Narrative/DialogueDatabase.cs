using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueDatabase : MonoBehaviour
{
	public TextAsset dialogueDataFile;
	
	private Dictionary<string, Dictionary<string, string>> dialogueOptions;
	
	// Start is called before the first frame update
    void Start()
    {
		dialogueOptions = new Dictionary<string, Dictionary<string, string>>();

		PopulateDialogueOptionsFromFileContents(dialogueDataFile.text);
    }

	void PopulateDialogueOptionsFromFileContents(string fileContents)
	{
		foreach (string dialogueTree in fileContents.Split('\n'))
		{
			if (dialogueTree != "")
			{
				string[] dialogue = dialogueTree.Split(';');
				
				string condition = dialogue[0] + "," + dialogue[1] + "," + dialogue[2];
				
				Dictionary<string, string> attributes = new Dictionary<string, string>();
				
				attributes.Add("first sentence", dialogue[3]);
				attributes.Add("option1", dialogue[4]);
				attributes.Add("option2", dialogue[5]);
				attributes.Add("response1", dialogue[6]);
				attributes.Add("response2", dialogue[7]);
				attributes.Add("outcome1", dialogue[8]);
				attributes.Add("outcome2", dialogue[9]);
				
				dialogueOptions.Add(condition, attributes);
			}
		}
	}
	
	public bool IsConditionInDialogueOptions(string condition)
	{
		return dialogueOptions.ContainsKey(condition);
	}
	
	public Dictionary<string, string> GetDialogueAttributes(string condition)
	{
		return dialogueOptions[condition];
	}
	
	public void RemoveDialogueCondition(string condition)
	{
		dialogueOptions.Remove(condition);
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
