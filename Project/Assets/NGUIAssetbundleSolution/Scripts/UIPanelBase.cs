using System;
using System.Collections.Generic;
using UnityEngine;
public class UIPanelBase : MonoBehaviour
{
    void OnDestroy()
    {
        if (Application.isPlaying)
        {
            Debug.Log("destroy:" + gameObject.name);
            if (UIAssetbundleManager.instance!=null)
                UIAssetbundleManager.instance.UnloadResource(gameObject.name);
        }
    }
}
