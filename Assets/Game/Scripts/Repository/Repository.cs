using System.IO;
using Models;
using UnityEngine;

public enum RepositoryKey
{
	UsedCharacterID,
	UsedLandID,
	UsedLanguage,
	Cash, 
	Sound
}

public static class Repository
{
	private static string _repositoryAssetPath = "Assets/Game/Resources/LocalStorage.asset";
	private static LocalStorage _storage;

	public static string RepositoryAssetPath
	{
		get { return _repositoryAssetPath; }
		set { _repositoryAssetPath = value; }
	}
	public static LocalStorage Storage
	{
		get
		{
			if (_storage == null)//automatically load settings from resources if the pointer is null (to avoid null-ref-exceptions!)
			{
				string storageFile = /* "Languages/" +*/ Path.GetFileNameWithoutExtension(RepositoryAssetPath);
				_storage = (LocalStorage) Resources.Load(storageFile, typeof (LocalStorage));
				LoadIn(_storage);
			}
			return _storage;
		}
	}

	private static void LoadIn(LocalStorage storage)
	{
		storage.UsedCharacterID = PlayerPrefs.GetString(RepositoryKey.UsedCharacterID.ToString());
		storage.UsedLandID = PlayerPrefs.GetString(RepositoryKey.UsedLandID.ToString());
		storage.UsedLanguage = (GameLang)PlayerPrefs.GetInt(RepositoryKey.UsedLanguage.ToString());
		storage.Cash = PlayerPrefs.GetInt(RepositoryKey.Cash.ToString());
		Storage.IsSound = PlayerPrefs.GetInt(RepositoryKey.Sound.ToString()) == 1;
		for (int i = 0; i < storage.Lands.Length; i++)
		{
			Land land = storage.Lands[i];
			land.Score = PlayerPrefs.GetInt(string.Format("Land.{0}.Score", land.ID));
			land.IsBought = land.IsBought != default(bool) || PlayerPrefs.GetInt(string.Format("Land.{0}.IsBought", land.ID)) == 1;
		}
		for (int i = 0; i < storage.Characters.Length; i++)
		{
			Character character = storage.Characters[i];
			character.IsBought = character.IsBought != default(bool) || PlayerPrefs.GetInt(string.Format("Character.{0}.IsBought", character.ID)) == 1;
		}
	}

	public static void Save()
	{
		PlayerPrefs.SetString(RepositoryKey.UsedCharacterID.ToString(), Storage.UsedCharacterID);
		PlayerPrefs.SetString(RepositoryKey.UsedLandID.ToString(), Storage.UsedLandID);
		PlayerPrefs.SetInt(RepositoryKey.UsedLanguage.ToString(), (int)Storage.UsedLanguage);
		PlayerPrefs.SetInt(RepositoryKey.Cash.ToString(), Storage.Cash);
		PlayerPrefs.SetInt(RepositoryKey.Sound.ToString(), Storage.IsSound ? 1 : 0);
		for (int i = 0; i < _storage.Lands.Length; i++)
		{
			Land land = _storage.Lands[i];
			PlayerPrefs.SetInt(string.Format("Land.{0}.Score", land.ID), land.Score);
			PlayerPrefs.GetInt(string.Format("Land.{0}.IsBought", land.ID), land.IsBought ? 1 : 0);
		}
		for (int i = 0; i < _storage.Characters.Length; i++)
		{
			Character character = _storage.Characters[i];
			PlayerPrefs.GetInt(string.Format("Character.{0}.IsBought", character.ID), character.IsBought ? 1 : 0);
		}
		PlayerPrefs.Save();
	}
	public static void Save(RepositoryKey key, int value)
	{
		PlayerPrefs.SetInt(key.ToString(), value);
		PlayerPrefs.Save();
	}
	public static void Save(RepositoryKey key, float value)
	{
		PlayerPrefs.SetFloat(key.ToString(), value);
		PlayerPrefs.Save();
	}
	public static void Save<T>(RepositoryKey key, T value)
	{
		PlayerPrefs.SetString(key.ToString(), value.ToString());
		PlayerPrefs.Save();
	}
	
	public static Character GetCharacterById(string id)
	{
		for (int i = 0; i < Storage.Characters.Length; i++)
		{
			Character character = Storage.Characters[i];
			if (character != null && string.Equals(character.ID, id))
				return character;
		}
		return null;
	}
	public static Land GetLandByID(string id)
	{
		for (int i = 0; i < Storage.Lands.Length; i++)
		{
			Land land = Storage.Lands[i];
			if (land != null && string.Equals(land.ID, id))
				return land;
		}
		return null;
	}
}