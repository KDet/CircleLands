using System;
using UnityEngine;
using UnityEngine.Events;

public class ScrollSelectorItemModelIDConverter : ValueConverterBase<ScrollSelectorItem, string>
{
	[SerializeField] private MenuViewModel _toSource;
	[SerializeField] private ScrollSelector _fromSource;
	[SerializeField] private ItemInteractibleChanged _onItemInteractibleChanged = new ItemInteractibleChanged();

	public override void Conver(ScrollSelectorItem value)
	{
		LandItemViewModel viewModel = value.GetComponent<LandItemViewModel>();
		if (viewModel)
		{
			Value = viewModel.ModelID;
			if (_toSource != null)
			{
				_toSource.SetLandID(viewModel.ModelID);
				_onItemInteractibleChanged.Invoke(viewModel.IsBought);
			}
		}
	}

	public override void ConverBack(string value)
	{
		if (_fromSource != null)
			for (int i = 0; i < _fromSource.Items.Length; i++)
			{
				ScrollSelectorItem item = _fromSource.Items[i];
				LandItemViewModel viewModel = item.GetComponent<LandItemViewModel>();
				if (viewModel && string.Equals(viewModel.ModelID, value))
				{
					Value = item;
					_fromSource.SelectItem(item);
				}
			}
	}

	[Serializable] public class ItemInteractibleChanged : UnityEvent<bool> { }
}
