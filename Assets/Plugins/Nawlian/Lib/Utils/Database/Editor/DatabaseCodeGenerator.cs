using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Nawlian.Lib.Utils.Database.Editor
{
	public class DatabaseCodeGenerator
	{
		private static readonly string REFERENCE_PROPERTY = "DatabaseAsset";

		public static void GenerateCode(DatabaseData databaseAsset)
		{
			if (string.IsNullOrEmpty(databaseAsset.AssetPathFromResources))
			{
				Debug.LogError($"Nawlian/Lib - Cannot generate code for '{databaseAsset.name}'. A database asset must be within a Resources folder.");
				return;
			}
			Debug.Log($"Nawlian/Lib - Generating code for '{databaseAsset.name}'...");
			Generate(databaseAsset);
		}

		private static CodeNamespace ImportNamespaces(CodeCompileUnit unit, DatabaseData databaseAsset)
		{
			CodeNamespace usedNamespace = new CodeNamespace("Databases");

			usedNamespace.Imports.Add(new CodeNamespaceImport("System.Collections"));
			usedNamespace.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));
			usedNamespace.Imports.Add(new CodeNamespaceImport("UnityEngine"));
			usedNamespace.Imports.Add(new CodeNamespaceImport("Nawlian.Lib.Utils.Database"));
			usedNamespace.Imports.Add(new CodeNamespaceImport("Nawlian.Lib.Utils"));
			usedNamespace.Imports.Add(new CodeNamespaceImport("System.Linq"));
			unit.Namespaces.Add(usedNamespace);
			return usedNamespace;
		}

		private static void CreateDatabaseReferenceProperty(CodeTypeDeclaration baseClass, DatabaseData databaseAsset)
		{
			// add field
			CodeMemberField field = new CodeMemberField(typeof(DatabaseData), "_databaseAsset");
			baseClass.Members.Add(field);

			// add property
			CodeMemberProperty p = new CodeMemberProperty();
			p.Type = field.Type;
			p.Name = REFERENCE_PROPERTY;
			p.HasGet = true;
			p.Attributes = MemberAttributes.Public;
			p.GetStatements.Add(
				new CodeSnippetExpression($"return {field.Name} ??= Resources.Load<DatabaseData>(\"{databaseAsset.AssetPathFromResources}\")")
			);
			baseClass.Members.Add(p);
		}

		private static CodeMemberField CreateAssetField(DatabaseData.DatabaseAsset asset, DatabaseData.Section section)
		{
			var result = new CodeMemberField()
			{
				Name = asset.Name,
				Type = new CodeTypeReference(asset.Prefab.GetType()),
				Attributes = MemberAttributes.Public | MemberAttributes.Static,
			};
			string indexing = $"Sections[{section.GraphPath.Replace(".", "].Sections[")}]";

			result.InitExpression = new CodeSnippetExpression($"({asset.Prefab.GetType()})Instance.{REFERENCE_PROPERTY}.{indexing}.Assets[{section.Assets.IndexOf(asset)}].Prefab");
			return result;
		}

		private static CodeMemberMethod CreateFetchMethod(IList<DatabaseData.DatabaseAsset> validAssets)
		{
			string allFieldsArray = $"{typeof(UnityEngine.Object[])} all = new {typeof(UnityEngine.Object)}[{validAssets.Count}] {{{string.Join(", ", validAssets.Select(x => x.Name))}}}";
			CodeMemberMethod result = new CodeMemberMethod();
			var generic = new CodeTypeParameter("T");

			generic.Constraints.Add(typeof(UnityEngine.Object));
			result.Name = "All";
			result.TypeParameters.Add(generic);
			result.Attributes = MemberAttributes.Public | MemberAttributes.Static;
			result.ReturnType = new CodeTypeReference("IEnumerable<T>");
			result.Statements.Add(new CodeSnippetExpression(allFieldsArray));
			result.Statements.Add(new CodeSnippetExpression($"return all.Where(x => x is T).Select(x => (T)x)"));
			return result;
		}

		private static void RecursiveSectionGeneration(DatabaseData.Section section, CodeTypeDeclaration parentClass)
		{
			var topMember = new CodeTypeDeclaration() { Name = section.Name, Attributes = MemberAttributes.Public | MemberAttributes.Static };
			List<DatabaseData.DatabaseAsset> validAssets = new List<DatabaseData.DatabaseAsset>();
			parentClass.Members.Add(topMember);

			// Creating assets as properties
			foreach (DatabaseData.DatabaseAsset asset in section.Assets)
			{
				if (!section.IsAssetLegal(asset))
					continue;
				topMember.Members.Add(CreateAssetField(asset, section));
				validAssets.Add(asset);
			}

			foreach (DatabaseData.Section subSection in section.Sections)
				if (section.IsNameLegal(subSection.Name))
					RecursiveSectionGeneration(subSection, topMember);

			if (validAssets.Count > 0)
				topMember.Members.Add(CreateFetchMethod(validAssets));
		}

		private static void GenerateTypes(CodeNamespace @namespace, DatabaseData databaseAsset)
		{
			CodeTypeDeclaration baseClass = new CodeTypeDeclaration(databaseAsset.name) {Attributes = MemberAttributes.Public | MemberAttributes.Static};

			foreach (DatabaseData.Section mainSection in databaseAsset.Sections)
			{
				if (databaseAsset.IsNameLegal(mainSection.Name))
					RecursiveSectionGeneration(mainSection, baseClass);
			}
			CreateDatabaseReferenceProperty(baseClass, databaseAsset);
			baseClass.BaseTypes.Add("Singleton<Database>");
			@namespace.Types.Add(baseClass);
		}

		private static void GenerateCSharpCode(CodeCompileUnit unit, DatabaseData databaseAsset)
		{
			CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
			CodeGeneratorOptions options = new CodeGeneratorOptions();
			options.BracingStyle = "C";
			using (StreamWriter sourceWriter = new StreamWriter(Path.Combine(databaseAsset.DatabaseClassPath)))
				provider.GenerateCodeFromCompileUnit(unit, sourceWriter, options);
		}

		private static void Generate(DatabaseData databaseAsset)
		{
			CodeCompileUnit compileUnit = new CodeCompileUnit();
			CodeNamespace genNamespace = ImportNamespaces(compileUnit, databaseAsset);

			GenerateTypes(genNamespace, databaseAsset);
			GenerateCSharpCode(compileUnit, databaseAsset);
			AssetDatabase.Refresh();
		}
	}
}