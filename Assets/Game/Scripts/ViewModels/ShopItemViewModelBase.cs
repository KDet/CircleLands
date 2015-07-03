using System;
using UnityEngine;
using UnityEngine.Events;

public abstract class ShopItemViewModelBase : ItemViewModelBase
{
//		public enum ShopItemStatus
//		{
//			Bought,
//			Available,
//			Unavailable
//		}
	[SerializeField] protected string _status;
	[SerializeField][Range(0f, 1f)]	protected float _closed;

	[SerializeField] protected ShopItemStatusChanged _onStatusChanged = new ShopItemStatusChanged();
	[SerializeField] protected ShopItemClosedChanged _onClosedChanged = new ShopItemClosedChanged();
	[SerializeField] protected ShopItemPriceChanged _onPriceChanged = new ShopItemPriceChanged();

	public string Status
	{
		get { return _status; }
		set
		{
			_status = value;
			OnValueChanged(ref _status, _onStatusChanged);
		}
	}
	public float Closed
	{
		get { return _closed; }
		set
		{
			_closed = value;
			OnValueChanged(ref _closed, _onClosedChanged);
		}
	}
	public int Price
	{
		get { return Model.Price; }
		private set
		{
			Model.Price = value;
			OnValueChanged(ref value, _onPriceChanged);
		}
	}

	protected override void OnStateLoad()
	{
		base.OnStateLoad();
		Price = Model.Price;
	}

	public void Buy()
	{
		IsBought = true;
	}

	[Serializable] public class ShopItemStatusChanged : UnityEvent<string> { }
	[Serializable] public class ShopItemClosedChanged : UnityEvent<float> { }
	[Serializable] public class ShopItemPriceChanged : UnityEvent<int> { }
}