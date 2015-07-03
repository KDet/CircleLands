using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof (Text))]
public class LangPickerItem : MonoBehaviour
{
	[SerializeField] private GameLang _language;
	private Text _text;

	private Text UIText
	{
		get { return _text ?? (_text = GetComponent<Text>()); }
	}

	public GameLang Language
	{
		get { return _language; }
		set
		{
			_language = value;
			UIText.text = LangToString(ref value);
		}
	}

	private static string LangToString(ref GameLang gamelang)
	{
		switch (gamelang)
		{
			case GameLang.EN:
				return ".EN";
			case GameLang.UK:
				return ".UK";
			case GameLang.RU:
				return ".RU";
			default:
				return string.Empty;
		}
	}

	private void Start()
	{
		UIText.text = LangToString(ref _language);
	}
}