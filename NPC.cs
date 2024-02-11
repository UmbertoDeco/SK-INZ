using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

[System.Serializable]
public class NPC : MonoBehaviour
{
    public string npcName;
    public Sprite npcSprite;
    public List<string> quests; 
    public GameObject prefab;
    public string quest;

    public GameObject dialogPanel;
    public Image npcImage;
    public Text questTextUI;

    public NPC(string name, Sprite sprite, List<string> quests, GameObject prefab)
    {
        npcName = name;
        npcSprite = sprite;
        this.quests = quests;
        this.prefab = prefab;
    }
    public void Interact()
    {
        dialogPanel.SetActive(true);
    }

    // Przyk³ad metody wywo³ywanej przez Unity, gdy gracz wchodzi w obszar NPC
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Interact();
        }
    }
    // Opcjonalnie: Metoda do ukrywania panelu dialogowego
    public void HideDialog()
    {
        dialogPanel.SetActive(false);
    }

}

