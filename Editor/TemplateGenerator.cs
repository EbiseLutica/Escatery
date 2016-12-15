using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;
using System;

public class TemplateGenerator : AssetPostprocessor
{

	// テンプレートファイルの格納ディレクトリパス
	private const string TEMPLATE_DIR = "ScriptTemplates";
	private static Dictionary<string, string> templateFiles =
 new Dictionary<string, string>() {
		{ ".cs", "81-C# Script-NewBehaviourScript.cs.txt" }
 };
	// テンプレート内のシンボル置換定義
	private delegate string Replace(string path);
	private static Dictionary<string, Replace> replaceDef =
	new Dictionary<string, Replace>() {
		{ "NAME", path => Regex.Replace(path, @"(.*/)|(\..*$)", "") },
		{ "SCRIPTNAME", path => Regex.Replace(path, @"(.*/)|(\..*$)|(\s)", "")},
		{ "SCRIPTNAME_LOWER", path => {
			var ti = CultureInfo.CurrentCulture.TextInfo;

			path = Regex.Replace(path, @"(.*/)|(\..*$)|(\s)", "");

			if ( Regex.Match(path, "^[A-Z]").Success ) {
                // 先頭が大文字なら小文字に変換
                path = ti.ToLower(path[0]) + path.Substring(1);
			} else if ( Regex.Match(path, "^[a-z]").Success ) {
                // 先頭が小文字なら大文字に変換して「my」を先頭に付加
                path = "my" + ti.ToUpper(path[0]) + path.Substring(1);
			} else {
                // アルファベット以外ならそのまま「my」を先頭に付加
                path = "my" + path;
			}

			return path;
		} },
        // ★必要に応じて置換するシンボルを追加してください,
		{ "DATE", path => DateTime.Now.ToString("yyyy.MM.dd")},
		{ "NOTRIM", path => "" }
	};

	//-------------------------------------------------------------------------
	// テンプレートファイルからファイルを生成する
	//-------------------------------------------------------------------------
	private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromPath)
	{
		var templateDir = Path.Combine(Application.dataPath, TEMPLATE_DIR);
		var unityTemplateDir = Path.Combine(EditorApplication.applicationContentsPath, "Resources");
		unityTemplateDir = Path.Combine(unityTemplateDir, "ScriptTemplates");

		//Debug.Log("unityTemplateDir = " + unityTemplateDir);

		foreach (var path in importedAssets)
		{
			// 拡張子チェック
			string templatePath;
			if (!templateFiles.TryGetValue(Path.GetExtension(path), out templatePath))
			{
				continue;
			}

			// インポートされたスクリプトとUnity側のテンプレートとの比較

			// Unityテンプレートファイル読み込み
			var unityTemplateData = File.ReadAllText(Path.Combine(unityTemplateDir, templatePath));
			unityTemplateData = ReplaceSymbol(unityTemplateData, path);

			// インポートしたスクリプトファイル読み込み
			var importedData = File.ReadAllText(path);

			// 比較
			if (string.Compare(unityTemplateData, importedData) != 0)
			{
				// 異なっていたら新規作成でない
				continue;
			}
			// Event 接尾辞がつくスクリプトはEvent用テンプレートにする
			templatePath = Path.GetFileNameWithoutExtension(path).EndsWith("Event") ? "EventBehavior.cs.txt" : "BaseBehavior.cs.txt";
			// プロジェクト内テンプレートファイル読み込み
			var templateData = File.ReadAllText(Path.Combine(templateDir, templatePath));
			templateData = ReplaceSymbol(templateData, path);

			// プロジェクト内テンプレートファイルのデータで上書き
			var sr = new StreamWriter(path);
			sr.Write(templateData);
			sr.Close();

			// 強制的にインポート
			AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
		}
	}

	//-------------------------------------------------------------------------
	// #～#のシンボル置換
	//-------------------------------------------------------------------------
	private static string ReplaceSymbol(string text, string path)
	{
		foreach (var def in replaceDef)
		{
			text = Regex.Replace(text, "#" + def.Key + "#", def.Value(path), RegexOptions.Singleline);
		}

		return text;
	}
}