using Nawlian.Lib.Extensions;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Nawlian.Lib.Systems.Pooling.Editor
{
	[CustomEditor(typeof(ObjectPooler))]
	public class ObjectPoolerEditor : OdinEditor
	{
		private List<string> _enumNames;
		private string _path;

		private GameObject _prefab;

		#region Generation

		private void RegenerateEnum(List<string> enumNames)
		{
			var targetUnit = new CodeCompileUnit();
			CodeNamespace codeNamespace = new CodeNamespace("");
			var enumLayer = new CodeTypeDeclaration
			{
				Name = "PoolIdEnum",
				IsEnum = true,
				TypeAttributes = TypeAttributes.Public
			};

			foreach (string name in enumNames)
			{
				// Creates the enum member
				CodeMemberField f = new CodeMemberField(enumLayer.Name, name);
				enumLayer.Members.Add(f);
			}

			codeNamespace.Types.Add(enumLayer);
			targetUnit.Namespaces.Add(codeNamespace);

			CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
			CodeGeneratorOptions options = new CodeGeneratorOptions { BracingStyle = "C" };
			string path = Path.Combine(_path);

			using (StreamWriter sourceWriter = new StreamWriter(path))
				provider.GenerateCodeFromCompileUnit(targetUnit, sourceWriter, options);
			AssetDatabase.Refresh();
			Debug.Log("Nawlian.Lib.Systems.Pooling - Regenerating PoolIdEnum.cs...");
		}

		#endregion

		protected override void OnEnable()
		{
			_path = $"{Application.dataPath}/Plugins/Nawlian/Lib/Systems/Pooling/PoolIdEnum.cs";
			_enumNames = Enum.GetNames(typeof(PoolIdEnum)).ToList();
		}

		public override void OnInspectorGUI()
        {
			base.OnInspectorGUI();

			Draw();
		}

		[UnityEditor.Callbacks.DidReloadScripts]
		private static void OnScriptsReloaded()
		{
			string asset = EditorPrefs.GetString("AssetName");
			bool generate = EditorPrefs.GetBool("WaitingToCreate");

			EditorPrefs.SetString("AssetName", string.Empty);
			EditorPrefs.SetBool("WaitingToCreate", false);

			if (generate)
			{
				PoolIdEnum id = (PoolIdEnum)Enum.Parse(typeof(PoolIdEnum), asset.ToSnakeCase().ToUpper());
				APoolableObject obj = Resources.LoadAll<APoolableObject>("").FirstOrDefault(x => x.name == asset);

				if (obj == null)
					return;
				else
					ObjectPooler.AddToPool(id, obj.gameObject);
			}
		}

		/// <summary>
		/// Draws the custom editor
		/// </summary>
		private void Draw()
		{
			GUILayout.BeginHorizontal();
			{
				EditorGUILayout.LabelField("Prefab", GUILayout.Width(40));
				_prefab = (GameObject)EditorGUILayout.ObjectField("", _prefab, typeof(GameObject), false);

				string entry = _prefab == null ? string.Empty : _prefab.name;
				Color baseColor = GUI.backgroundColor;
				if (_enumNames.Contains(entry.ToSnakeCase().ToUpper()) || _prefab == null || _prefab.GetComponent<IPoolableObject>() == null)
					GUI.backgroundColor = Color.red;
				if (GUILayout.Button("Add"))
				{
					if (_enumNames.Contains(entry.ToSnakeCase().ToUpper()))
						return;
					GUI.backgroundColor = baseColor;
					_enumNames.Add(entry.ToSnakeCase().ToUpper());
					EditorPrefs.SetString("AssetName", entry);
					EditorPrefs.SetBool("WaitingToCreate", true);
					RegenerateEnum(_enumNames);
				}
			}
			GUILayout.EndHorizontal();
			if (GUILayout.Button("Regenerate Enum from Pools"))
			{
				string[] names = ObjectPooler.GetPoolNames();

				for (int i = 0; i < names.Length; i++)
					names[i] = names[i].ToSnakeCase().ToUpper();
				RegenerateEnum(names.ToList());
			}
		}
	}
}
