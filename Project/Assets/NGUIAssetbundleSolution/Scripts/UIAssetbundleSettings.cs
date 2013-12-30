using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
///   just test on pc
/// </summary>
public class UIAssetbundleSettings : ScriptableObject
{
    static UIAssetbundleSettings mInstance;
    public static UIAssetbundleSettings instance
    {
        get
        {
            if (!mInstance)
                mInstance = Resources.Load("AssetbundleSettings") as UIAssetbundleSettings;
            return mInstance;
        }
    }
    public static string buildTargetPath = "Assets/StreamingAssets/";
    public static string buildTextureTargetPath { get { return buildTargetPath + "Textures/"; } }

#if UNITY_EDITOR ||  UNITY_STANDALONE_WIN
    public static string pathUrl = "file://" + Application.dataPath + "/StreamingAssets/";
#elif UNITY_ANDROID
    public static string pathUrl = "jar:file://" + Application.dataPath + "!/assets/";
#elif UNITY_IPHONE
    public static string pathUrl = Application.dataPath + "/Raw/";
#endif
    public static string texturePathUrl = pathUrl + "Textures/";

    public const string ext = ".unity3d";


    [System.Serializable]
    public class Dependency
    {
        public string name;
        public List<string> atlasPaths = new List<string>();
        public List<string> fontPaths = new List<string>();
    }
    [SerializeField]
    public List<Dependency> dependencies = new List<Dependency>();

}
