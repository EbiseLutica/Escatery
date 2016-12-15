// UIController
// 2016.12.08
// (C) Chikuwabu
// Coder: Citrine

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Extension : MonoBehaviour {
	[MenuItem("Assets/Create/MapData")]
	static void CreateMapData()
	{
		GameObject go = new GameObject("MapData", typeof(MapData));
		var id = go.GetInstanceID();
		var icon = AssetPreview.GetMiniTypeThumbnail(typeof(GameObject));

		ProjectWindowUtil.StartNameEditingIfProjectWindowExists(id, ScriptableObject.CreateInstance<EndNameEditAction>(), "MapData.prefab", icon, "");
	}

	class EndNameEditAction : UnityEditor.ProjectWindowCallback.EndNameEditAction
	{
		public override void Action(int instanceId, string pathName, string resourceFile)
		{
			var go = (GameObject)EditorUtility.InstanceIDToObject(instanceId);
			
			PrefabUtility.CreatePrefab(pathName, go);
			AssetDatabase.ImportAsset(pathName);
			ProjectWindowUtil.ShowCreatedAsset(go);
		}
	}

}
