using System;
using System.Collections.Generic;
using UnityEngine;


public class UIAssetbundleInfo : ScriptableObject
{
    static UIAssetbundleInfo mInstance;
    public static UIAssetbundleInfo instance
    {
        get
        {
            if (!mInstance)
                mInstance = Resources.Load("AssetbundleInfo") as UIAssetbundleInfo;
            return mInstance;
        }
    }
    public const string ext = ".unity3d";

#if UNITY_EDITOR ||  UNITY_STANDALONE_WIN
    public static string pathUrl = "file://" + Application.dataPath + "/StreamingAssets/UI";
#elif UNITY_ANDROID
    public static string pathUrl = "jar:file://" + Application.dataPath + "!/assets/UI";
#elif UNITY_IPHONE
    public static string pathUrl = Application.dataPath + "/Raw/UI";
#endif
    public static string texturePathUrl = pathUrl + "/Textures";
    public static string fontPathUrl = pathUrl + "/Fonts";
    public static string audioPathUrl = pathUrl + "/Audios";


    [System.Serializable]
    public class Dependency
    {
        public string name;
        public List<string> atlasPaths = new List<string>();
        public List<string> fontPaths = new List<string>();
        public List<string> audioPaths = new List<string>();
    }
    [SerializeField]
    public List<Dependency> dependencies = new List<Dependency>();



}
