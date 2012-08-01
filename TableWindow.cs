using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityTableViewer.Viewer;
using UnityTableViewer.Provider;
using PAG.Accessor;
using Test.Generate;

public class TableWindow : EditorWindow {
	private TableViewer viewer;
	
	public TableWindow() {
		IAccessorManager manager = new SampleAccessorManager();
		IList<ICellData> datas = new List<ICellData>();
		
		foreach(KeyValuePair<string, IDataAccessor> pair in manager.GetAll()) {
			ICellData data = new PrefabData(pair.Value, pair.Key);
			datas.Add (data);
		}
		
		IContentProvider contents = new ContentProvider(datas);
		TableViewer viewer = new TableViewer(contents, new SampleCellProvider());
	}
    
    [MenuItem ("Window/Table Window %v")]
    static void Init () {
         TableWindow window = (TableWindow)EditorWindow.GetWindow (typeof (TableWindow));
    }
    
    void OnGUI () {
		viewer.OnGUI();
	}
}

class SampleCellProvider : CellProvider {
}

class PrefabData : ICellData {
	
	private string name;
	private IDataAccessor accessor;
	
	public PrefabData(IDataAccessor accessor, string name) {
		this.accessor = accessor;
		this.name = name;
	}
	
	public string Name {
		get { return name; }
	}
	
	public System.Object Data {
		set { accessor.SetValue(value, name); }
		get { return accessor.GetValue(name); }
	}
}