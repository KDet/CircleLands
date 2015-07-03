using System;
using Models;
using UnityEngine;
using UnityEngine.Events;

public class GamePlayViewModel : ViewModelBase
{
	[SerializeField] private GamePlayState _state = GamePlayState.Start;
	[SerializeField] private int _loops;
	[SerializeField] private int _distanse;
	[SerializeField] private int _cash;

	[SerializeField] private bool _isClockwise = true;

	[SerializeField] private string _characterID;
	[SerializeField] private string _landID;

	[SerializeField] private DistanceChanged _onDistanceChanged = new DistanceChanged();
	[SerializeField] private CashChanged _onCashChanged = new CashChanged();
	[SerializeField] private GamePlayStateChanged _onGamePlayStateChanged = new GamePlayStateChanged();
	[SerializeField] private ClockwiseChanged _onClockwiseChanged = new ClockwiseChanged();
	[SerializeField] private LoopsChanged _onLoopsChanged = new LoopsChanged();
	[SerializeField] private ItemIDChanged _onCharacterSetUp = new ItemIDChanged();
	[SerializeField] private ItemIDChanged _onLandSetUp = new ItemIDChanged();

	private static void SaveBestScore(string landID, int score)
	{
		Land land = Repository.GetLandByID(landID);
		land.Score = Mathf.Max(score, land.Score);
		Repository.Save();
	}
//	public string CharacterID
//	{
//		get { return _characterID; }
//	}
//	public string LandID
//	{
//		get { return _landID; }
//	}
	public int Distanse
	{
		get { return _distanse; }
		set
		{
			_distanse = value;
			OnValueChanged(ref _distanse, _onDistanceChanged);
		}
	}
	public int Cash 
	{
		get { return _cash; }
		private set 
		{
			_cash = value;
			OnValueChanged(ref _cash, _onCashChanged);
			Repository.Save(RepositoryKey.Cash, _cash);
		}
	}
	public GamePlayState State
	{
		get { return _state; }
		private set
		{
			_state = value;
			OnValueChanged(ref _state, _onGamePlayStateChanged);
		}
	}
	public bool IsClockwise
	{
		get { return _isClockwise; }
		set
		{
			_isClockwise = value;
			OnValueChanged(ref _isClockwise, _onClockwiseChanged);
		}
	}
	public int Loops
	{
		get { return _loops; }
		private set
		{
			_loops = value;
			OnValueChanged(ref _loops, _onLoopsChanged);
		}
	}

	public string CharacterID
	{
		get { return _characterID; }
		private set
		{
			_characterID = value;
			OnValueChanged(ref _characterID, _onCharacterSetUp);
		}
	}
	public string LandID
	{
		get { return _landID; }
		private set
		{
			_landID = value;
			OnValueChanged(ref _landID, _onLandSetUp);
		}
	}

	public void RunGame()
	{
		State = GamePlayState.Running;
	}
	public void GameOver()
	{
		State = GamePlayState.GameOver;
	}
	public void PauseGame()
	{
		State = GamePlayState.Pause;
	}
	public void RestartGame()
	{
		State = GamePlayState.Start;
		Loops = 0;
		Distanse = 0;
		//RunGame();
	}
	public void IncrementLoop()
	{
		Loops++;
		if(Distanse > 1)
			Cash++;
	}
	public void ExtraCash(int cash = 10)
	{
		Cash += cash;
	}

	protected override void OnStateLoad()
	{
		base.OnStateLoad();

		CharacterID = GameStorage.UsedCharacterID;
		LandID = GameStorage.UsedLandID;

		Loops = 0;
		Cash = GameStorage.Cash;
		State = GamePlayState.Start;
		Distanse = 0;
	}
	protected override void OnStateSave()
	{
		Repository.Storage.Cash = Cash;
		SaveBestScore(_landID, _loops);
		base.OnStateSave();
	}

	[Serializable] public class DistanceChanged : UnityEvent<int> { }
	[Serializable] public class CashChanged : UnityEvent<int> { }
	[Serializable] public class GamePlayStateChanged : UnityEvent<GamePlayState> { }
	[Serializable] public class ClockwiseChanged : UnityEvent<bool> { }
	[Serializable] public class LoopsChanged : UnityEvent<int> { }
	[Serializable] public class ItemIDChanged : UnityEvent<string> { }
}
