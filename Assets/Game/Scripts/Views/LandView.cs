using UnityEngine;

public class LandView : ViewBase
{
	[SerializeField] private PlayerView _player;
	//[SerializeField] private RectTransform _land;
	//[SerializeField] 
	private bool _isClockwise = true;

	[SerializeField] private ObstacleGeneratorBase _currentObstacleGenerators;
	[SerializeField] private ObstacleGeneratorBase[] _obstacleGenerators;

	public bool IsClockwise
	{
		get { return _isClockwise; }
		set { _isClockwise = value; }
	}

	public void SetUpObstacleGenerator(string generatorName)
	{
		for (int i = 0; i < _obstacleGenerators.Length; i++)
		{
			ObstacleGeneratorBase generator = _obstacleGenerators[i];
			if (string.Equals(generator.name, generatorName))
			{
				_currentObstacleGenerators = generator;
				generator.gameObject.SetActive(true);
			}
			else
				generator.gameObject.SetActive(false);
		}
	}

	public void GenerateLoopItems(int loops)
	{
		_currentObstacleGenerators.Next(loops, _isClockwise);
	}
	public void SwipeObstacles()
	{
		_currentObstacleGenerators.Swipe();
	}
	public void ClearObstacles()
	{
		_currentObstacleGenerators.Clear();
	}
	public void ReverseObstacles()
	{
		//_currentObstacleGenerators.IsClockwise = !_currentObstacleGenerators.IsClockwise;
		//_land.FlagSide(_currentObstacleGenerators.IsClockwise);
	}

	private void SetUpGenerators()
	{
		//GameManager.Instance.NewGame();
		_currentObstacleGenerators.On(UIRectTransform, _player.ScaledHeigth/2f + _player.ScaledBorderOffset, _player.ScaledWidth);
	}
	private void ShutDownGenerators()
	{
		_currentObstacleGenerators.Off();
	}
	public void OnGamePlayeStateChanged(GamePlayState state)
	{
		switch (state)
		{
			case GamePlayState.Start:
			{
				ClearObstacles();
				SetUpGenerators();
				break;
			}
			case GamePlayState.Running:
			{
//				if (!_currentObstacleGenerators.IsTurnOn)
//					SetUpGenerators();
				break;
			}
			case GamePlayState.Pause:
			{
				break;
			}
			case GamePlayState.GameOver:
			{
				ShutDownGenerators();
				break;
			}
			//			default:
			//				throw new ArgumentOutOfRangeException("state");
		}
	}
}