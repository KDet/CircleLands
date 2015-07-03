//Mike Hergaarden - M2H.nl

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using UnityEditor;
using UnityEngine;

public class LanguageEditor : EditorWindow
{
	//Settings
	private string urlRequestProxy = "http://www.M2H.nl/unity/editorPageProxy.php";
	static string settingsAssetPath = "Assets/Localization/Resources/Languages/LocalizationSettings.asset";
	public static LocalizationSettings settings = (LocalizationSettings)AssetDatabase.LoadAssetAtPath(settingsAssetPath, typeof(LocalizationSettings));

	[MenuItem("Tools/Localization")]
	static void OpenWindow()
	{
		GetWindow(typeof(LanguageEditor));
	}

	string gDocsURL = "";
	int unresolvedErrors;
	int foundSheets;
	bool useSystemLang = true;
	LanguageCode langCode = LanguageCode.EN;
	bool loadedSettings;

	LanguageEditor()
	{
		LoadSettings();
	}

	void LoadSettings()
	{
		if(loadedSettings)
			return;
		loadedSettings = true;
		gDocsURL = EditorPrefs.GetString(PlayerSettings.productName + "gDocs");
		
		
		if(File.Exists(settingsAssetPath))
			settings = (LocalizationSettings)AssetDatabase.LoadAssetAtPath(settingsAssetPath, typeof(LocalizationSettings));
		else {
			Debug.LogError("does not exist, moved");	
		}
		
		useSystemLang = settings.UseSystemLanguagePerDefault;
		langCode = LocalizationSettings.GetLanguageEnum(settings.DefaultLangCode);
	}

	Vector2 scrollView;

	void OnGUI()
	{
		if(EditorApplication.isPlaying) {
			GUILayout.Label("Editor is in play mode.");
			return;
		}
		LoadSettings();
		scrollView = GUILayout.BeginScrollView(scrollView);

		GUILayout.Label("Settings", EditorStyles.boldLabel);
		useSystemLang = EditorGUILayout.Toggle("Try system language", useSystemLang);
		langCode = (LanguageCode)EditorGUILayout.EnumPopup("Default language", langCode);
		gDocsURL = EditorGUILayout.TextField("gDocs Link", gDocsURL);
		if(GUI.changed) {
			SaveSettingsFile();
		}
		if(GUILayout.Button("Update translations")) {
			gDocsURL = gDocsURL.Trim();
			if(gDocsURL.Contains("&output")) {
				//OLD FORMAT
				if(!gDocsURL.Contains("&single="))
					gDocsURL += "&single=true";
				if(!gDocsURL.Contains("&gid="))
					gDocsURL += "&gid=0";
				if(!gDocsURL.Contains("&output=csv"))
					gDocsURL += "&output=csv";
				if(gDocsURL.Contains("&output=html"))
					gDocsURL = gDocsURL.Replace("&output=html", "&output=csv");
			}
			if(!gDocsURL.Contains(".google.com")) {
				EditorUtility.DisplayDialog("Error", "You have entered an incorrect spreadsheet URL. Please read the manuals instructions (See readme.txt)", "OK");
			} else {
				EditorPrefs.SetString(PlayerSettings.productName + "gDocs", gDocsURL);
				LoadSettings(gDocsURL);
			}
		}
		if(unresolvedErrors > 0) {
			Rect rec = GUILayoutUtility.GetLastRect();
			GUI.color = Color.red;
			EditorGUI.DropShadowLabel(new Rect(0, rec.yMin + 15, 200, 20), "Unresolved errors: " + unresolvedErrors);
			GUI.color = Color.white;
		}


		GUILayout.Space(25);
		GUILayout.Label("For full instructions read the localization package documentation.", EditorStyles.miniLabel);
		if(GUILayout.Button("Open documentation")) {
			Application.OpenURL("http://www.M2H.nl/files/LocalizationPackage.pdf");
		}
		if(GUILayout.Button("Verify localized assets")) {
			VerifyLocAssets();
		}
		if(GUILayout.Button("More Unity resources")) {
			Application.OpenURL("http://www.M2H.nl/unity/");
		}

		GUILayout.EndScrollView();
	}

	static string PregReplace(string input, string[] pattern, string[] replacements)
	{
		if(replacements.Length != pattern.Length)
			throw new ArgumentException("Replacement and Pattern Arrays must be balanced");

		for(var i = 0; i < pattern.Length; i++) {
			input = Regex.Replace(input, pattern[i], replacements[i]);
		}

		return input;
	}

	void VerifyLocAssets()
	{
		string langRootFolder = GetLanguageFolder() + "/Assets/";
		string[] files = Directory.GetFiles(langRootFolder, "*", SearchOption.AllDirectories);

		List<string> langList = new List<string>();
		List<string> uniqueAssets = new List<string>();
		foreach(string file in files) {
			if(file.Length > 5 && file.Substring(file.Length - 5, 5) == ".meta")
				continue;//Ignore meta files

			string file2 = file.Substring(langRootFolder.Length);

			string lang = file2.Substring(0, 2);
			if(!langList.Contains(lang))
				langList.Add(lang);
			string uniqueAsset = file2.Substring(3);
			if(!uniqueAssets.Contains(uniqueAsset)) {
				uniqueAssets.Add(uniqueAsset);
			}
		}

		int missing = 0;
		//Test assets
		foreach(string lang in langList) {
			foreach(string asset in uniqueAssets) {
				if(!File.Exists(langRootFolder + lang + "/" + asset)) {
					missing++;
					Debug.LogError("[" + lang + "] MISSING " + langRootFolder + lang + "/" + asset);
				}
			}
		}

		if(missing > 0) {
			EditorUtility.DisplayDialog("Verifying assets", "Missing asset translations: " + missing + ". See console for details.", "OK");
		} else {
			EditorUtility.DisplayDialog("Verifying assets", "All seems to be OK!", "OK");
		}
	}

	void LoadSettings(string gDocsPage)
	{
		SaveSettingsFile();


		gDocsPage += ((gDocsPage.Contains("&")) ? "&" : "?") + "timestamp=" + EditorApplication.timeSinceStartup;//Prevent caching

		float progress = 0.1f;

		List<SheetInfo> sheetIDs = GetSpreadSheetIDs(gDocsPage);
		settings.SheetTitles = new string[0];
		unresolvedErrors = 0;
		foundSheets = 0;

		Debug.Log(gDocsPage);
	

		for(int i = sheetIDs.Count - 1; i >= 0; i--) {
			int downloadSheet = sheetIDs[i].ID;

			string thisURL = gDocsPage;
			bool oldFormat = false;
			if(gDocsPage.Contains("&output=")) { 
				//Old format
				oldFormat = true;
				thisURL = gDocsPage.Replace("gid=0", "gid=" + (downloadSheet)) + "&" + EditorApplication.timeSinceStartup;
			}
			//Debug.Log(thisURL);

			EditorUtility.DisplayProgressBar("Downloading gDoc data", "Page: " + downloadSheet, progress);
			string data = GetWebpage(thisURL);
			Debug.Log("WWWDATA from Gdocs:[" + thisURL + "] " + data);
			if(data != "") {
				if(oldFormat) {
					data = CleanData(data);
					Debug.Log("data now=" + data);
					if(data.StartsWith("<html>") || data.StartsWith("<!DOCTYPE html>")) {//Sheet does not exist
						Debug.LogError("Sheet #" + downloadSheet + " does not exist!");
						continue; //If more than X in a row do now exist, it's probably the end
					}
				}
				if(!ParseData(data, sheetIDs[i].title, i)) {
					//Failed
				}
			}
		}		
		if(foundSheets == 0) {
			EditorUtility.DisplayDialog("Error", "No sheets could be imported. Either they were all empty, or you entered a wrong link. Please copy your LINK again and verify your spreadsheet.", "OK");
		}


		EditorUtility.ClearProgressBar();
		if(unresolvedErrors > 0) {
			EditorUtility.DisplayDialog("Errors", "There are " + unresolvedErrors + " open errors in your localization. See the console for more information.", "OK");
		}
	}

	void LoadCSV(Hashtable loadLanguages, Hashtable loadEntries, string data, string debugSheetTitle)
	{
		List<string> sheetNames = new List<string>(settings.SheetTitles);
		if(!sheetNames.Contains(debugSheetTitle))
			sheetNames.Add(debugSheetTitle);
		settings.SheetTitles = sheetNames.ToArray();
		SaveSettingsFile();

		List<string> lines = GetCVSLines(data);

		for(int i = 0; i < lines.Count; i++) {
			string line = lines[i];
			List<string> contents = GetCVSLine(line);
			if(i == 0) {//Language titles
				for(int j = 1; j < contents.Count; j++) {
					loadLanguages[j] = contents[j];
					loadEntries[j] = new Hashtable();
				}
			} else if(contents.Count > 1) {
				string key = contents[0];
				if(key == "")
					continue; //Skip entries with empty keys (the other values can be used as labels)
				for(int j = 1; j < (loadLanguages.Count + 1) && j < contents.Count; j++) {
				
					string content = contents[j];
					Hashtable hTable = (Hashtable)loadEntries[j];
					if(hTable.ContainsKey(key)) {
						Debug.LogError("ERROR: Double key [" + key + "] Sheet: " + debugSheetTitle);
						unresolvedErrors++;
					}
					hTable[key] = SecurityElement.Escape(content);
				}
			}
		}
	}

	void LoadHTML(Hashtable loadLanguages, Hashtable loadEntries, string data, string sheetTitle, int sheetID)
	{
		//all languages on 1 page.

		var document = new HtmlDocument();
		document.LoadHtml(data);
			
		int tableNR = 0;
		foreach(HtmlNode node in document.DocumentNode.SelectNodes("//table")) {
			if(tableNR == sheetID) {
				//Only parse 1 "sheet"
				ParseHTMLTable(loadLanguages, loadEntries, node);
			}
			tableNR++;
			foundSheets++;
		}

		//Parse sheet titles
		List<string> sheetNames = new List<string>(settings.SheetTitles);

		List<string> newTitles = GetSheetTitles(document);
		foreach(string newTitl in newTitles) {
			if(!sheetNames.Contains(newTitl))
				sheetNames.Add(newTitl);
		}
		settings.SheetTitles = sheetNames.ToArray();
		SaveSettingsFile();

	}

	void ParseHTMLTable(Hashtable loadLanguages, Hashtable loadEntries, HtmlNode node)
	{
		int row = -1;
		bool firstRow = true;
		foreach(HtmlNode trNode in node.SelectNodes(".//tr")) 
		{		

			if(trNode.SelectNodes(".//td")==null){
				continue;
			}
			row++;
			int i = -1;
			string key = "";


			foreach(HtmlNode tdNode in trNode.SelectNodes(".//td"))	
			{
				i++;

				if(firstRow && row == 0) {
					//Language header
					if(i == 0)
						continue; //Ignore this top-left empty cell	
					loadLanguages[i] = tdNode.InnerText;
					if(!loadEntries.ContainsKey(i))
						loadEntries[i] = new Hashtable();
				} else {
					//Data rows				

					if(i == 0) {
						key = tdNode.InnerText;
						continue;
					}
					if(key == "")
						continue; //Skip entries with empty keys (the other values can be used as labels)
							
					string content = tdNode.InnerText;
					Hashtable hTable = (Hashtable)loadEntries[i];
					if(hTable.ContainsKey(key)) {
						Debug.LogError("ERROR: Double key [" + key + "]");
						unresolvedErrors++;
					}
					hTable[key] = content;
				}
			}
			firstRow = false;
		}
	}

	bool ParseData(string data, string sheetTitle, int sheetID)
	{
		string langFolder = GetLanguageFolder();

		Hashtable loadLanguages = new Hashtable();
		Hashtable loadEntries = new Hashtable();

		if(data.Contains("<html>")) {
			LoadHTML(loadLanguages, loadEntries, data, sheetTitle, sheetID);
		} else {
			//CSV
			LoadCSV(loadLanguages, loadEntries, data, sheetTitle);
		}
       

		if(loadEntries.Count < 1) {
			unresolvedErrors++;
			Debug.LogError("Sheet " + sheetTitle + " contains no languages!");
			return false;
		}
		//Verify loaded data
		Hashtable sampleData = (Hashtable)loadEntries[1];
		for(int j = 2; j < loadEntries.Count; j++) {
			Hashtable otherData = ((Hashtable)loadEntries[j]);

			foreach(DictionaryEntry item in otherData) {
				if(!sampleData.ContainsKey(item.Key)) {
					Debug.LogError("[" + loadLanguages[1] + "] [" + item.Key + "] Key is missing!");
					unresolvedErrors++;
				}
			}
			foreach(DictionaryEntry item in sampleData) {
				if(!otherData.ContainsKey(item.Key)) {
					Debug.LogError("Sheet(" + sheetTitle + ") [" + loadLanguages[j] + "] [" + item.Key + "] Key is missing!");
					unresolvedErrors++;
				}
			}
		}

		//Save the loaded data
		foreach(DictionaryEntry langs in loadLanguages) {
			string langCode = (string)langs.Value;
			int langID = (int)langs.Key;

			string output = "<entries>\n";
			Hashtable entries = (Hashtable)loadEntries[langID];
			foreach(DictionaryEntry item in entries) {
				string key = item.Key + "";
				string tvalue = item.Value + "";
				key.Replace("\"", "\\\"");
				if(IsArabic(tvalue)) {
					tvalue = ReverseString(tvalue);
				}
				output += "<entry name=\"" + key + "\">" + tvalue + "</entry>\n";
			}
			output += "</entries>\n";
			foundSheets++;
			SaveFile(langFolder + "/" + langCode + "_" + sheetTitle + ".xml", output);
		}
        

	

		return true;
	}

	string CleanData(string data)
	{
		//Debug.Log("EBFORE="+data);
		//Cut of formula data
		int formulaIndex = data.IndexOf("\n\n\n[");
		if(formulaIndex != -1)
			data = data.Substring(0, formulaIndex);

		string[] patterns = new string[4];
		string[] replacements = new string[4];
		int patrs = 0;
		int reps = 0;

		patterns[patrs++] = @" \[[0-9]+\],";
		replacements[reps++] = ",";

		patterns[patrs++] = @" \[[0-9]+\]""";
		replacements[reps++] = "\"";

		patterns[patrs++] = @" \[[0-9]+\]([\n\r$]+)";
		replacements[reps++] = "$1";
		patterns[patrs++] = @" \[[0-9]+\]\Z";
		replacements[reps++] = "";

		data = PregReplace(data, patterns, replacements);



		return data;

	}

	void SaveFile(string file, string data)
	{
		if(File.Exists(file))
			File.Delete(file);
		try {
			TextWriter tw = new StreamWriter(file);
			tw.Write(data);
			tw.Close();
		} catch(IOException IOEx) {
			Debug.LogError("Incorrect file permissions? " + IOEx);
		}

		AssetDatabase.ImportAsset(file, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ImportRecursive);
		AssetDatabase.Refresh();

	}

	List<string> GetCVSLines(string data)
	{
		List<string> lines = new List<string>();
		int i = 0;
		int searchCloseTags = 0;
		int lastSentenceStart = 0;
		while(i < data.Length) {
			if(data[i] == '"') {
				if(searchCloseTags == 0)
					searchCloseTags++;
				else
					searchCloseTags--;
			} else if(data[i] == '\n') {
				if(searchCloseTags == 0) {
					lines.Add(data.Substring(lastSentenceStart, i - lastSentenceStart));
					lastSentenceStart = i + 1;
				}
			}
			i++;
		}
		if(i - 1 > lastSentenceStart) {
			lines.Add(data.Substring(lastSentenceStart, i - lastSentenceStart));
		}
		return lines;
	}

	List<string> GetCVSLine(string line)
	{
		List<string> list = new List<string>();
		int i = 0;
		int searchCloseTags = 0;
		int lastEntryBegin = 0;
		while(i < line.Length) {
			if(line[i] == '"') {
				if(searchCloseTags == 0)
					searchCloseTags++;
				else
					searchCloseTags--;
			} else if(line[i] == ',') {
				if(searchCloseTags == 0) {
					list.Add(StripQuotes(line.Substring(lastEntryBegin, i - lastEntryBegin)));
					lastEntryBegin = i + 1;
				}
			}
			i++;
		}
		if(line.Length > lastEntryBegin) {
			list.Add(StripQuotes(line.Substring(lastEntryBegin)));//Add last entry
		}
		return list;
	}

	//Remove the double " that CVS adds inside the lines, and the two outer " as well
	string StripQuotes(string input)
	{
		if(input.Length < 1 || input[0] != '"')
			return input;//Not a " formatted line

		string output = "";
		;
		int i = 1;
		bool allowNextQuote = false;
		while(i < input.Length - 1) {
			string curChar = input[i] + "";
			if(curChar == "\"") {
				if(allowNextQuote)
					output += curChar;
				allowNextQuote = !allowNextQuote;
			} else {
				output += curChar;
			}
			i++;
		}
		return output;
	}

	string GetWebpage(string url)
	{
		//Switch to standalone as we need to be able to do unrestricted WWW calls: Google does not host a crossdomain.xml
		if(EditorUserBuildSettings.selectedBuildTargetGroup == BuildTargetGroup.WebPlayer) {
			EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.StandaloneWindows);
		}

		WWW wwwReq = new WWW(url);
		while(!wwwReq.isDone) {
		}
		if(wwwReq.error == null) {
			return wwwReq.text;
		}
		//Unity Editor needs Crossdomain.xml when running in Editor with Webplayer target
		//If you are concerned about privacy/performance: You can also host this proxy yourself, see PDF
		Debug.LogWarning("Error grabbing gDocs data:" + wwwReq.error + " (URL=" + url + ")");
		Debug.LogWarning("Trying again via proxy. Switch to standalone target to prevent this!");

		WWWForm form = new WWWForm();
		form.AddField("page", url);
		WWW wwwReq2 = new WWW(urlRequestProxy, form);
		while(!wwwReq2.isDone) {
		}
		if(wwwReq2.error == null) {
			return wwwReq2.text;
		}
		Debug.LogError(wwwReq2.error);
		return "";

	}

	string GetLanguageFolder()
	{
		string[] subdirEntries = Directory.GetDirectories(Application.dataPath, "Languages", SearchOption.AllDirectories);
		foreach(string subDir in subdirEntries) {
			if(subDir.Contains("Resources"))
				return subDir;
		}

		//Create folder
		string folder = Application.dataPath + "/Localization";
		if(!Directory.Exists(folder))
			Directory.CreateDirectory(folder);

		folder = folder + "/Resources";
		if(!Directory.Exists(folder)) {
			Directory.CreateDirectory(folder);
			AssetDatabase.ImportAsset(folder, ImportAssetOptions.ForceUpdate);
		}
		folder = folder + "/Languages";
		if(!Directory.Exists(folder)) {
			Directory.CreateDirectory(folder);
			AssetDatabase.ImportAsset(folder, ImportAssetOptions.ForceUpdate);
		}
		AssetDatabase.Refresh();

		return folder;
	}


	List<string> GetSheetTitles(HtmlDocument document){
		List<string> titles = new List<string>();
		
		if(document.DocumentNode.SelectNodes("//a")==null){
			//Single sheet in this document
			HtmlNode node = document.DocumentNode.SelectSingleNode("//span ");
			
			int cutOffFrom = node.InnerText.IndexOf(":") + 2;
			string title = node.InnerText.Substring(cutOffFrom);
			
			titles.Add(title);
			
		}else{
			//Parse sheet titles
			foreach(HtmlNode node in document.DocumentNode.SelectNodes("//a")) {
				titles.Add(node.InnerText);
			}
		}
		return titles;

	}


	List<SheetInfo> GetSpreadSheetIDs(string gDocsUrl)
	{
		List<SheetInfo> res = new List<SheetInfo>();

		if(!gDocsUrl.Contains("output=")){
			//2014 format
			string output = GetWebpage(gDocsUrl);

			var document = new HtmlDocument();
			document.LoadHtml(output);		

			List<string> titles = GetSheetTitles(document);

			int i = 0;
			foreach(string titl in titles){
				SheetInfo inf = new SheetInfo();
				inf.ID = i;
				inf.title = titl;
				res.Add(inf);
				i++;
			}

		}else{
			//2013 format
			Match match = Regex.Match(gDocsUrl, "key=(?<gDocsKey>[^&#]+)");
			string gDocskey = match.Groups["gDocsKey"].Value;
			string URL = "https://spreadsheets.google.com/spreadsheet/pub?key=" + gDocskey;
			res = GetSheetIDs(URL, res);
			//Also fetch the ID of the default sheet.
			if(res.Count > 0)
				GetSheetIDs(URL + "&gid=" + res[0].ID, res);		
		}

		if(res.Count == 0) {
			Debug.LogWarning("No sheets found, or your spreadsheet has only 1 sheet. We are assuming that the first sheet has ID '0'. (You can fix this by simply adding a second sheet as this will allow ID lookup via HTML output)");
			SheetInfo info = new SheetInfo();
			info.ID = 0;
			info.title = "Sheet1";
			res.Add(info);
		}
		return res;
	}

	//Parse sheet IDs from the HTML view.
	List<SheetInfo> GetSheetIDs(string URL, List<SheetInfo> sheetList)
	{
		string output = GetWebpage(URL);
                
		MatchCollection matches = Regex.Matches(output, ";gid=(?<sheetID>[0-9]+)\">(?<sheetTitle>[^<]+)</");
		foreach(Match mat in matches) {
			int sheetID = int.Parse(mat.Groups["sheetID"].Value);
			string sheetTitle = mat.Groups["sheetTitle"].Value;
			bool present = false;
			foreach(SheetInfo info in sheetList) {
				if(info.ID == sheetID) {
					present = true;
					break;
				}
			}
			if(!present) {
				SheetInfo inf = new SheetInfo();
				inf.ID = sheetID;
				inf.title = sheetTitle;
				sheetList.Add(inf);
			}
		}
        
		return sheetList;
	}

	struct SheetInfo
	{
		public int ID;
		public string title;
	}

	void SaveSettingsFile()
	{
		if(settings == null) {
			settings = (LocalizationSettings)CreateInstance(typeof(LocalizationSettings));
			string settingsPath = Path.GetDirectoryName(settingsAssetPath);
			if(!Directory.Exists(settingsPath)) {
				Directory.CreateDirectory(settingsPath);
				AssetDatabase.ImportAsset(settingsPath);
			}

			AssetDatabase.CreateAsset(settings, settingsAssetPath);
		}
		settings.DefaultLangCode = langCode.ToString();
		settings.UseSystemLanguagePerDefault = useSystemLang;
		EditorUtility.SetDirty(settings);

	}
	
	public bool IsArabic(string strCompare)
	{
		char[] chars = strCompare.ToCharArray();
		foreach(char ch in chars)
			if(ch >= '\u0627' && ch <= '\u0649')
				return true;
		return false;
	}
	
	public static string ReverseString(string s)
	{
		char[] arr = s.ToCharArray();
		Array.Reverse(arr);
		return new string(arr);
	}
}
