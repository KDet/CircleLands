using System;
using UnityEngine;
using UnityEngine.Events;

public abstract class ViewModelBase : NotifyPropertyChanged
{
	protected LocalStorage GameStorage
	{
		get { return Repository.Storage; }
	}
	
	protected virtual void OnStateLoad()
	{
		
	}
	protected virtual void OnStateSave()
	{
		Repository.Save();
	}

	private void OnDisable()
	{
		OnStateSave();
	}
	private void OnEnable()
	{
		OnStateLoad();
	}
}

public abstract class NotifyPropertyChanged : MonoBehaviour 
{
	protected virtual bool OnValueChanged<T>(ref T value, UnityEvent<T> valueEvent)
	{
		if (valueEvent != null)
		{
			valueEvent.Invoke(value);
			return true;
		}
		return false;
	}
}

[Serializable] public class LanguageValueChanged : UnityEvent<GameLang> { }

public enum GamePlayState
{
	Start,
	Running,
	GameOver,
	Pause
}