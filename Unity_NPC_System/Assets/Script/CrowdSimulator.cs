using Palmmedia.ReportGenerator.Core;
using Unity.Mathematics;
using UnityEngine;

public class CrowdSettings {

    public int NpcCount;
    public float[] CostData; //Negative cost is impassible
    public Vector2Int[] SpawnPositionsNpc;
    public int[] SeedNpc;
    public Vector2Int PathDataDimensions;
    public float[] HeightMap;
    public Mesh NpcMesh;

}

public class CrowdSimulator
{

    ComputeShader NpcCompute;
    Material NpcRender;

    //public ComputeBuffer DataNpcId; 
    public ComputeBuffer DataNpcSeed;
	public ComputeBuffer DataNpcPos;
    public ComputeBuffer FinalNpcPos;
	public ComputeBuffer CellsNpc;
    private ComputeBuffer CellsRace;

    int ComputeNpcKID;

    //int DataNpcIdBID = Shader.PropertyToID("DataNpcId_bAI");
    int DataNpcSeedBID = Shader.PropertyToID("DataNpcSeed_bAI");
	int DataNpcPosBID = Shader.PropertyToID("DataNpcPos_bAI");
	int FinalNpcPosBID = Shader.PropertyToID("FinalNpcPos_bAI");
    int FinalPosVertexBID = Shader.PropertyToID("FinalNpcPos_bNpcSurf");
	int CellsNpcBID = Shader.PropertyToID("CellsNpc_bAI");
	int CellsRaceBID = Shader.PropertyToID("DCellsRace_bAI");

    int PathBoundsUID = Shader.PropertyToID("CellBounds_uAI"); // Int2

	public CrowdSettings Settings; 

    public void Initialize (CrowdSettings InitSettings) {

		NpcCompute = Resources.Load<ComputeShader>("Shader/ComputeAI");
		NpcRender = Resources.Load<Material>("Shader/RenderNpc");

		ComputeNpcKID = NpcCompute.FindKernel("ComputeAI");

        Settings = InitSettings;

        int Area = Settings.PathDataDimensions.x * Settings.PathDataDimensions.y;
		//DataNpcId = new ComputeBuffer(Settings.NpcCount, 4); // Int (32)
		DataNpcPos = new ComputeBuffer(Settings.NpcCount, 4); // Half Vec2 (32)
        DataNpcSeed = new ComputeBuffer(Settings.NpcCount, 4); // uInt (32) (Is just a signed int on cpu side)
        FinalNpcPos = new ComputeBuffer(Settings.NpcCount, 12); // Float Vec3 (96)
		CellsNpc = new ComputeBuffer(Area, 4); // Int (32)
        CellsRace = new ComputeBuffer(Area, 8); // Int Vec2 (64)

        DataNpcSeed.SetData(Settings.SeedNpc);
        int[] StartingCells = new int[Area];

        for (int Index = 0; Index < Area; Index++) {
            StartingCells[Index] = -1;
        }

        for (int Index = 0; Index < Settings.NpcCount; Index++) {
            Vector2Int Pos = Settings.SpawnPositionsNpc[Index];
            int PosIndex = Pos.x + Pos.y * Settings.PathDataDimensions.x;
            StartingCells[PosIndex] = Index;
        }

        CellsNpc.SetData(StartingCells);

        NpcCompute.SetInts(PathBoundsUID, InitSettings.PathDataDimensions.x * InitSettings.PathDataDimensions.y);

		// NpcCompute.SetBuffer(ComputeNpcKID, DataNpcIdBID, DataNpcId);
		NpcCompute.SetBuffer(ComputeNpcKID, DataNpcSeedBID, DataNpcSeed);
		NpcCompute.SetBuffer(ComputeNpcKID, DataNpcPosBID, DataNpcPos);
		NpcCompute.SetBuffer(ComputeNpcKID, FinalNpcPosBID, FinalNpcPos);
		NpcCompute.SetBuffer(ComputeNpcKID, CellsNpcBID, CellsNpc);
		NpcCompute.SetBuffer(ComputeNpcKID, CellsRaceBID, CellsRace);


	}

    public void StepSimulation () {
        Vector2Int Size = Settings.PathDataDimensions;

        NpcCompute.Dispatch(ComputeNpcKID, Mathf.FloorToInt(((float)Size.x) / 8.0f), Mathf.FloorToInt(((float)Size.y) / 8.0f), 1);

        Bounds NpcBounds = new Bounds(new Vector3 (((float)Size.x)/2.0f, 0.0f, ((float)Size.y) / 2.0f), new Vector3((float)Size.x, 2.0f, (float)Size.y));
        RenderParams NpcRenderParams = new RenderParams(NpcRender);
        NpcRenderParams.worldBounds = NpcBounds;

        NpcRenderParams.material.SetBuffer(FinalPosVertexBID, FinalNpcPos);

        Graphics.RenderMeshPrimitives(NpcRenderParams, Settings.NpcMesh, 0, Settings.NpcCount);
     }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    /*void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }*/
}
