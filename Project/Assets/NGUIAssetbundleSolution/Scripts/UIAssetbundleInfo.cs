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
    //public const string ext = ".unity3d";
    public const string ext = ".ab";



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
    public static string materialPathUrl = pathUrl + "/Materials";



    //public class AtlasDependency
    //{
    //    public string atlasName;
    //    public string material;
    //    public string texture;
    //}
    //public class BMFontDependency
    //{
    //    public string name;
    //    public AtlasDependency atlasDep;
    //}

    [System.Serializable]
    public class Dependency
    {
        public string name;
        public List<string> atlasPaths = new List<string>();
        public List<string> materialPaths = new List<string>();
        public List<string> dynamicFontPaths = new List<string>();
        public List<string> audioPaths = new List<string>();
    }
    [SerializeField]
    public List<Dependency> dependencies = new List<Dependency>();

    //icon is always runtime changed,so it should be manage standalone
    [SerializeField]
    public List<string> iconAtlases = new List<string>();

}
