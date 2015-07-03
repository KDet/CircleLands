using System;
using Models;
using UnityEngine;
using UnityEngine.Events;

public class MenuViewModel : ViewModelBase
{
	[SerializeField] private string _characterID;
	[SerializeField] private GameLang _language;
	[SerializeField] private string _landID;
	[SerializeField] private bool _isSound;

	[SerializeField] private CharacterIDChanged _onCharacterIDChanged = new CharacterIDChanged();
	[SerializeField] private LanguageValueChanged _onLanguageValueChanged = new LanguageValueChanged();
	[SerializeField] private LandIDChanged _onLandIDChanged = new LandIDChanged();
	[SerializeField] private IsSoundChanged _isSoundChanged = new IsSoundChanged();
	[SerializeField] private IsSoundChanged _canNavigateChanged = new IsSoundChanged();
	
	[SerializeField] private bool _canNavigate;

	private bool CheckCanNavigate()
	{
		Character character = Repository.GetCharacterById(_characterID);
		Land land = Repository.GetLandByID(_landID);
		return character != null && land != null && character.IsBought && land.IsBought;
	}

	public string CharacterID
	{
		get { return _characterID; }
		set
		{
			_characterID = value;
			OnValueChanged(ref _characterID, _onCharacterIDChanged);
			CanNavigate = CheckCanNavigate();
		}
	}
	public bool CanNavigate
	{
		get { return _canNavigate; }
		set
		{
			_canNavigate = value;
			OnValueChanged(ref value, _canNavigateChanged);
		}
	}
	public string LandID
	{
		get { return _landID; }
		set
		{
			_landID = value;
			OnValueChanged(ref value, _onLandIDChanged);
			CanNavigate = CheckCanNavigate();
		}
	}
	public bool IsSound
	{
		get { return _isSound; }
		set
		{
			_isSound = value;
			OnValueChanged(ref value, _isSoundChanged);
		}
	}
	public GameLang Language
	{
		get { return _language; }
		set
		{
			_language = value;
			OnValueChanged(ref value, _onLanguageValueChanged);
		}
	}

	public void SetCharacterID(string id)
	{
		_characterID = id;
		CanNavigate = CheckCanNavigate();
	}
	public void SetLandID(string id)
	{
		_landID = id;
		CanNavigate = CheckCanNavigate();
	}
	public void SwitchSound(bool isSound)
	{
		_isSound = isSound;
		GameStorage.IsSound = isSound;
	}

	protected override void OnStateSave()
	{
		GameStorage.UsedCharacterID = CharacterID;
		GameStorage.UsedLandID = LandID;
		GameStorage.UsedLanguage = Language;
		GameStorage.IsSound = IsSound;
		base.OnStateSave();
	}
	protected override void OnStateLoad()
	{
		CharacterID = GameStorage.UsedCharacterID;
		LandID = GameStorage.UsedLandID;
		Language = GameStorage.UsedLanguage;
		IsSound = GameStorage.IsSound;
	}

	[Serializable] public class CharacterIDChanged : UnityEvent<string> { }
	[Serializable] public class LandIDChanged : UnityEvent<string> { }
	[Serializable] public class IsSoundChanged : UnityEvent<bool> { }
	[Serializable] public class CanNavigateChanged : UnityEvent<bool> { }
}