using UnityEngine;
using UnityEngine.UI;

public class OpenCloseToggle : MonoBehaviour 
{
	[SerializeField] private bool _isAvailable;
	[SerializeField] private Image _openImage;
	[SerializeField] private Image _closeImage;
	private Toggle _toggle;

	protected Toggle UIToggle
	{
		get { return _toggle ?? (_toggle = GetComponent<Toggle>()); }
	}

	public bool IsAvailable
	{
		get { return _isAvailable; }
		set
		{
			_isAvailable = value;
			SwitchOpenCloseItems(value);
		}
	}

	private void SwitchOpenCloseItems(bool isAvailable)
	{
		if (_openImage != null && _closeImage != null)
		{
			UIToggle.targetGraphic = isAvailable ? _openImage : _closeImage;
			_openImage.gameObject.SetActive(isAvailable);
			_closeImage.gameObject.SetActive(!isAvailable);
		}
	}
}
