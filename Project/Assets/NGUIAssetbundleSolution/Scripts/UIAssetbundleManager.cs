using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;


/// <summary>
///   the unload stategy: i am thinking
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
                    string fullPath = UIAssetbundleSettings.texturePathUrl + path;
                    WWW ret = new WWW(fullPath);
                    yield return ret;
                    Texture tex = ret.assetBundle.Load(texName, typeof(Texture)) as Texture;
                   loadedTextures.Add(tex);
                }
            }
        }
        //then load the prefab
        WWW bundle = new WWW(UIAssetbundleSettings.pathUrl + name + UIAssetbundleSettings.ext);
        yield return bundle;
        GameObject go = Instantiate(bundle.assetBundle.mainAsset) as GameObject;
        go.transform.parent = root;
        go.transform.localScale = Vector3.one;
        //if i want create a lot,i should't unload
        bundle.assetBundle.Unload(false);
    }



    //just for test
#if UNITY_EDITOR
    public string name;
    public Transform root;
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