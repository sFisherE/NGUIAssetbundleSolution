using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;


/// <summary>
///   the unload stategy: i am thinking
///   how to download ui form internet
/// </summary>
public class UIAssetbundleManager:MonoBehaviour
{
    static UIAssetbundleManager mInstance;
    public static UIAssetbundleManager instance
    {
        get
        {
            if (!mInstance)
            {
                mInstance = FindObjectOfType(typeof(UIAssetbundleManager)) as UIAssetbundleManager;
                if (!mInstance)
                {
                    var obj = new GameObject("ArtFontManager");
                    mInstance = obj.AddComponent<UIAssetbundleManager>();
                    DontDestroyOnLoad(obj);
                }
            }
            return mInstance;
        }
    }
    public List<Texture> loadedTextures = new List<Texture>();
    public List<AudioClip> loadedAudioClips = new List<AudioClip>();
    public List<Font> loadedFonts = new List<Font>();
    public void LoadUIAssetbundle(string name)
    {
        StartCoroutine(CoLoadUIAssetbundle(name));
    }
    IEnumerator CoLoadUIAssetbundle(string name)
    {
        UIAssetbundleSettings.Dependency dep = UIAssetbundleSettings.instance.dependencies.Find(p => p.name == name);
        //first load texture
        if (dep.atlasPaths != null && dep.atlasPaths.Count > 0)
        {
            foreach (var path in dep.atlasPaths)
            {
                string texName = path.Substring(0, path.Length - UIAssetbundleSettings.ext.Length);
                //the texture name should be unique
                if (loadedTextures.Find(p => p.name == texName) == null)
                {
                    string fullPath = UIAssetbundleSettings.texturePathUrl + "/" + path;
                    WWW ret = new WWW(fullPath);
                    yield return ret;
                    Texture tex = ret.assetBundle.Load(texName, typeof(Texture)) as Texture;
                   loadedTextures.Add(tex);
                }
            }
        }
        //load font
        if (dep.fontPaths != null && dep.fontPaths.Count > 0)
        {
            foreach (var path in dep.fontPaths)
            {
                string fontName = path.Substring(0, path.Length - UIAssetbundleSettings.ext.Length);
                if (loadedFonts.Find(p => p.name == fontName) == null)
                {
                    string fullPath = UIAssetbundleSettings.fontPathUrl+"/" + path;
                    WWW ret = new WWW(fullPath);
                    yield return ret;
                    Font font = ret.assetBundle.Load(fontName, typeof(Font)) as Font;
                    loadedFonts.Add(font);
                }
            }
        }
        //load audioClip
        if (dep.audioPaths != null && dep.audioPaths.Count > 0)
        {
            foreach (var path in dep.audioPaths)
            {
                string audioName = path.Substring(0, path.Length - UIAssetbundleSettings.ext.Length);
                if (loadedAudioClips.Find(p => p.name == audioName) == null)
                {
                    string fullPath = UIAssetbundleSettings.audioPathUrl + "/" + path;
                    WWW ret = new WWW(fullPath);
                    yield return ret;
                    AudioClip audioClip = ret.assetBundle.Load(audioName, typeof(AudioClip)) as AudioClip;
                    loadedAudioClips.Add(audioClip);
                }
            }
        }
        //then load the prefab
        WWW bundle = new WWW(UIAssetbundleSettings.pathUrl + "/" + name + UIAssetbundleSettings.ext);
        yield return bundle;
        GameObject go = Instantiate(bundle.assetBundle.mainAsset) as GameObject;
        go.transform.parent = root;
        go.transform.localScale = Vector3.one;
        go.transform.localPosition = Vector3.zero;

        //set UIAnchor's camera

        bundle.assetBundle.Unload(false);
    }



    //just for test
    public string name;
    public Transform root;
#if UNITY_EDITOR
    void OnGUI()
    {
        if (GUILayout.Button("Test",GUILayout.Width(100),GUILayout.Height(50)))
        {
            if (!string.IsNullOrEmpty(name))
                LoadUIAssetbundle(name);
        }
    }
#endif
}