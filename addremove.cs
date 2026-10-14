using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
	// Drag these in the Inspector
	public Transform content;          
	public Button itemButtonPrefab;   
	public Button addButton;
	public Button removeButton;
	public GameObject promptPanel;
	public TMP_Text promptLabel;
	public TMP_InputField promptInput;
	     
	List<string> names = new List<string>();
	List<int> counts = new List<int>();

	bool adding;          
	bool askingName;      
	string pendingName;

	void Start()
	{
		addButton.onClick.AddListener(() => StartAsk(true));
		removeButton.onClick.AddListener(() => StartAsk(false));
		promptInput.onSubmit.AddListener(delegate { Confirm(); });
		promptPanel.SetActive(false);
	}

	void StartAsk(bool isAdd)
	{
		adding = isAdd;
		askingName = true;
		promptLabel.text = "Item name?";
		promptInput.text = "";
		promptPanel.SetActive(true);
		promptInput.Select();
		promptInput.ActivateInputField();
	}

	void Confirm()
	{
		string answer = promptInput.text.Trim();

		if (askingName)
		{
			if (answer == "") return;
			pendingName = answer;
			askingName = false;
			promptLabel.text = "How many?";
			promptInput.text = "";
			promptInput.Select();
			promptInput.ActivateInputField();
		}
		else
		{
			int amount;
			if (!int.TryParse(answer, out amount) || amount <= 0) return;

			if (adding) AddItem(pendingName, amount);
			else RemoveItem(pendingName, amount);

			promptPanel.SetActive(false);
			Refresh();
		}
	}

	void AddItem(string itemName, int amount)
	{
		for (int i = 0; i < names.Count; i++)
		{
			if (names[i].ToLower() == itemName.ToLower())
			{
				counts[i] += amount;
				return;
			}
		}
		names.Add(itemName);
		counts.Add(amount);
	}

	void RemoveItem(string itemName, int amount)
	{
		for (int i = 0; i < names.Count; i++)
		{
			if (names[i].ToLower() == itemName.ToLower())
			{
				counts[i] -= amount;
				if (counts[i] <= 0)
				{
					names.RemoveAt(i);
					counts.RemoveAt(i);
				}
				return;
			}
		}
	}
	void Refresh()
	{
		foreach (Transform child in content)
			Destroy(child.gameObject);

		for (int i = 0; i < names.Count; i++)
		{
			Button b = Instantiate(itemButtonPrefab, content);
			b.GetComponentInChildren<TMP_Text>().text = names[i] + "  x" + counts[i];
		}
	}
}