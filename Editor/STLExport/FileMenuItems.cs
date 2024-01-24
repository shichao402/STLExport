#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Parabox.Stl.Editor
{
	sealed class FileMenuItems : UnityEditor.Editor
	{
		[MenuItem("GameObject/ExportSTL(Ascii)", true)]
		[MenuItem("GameObject/ExportSTL(Binary)", true)]
		static bool VerifyExport()
		{
			return Selection.transforms.SelectMany(x => x.GetComponentsInChildren<SkinnedMeshRenderer>()).FirstOrDefault(y => y.sharedMesh != null) != null;
		}

		[MenuItem("GameObject/ExportSTL(Ascii)", false, 30)]
		static void MenuExportAscii(MenuCommand menuCommand)
		{
			if (Selection.objects.Length > 1)
			{
				if (menuCommand.context != Selection.objects[0])
				{
					return;
				}
			}
			ExportWithFileDialog(Selection.gameObjects, FileType.Ascii);
		}

		[MenuItem("GameObject/ExportSTL(Binary)", false, 30)]
		static void MenuExportBinary(MenuCommand menuCommand)
		{
			if (Selection.objects.Length > 1)
			{
				if (menuCommand.context != Selection.objects[0])
				{
					return;
				}
			}
			ExportWithFileDialog(Selection.gameObjects, FileType.Binary);
		}

		private static void ExportWithFileDialog(GameObject[] gameObjects, FileType type)
		{
			if(gameObjects == null || gameObjects.Length < 1)
			{
				Debug.LogWarning("Attempting to export STL file with no GameObject selected. For reasons that should be obvious this is not allowed.");
				return;
			}

			string path = EditorUtility.SaveFilePanel("Save Mesh to STL", "", gameObjects.FirstOrDefault().name, "stl");

			if( Exporter.Export(path, gameObjects, type) )
			{
				string full = path.Replace("\\", "/");

				// if the file was saved in project, ping it
				if(full.Contains(Application.dataPath))
				{
					string relative = full.Replace(Application.dataPath, "Assets");

#if UNITY_4_7
					Object o = (Object) AssetDatabase.LoadAssetAtPath(relative, typeof(Object));
#else
					Object o = AssetDatabase.LoadAssetAtPath<Object>(relative);
#endif

					if(o != null)
						EditorGUIUtility.PingObject(o);

					AssetDatabase.Refresh();
				}
			}
		}
	}
}

#endif