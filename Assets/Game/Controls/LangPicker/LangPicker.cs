using UnityEngine;

public class LangPicker : MonoBehaviour
{
	[SerializeField] private bool _langSwiping = true;
	[SerializeField] private LangPickerItem[] _items;
	[SerializeField] private GameLang _selectedLang;
	[SerializeField] private LanguageValueChanged _onValueChanged = new LanguageValueChanged();

	private void ChangeLangTo(GameLang lang)
	{
		if (lang == _selectedLang)
			return;
		GameLang oldLang = _selectedLang;
		if (_langSwiping)
		{
			int lastIndex = _items.Length - 1;
			for (int i = 0; i < _items.Length; i++)
			{
				if (_items[i].Language == lang)
					Swipe(i, 0);
				else if (_items[i].Language == oldLang)
					Swipe(i, lastIndex);
				else if ((i != 0 && i != lastIndex) && (i + 1 != lastIndex))
					Swipe(i, i + 1);
			}
		}
		else
		{
			for (int i = 0; i < _items.Length; i++)
			{
				if (_items[i].Language == lang)
					_items[i].Language = oldLang;
				else if (_items[i].Language == oldLang)
					_items[i].Language = lang;
			}
		}
		_selectedLang = lang;
		_onValueChanged.Invoke(_selectedLang);
	}
	private void Swipe(int first, int second)
	{
		GameLang lang = _items[first].Language;
		_items[first].Language = _items[second].Language;
		_items[second].Language = lang;
	}

//	protected void OnEnable()
//	{
//		
//		if ((bool)((UnityEngine.Object)this.m_HorizontalScrollbar))
//			this.m_HorizontalScrollbar.onValueChanged.AddListener(new UnityAction<float>(this.SetHorizontalNormalizedPosition));
//		if ((bool)((UnityEngine.Object)this.m_VerticalScrollbar))
//			this.m_VerticalScrollbar.onValueChanged.AddListener(new UnityAction<float>(this.SetVerticalNormalizedPosition));
//		CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild((ICanvasElement)this);
//	}
//
//	protected override void OnDisable()
//	{
//		CanvasUpdateRegistry.UnRegisterCanvasElementForRebuild((ICanvasElement)this);
//		if ((bool)((UnityEngine.Object)this.m_HorizontalScrollbar))
//			this.m_HorizontalScrollbar.onValueChanged.RemoveListener(new UnityAction<float>(this.SetHorizontalNormalizedPosition));
//		if ((bool)((UnityEngine.Object)this.m_VerticalScrollbar))
//			this.m_VerticalScrollbar.onValueChanged.RemoveListener(new UnityAction<float>(this.SetVerticalNormalizedPosition));
//		this.m_HasRebuiltLayout = false;
//		base.OnDisable();
//	}

	public void ChangeLangTo(LangPickerItem langPickerItem)
	{
		ChangeLangTo(langPickerItem.Language);
	}

	public GameLang Language
	{
		get { return _selectedLang; }
		set { ChangeLangTo(value); }
	}
}