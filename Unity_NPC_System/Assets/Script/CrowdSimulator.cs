using Palmmedia.ReportGenerator.Core;
using System.Drawing;
using Unity.Mathematics;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

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

	RenderParams NpcRenderParams;

    public int Step = 0;

    //public ComputeBuffer DataNpcId; 
    public ComputeBuffer DataNpcSeed;
	public ComputeBuffer DataNpcPos;
    public ComputeBuffer DataNpcDir;
    public ComputeBuffer FinalNpcPos;
    private ComputeBuffer DataCellMovement;
	public ComputeBuffer CellsNpc;
    private ComputeBuffer CellsRace;

    int ComputeNpcKID;
    int CheckRaceKID;
    int FinalizeDataKID;

    //int DataNpcIdBID = Shader.PropertyToID("DataNpcId_bAI");
    int DataNpcSeedBID = Shader.PropertyToID("DataNpcSeed_bAI");
	int DataNpcPosBID = Shader.PropertyToID("DataNpcPos_bAI");
    int DataNpcDirBID = Shader.PropertyToID("DataNpcDir_bAI");
	int FinalNpcPosBID = Shader.PropertyToID("FinalNpcPos_bAI");
    int FinalPosVertexBID = Shader.PropertyToID("FinalNpcPos_bNpcSurf");
    int DataCellMovementBID = Shader.PropertyToID("DataCellMovement_bAI");
	int CellsNpcBID = Shader.PropertyToID("CellsNpc_bAI");
	int CellsRaceBID = Shader.PropertyToID("CellsRace_bAI");

    int PathBoundsUID = Shader.PropertyToID("CellBounds_uAI"); // Int2
    int NpcCountUID = Shader.PropertyToID("NpcCount_bNpcSurf"); //Int
    int StepUID = Shader.PropertyToID("Step_uAI"); //Int

	public CrowdSettings Settings; 

    public void Initialize (CrowdSettings InitSettings) {

		NpcCompute = Resources.Load<ComputeShader>("Shader/ComputeAI");
		NpcRender = Resources.Load<Material>("Shader/RenderNpc");
        
        ComputeNpcKID = NpcCompute.FindKernel("ComputeAI");
		CheckRaceKID = NpcCompute.FindKernel("CheckRace");
	    FinalizeDataKID = NpcCompute.FindKernel("FinalizeData");

		Settings = InitSettings;

		Vector3 BoundPosition = new Vector3(((float)Settings.PathDataDimensions.x) / 2.0f, 0.0f, ((float)Settings.PathDataDimensions.x) / 2.0f);
		Vector3 BoundSize = new Vector3((float)Settings.PathDataDimensions.x + 2.0f, 2.0f, (float)Settings.PathDataDimensions.x + 2.0f);
		Bounds NpcBounds = new Bounds(BoundPosition, BoundSize);
		NpcRenderParams = new RenderParams(NpcRender);
		NpcRenderParams.worldBounds = NpcBounds;

        NpcRenderParams.material.SetInt(NpcCountUID, Settings.NpcCount);

		int Area = Settings.PathDataDimensions.x * Settings.PathDataDimensions.y;
		//DataNpcId = new ComputeBuffer(Settings.NpcCount, 4); // Int (32)
		DataNpcPos = new ComputeBuffer(Settings.NpcCount, 8); // Float Vec2 (64)
		DataNpcDir = new ComputeBuffer(Settings.NpcCount, 8); // Float Vec2 (64)
        DataNpcSeed = new ComputeBuffer(Settings.NpcCount, 4); // uInt (32) (Is just a signed int on cpu side)
        FinalNpcPos = new ComputeBuffer(Settings.NpcCount, 12); // Float Vec3 (96)
        DataCellMovement = new ComputeBuffer(Settings.NpcCount, 8); //Int Vec2 (64)
		CellsNpc = new ComputeBuffer(Area, 4); // Int (32)
        CellsRace = new ComputeBuffer(Area, 8); // Int Vec2 (64)

        DataNpcSeed.SetData(Settings.SeedNpc);
        int[] StartingCells = new int[Area];


		for (int Index = 0; Index < Area; Index++) {
            StartingCells[Index] = -1;
		}

        for (int Index = 0; Index < Settings.NpcCount; Index++) {
            Vector2Int Pos = Settings.SpawnPositionsNpc[Index];
            int PosIndex = Pos.x + (Pos.y * Settings.PathDataDimensions.x);
            StartingCells[PosIndex] = Index;
        }

        

        CellsNpc.SetData(StartingCells);

        NpcCompute.SetInts(PathBoundsUID, InitSettings.PathDataDimensions.x, InitSettings.PathDataDimensions.y);

		// NpcCompute.SetBuffer(ComputeNpcKID, DataNpcIdBID, DataNpcId);
		NpcCompute.SetBuffer(ComputeNpcKID, DataNpcSeedBID, DataNpcSeed);
		NpcCompute.SetBuffer(ComputeNpcKID, DataNpcPosBID, DataNpcPos);
        NpcCompute.SetBuffer(ComputeNpcKID, DataNpcDirBID, DataNpcDir);
        NpcCompute.SetBuffer(ComputeNpcKID, DataCellMovementBID, DataCellMovement);
		//NpcCompute.SetBuffer(ComputeNpcKID, FinalNpcPosBID, FinalNpcPos);
		NpcCompute.SetBuffer(ComputeNpcKID, CellsNpcBID, CellsNpc);
		NpcCompute.SetBuffer(ComputeNpcKID, CellsRaceBID, CellsRace);

        NpcCompute.SetBuffer(CheckRaceKID, CellsRaceBID, CellsRace);
		//NpcCompute.SetBuffer(CheckRaceKID, DataNpcDirBID, DataNpcDir);
		NpcCompute.SetBuffer(CheckRaceKID, CellsNpcBID, CellsNpc);
		NpcCompute.SetBuffer(CheckRaceKID, DataCellMovementBID, DataCellMovement);

		NpcCompute.SetBuffer(FinalizeDataKID, CellsRaceBID, CellsRace);
		NpcCompute.SetBuffer(FinalizeDataKID, FinalNpcPosBID, FinalNpcPos);
		NpcCompute.SetBuffer(FinalizeDataKID, CellsNpcBID, CellsNpc);

		NpcRenderParams.material.SetBuffer(FinalPosVertexBID, FinalNpcPos);
	}

    public void StepSimulation () {
		Vector2Int Size = Settings.PathDataDimensions;

        NpcCompute.SetInt(StepUID, Step);
        Step++;
        Vector3Int DispatchSize = new Vector3Int(Mathf.FloorToInt(((float)Size.x) / 8.0f), Mathf.FloorToInt(((float)Size.y) / 8.0f), 1);
        NpcCompute.Dispatch(ComputeNpcKID, DispatchSize.x, DispatchSize.y, DispatchSize.z);
        //GraphicsFence NpcFence = Graphics.CreateGraphicsFence(GraphicsFenceType.AsyncQueueSynchronisation, SynchronisationStageFlags.AllGPUOperations);
        //Graphics.WaitOnAsyncGraphicsFence(NpcFence);
		NpcCompute.Dispatch(CheckRaceKID, DispatchSize.x, DispatchSize.y, DispatchSize.z);
        //MonoBehaviour.print(NpcFence.passed);

		//GraphicsFence RaceFence = Graphics.CreateGraphicsFence(GraphicsFenceType.AsyncQueueSynchronisation, SynchronisationStageFlags.AllGPUOperations);
		//Graphics.WaitOnAsyncGraphicsFence(RaceFence);
		NpcCompute.Dispatch(FinalizeDataKID, DispatchSize.x, DispatchSize.y, DispatchSize.z);
		//MonoBehaviour.print(RaceFence.passed);

	}

     public void Draw () {

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
