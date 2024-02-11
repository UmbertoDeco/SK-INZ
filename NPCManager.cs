using UnityEngine;
using System.Collections.Generic;

public class NPCManager : MonoBehaviour
{
    public List<NPC> predefinedNPCs; // Lista predefiniowanych NPC
    public GameObject npcPrefab;

    // Funkcja do generowania losowego NPC na mapie
    public GameObject SpawnRandomNPC()
    {
        if (predefinedNPCs.Count == 0) return null;

        int index = Random.Range(0, predefinedNPCs.Count);
        NPC selectedNPC = predefinedNPCs[index];

        return selectedNPC.prefab;
    }
}
