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

   //public static string buildAtlasTargetPath { get { return CreateAssetPath("UI/Atlases"); } }
   public static string buildMaterialTargetPath { get { return CreateAssetPath("UI/Materials"); } }
   public static string buildTextureTargetPath { get { return CreateAssetPath("UI/Textures"); } }

   public static string buildFontTargetPath { get { return CreateAssetPath("UI/Fonts"); } }
   public static string buildAudioTargetPath { get { return CreateAssetPath("UI/Audios"); } }

#if UNITY_EDITOR
    [SerializeField]
    public BuildTarget buildTarget=BuildTarget.Android;
#endif
    [SerializeField]
    public BuildTarget[] platforms;

    public GUIStyle addButtonStyle;
    public GUIStyle deleteButtonStyle;
}
