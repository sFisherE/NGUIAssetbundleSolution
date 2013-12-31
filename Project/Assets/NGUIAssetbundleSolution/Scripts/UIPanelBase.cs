using System;
using System.Collections.Generic;
using UnityEngine;
public class UIPanelBase : MonoBehaviour
{
    void OnDestroy()
    {
        Debug.Log("destroy:" + gameObject.name);
        UIAssetbundleManager.instance.UnloadResource(gameObject.name);
    }
}
