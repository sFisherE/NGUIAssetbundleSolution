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
    [MenuItem("NGUIAssetbundleSolution/Create AssetbundleInfo")]
    public static UIAssetbundleInfo CreateAssetbundleInfo()
    {
        var path = System.IO.Path.Combine("Assets/NGUIAssetbundleSolution/Resources", "AssetbundleInfo.asset");
        var so = AssetDatabase.LoadMainAssetAtPath(path) as UIAssetbundleInfo;
        if (so)
            return so;
        so = ScriptableObject.CreateInstance<UIAssetbundleInfo>();
        DirectoryInfo di = new DirectoryInfo(Application.dataPath + "/NGUIAssetbundleSolution/Resources");
        if (!di.Exists)
            di.Create();
        AssetDatabase.CreateAsset(so, path);
        AssetDatabase.SaveAssets();
        return so;
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
            GameObject go = obj as GameObject;
            if (go != null)
            {
                UIPanelBase com = go.GetComponent<UIPanelBase>();
                if (com == null)
                {
                    Debug.LogError("the gameobject must have UIPanelBase component");
                }
                else
                    Build(obj as GameObject);
            }
        }
        EditorUtility.SetDirty(UIAssetbundleInfo.instance);
        AssetDatabase.Refresh();
    }

    static void Build(GameObject go)
    {
        if (go == null)
            return;

        BuildAssetBundleOptions options =
             BuildAssetBundleOptions.CollectDependencies |
             BuildAssetBundleOptions.CompleteAssets |
             BuildAssetBundleOptions.DeterministicAssetBundle;

        UIAssetbundleInfo.Dependency dep = new UIAssetbundleInfo.Dependency();
        dep.name = go.name;
        UIAssetbundleInfo.Dependency exist = UIAssetbundleInfo.instance.dependencies.Find(p => p.name == go.name);
        if (exist != null)
            UIAssetbundleInfo.instance.dependencies.Remove(exist);
        UIAssetbundleInfo.instance.dependencies.Add(dep);

        UISprite[] ws = go.GetComponentsInChildren<UISprite>(true);
        List<Texture> textures = new List<Texture>();
        List<Material> materials = new List<Material>();
        foreach (var w in ws)
        {
            Texture tex = w.atlas.texture;
            if (tex != null && !textures.Contains(tex))
            {
                textures.Add(tex);
            }
            Material mat = w.atlas.spriteMaterial;
            if (mat!=null && !materials.Contains(mat))
                materials.Add(mat);
        }
        //UITexture will use another strategy



        UILabel[] labels = go.GetComponentsInChildren<UILabel>(true);
        List<UIFont> uiFonts = new List<UIFont>();
        List<Font> dynamicfonts = new List<Font>();
        foreach (var label in labels)
        {
            UIFont uifont = label.font;
            if (uifont.isDynamic)
            {
                Font f = uifont.dynamicFont;
                if (f != null && !dynamicfonts.Contains(f))
                    dynamicfonts.Add(f);
            }
            else
            {
                Texture tex = uifont.texture;
                if (tex != null && !textures.Contains(tex))
                    textures.Add(tex);
                Material mat = uifont.material;
                if (mat != null && !materials.Contains(mat))
                    materials.Add(mat);
            }
        }

        UIPlaySound[] sounds = go.GetComponentsInChildren<UIPlaySound>(true);
        List<AudioClip> clips = new List<AudioClip>();
        foreach (var sound in sounds)
        {
            AudioClip audio = sound.audioClip;
            if (audio != null && !clips.Contains(sound.audioClip))
                clips.Add(sound.audioClip);
        }
        //////////////////////////////////////////////////////////////////////////
        BuildPipeline.PushAssetDependencies();
        foreach (var tex in textures)
        {
            string path = AssetDatabase.GetAssetPath(tex.GetInstanceID());
            //Debug.Log(UIAssetbundleSettings.buildTextureTargetPath);
            string fileName = "tex_" + tex.name + UIAssetbundleInfo.ext;
            BuildPipeline.BuildAssetBundle(
               AssetDatabase.LoadMainAssetAtPath(path),
               null,
               UIAssetbundleSettings.buildTextureTargetPath + "/" + fileName,
               options,
               UIAssetbundleSettings.instance.buildTarget);

            dep.atlasPaths.Add(fileName);
        }
        foreach (var font in dynamicfonts)
        {
            string path = AssetDatabase.GetAssetPath(font.GetInstanceID());
            string fileName = "font_" + font.name + UIAssetbundleInfo.ext;
            BuildPipeline.BuildAssetBundle(
               AssetDatabase.LoadMainAssetAtPath(path),
               null,
               UIAssetbundleSettings.buildFontTargetPath + "/" + fileName,
               options,
               UIAssetbundleSettings.instance.buildTarget);
            dep.dynamicFontPaths.Add(fileName);
        }
        foreach (var clip in clips)
        {
            string path = AssetDatabase.GetAssetPath(clip.GetInstanceID());
            string fileName ="audio_"+ clip.name + UIAssetbundleInfo.ext;
            BuildPipeline.BuildAssetBundle(
               AssetDatabase.LoadMainAssetAtPath(path),
               null,
               UIAssetbundleSettings.buildAudioTargetPath + "/" + fileName,
               options,
               UIAssetbundleSettings.instance.buildTarget);
            dep.audioPaths.Add(fileName);
        }
        foreach (var mat in materials)
        {
            string path = AssetDatabase.GetAssetPath(mat.GetInstanceID());
            string fileName = "mat_" + mat.name + UIAssetbundleInfo.ext;
            BuildPipeline.BuildAssetBundle(
               AssetDatabase.LoadMainAssetAtPath(path),
               null,
               UIAssetbundleSettings.buildMaterialTargetPath + "/" + fileName,
               options,
               UIAssetbundleSettings.instance.buildTarget);
            dep.materialPaths.Add(fileName);
        }
        //////////////////////////////////////////////////////////////////////////



        BuildPipeline.PushAssetDependencies();
        string goPath = AssetDatabase.GetAssetPath(go.GetInstanceID());
        BuildPipeline.BuildAssetBundle(
          AssetDatabase.LoadMainAssetAtPath(goPath),
          null,
          UIAssetbundleSettings.buildTargetPath + "/" + go.name + UIAssetbundleInfo.ext,
          options,
          UIAssetbundleSettings.instance.buildTarget);

        BuildPipeline.PopAssetDependencies();
        BuildPipeline.PopAssetDependencies();
    }

    [MenuItem("NGUIAssetbundleSolution/Create AssetBundleListing")]
    public static void CreateAssetBundleListing()
    {
        var so = ScriptableObject.CreateInstance<UIAssetbundleListing>();
        var path = System.IO.Path.Combine(FolderPathFromSelection(), "Listing.asset");
        path = AssetDatabase.GenerateUniqueAssetPath(path);//interesting
        AssetDatabase.CreateAsset(so, path);
        Selection.activeObject = so;
        AssetDatabase.SaveAssets();
    }
    private static string FolderPathFromSelection()
    {
        var selection = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);
        string path = "Assets";
        if (selection.Length > 0)
        {
            path = AssetDatabase.GetAssetPath(selection[0]);
            var dummypath = System.IO.Path.Combine(path, "fake.asset");
            var assetpath = AssetDatabase.GenerateUniqueAssetPath(dummypath);
            if (assetpath != "")
            {
                return path;
            }
            else
            {
                return System.IO.Path.GetDirectoryName(path);
            }
        }
        return path;
    }
}
