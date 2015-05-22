using UnityEditor;
using UnityEngine;
using System.Collections;

public class LoadPrefabsToScene : EditorWindow
{
	bool showBtn = true;
	
	[MenuItem ("Prefab/LoadPrefabToScene")]
	static public void Init() {
		EditorWindow window = EditorWindow.GetWindow<LoadPrefabsToScene>("LoadPrefabsToScene");
		window.Show();
	}
	public void OnGUI() {

		string assetPath = Application.dataPath;



		showBtn = EditorGUILayout.Toggle("Show Button", showBtn);
		if(showBtn)
			if(GUILayout.Button("Close"))
				this.Close();
	}
}