using System;
using UnityEngine;
using UnityEngine.Events;

public class ScrollSelector : MonoBehaviour
{
	[SerializeField] private ScrollSelectorItem[] _items;
	[SerializeField] private ScrollSelectorItem _selectedItem;
	[SerializeField] private DragSnapper _dragSnapper;
	[SerializeField] private SelectedItemChanged _selectedItemChanged = new SelectedItemChanged();

	public void SelectItem(int index)
	{
		SelectedItem = index >= 0 && index < _items.Length ? _items[index] : null;
		_dragSnapper.SetItemIndex(index);
	}
	public void SelectItem(ScrollSelectorItem item)
	{
		for (int i = 0; i < Items.Length; i++)
			if (string.Equals(_items[i].ItemID, item.ItemID))
			{
				SelectedItem = _items[i];
				_dragSnapper.SetItemIndex(i);
				break;
			}
	}

	public ScrollSelectorItem SelectedItem
	{
		get { return _selectedItem; }
		private set
		{
			_selectedItem = value; 
			_selectedItemChanged.Invoke(value);
		}
	}
	public ScrollSelectorItem[] Items
	{
		get { return _items; }
	}

	[Serializable] public class SelectedItemChanged : UnityEvent<ScrollSelectorItem> { }
}
