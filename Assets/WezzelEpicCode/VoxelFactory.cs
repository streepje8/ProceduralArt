using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;

struct Voxel
{
    public int type;
    public Vector3 position;
    public Vector3 velocity;
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
    //private List<ComputeBuffer> otherBuffers = new List<ComputeBuffer>();
    private int kernelIndex = 0;
    //private int initKernelIndex = 0;
    public Material voxelMaterial;
    public int cellSize = 1;
    private bool voxelsUpdated = false;
    private void Awake()
    {
        //Setup shaders
        kernelIndex = voxelProcessor.FindKernel("CSMain");
        //initKernelIndex = voxelProcessor.FindKernel("Init");
        if(voxelMaterial == null) voxelMaterial = new Material(voxelRenderer);
        
        //Allocate cubes
        CreateVoxels(cubeCount);
        
        //More info in the shader
        // voxelProcessor.SetVector("position", transform.position);
        // voxelProcessor.SetFloat("deltaTime", Time.deltaTime);
        // voxelProcessor.SetFloat("time", Time.time);
        // voxelProcessor.SetBuffer(kernelIndex,"_VoxelData",voxelBuffer);
        // voxelProcessor.SetInt("cellSize", cellSize);
        // otherBuffers.Add(new ComputeBuffer(300*300*300, sizeof(int)));
        // otherBuffers.Add(new ComputeBuffer(cubeCount, sizeof(int)));
        // voxelProcessor.SetBuffer(initKernelIndex,"_CellIndices",otherBuffers[0]);
        // voxelProcessor.SetBuffer(kernelIndex,"_CellIndices",otherBuffers[0]);
        // voxelProcessor.SetBuffer(initKernelIndex,"_VoxelIndices",otherBuffers[1]);
        // voxelProcessor.SetBuffer(kernelIndex,"_VoxelIndices",otherBuffers[1]);
        // voxelProcessor.Dispatch(initKernelIndex,1,1,1);
    }


    private float timer = 0f;
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > 1 / stepsPerSecond)
        {
            UpdateVoxels();
            timer = 0;
        }
    }
    
    private void CreateVoxels(int amount)
    {
        Voxel[] voxels = new Voxel[amount];
        for (int i = 0; i < amount; i++)
        {
            voxels[i] = new Voxel();
            voxels[i].position = new Vector3(0, i * 2, 0);
        }
        if(voxelBuffer != null) voxelBuffer.Release();
        voxelBuffer = new ComputeBuffer(amount, Marshal.SizeOf<Voxel>());
        voxelBuffer.SetData(voxels);
        voxelProcessor.SetInt("voxelCount", amount);
    }

    private void UpdateVoxels()
    {
        voxelsUpdated = true;
        voxelProcessor.SetVector("position", transform.position);
        voxelProcessor.SetFloat("deltaTime", Time.deltaTime);
        voxelProcessor.SetFloat("time", Time.time);
        voxelProcessor.SetBuffer(kernelIndex,"_VoxelData",voxelBuffer);
        voxelProcessor.SetInt("cellSize", cellSize);
        voxelProcessor.Dispatch(kernelIndex,cubeCount/2,cubeCount/2,1);
    }

    private void OnRenderObject()
    {
        if(!voxelsUpdated)UpdateVoxels();
        voxelMaterial.SetPass(0);
        voxelMaterial.SetBuffer("_VoxelData", voxelBuffer);
        Graphics.DrawProceduralNow(MeshTopology.Points,cubeCount);
    }

    private void OnDestroy()
    {
        voxelBuffer.Release();
        // foreach (var buffer in otherBuffers)
        // {
        //     buffer.Release();
        // }
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
