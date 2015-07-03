using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using UnityEngine;
using Object = UnityEngine.Object;

public static class Language
{
	private static string _settingsAssetPath = "Assets/Localization/Resources/Languages/LocalizationSettings.asset";
	private static LocalizationSettings _settings;
	private static List<string> _availableLanguages;
	private static LanguageCode _currentLanguage = LanguageCode.N;
	private static Dictionary<string, Dictionary<string, string>> _currentEntrySheets;

	public static string SettingsAssetPath
	{
		get { return _settingsAssetPath; }
		set { _settingsAssetPath = value; }
	}
	public static LocalizationSettings Settings
	{
		get
		{
			//automatically load settings from resources if the pointer is null (to avoid null-ref-exceptions!)
			if(_settings == null)
			{
				string settingsFile = "Languages/" + Path.GetFileNameWithoutExtension(SettingsAssetPath);
				_settings = (LocalizationSettings)Resources.Load(settingsFile, typeof(LocalizationSettings));
			}
			return _settings;
		}
	}

	static Language()
	{
		LoadAvailableLanguages();

		bool useSystemLanguagePerDefault = Settings.UseSystemLanguagePerDefault;
		LanguageCode useLang = LocalizationSettings.GetLanguageEnum(Settings.DefaultLangCode);
			//ISO 639-1 (two characters). See: http://en.wikipedia.org/wiki/List_of_ISO_639-1_codes

		//See if we can use the last used language (playerprefs)
		//string lastLang = PlayerPrefs.GetString("M2H_lastLanguage", "");

//	if (lastLang != "" && _availableLanguages.Contains(lastLang))
//	{
//		SwitchLanguage(lastLang);
//	}
//	else
		{
			//See if we can use the local system language: if so, we overwrite useLang
			if (useSystemLanguagePerDefault)
			{
				//Attempt 1. Use Unity system lang
				LanguageCode localLang = LanguageNameToCode(Application.systemLanguage);
				if (localLang == LanguageCode.N)
				{
					//Attempt 2. Otherwise try .NET cultureinfo; doesnt work on mobile systems
					// Also returns EN (EN-US) on my dutch pc (interface is english but Country&region is Netherlands)
					//BUGGED IN MONO? See: http://forum.unity3d.com/threads/5452-Getting-user-s-language-preference
					string langISO = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
					if (langISO != "iv") //IV = InvariantCulture
						localLang = LocalizationSettings.GetLanguageEnum(langISO);
				}
				if (_availableLanguages.Contains(string.Format("{0}", localLang)))
				{
					useLang = localLang;
				}
				else
				{
					//We dont have the local lang..try a few common exceptions
					if (localLang == LanguageCode.PT)
					{
						//  we did't have PT, can we show PT_BR instead?
						if (_availableLanguages.Contains(string.Format("{0}", LanguageCode.PT_BR)))
						{
							useLang = LanguageCode.PT_BR;
						}
					}
					else if (localLang == LanguageCode.EN)
					{
						// Same idea as above..
						if (_availableLanguages.Contains(string.Format("{0}", LanguageCode.EN_GB)))
						{
							useLang = LanguageCode.EN_GB;
						}
					}
					else if (localLang == LanguageCode.EN)
					{
						// Same idea as above..
						if (_availableLanguages.Contains(string.Format("{0}", LanguageCode.EN_US)))
						{
							useLang = LanguageCode.EN_US;
						}
					}
				}
			}
			SwitchLanguage(useLang);
		}
	}



	static void LoadAvailableLanguages()
	{
		_availableLanguages = new List<string>();
		if (Settings.SheetTitles == null || Settings.SheetTitles.Length <= 0)
		{
			Debug.Log("None available");
			return;
			
		}
		foreach (LanguageCode item in Enum.GetValues(typeof(LanguageCode)))
		{
			if (HasLanguageFile(item + "", Settings.SheetTitles[0]))
			{
				_availableLanguages.Add(item + "");
			}
		}
		Resources.UnloadUnusedAssets();//Clear all loaded language files
	}
	
	
	
	public static string[] GetLanguages()
	{
		return _availableLanguages.ToArray();
	}
	
	public static bool SwitchLanguage(string langCode)
	{
		return SwitchLanguage(LocalizationSettings.GetLanguageEnum(langCode));
	}
	public static bool SwitchLanguage(LanguageCode code)
	{
		if (_availableLanguages.Contains(code + ""))
		{
			DoSwitch(code);
			return true;
		}
		Debug.LogError("Could not switch from language " + _currentLanguage + " to " + code);
		if (_currentLanguage == LanguageCode.N)
		{
			if (_availableLanguages.Count > 0)
			{
				DoSwitch(LocalizationSettings.GetLanguageEnum(_availableLanguages[0]));
				Debug.LogError("Switched to " + _currentLanguage + " instead");
			}
			else
			{
				Debug.LogError("Please verify that you have the file: Resources/Languages/" + code + "");
				Debug.Break();
			}
		}
			
		return false;
	}
	
	
	static void DoSwitch(LanguageCode newLang)
	{
		//PlayerPrefs.SetString("M2H_lastLanguage", newLang + "");
		
		_currentLanguage = newLang;
		_currentEntrySheets = new Dictionary<string, Dictionary<string, string>>();
		
		
		foreach (string sheetTitle in Settings.SheetTitles)
		{
			_currentEntrySheets[sheetTitle] = new Dictionary<string, string>();
			
			string sheetContent = GetLanguageFileContents(sheetTitle);
			if(sheetContent != "")
			{
				using (XmlReader reader = XmlReader.Create(new StringReader(sheetContent)))
				{
					while(reader.ReadToFollowing("entry")){	
						reader.MoveToFirstAttribute();
						string tag = reader.Value;	
						reader.MoveToElement();				
						string data =  reader.ReadElementContentAsString().Trim();		               
						data = data.UnescapeXML();
						(_currentEntrySheets[sheetTitle])[tag] = data;
					}
				}
			}
		}
		
		//Update all localized assets
		LocalizedAsset[] assets = (LocalizedAsset[])Object.FindObjectsOfType(typeof(LocalizedAsset));
		foreach (LocalizedAsset asset in assets)
		{
			asset.LocalizeAsset();
		}
		OnChangedLanguage();
		//SendMonoMessage("ChangedLanguage", currentLanguage);
	}
	
	//Get a localized asset for the current language
	static public Object GetAsset(string name)
	{
		return Resources.Load("Languages/Assets/" + CurrentLanguage() + "/" + name);
	}
	
	//Lang files
	static bool HasLanguageFile(string lang, string sheetTitle)
	{
		return ((TextAsset)Resources.Load("Languages/" + lang + "_" + sheetTitle, typeof(TextAsset)) != null);
	}
	
	static string GetLanguageFileContents(string sheetTitle)
	{
		TextAsset ta = (TextAsset)Resources.Load("Languages/" + _currentLanguage + "_" + sheetTitle, typeof(TextAsset));
		return ta != null ? ta.text : "";
	}
	
	
	public static LanguageCode CurrentLanguage()
	{
		return _currentLanguage;
	}
	
	public static string Get(string key)
	{
		return Get(key, Settings.SheetTitles[0]);
	}
	
	
	public static string Get(string key, string sheetTitle)
	{
		if (_currentEntrySheets == null || !_currentEntrySheets.ContainsKey(sheetTitle))
		{
			Debug.LogError("The sheet with title \""+sheetTitle+"\" does not exist!");
			return "";
		}
		if ((_currentEntrySheets[sheetTitle]).ContainsKey(key))
		{
			return (_currentEntrySheets[sheetTitle])[key];
		}
		//Debug.LogError("MISSING LANG:" + key);
		return "#!#"+ key+"#!#";
	}
	
	public static bool Has(string key)
	{
		return Has(key, Settings.SheetTitles[0]);
	}
	
	public static bool Has(string key, string sheetTitle)
	{
		if (_currentEntrySheets == null || !_currentEntrySheets.ContainsKey(sheetTitle)) return false;
		return _currentEntrySheets[sheetTitle].ContainsKey(key);
    }
	

	/// <summary>
	/// Used to send ChangedLanguage
	/// </summary>
	/// <param name="methodString">Method string.</param>
	/// <param name="parameters">Parameters.</param>
    static void SendMonoMessage(string methodString, params object[] parameters)
    {
		if (parameters != null && parameters.Length > 1) Debug.LogError("We cannot pass more than one argument currently!");
		GameObject[] gos = (GameObject[])GameObject.FindObjectsOfType(typeof(GameObject));
		foreach (GameObject go in gos)
		{
			if (go && go.transform.parent == null)
			{
				if (parameters != null && parameters.Length == 1)
				{
					go.gameObject.BroadcastMessage(methodString, parameters[0], SendMessageOptions.DontRequireReceiver);
				}
				else
				{
					go.gameObject.BroadcastMessage(methodString, SendMessageOptions.DontRequireReceiver);
				}
            }
        }
    }

	/// <summary>
	/// Languagesname to code. Note that this will not take variations such as EN_GB and PT_BR into account
	/// </summary>
	/// <returns>The name to code.</returns>
	/// <param name="name">Name.</param>
    public static LanguageCode LanguageNameToCode(SystemLanguage name)
    {
        if (name == SystemLanguage.Afrikaans) return LanguageCode.AF;
		if (name == SystemLanguage.Arabic) return LanguageCode.AR;
		if (name == SystemLanguage.Basque) return LanguageCode.BA;
		if (name == SystemLanguage.Belarusian) return LanguageCode.BE;
		if (name == SystemLanguage.Bulgarian) return LanguageCode.BG;
		if (name == SystemLanguage.Catalan) return LanguageCode.CA;
		if (name == SystemLanguage.Chinese) return LanguageCode.ZH;
		if (name == SystemLanguage.Czech) return LanguageCode.CS;
		if (name == SystemLanguage.Danish) return LanguageCode.DA;
		if (name == SystemLanguage.Dutch) return LanguageCode.NL;
		if (name == SystemLanguage.English) return LanguageCode.EN;
		if (name == SystemLanguage.Estonian) return LanguageCode.ET;
		if (name == SystemLanguage.Faroese) return LanguageCode.FA;
		if (name == SystemLanguage.Finnish) return LanguageCode.FI;
		if (name == SystemLanguage.French) return LanguageCode.FR;
		if (name == SystemLanguage.German) return LanguageCode.DE;
		if (name == SystemLanguage.Greek) return LanguageCode.EL;
		if (name == SystemLanguage.Hebrew) return LanguageCode.HE;
		if (name == SystemLanguage.Hungarian) return LanguageCode.HU;
		if (name == SystemLanguage.Icelandic) return LanguageCode.IS;
		if (name == SystemLanguage.Indonesian) return LanguageCode.ID;
		if (name == SystemLanguage.Italian) return LanguageCode.IT;
		if (name == SystemLanguage.Japanese) return LanguageCode.JA;
		if (name == SystemLanguage.Korean) return LanguageCode.KO;
		if (name == SystemLanguage.Latvian) return LanguageCode.LA;
		if (name == SystemLanguage.Lithuanian) return LanguageCode.LT;
		if (name == SystemLanguage.Norwegian) return LanguageCode.NO;
		if (name == SystemLanguage.Polish) return LanguageCode.PL;
		if (name == SystemLanguage.Portuguese) return LanguageCode.PT;
		if (name == SystemLanguage.Romanian) return LanguageCode.RO;
		if (name == SystemLanguage.Russian) return LanguageCode.RU;
		if (name == SystemLanguage.SerboCroatian) return LanguageCode.SH;
		if (name == SystemLanguage.Slovak) return LanguageCode.SK;
		if (name == SystemLanguage.Slovenian) return LanguageCode.SL;
		if (name == SystemLanguage.Spanish) return LanguageCode.ES;
		if (name == SystemLanguage.Swedish) return LanguageCode.SW;
		if (name == SystemLanguage.Thai) return LanguageCode.TH;
		if (name == SystemLanguage.Turkish) return LanguageCode.TR;
		if (name == SystemLanguage.Ukrainian) return LanguageCode.UK;
		if (name == SystemLanguage.Vietnamese) return LanguageCode.VI;
		if (name == SystemLanguage.Hungarian) return LanguageCode.HU;
		if (name == SystemLanguage.Unknown) return LanguageCode.N;
		return LanguageCode.N;
	}

	private static void OnChangedLanguage()
	{
		var handler = ChangedLanguage;
		if (handler != null) 
			handler(null, EventArgs.Empty);
	}

	public static event EventHandler ChangedLanguage;
	//For settings, see TOOLS->LOCALIZATION
}

/// <summary>
/// Language code. Those marked with a * are not auto-detected 
/// </summary>
public enum LanguageCode
{
    N,//null
    AA, //Afar
    AB, //Abkhazian
    AF, //(Zuid) Afrikaans
    AM, //Amharic
    AR, //Arabic
	AR_SA, //* Arabic (Saudi Arabia)
	AR_EG, //* Arabic (Egypt)
	AR_DZ, //* Arabic (Algeria)
	AR_YE, //* Arabic (Yemen)
	AR_JO, //* Arabic (Jordan)
	AR_KW, //* Arabic (Kuwait)
	AR_BH, //* Arabic (Bahrain)
	AR_IQ, //* Arabic (Iraq)
	AR_MA, //* Arabic (Libya) 
	AR_LY, //* Arabic (Morocco)
	AR_OM, //* Arabic (Oman)
	AR_SY, //* Arabic (Syria)
	AR_LB, //* Arabic (Lebanon)
	AR_AE, //* Arabic (U.A.E.)
	AR_QA, //* Arabic (Qatar)
    AS, //Assamese
    AY, //Aymara
    AZ, //Azerbaijani
    BA, //Bashkir
    BE, //Byelorussian
    BG, //Bulgarian
    BH, //Bihari
    BI, //Bislama
    BN, //Bengali
    BO, //Tibetan
    BR, //Breton
    CA, //Catalan
    CO, //Corsican
    CS, //Czech
    CY, //Welch
    DA, //Danish
    DE, //German
	DE_AT, //* German (Austria)
	DE_LI, //* German (Liechtenstein)
	DE_CH, //* German (Switzerland)
	DE_LU, //* German (Luxembourg)
    DZ, //Bhutani
    EL, //Greek
    EN, //English
	EN_US, //* English (United States)
	EN_AU, //* English (Australia)
	EN_NZ, //* English (New Zealand)
	EN_ZA, //* English (South Africa)
	EN_CB, //* English (Caribbean)
	EN_TT, //* English (Trinidad)
	EN_GB, //* English (United Kingdom)
	EN_CA, //* English (Canada)
	EN_IE, //* English (Ireland)
	EN_JM, //* English (Jamaica)
	EN_BZ, //* English (Belize)
    EO, //Esperanto
    ES, //Spanish (Spain)
	ES_MX, //* Spanish (Mexico)
	ES_CR, //* Spanish (Costa Rica)
	ES_DO, //* Spanish (Dominican Republic)
	ES_CO, //* Spanish (Colombia)
	ES_AR, //* Spanish (Argentina)	
	ES_CL, //* Spanish (Chile)	
	ES_PY, //* Spanish (Paraguay)	
	ES_SV, //* Spanish (El Salvador)	
	ES_NI, //* Spanish (Nicaragua)	
	ES_GT, //* Spanish (Guatemala)	
	ES_PA, //* Spanish (Panama)	
	ES_VE, //* Spanish (Venezuela)	
	ES_PE, //* Spanish (Peru)
	ES_EC, //* Spanish (Ecuador)
	ES_UY, //* Spanish (Uruguay)
	ES_BO, //* Spanish (Bolivia)
	ES_HN, //* Spanish (Honduras)
	ES_PR, //* Spanish (Puerto Rico)
    ET, //Estonian
    EU, //Basque
    FA, //Persian
    FI, //Finnish
    FJ, //Fiji
    FO, //Faeroese
    FR, //French (Standard)
	FR_BE, //* French (Belgium)
	FR_CH, //* French (Switzerland)
	FR_CA, //* French (Canada)
	FR_LU, //* French (Luxembourg)
    FY, //Frisian
    GA, //Irish
    GD, //Scots Gaelic
    GL, //Galician
    GN, //Guarani
    GU, //Gujarati
    HA, //Hausa
    HI, //Hindi
    HE, //Hebrew
    HR, //Croatian
    HU, //Hungarian
    HY, //Armenian
    IA, //Interlingua
    ID, //Indonesian
    IE, //Interlingue
    IK, //Inupiak
    IN, //former Indonesian
    IS, //Icelandic
    IT, //Italian
	IT_CH, //* Italian (Switzerland)
    IU, //Inuktitut (Eskimo)
	IW, //DEPRECATED: former Hebrew
    JA, //Japanese
    	JI, //DEPRECATED: former Yiddish
    JW, //Javanese
    KA, //Georgian
    KK, //Kazakh
    KL, //Greenlandic
    KM, //Cambodian
    KN, //Kannada
    KO, //Korean
    KS, //Kashmiri
    KU, //Kurdish
    KY, //Kirghiz
    LA, //Latin
    LN, //Lingala
    LO, //Laothian
    LT, //Lithuanian
    LV, //Latvian, Lettish
    MG, //Malagasy
    MI, //Maori
    MK, //Macedonian
    ML, //Malayalam
    MN, //Mongolian
    MO, //Moldavian
    MR, //Marathi
    MS, //Malay
    MT, //Maltese
    MY, //Burmese
    NA, //Nauru
    NE, //Nepali
    NL, //Dutch (Standard)
	NL_BE, //*  Dutch (Belgium)
    NO, //Norwegian
    OC, //Occitan
    OM, //(Afan) Oromo
    OR, //Oriya
    PA, //Punjabi
    PL, //Polish
    PS, //Pashto, Pushto
    PT, //Portuguese
	PT_BR, //* BRazilian PT - Only used if you manually set it
    QU, //Quechua
    RM, //Rhaeto-Romance
    RN, //Kirundi
    RO, //Romanian
	RO_MO, //* Romanian (Republic of Moldova)
    RU, //Russian
	RU_MO, //* Russian (Republic of Moldova)
    RW, //Kinyarwanda
    SA, //Sanskrit
    SD, //Sindhi
    SG, //Sangro
    SH, //Serbo-Croatian
    SI, //Singhalese
    SK, //Slovak
    SL, //Slovenian
    SM, //Samoan
    SN, //Shona
    SO, //Somali
    SQ, //Albanian
    SR, //Serbian
    SS, //Siswati
    ST, //Sesotho
    SU, //Sudanese
    SV, //Swedish
	SV_FI, //* Swedish (finland)
    SW, //Swahili
    TA, //Tamil
    TE, //Tegulu
    TG, //Tajik
    TH, //Thai
    TI, //Tigrinya
    TK, //Turkmen
    TL, //Tagalog
    TN, //Setswana
    TO, //Tonga
    TR, //Turkish
    TS, //Tsonga
    TT, //Tatar
    TW, //Twi
    UG, //Uigur
    UK, //Ukrainian
    UR, //Urdu
    UZ, //Uzbek
    VI, //Vietnamese
    VO, //Volapuk
    WO, //Wolof
    XH, //Xhosa
    YI, //Yiddish
    YO, //Yoruba
    ZA, //Zhuang
    ZH, //Chinese Simplified
	ZH_TW, //* Chinese Traditional / Chinese (Taiwan)
	ZH_HK, //* Chinese (Hong Kong SAR)
	ZH_CN, //* Chinese (PRC)
	ZH_SG, //* Chinese (Singapore)
    ZU  //Zulu
}

public static class StringExtensions
{
	public static string UnescapeXML(this string xml)
	{
		if (!string.IsNullOrEmpty(xml))
		{
			StringBuilder returnString = new StringBuilder(xml);
			returnString = returnString.Replace("&apos;", "'");
			returnString = returnString.Replace("&quot;", "\"");
			returnString = returnString.Replace("&gt;", ">");
			returnString = returnString.Replace("&lt;", "<");
			returnString = returnString.Replace("&amp;", "&");
			return returnString.ToString();
		}
		return xml;
	}
}