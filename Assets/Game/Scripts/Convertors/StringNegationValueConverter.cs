using System;
using UnityEngine;

public class StringNegationValueConverter : ValueConverterBase<string, bool>
{
	[SerializeField] private GameObject[] _items;

	public override void Conver(string value)
	{
		bool found = false;
		if (_items != null)
			for (int i = _items.Length - 1; i >= 0; i--)
				if(_items[i] != null) 
				{
					bool visible = string.Equals(value, _items[i].name, StringComparison.OrdinalIgnoreCase);
					_items[i].SetActive(visible);
					if (visible)
						found = true;
				}
		Value = found;
	}
}