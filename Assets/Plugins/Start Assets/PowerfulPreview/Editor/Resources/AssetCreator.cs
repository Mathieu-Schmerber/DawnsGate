using System.IO;
using UnityEngine;
using UnityEditor;

namespace StartAssets.PowerfulPreview.Utils
{
    /// <summary>
    /// Utility class that helps creating a scriptable object as an asset file.
    /// </summary>
	public class AssetCreator
	{
        /// <summary>
        /// Creates an asset file of selected type.
        /// </summary>
        /// <typeparam name="T">Type of the asset file. Should be derived from UnityEngine.ScriptableObject.</typeparam>
        /// <param name="fileName">Name of the asset file. </param>
        public static void CreateAsset<T>( string fileName = "" ) where T : ScriptableObject
		{
			T asset = ScriptableObject.CreateInstance<T>();

			string path = AssetDatabase.GetAssetPath( Selection.activeObject );
			if( path == "" )
			{
				path = "Assets";
			}
			else if( Path.GetExtension( path ) != "" )
			{
				path = path.Replace( Path.GetFileName( AssetDatabase.GetAssetPath( Selection.activeObject ) ), "" );
			}

			string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath( path + "/" + fileName + ".asset" );

			AssetDatabase.CreateAsset( asset, assetPathAndName );

			AssetDatabase.SaveAssets();
			EditorUtility.FocusProjectWindow();
			Selection.activeObject = asset;
		}
	}
}