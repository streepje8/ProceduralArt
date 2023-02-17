using System;
using System.Runtime.InteropServices;
using UnityEngine;

struct Voxel
{
    public int type;
    public Vector3 position;
}

public class VoxelFactory : MonoBehaviour
{
    [Header("Settings")]
    public int cubeCount = 10;
    public float stepsPerSecond = 5;
    
    [Header("Rendering")]
    public Shader voxelRenderer;

    [Header("Processing")] 
    public ComputeShader voxelProcessor;
    
//  [Header("Buffers")]  
    private ComputeBuffer voxelBuffer;
    private int kernelIndex = 0;
    public Material voxelMaterial;
    private void Awake()
    {
        //Setup shaders
        kernelIndex = voxelProcessor.FindKernel("CSMain");
        voxelMaterial = new Material(voxelRenderer);
        
        //Allocate cubes
        CreateVoxels(cubeCount);
    }


    private float timer = 0f;
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > 1 / stepsPerSecond)
        {
            //UpdateVoxels();
            timer = 0;
        }
    }
    
    private void CreateVoxels(int amount)
    {
        Voxel[] voxels = new Voxel[amount];
        for (int i = 0; i < amount; i++)
        {
            voxels[i] = new Voxel();
        }
        if(voxelBuffer != null) voxelBuffer.Release();
        voxelBuffer = new ComputeBuffer(amount, Marshal.SizeOf<Voxel>());
        voxelBuffer.SetData(voxels);
        voxelProcessor.SetInt("voxelCount", amount);
    }

    private void UpdateVoxels()
    {
        voxelProcessor.SetBuffer(kernelIndex,"_VoxelData",voxelBuffer);
        voxelProcessor.Dispatch(kernelIndex,cubeCount/2,cubeCount/2,1);
    }

    private void OnRenderObject()
    {
        UpdateVoxels();
        voxelMaterial.SetPass(0);
        voxelMaterial.SetBuffer("_VoxelData", voxelBuffer);
        Graphics.DrawProceduralNow(MeshTopology.Points,cubeCount);
    }

    private void OnDestroy()
    {
        voxelBuffer.Release();
    }

    #region Code that i did not end up using, but that's still really cool
    /*
    public void RegenerateMesh()
    {
        Debug.Log("Im called on a hot reload!");
    }

    #region DEV
    public static bool hasReloaded;

    private void Update()
    {
        if (hasReloaded)
        {
            RegenerateMesh();
            hasReloaded = false;
        }
    }

    [DidReloadScripts]
    public static void OnScriptReload()
    {
        hasReloaded = true;
    }
    #endregion
    */
    #endregion
}
