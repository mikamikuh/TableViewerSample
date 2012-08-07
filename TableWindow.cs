using System;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityTableViewer.Viewer;
using UnityTableViewer.Provider;
using PAG.Accessor;

public class TableWindow : EditorWindow {
	private TableViewer viewer;
	public IAccessorManager manager;
	
	public TableWindow() {
		viewer = new TableViewer();
	}
    
    [MenuItem ("Window/Table Window %v")]
    static void Init () {
         TableWindow window = (TableWindow)EditorWindow.GetWindow (typeof (TableWindow));
    }
    
    void OnGUI () {
		if(manager == null) {
			Type asset = Type.GetType("AccessorManager");
			if(asset == null) return;
			manager = (IAccessorManager) Activator.CreateInstance(asset);
		}
		
		GameObject gameObject = Selection.activeGameObject;
		if(!gameObject){
			return;
		}
		
		TableViewerInfo viewerInfo = gameObject.GetComponent<TableViewerInfo>();
		if(!viewerInfo){
			return;
		}
		
		string folderPath = viewerInfo.folderPath;
		
		IList<ICellData> datas = new List<ICellData>();
		
		try {
			foreach(string filePath in Directory.GetFiles(folderPath)) {
				GameObject prefab = (GameObject) AssetDatabase.LoadAssetAtPath(filePath, typeof(GameObject));
				if(prefab != null) {
					IDataAccessor accessor =  manager.GetDataAccessor(prefab);
					ICellData data = new PrefabData(accessor, prefab, folderPath);
					datas.Add (data);
				}
			}
			
			IContentProvider contentProvider = new ContentProvider(datas);
			viewer.ContentProvider = contentProvider;
			
			ICellProvider cellProvider = new TableCellProvider(viewerInfo.labels);
			viewer.CellProvider = cellProvider;
			
			viewer.OnGUI();
		} catch {
		}
	}
}

class TableCellProvider : CellProvider {
	private string[] labels;
	
	public TableCellProvider(string[] labels) {
		this.labels = labels;
	}
	
	public override int Count {
		get { return labels.Length; }
	}

	public override string GetLabel(int col, System.Object obj) {
		return labels[col];
	}
	
	public override string[] GetAllLabel() {
		return labels;
	}
}

class PrefabData : ICellData {
	
	private GameObject prefab;
	private IDataAccessor accessor;
	private string folderPath;
	
	public PrefabData(IDataAccessor accessor, GameObject prefab, string folderPath) {
		this.accessor = accessor;
		this.prefab = prefab;
		this.folderPath = folderPath;
	}
	
	public string Name {
		get { return prefab.name; }
		set { prefab.name = value; }
	}
	
	public System.Object GetData(string name) {
		return accessor.GetValue(name);
	}
	
	public void SetData(System.Object obj, string name) {
		accessor.SetValue(obj, name);
	}
}