using Models;
using UnityEngine;

public class LandShopItemViewModel : ShopItemViewModelBase
{
	[SerializeField] private Land _model;

	protected override ModelBase Model
	{
		get { return _model ?? (_model = Repository.GetLandByID(_modelID)); }
	}

	protected override void OnStateLoad()
	{
		_model = Repository.GetLandByID(_modelID);
		base.OnStateLoad();
	}
}