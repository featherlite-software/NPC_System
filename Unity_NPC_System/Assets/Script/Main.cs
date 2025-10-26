using System;
using UnityEngine;
using UnityEngine.VFX;

public class Main : MonoBehaviour
{

    [SerializeField]
    Mesh NpcMesh;

    CrowdSimulator Sim = new CrowdSimulator();
    CrowdSettings Settings = new CrowdSettings();

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        Settings.NpcCount = 32;
        Settings.PathDataDimensions = new Vector2Int(64,64);
        Settings.NpcMesh = NpcMesh;

        Settings.SpawnPositionsNpc = new Vector2Int[Settings.NpcCount];

		Settings.SeedNpc = new int[Settings.NpcCount];
		Settings.SpawnPositionsNpc = new Vector2Int[Settings.NpcCount];

		for (int Npc = 0; Npc < Settings.NpcCount; Npc++) {

            int XPos = Npc;//(Npc%(Settings.PathDataDimensions.x/2))*2;
            int YPos = 0;//(Npc / (Settings.PathDataDimensions.x / 2))*2;

			Settings.SpawnPositionsNpc[Npc] = new Vector2Int(XPos, YPos);
            Settings.SeedNpc[Npc] = UnityEngine.Random.Range(0, 665536);

        }
        
        Console.WriteLine("e");
        Console.WriteLine(Settings.SeedNpc);

        Sim.Initialize(Settings);
    }

    // Update is called once per frame
    void Update()
    {
		Sim.StepSimulation();
	}
}
