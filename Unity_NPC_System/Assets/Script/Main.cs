using UnityEngine;

public class Main : MonoBehaviour
{

    CrowdSimulator Sim = new CrowdSimulator();
    CrowdSettings Settings = new CrowdSettings();

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        Settings.NpcCount = 128;
        Settings.PathDataDimensions = new Vector2Int(64,64);
        Sim.Initialize(Settings);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
