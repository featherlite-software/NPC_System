using System;
using UnityEngine;
using UnityEngine.VFX;

public class Main : MonoBehaviour
{

    [SerializeField]
    Mesh NpcMesh;

    CrowdSimulator Sim = new CrowdSimulator();
    CrowdSettings Settings = new CrowdSettings();

    float LastStepTime = 0.0f;
    float StepSpeed = 0.25f;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        Settings.NpcCount = 1024;
        Settings.PathDataDimensions = new Vector2Int(128,128);
        Settings.NpcMesh = NpcMesh;

        Settings.SpawnPositionsNpc = new Vector2Int[Settings.NpcCount];

		Settings.SeedNpc = new int[Settings.NpcCount];
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
        if (LastStepTime+StepSpeed < Time.time) {
			Sim.StepSimulation();
            LastStepTime = Time.time;
	    }
        Sim.Draw();
	}
}
