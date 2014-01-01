using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;


/// <summary>
///   the unload stategy: i am thinking
///   how to download ui form internet
/// </summary>
public class UIAssetbundleManager : MonoBehaviour
{
    void Awake()
    {
        gameObject.name = "AssetbundleManager";
        UIAssetbundleManager.instance = this;
    }
    public static UIAssetbundleManager instance;
    private void OnApplicationQuit()
    {
        instance = null;
        Destroy(gameObject);
    }

    public static void Init()
    {
        if (!instance)
        {
            // check if there is a GoKitLite instance already available in the scene graph before creating one
            instance = FindObjectOfType(typeof(UIAssetbundleManager)) as UIAssetbundleManager;
            if (!instance)
            {
                var obj = new GameObject("AssetbundleManager");
                instance = obj.AddComponent<UIAssetbundleManager>();
                DontDestroyOnLoad(obj);
            }
        }
    }

    //public static UIAssetbundleManager instance
    //{
    //    get
    //    {
    //        return mInstance;
    //    }
    //}

    public class AssetbundleEntry
    {
        public string name;
        public int refCount;
        public AssetBundle assetBundle;
    }

    public List<AssetbundleEntry> loadedTextures = new List<AssetbundleEntry>();
    public List<AssetbundleEntry> loadedAudioClips = new List<AssetbundleEntry>();
    public List<AssetbundleEntry> loadedFonts = new List<AssetbundleEntry>();
    public List<AssetbundleEntry> loadedMaterials = new List<AssetbundleEntry>();

    public void UnloadResource(string name)
    {
        UIAssetbundleInfo.Dependency dep = UIAssetbundleInfo.instance.dependencies.Find(p => p.name == name);
        if (dep.atlasPaths != null && dep.atlasPaths.Count > 0)
        {
            foreach (var path in dep.atlasPaths)
            {
                string texName = path.Substring(0, path.Length - UIAssetbundleInfo.ext.Length);
                AssetbundleEntry abe = loadedTextures.Find(p => p.name == texName);
                if (abe!=null)
                {
                    abe.refCount--;
                    if (abe.refCount <= 0)
                    {
                        abe.assetBundle.Unload(true);
                        loadedTextures.Remove(abe);
                        Logger.Log("unload " + abe.name);
                    }
                }

            }
        }
        if (dep.dynamicFontPaths != null && dep.dynamicFontPaths.Count > 0)
        {
            foreach (var path in dep.dynamicFontPaths)
            {
                string fontName = path.Substring(0, path.Length - UIAssetbundleInfo.ext.Length);
                AssetbundleEntry abe = loadedFonts.Find(p => p.name == fontName);
                if (abe!=null)
                {
                    abe.refCount--;
                    if (abe.refCount <= 0)
                    {
                        abe.assetBundle.Unload(true);
                        loadedFonts.Remove(abe);
                        Logger.Log("unload " + abe.name);
                    }
                }
            }
        }
        if (dep.audioPaths != null && dep.audioPaths.Count > 0)
        {
            foreach (var path in dep.audioPaths)
            {
                string audioName = path.Substring(0, path.Length - UIAssetbundleInfo.ext.Length);
                AssetbundleEntry abe = loadedAudioClips.Find(p => p.name == audioName);
                if (abe!=null)
                {
                    abe.refCount--;
                    if (abe.refCount <= 0)
                    {
                        abe.assetBundle.Unload(true);
                        loadedAudioClips.Remove(abe);
                        Logger.Log("unload " + abe.name);
                    }
                }
            }
        }
        if (dep.materialPaths != null && dep.materialPaths.Count > 0)
        {
            foreach (var path in dep.materialPaths)
            {
                string audioName = path.Substring(0, path.Length - UIAssetbundleInfo.ext.Length);
                AssetbundleEntry abe = loadedMaterials.Find(p => p.name == audioName);
                if (abe != null)
                {
                    abe.refCount--;
                    if (abe.refCount <= 0)
                    {
                        abe.assetBundle.Unload(true);
                        loadedMaterials.Remove(abe);
                        Logger.Log("unload " + abe.name);
                    }
                }
            }
        }
    }

    public void LoadUIAssetbundle(string name)
    {
        StartCoroutine(CoLoadUIAssetbundle(name));
    }
    IEnumerator CoLoadUIAssetbundle(string name)
    {
        UIAssetbundleInfo.Dependency dep = UIAssetbundleInfo.instance.dependencies.Find(p => p.name == name);
        //first load texture
        if (dep.atlasPaths != null && dep.atlasPaths.Count > 0)
        {
            foreach (var path in dep.atlasPaths)
            {
                string texName = path.Substring(0, path.Length - UIAssetbundleInfo.ext.Length);
                string fullPath = UIAssetbundleInfo.texturePathUrl + "/" + path;
                AssetbundleEntry abe = loadedTextures.Find(p => p.name == texName);
                if (abe != null)
                {
                    abe.refCount++;
                    yield return null;
                }
                else
                {
                    using (WWW www = new WWW(fullPath))
                    {
                        yield return www;
                        if (www.error != null)
                            throw new Exception("WWW download:" + www.error);
                        abe = new AssetbundleEntry();
                        abe.name = texName;
                        abe.assetBundle = www.assetBundle;
                        abe.refCount++;
                        Debug.Log("add to textures");
                        loadedTextures.Add(abe);
                        www.assetBundle.LoadAll();
                    }
                }
            }
        }
        //load font
        if (dep.dynamicFontPaths != null && dep.dynamicFontPaths.Count > 0)
        {
            foreach (var path in dep.dynamicFontPaths)
            {
                string fontName = path.Substring(0, path.Length - UIAssetbundleInfo.ext.Length);
                string fullPath = UIAssetbundleInfo.fontPathUrl + "/" + path;
                AssetbundleEntry abe = loadedFonts.Find(p => p.name == fontName);
                if (abe != null)
                {
                    abe.refCount++;
                    yield return null;
                }
                else
                {
                    using (WWW www = new WWW(fullPath))
                    {
                        yield return www;
                        if (www.error != null)
                            throw new Exception("WWW download:" + www.error);
                        abe = new AssetbundleEntry();
                        abe.name = fontName;
                        abe.assetBundle = www.assetBundle;
                        abe.refCount++;
                        loadedFonts.Add(abe);
                        www.assetBundle.LoadAll();
                    }
                }
            }
        }
        //load audioClip
        if (dep.audioPaths != null && dep.audioPaths.Count > 0)
        {
            foreach (var path in dep.audioPaths)
            {
                string audioName = path.Substring(0, path.Length - UIAssetbundleInfo.ext.Length);
                string fullPath = UIAssetbundleInfo.audioPathUrl + "/" + path;
                AssetbundleEntry abe = loadedAudioClips.Find(p => p.name == audioName);
                if (abe != null)
                {
                    abe.refCount++;
                    yield return null;
                }
                else
                {
                    using (WWW www = new WWW(fullPath))
                    {
                        yield return www;
                        if (www.error != null)
                            throw new Exception("WWW download:" + www.error);
                        abe = new AssetbundleEntry();
                        abe.name = audioName;
                        abe.assetBundle = www.assetBundle;
                        abe.refCount++;
                        loadedAudioClips.Add(abe);
                        www.assetBundle.LoadAll();
                    }
                }
            }
        }
        if (dep.materialPaths != null && dep.materialPaths.Count > 0)
        {
            foreach (var path in dep.materialPaths)
            {
                string materialName = path.Substring(0, path.Length - UIAssetbundleInfo.ext.Length);
                string fullPath = UIAssetbundleInfo.materialPathUrl + "/" + path;
                AssetbundleEntry abe = loadedMaterials.Find(p => p.name == materialName);
                if (abe != null)
                {
                    abe.refCount++;
                    yield return null;
                }
                else
                {
                    using (WWW www = new WWW(fullPath))
                    {
                        yield return www;
                        if (www.error != null)
                            throw new Exception("WWW download:" + www.error);
                        abe = new AssetbundleEntry();
                        abe.name = materialName;
                        abe.assetBundle = www.assetBundle;
                        abe.refCount++;
                        loadedMaterials.Add(abe);
                        www.assetBundle.LoadAll();
                    }
                }
            }
        }
        //then load the prefab
        WWW bundle = new WWW(UIAssetbundleInfo.pathUrl + "/" + name + UIAssetbundleInfo.ext);
        yield return bundle;
        GameObject go = Instantiate(bundle.assetBundle.mainAsset) as GameObject;
        go.name = name;
        go.transform.parent = root;
        go.transform.localScale = Vector3.one;
        //if i want create a lot,i should't unload
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
        if (GUILayout.Button("Test", GUILayout.Width(100), GUILayout.Height(50)))
        {
            if (!string.IsNullOrEmpty(name))
                LoadUIAssetbundle(name);
        }
    }
#endif
}