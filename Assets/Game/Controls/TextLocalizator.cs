using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TextLocalizator : MonoBehaviour
{
	[SerializeField] private string _textKey;
	
	private Text _text;
	private Text UIText
	{
		get { return _text ?? (_text = GetComponent<Text>()); }
	}

	public string TextKey
	{
		get { return _textKey; }
		set
		{
			_textKey = value; 
			UpdateText();
		}
	}

	private void UpdateText()
	{
		UIText.text = Language.Get(_textKey);
	}

	private void OnEnable()
	{
		UpdateText();
		Language.ChangedLanguage += OnChangedLanguage;
	}
	private void OnDisable()
	{
		Language.ChangedLanguage -= OnChangedLanguage;
	}
	private void OnChangedLanguage(object sender, EventArgs eventArgs)
	{
		UpdateText();
	}
}
