using System;
using UnityEngine;

[Serializable]
public class LocalizationSettings : ScriptableObject
{
	[SerializeField] private string[] _sheetTitles;
	[SerializeField] private bool _useSystemLanguagePerDefault = true;
	[SerializeField] private string _defaultLangCode = "EN";

	public string DefaultLangCode
	{
		get { return _defaultLangCode; }
		set { _defaultLangCode = value; }
	}
	public bool UseSystemLanguagePerDefault
	{
		get { return _useSystemLanguagePerDefault; }
		set { _useSystemLanguagePerDefault = value; }
	}
	public string[] SheetTitles
	{
		get { return _sheetTitles; }
		set { _sheetTitles = value; }
	}

	public static LanguageCode GetLanguageEnum(string langCode)
	{
		langCode = langCode.ToUpper();
		foreach (LanguageCode item in Enum.GetValues(typeof (LanguageCode)))
			if (string.Format("{0}", item) == langCode)
				return item;
		Debug.LogError("ERORR: There is no language: [" + langCode + "]");
		return LanguageCode.EN;
	}
}