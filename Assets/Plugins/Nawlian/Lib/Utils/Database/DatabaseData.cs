using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Nawlian.Lib.Utils.Database
{
	/// <summary>
	/// Database scriptable object, caching all assets
	/// </summary>
	[CreateAssetMenu(menuName = "Tools/Lib/Database", fileName = "Database")]
	public class DatabaseData : SerializedScriptableObject
	{
		#region Types

		public class DatabaseAsset
		{
			public string Name;
			public UnityEngine.Object Prefab;

			public bool IsLegal() => Prefab != null && !string.IsNullOrEmpty(Name) && Name.All(char.IsLetterOrDigit) && Name != "All" && (Prefab is GameObject || Prefab is ScriptableObject);
		}

		public class DatabaseAssetArray
		{
			public string Name;
			public List<UnityEngine.Object> Prefabs = new List<UnityEngine.Object>();
		}

		public class Section
		{
			public string Name;
			public string GraphPath;
			public List<Section> Sections = new List<Section>();
			public List<DatabaseAsset> Assets = new List<DatabaseAsset>();

#if UNITY_EDITOR
			public Section AddSection(string name)
			{
				Section newSection = new Section() { Name = name, GraphPath = $"{GraphPath}.{Sections.Count}" };

				Sections.Add(newSection);
				return newSection;
			}

			public bool IsNameLegal(string name) => name != Name && name.All(char.IsLetter) && Sections.Count(x => x.Name == name) < 2;
			public bool IsAssetLegal(DatabaseAsset asset) => asset.IsLegal() && asset.Name != Name && Assets.Count(x => x.Name == asset.Name) < 2;
#endif
		}

		[Serializable]
		public class SectionDictionary : SerializedDictionary<string, Section> { }

		#endregion

		public List<Section> Sections = new List<Section>();

#if UNITY_EDITOR
		public string DatabaseClassPath => $"{DatabaseFolder}\\{DatabaseClassName}";

		public string DatabaseFolder;
		public string DatabaseClassName => $"{name}.cs";
		public string AssetPathFromResources
		{
			get
			{
				string fullPath = AssetDatabase.GetAssetPath(GetInstanceID());

				if (!fullPath.Contains("Resources"))
					return "";
				return fullPath.Substring(fullPath.IndexOf("Resources") + "Resources/".Length).Replace(".asset", "");
			}
		}

		public Section AddSection(string name)
		{
			Section newSection = new Section() { Name = name, GraphPath = $"{Sections.Count}" };
			Sections.Add(newSection);
			return newSection;
		}

		public bool IsNameLegal(string name) => !string.IsNullOrEmpty(name) && name.All(char.IsLetter) && Sections.Count(x => x.Name == name) < 2;

#endif
	}
}