using Palmmedia.ReportGenerator.Core;
using Unity.Mathematics;
using UnityEngine;

public class CrowdSettings {

    public int NpcCount; //Max 60,000 without using full sized ints
    public Texture PathData;
    public Vector2Int PathDataDimensions;

}

public class CrowdSimulator : MonoBehaviour
{

    ComputeShader NpcCompute;

    public ComputeBuffer DataNpc; //Layout: 16Int:ID, 16Int:Seed, 32Vec2:RelativePosition
    public ComputeBuffer CellsNpc; //Layout 16Int:Index
    private ComputeBuffer CellsRace; //Layout 16Int:MovePriority, 16Int:OccupyPriority (It's not what you think it means!)
    public CrowdSettings Settings; 

    public void Initialize (CrowdSettings InitSettings) {

        int StrideNpcData = 4 + 4 + 8;
        int Area = InitSettings.PathDataDimensions.x * InitSettings.PathDataDimensions.y;
		DataNpc = new ComputeBuffer(Settings.NpcCount, StrideNpcData);
        CellsNpc = new ComputeBuffer(Area, 4);
        CellsRace = new ComputeBuffer(Area, 8);

        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
