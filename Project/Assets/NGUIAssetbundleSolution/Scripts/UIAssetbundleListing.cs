using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Object = UnityEngine.Object;
public class UIAssetbundleListing : ScriptableObject
{
    [System.Serializable]
    public class IconAtlasAssetbundleEntry
    {
        public string name = "";
        public UIAtlas atlas;
    }

    public List<IconAtlasAssetbundleEntry> iconAtlases = new List<IconAtlasAssetbundleEntry>();


}
