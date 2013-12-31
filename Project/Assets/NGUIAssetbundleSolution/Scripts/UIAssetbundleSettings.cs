using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
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
    const string BasePath = "Assets/StreamingAssets";
   static string CreateAssetPath(string path)
    {
        string assetPath = BasePath + "/" + path;
#if UNITY_EDITOR
        DirectoryInfo di = new DirectoryInfo(assetPath);
        if (!di.Exists)
        {
            di.Create();
            AssetDatabase.Refresh();
        }
#endif
        return assetPath;
    }

   public static string buildTargetPath { get { return CreateAssetPath("UI"); } }
   public static string buildTextureTargetPath { get { return CreateAssetPath("UI/Textures"); } }
   public static string buildFontTargetPath { get { return CreateAssetPath("UI/Fonts"); } }
   public static string buildAudioTargetPath { get { return CreateAssetPath("UI/Audios"); } }

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


    public const string ext = ".unity3d";

#if UNITY_EDITOR
    [SerializeField]
    public BuildTarget buildTarget=BuildTarget.Android;
#endif

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
