using Models;
using UnityEngine;

public class CharacterShopItemViewModel : ShopItemViewModelBase
{
	[SerializeField] private Character _model;

	protected override ModelBase Model
	{
		get { return _model ?? (_model = Repository.GetCharacterById(_modelID)); }
	}

	protected override void OnStateLoad()
	{
		_model = Repository.GetCharacterById(_modelID);
		base.OnStateLoad();
	}
}