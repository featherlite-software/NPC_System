using System;
using UnityEngine;
using UnityEngine.VFX;

public class Main : MonoBehaviour
{

    CrowdSimulator Sim = new CrowdSimulator();
    CrowdSettings Settings = new CrowdSettings();

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        Settings.NpcCount = 128;
        Settings.PathDataDimensions = new Vector2Int(64,64);

        Settings.SpawnPositionsNpc = new Vector2Int[Settings.NpcCount];

        for (int Npc = 0; Npc < Settings.NpcCount; Npc++) {

            int XPos = (Npc%(Settings.PathDataDimensions.x/2))*2;
            int YPos = (Npc / (Settings.PathDataDimensions.x / 2))*2;


			Settings.SpawnPositionsNpc[Npc] = new Vector2Int(XPos, YPos);
            Settings.SeedNpc[Npc] = UnityEngine.Random.Range(0, 665536);

        }

        Sim.Initialize(Settings);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
