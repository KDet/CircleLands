using System;
using Models;
using UnityEngine;
using UnityEngine.Events;

public abstract class ItemViewModelBase : ViewModelBase
{
	[SerializeField] protected string _modelID;
	[SerializeField] protected ItemBoughtChanged _onIsBoughtChanged = new ItemBoughtChanged();

	public string ModelID
	{
		get { return _modelID; }
	}
	public virtual bool IsBought
	{
		get { return Model.IsBought; }
		protected set
		{
			Model.IsBought = value;
			OnValueChanged(ref value, _onIsBoughtChanged);
		}
	}
	protected abstract ModelBase Model { get; }

	protected override void OnStateLoad()
	{
		base.OnStateLoad();
		IsBought = Model.IsBought;
	}
	
	[Serializable] public class ItemBoughtChanged : UnityEvent<bool> { }
}
