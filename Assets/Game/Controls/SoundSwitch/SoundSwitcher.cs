using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SoundSwitcher : MonoBehaviour 
{
	[SerializeField] private Image _onImage;
	[SerializeField] private Image _offImage;
	[SerializeField] private AudioSource _audio;
	[SerializeField] private IsSoundChanged _isSoundChanged = new IsSoundChanged();

	private bool _isSound = true;
	private void Switch(bool isSound)
	{
		_onImage.gameObject.SetActive(isSound);
		_offImage.gameObject.SetActive(!isSound);
		_audio.gameObject.SetActive(_isSound);
		if (_isSound)
		{
			if (!_audio.isPlaying)
				_audio.Play();
		}
		else
			_audio.Stop();
		_isSoundChanged.Invoke(isSound);
	}

	public void SwitchSound()
	{
		_isSound = !_isSound;
		Switch(_isSound);
	}
	public bool IsSound
	{
		get { return _isSound; }
		set
		{
			_isSound = value;
			Switch(value);
		}
	}
	[Serializable] public class IsSoundChanged : UnityEvent<bool> { }
}
