using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

using Object = UnityEngine.Object;

class NGUIAssetbundleSolutionMenu
{

    [MenuItem("NGUIAssetbundleSolution/Unload All Resources")]
    //how to unload all the resources in editor???
    public static void UnloadAllResources()
    {
        Caching.CleanCache();
        Resources.UnloadUnusedAssets();
    }

    [MenuItem("NGUIAssetbundleSolution/Create AssetbundleSettings")]
    public static UIAssetbundleSettings CreateAssetbundleSettings()
    {
        var path = System.IO.Path.Combine("Assets/NGUIAssetbundleSolution/Resources", "AssetbundleSettings.asset");
        var so = AssetDatabase.LoadMainAssetAtPath(path) as UIAssetbundleSettings;
        if (so)
            return so;
        so = ScriptableObject.CreateInstance<UIAssetbundleSettings>();
        DirectoryInfo di = new DirectoryInfo(Application.dataPath + "/NGUIAssetbundleSolution/Resources");
        if (!di.Exists)
            di.Create();
        AssetDatabase.CreateAsset(so, path);
        AssetDatabase.SaveAssets();
        return so;
    }

    [MenuItem("NGUIAssetbundleSolution/Build Selected")]
    static void BuildUIAssetbundle()
    {
        Object[] SelectedAsset = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);

        foreach (Object obj in SelectedAsset)
        {
            Build(obj as GameObject);
        }
        EditorUtility.SetDirty(UIAssetbundleSettings.instance);
    }

    static void Build(GameObject go)
    {
        if (go == null)
            return;

        BuildAssetBundleOptions options =
             BuildAssetBundleOptions.CollectDependencies |
             BuildAssetBundleOptions.CompleteAssets |
             BuildAssetBundleOptions.DeterministicAssetBundle;

        UIAssetbundleSettings.Dependency dep = new UIAssetbundleSettings.Dependency();
        dep.name = go.name;
        UIAssetbundleSettings.Dependency exist=UIAssetbundleSettings.instance.dependencies.Find(p => p.name == go.name);
        if (exist!=null)
            UIAssetbundleSettings.instance.dependencies.Remove(exist);
        UIAssetbundleSettings.instance.dependencies.Add(dep);

        UISprite[] ws = go.GetComponentsInChildren<UISprite>(true);
        List<UIAtlas> atlases = new List<UIAtlas>();
        List<Texture> textures = new List<Texture>();
        foreach (var w in ws)
        {
            UIAtlas tempAtlas = w.atlas;
            if (!atlases.Contains(tempAtlas))
            {
                atlases.Add(tempAtlas);
                textures.Add(tempAtlas.texture);
            }
        }

        BuildPipeline.PushAssetDependencies();
        foreach (var tex in textures)
        {
            string path = AssetDatabase.GetAssetPath(tex.GetInstanceID());
            string fileName = tex.name + UIAssetbundleSettings.ext;
            BuildPipeline.BuildAssetBundle(
               AssetDatabase.LoadMainAssetAtPath(path),
               null,
               UIAssetbundleSettings.buildTextureTargetPath + fileName,
               options);

            dep.atlasPaths.Add(fileName);
        }
        BuildPipeline.PushAssetDependencies();
        string goPath = AssetDatabase.GetAssetPath(go.GetInstanceID());
        BuildPipeline.BuildAssetBundle(
          AssetDatabase.LoadMainAssetAtPath(goPath),
          null,
          UIAssetbundleSettings.buildTargetPath + go.name + UIAssetbundleSettings.ext,
          options);

        BuildPipeline.PopAssetDependencies();
        BuildPipeline.PopAssetDependencies();
    }

}
