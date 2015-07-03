using UnityEngine;

public class CharacterSelectorItemModelIDConverter : ValueConverterBase<CircularSelectorItem, string>
{
	public override void Conver(CircularSelectorItem value)
	{
		CharacterItemViewModel viewModel = value.GetComponent<CharacterItemViewModel>();
		if (viewModel)
		{
			Value = viewModel.ModelID;
			if (_toSource != null)
				_toSource.SetCharacterID(viewModel.ModelID);
		}
	}

	public override void ConverBack(string value)
	{
		if(_fromSource != null)
			for (int i = 0; i < _fromSource.Items.Length; i++)
			{
				CircularSelectorItem item = _fromSource.Items[i];
				CharacterItemViewModel viewModel = item.GetComponent<CharacterItemViewModel>();
				if (viewModel && string.Equals(viewModel.ModelID, value))
				{
					Value = item;
					item.IsSelected = true;
					_fromSource.Init(item);
				}
			}
	}

	[SerializeField] private MenuViewModel _toSource;
	[SerializeField] private CircularSelector _fromSource;
}
