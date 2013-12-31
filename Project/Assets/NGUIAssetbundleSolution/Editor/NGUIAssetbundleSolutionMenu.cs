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

        UIAssetbundleSettings.Dependency dep = new UIAssetbundleSettings.Dependency();
        dep.name = go.name;
        UIAssetbundleSettings.Dependency exist=UIAssetbundleSettings.instance.dependencies.Find(p => p.name == go.name);
        if (exist!=null)
            UIAssetbundleSettings.instance.dependencies.Remove(exist);
        UIAssetbundleSettings.instance.dependencies.Add(dep);

        UISprite[] ws = go.GetComponentsInChildren<UISprite>(true);
        List<Texture> textures = new List<Texture>();
        foreach (var w in ws)
        {
            Texture tex = w.atlas.texture;
            if (tex != null && !textures.Contains(tex))
                textures.Add(tex);
        }
        //UITexture will use another strategy



        UILabel[] labels = go.GetComponentsInChildren<UILabel>(true);
        List<UIFont> uiFonts = new List<UIFont>();
        List<Font> dynamicfonts = new List<Font>();
        foreach ( var label in labels)
        {
            UIFont uifont = label.font;
            if (uifont.isDynamic)
            {
                Font f = uifont.dynamicFont;
                if (f!=null && !dynamicfonts.Contains(f))
                    dynamicfonts.Add(f);
            }
            else
            {
                Texture tex = uifont.texture;
                if (tex != null && !textures.Contains(tex))
                    textures.Add(tex);
            }
        }

        UIButtonSound[] sounds = go.GetComponentsInChildren<UIButtonSound>(true);
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
            string fileName = tex.name + UIAssetbundleSettings.ext;
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
            string fileName = font.name + UIAssetbundleSettings.ext;
            BuildPipeline.BuildAssetBundle(
               AssetDatabase.LoadMainAssetAtPath(path),
               null,
               UIAssetbundleSettings.buildFontTargetPath +"/"+ fileName,
               options,
               UIAssetbundleSettings.instance.buildTarget);
            dep.fontPaths.Add(fileName);
        }
        foreach ( var clip in clips)
        {
            string path = AssetDatabase.GetAssetPath(clip.GetInstanceID());
            string fileName = clip.name + UIAssetbundleSettings.ext;
            BuildPipeline.BuildAssetBundle(
               AssetDatabase.LoadMainAssetAtPath(path),
               null,
               UIAssetbundleSettings.buildAudioTargetPath+"/" + fileName,
               options,
               UIAssetbundleSettings.instance.buildTarget);
            dep.audioPaths.Add(fileName);
        }
        //////////////////////////////////////////////////////////////////////////
        BuildPipeline.PushAssetDependencies();
        string goPath = AssetDatabase.GetAssetPath(go.GetInstanceID());
        BuildPipeline.BuildAssetBundle(
          AssetDatabase.LoadMainAssetAtPath(goPath),
          null,
          UIAssetbundleSettings.buildTargetPath + "/" + go.name + UIAssetbundleSettings.ext,
          options,
          UIAssetbundleSettings.instance.buildTarget);

        BuildPipeline.PopAssetDependencies();
        BuildPipeline.PopAssetDependencies();
    }

}
