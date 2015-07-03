using System;
using UnityEngine;
using UnityEngine.Events;

public class ShopViewModel : ViewModelBase
{
	[SerializeField] private CharacterShopItemViewModel[] _charactersShopItems;
	[SerializeField] private LandShopItemViewModel[] _lnderShopItems;

	[SerializeField] private CashChanged _onCashChanged = new CashChanged();

	public CharacterShopItemViewModel[] CharactersShopItems
	{
		get { return _charactersShopItems; }
	}
	public LandShopItemViewModel[] LnderShopItems
	{
		get { return _lnderShopItems; }
	}
	public int Cash
	{
		get { return GameStorage.Cash; }
		private set
		{
			GameStorage.Cash = value;
			OnValueChanged(ref value, _onCashChanged);
		}
	}

	public void Buy(ShopItemViewModelBase character)
	{
		if (CanBuy(character))
		{
			Cash -= character.Price;
			character.Buy();
			UpdateShopItemInfo(_charactersShopItems);
			UpdateShopItemInfo(_lnderShopItems);
		}
	}

	private bool CanBuy(ShopItemViewModelBase character)
	{
		return Math.Abs(character.Closed) < float.Epsilon && 
			   !character.IsBought && Cash > character.Price;
	}
	private void UpdateShopItemInfo<TViewModel>(TViewModel[] items) where TViewModel : ShopItemViewModelBase
	{
		for (int i = 0; i < items.Length; i++)
		{
			var viewModel = items[i];
			if (viewModel.IsBought)
			{
				viewModel.Status = "куплений";
				viewModel.Closed = 0;
			}
			else
			{
				float value = viewModel.Price >= 0 ? (float)Cash / viewModel.Price : 1f;
				if (value <= 1f)
				{
					viewModel.Closed = value <= 0f ? 1f : 1- value;
					viewModel.Status = string.Format("ще {0:F1}%", value * 100f);
				}
				else
				{
					viewModel.Closed = 0;
					viewModel.Status = "доступно";
				}
			}
		}
	}

	public void UpdatePage()
	{
		UpdateShopItemInfo(_charactersShopItems);
		UpdateShopItemInfo(_lnderShopItems);
	}

	private void Start()
	{
		UpdatePage();
	}

	protected override void OnStateLoad()
	{
		Cash = GameStorage.Cash;
		base.OnStateLoad();
		UpdatePage();
	}
	protected override void OnStateSave()
	{
		base.OnStateSave();
		GameStorage.Cash = Cash;
	}

	[Serializable] public class CashChanged : UnityEvent<int> { }
}