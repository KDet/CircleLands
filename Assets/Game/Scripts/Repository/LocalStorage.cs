using System;
using Models;
using UnityEngine;

[Serializable]
public class LocalStorage : ScriptableObject
{
	[SerializeField] private Character[] _characters;
	[SerializeField] private Land[] _lands;
	[SerializeField] private int _cash;

	[SerializeField] private string _usedCharacterID;
	[SerializeField] private string _usedLandID;
	[SerializeField] private GameLang _usedLanguage;
	[SerializeField] private bool _isSound;

	public Character[] Characters
	{
		get { return _characters; }
	}
	public Land[] Lands
	{
		get { return _lands; }
	}

	public string UsedCharacterID
	{
		get { return _usedCharacterID; }
		set { _usedCharacterID = value; }
	}
	public string UsedLandID
	{
		get { return _usedLandID; }
		set { _usedLandID = value; }
	}
	public GameLang UsedLanguage
	{
		get { return _usedLanguage; }
		set { _usedLanguage = value; }
	}
	public int Cash
	{
		get { return _cash; }
		set { _cash = value; }
	}

	public bool IsSound
	{
		get { return _isSound; }
		set { _isSound = value; }
	}
}
