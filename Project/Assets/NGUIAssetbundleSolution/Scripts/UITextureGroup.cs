using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
///   control multiple texture,when some script use multiple textures,it should use this componenet to
///   handle textures
/// </summary>
[RequireComponent(typeof(UITexture))]
public class UITextureGroup : MonoBehaviour
{
    public UITexture uiTexture;
    
    void Awake()
    {
        if (uiTexture == null)
            uiTexture = GetComponent<UITexture>();
    }
    public Texture[] textures;

}
