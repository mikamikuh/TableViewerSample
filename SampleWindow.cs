using System;
using System.Threading;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using TemplateEngine;
using PAG.Accessor;
using PAG.Generator;
using UnityTableViewer.Viewer;
using UnityTableViewer.Provider;

public class SampleWindow : EditorWindow {
	private Boolean flag = false;
    
    [MenuItem ("Window/Sample Window %g")]
    static void Init () {
         SampleWindow window = (SampleWindow)EditorWindow.GetWindow (typeof (SampleWindow));
    }
    
    void OnGUI () {
		if(GUILayout.Button("Generate AccessorManager")) {
			IList<string> prefabNames = new List<string>();
			DirectoryInfo info = new DirectoryInfo("Assets/SourcePrefab/");
			foreach(FileInfo f in info.GetFiles()) {
				prefabNames.Add (f.Name.Split('.')[0]);
			}
			
			//UnityEngine.Object[] objects = AssetDatabase.LoadAllAssetsAtPath("Assets/SourcePrefab/");
			//UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath("Assets/SourcePrefab/Cube", typeof(GameObject));
			//Debug.Log (obj.name);
			//IList<UnityEngine.Object> prefabs = new List<UnityEngine.Object>(objects);
			ICodeGenerator generator = new AccessorManagerGenerator("Test.Generate", "Sample", prefabNames);
			generator.execute();
		}
		
		if(GUILayout.Button ("Generate DataAccessor")) {
			string prefabFolder = "Assets/SourcePrefab/";
			
			IDictionary<string, string> variables = new Dictionary<string, string>();
			variables["sample1"] = "int";
			variables["sample2"] = "string";
			variables["sample3"] = "float";
			
			DirectoryInfo info = new DirectoryInfo("Assets/SourcePrefab/");
			foreach(FileInfo f in info.GetFiles()) {
				string name = f.Name.Split('.')[0];
				DataAccessorGenerator generator = new DataAccessorGenerator("Test.Generate", name, "TestClassScript", prefabFolder + name, variables);
				generator.execute();
			}
		}
		
		if(GUILayout.Button ("Generate DataScript")) {
			IDictionary<string, string> variables = new Dictionary<string, string>();
			variables["sample1"] = "int";
			variables["sample2"] = "string";
			variables["sample3"] = "float";
			
			DataScriptGenerator generator = new DataScriptGenerator("TestClass", variables);
			generator.execute();
		}
		
		if(GUILayout.Button("Click")){
			AssetDatabase.ImportAsset("Assets/GenerateClassName.cs");
			AssetDatabase.Refresh();
			flag = true;
		}
		
		string aaa = GUILayout.TextField("TEXT");
		Debug.Log (aaa);
    }
	
	void Update() {
		if(!EditorApplication.isCompiling && flag) {
			GameObject gameObject = new GameObject();
			Component comp = gameObject.AddComponent("GenerateClassName");
//			gameObject.GetComponent<GenerateClassName>
//			gameObject.test = 0;
			UnityEngine.Object prefab = EditorUtility.CreateEmptyPrefab("Assets/MyObject.prefab");
			EditorUtility.ReplacePrefab(gameObject, prefab, ReplacePrefabOptions.ConnectToPrefab);
			flag = false;
		}
	}
}