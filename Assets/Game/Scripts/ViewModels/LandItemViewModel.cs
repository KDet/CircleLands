using System;
using Models;
using UnityEngine;
using UnityEngine.Events;

public class LandItemViewModel : ItemViewModelBase
{
	[SerializeField] protected Land _model;
	[SerializeField] private HightScoreChanged _onHightScoreChanged = new HightScoreChanged();

	public int HightScore
	{
		get { return ((Land)Model).Score; }
		protected set
		{
			((Land)Model).Score = value;
			OnValueChanged(ref value, _onHightScoreChanged);
		}
	}

	protected override ModelBase Model
	{
		get { return _model ?? (_model = Repository.GetLandByID(_modelID)); }
	}
	protected override void OnStateLoad()
	{
		_model = Repository.GetLandByID(_modelID);
		HightScore = _model.Score;
		base.OnStateLoad();
	}

	[Serializable] public class HightScoreChanged : UnityEvent<int> { }
}