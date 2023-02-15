using System;
using UnityEditor.Callbacks;
using UnityEngine;

public class MarcherCPU : MonoBehaviour
{
    public void RegenerateMesh()
    {
        Debug.Log("Im called on a hot reload! EPIC!");
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
}
