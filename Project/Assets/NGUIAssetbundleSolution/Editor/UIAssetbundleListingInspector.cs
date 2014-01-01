using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;
[CustomEditor(typeof(UIAssetbundleListing))]
class UIAssetbundleListingInspector : Editor
{

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        UIAssetbundleListing listing = target as UIAssetbundleListing;
        EditorGUIUtility.LookLikeControls();
        GUILayout.Label("Select Panel", EditorStyles.boldLabel);
        GUILayout.BeginVertical(GUI.skin.box);

        GUILayout.BeginHorizontal(EditorStyles.toolbar);
        GUILayout.Label("Name", GUILayout.MinWidth(100));
        GUILayout.FlexibleSpace();
        //foreach (var plat in Settings.platforms)
        //{
        //    GUILayout.Label(new GUIContent(plat.name, plat.icon32), GUILayout.Height(14), GUILayout.Width(60));
        //}
        GUILayout.Space(16);
        GUILayout.EndHorizontal();
        List<UIAssetbundleListing.IconAtlasAssetbundleEntry> toRemove = new List<UIAssetbundleListing.IconAtlasAssetbundleEntry>();
        foreach (var entry in listing.iconAtlases)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(2);
            GameObject old = null;
            if (entry.atlas!=null)
            {
                old = entry.atlas.gameObject;
            }
            //entry.name = GUILayout.TextField(entry.name, GUILayout.MinWidth(100));
            GUILayout.Label(entry.name, GUILayout.MinWidth(100));
            GUILayout.FlexibleSpace();
            GameObject n = EditorGUILayout.ObjectField(old, typeof(GameObject), false, GUILayout.Width(100)) as GameObject;
            if (n!=null && n != old)
            {
                entry.atlas = n.GetComponent<UIAtlas>();
                if (entry.atlas!=null)
                    entry.name = entry.atlas.name;
                else
                    entry.name = string.Empty;
                EditorUtility.SetDirty(target);
            }

            if (GUILayout.Button("", UIAssetbundleSettings.instance.deleteButtonStyle))
            {
                toRemove.Add(entry);
            }
            GUILayout.Space(2);
            GUILayout.EndHorizontal();
        }
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("", UIAssetbundleSettings.instance.addButtonStyle))
        {
            listing.iconAtlases.Add(new UIAssetbundleListing.IconAtlasAssetbundleEntry());
        }
        GUILayout.Space(2);
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();

        if (toRemove.Count > 0)
        {
            listing.iconAtlases.RemoveAll((x) => toRemove.Contains(x));
            toRemove.Clear();
            EditorUtility.SetDirty(target);
        }
    }
}
