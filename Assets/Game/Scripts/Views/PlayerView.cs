using System;
using UnityEngine;
using UnityEngine.Events;

public class PlayerView : ViewBase
{
	[Serializable]
	private struct PlayerSpeedometer  : IDisposable
	{
		[SerializeField] private float _minLinearSpeed;
		[SerializeField] private float _maxLinearSpeed;
		[SerializeField] private float _startLinearSpeed;
		[SerializeField][Range(1,10)] private int _gear;

		private float _linearSpeed;

		private float _endLinearSpeed;
		private bool _isSmoothing;
		private bool _isTurnOn;
		
		public float LinearSpeed 
		{
			get { return _linearSpeed; }
		}

		public bool IsTurnOn
		{
			get { return _isTurnOn; }
		}

		//	public float Speed 
		//	{
		//		get { return _linearSpeed * 2f * Mathf.PI * _scaleFactor / _radius;; }
		//	}
		
		public void Working() 
		{
			if(IsTurnOn)
			{
				if(_isSmoothing) 
				{
					if(_linearSpeed != _endLinearSpeed) {
						float lerp = Mathf.Lerp(_linearSpeed, _endLinearSpeed, _gear*Time.deltaTime);
						_linearSpeed = Mathf.Clamp(lerp, _minLinearSpeed, _maxLinearSpeed);
						//_speed = _linearSpeed * 2f * Mathf.PI * _scaleFactor / _radius;
					}
					else 
						_isSmoothing = false;
				}
			}
		}
		public void On()
		{
			Reset();
			_isTurnOn = true;
		}
		public void Off()
		{
			_isTurnOn = false;
			Reset();
		}
		public void Reset()
		{
			_isSmoothing = false;
			_linearSpeed = _startLinearSpeed = Mathf.Clamp(_startLinearSpeed, _minLinearSpeed, _maxLinearSpeed);
			//_speed = _linearSpeed * 2f * Mathf.PI * _scaleFactor / _radius;
		}
		
		public void ChangeSpeedAt(float linearSpeedOffset, bool isSmoothlyChanging)
		{
			ChangeSpeed(_linearSpeed + linearSpeedOffset, isSmoothlyChanging);
		}
		public void ChangeSpeedTo(float linearSpeed, bool isSmoothlyChanging)
		{
			ChangeSpeed(linearSpeed, isSmoothlyChanging);
		}

		private void ChangeSpeed(float speed, bool isSmoothlyChanging)
		{
			if(!IsTurnOn)
				return;
			_isSmoothing = isSmoothlyChanging;
			if(isSmoothlyChanging)
			{
				_endLinearSpeed = speed;
			}
			else
			{
				_linearSpeed = Mathf.Clamp(speed, _minLinearSpeed, _maxLinearSpeed);
				_endLinearSpeed = _linearSpeed;
			}
		}
		
		#region IDisposable implementation
		void IDisposable.Dispose ()
		{
			Off();
		}
		#endregion
	}
	private const float PlayerStartPosition = 0.5f * Mathf.PI + 30 * Mathf.Deg2Rad;

	[SerializeField] private RectTransform _land;
	[SerializeField] private float _borderOffset = 10f;

	[SerializeField] private bool _isClockwise = true;
	[SerializeField] private PlayerSpeedometer _speedometer = new PlayerSpeedometer();
	
	[SerializeField] private AngleChanged _onAngleChanged = new AngleChanged();
	[SerializeField] private UserTiped _onUserTiped = new UserTiped();
	[SerializeField] private Collisioned _onBlockCollision = new Collisioned();
	[SerializeField] private Triggered _onFlagTrigger = new Triggered();

	private float _angleRad = PlayerStartPosition;
	public float AngleRad
	{
		get { return _angleRad; }
		private set
		{
			_angleRad = value;
			_onAngleChanged.Invoke(_angleRad);
		}
	}

	public float ScaledBorderOffset
	{
		get { return _borderOffset * UIRectTransform.localScale.y; }
	}
	public bool IsClockwise
	{
		get { return _isClockwise; }
		set { _isClockwise = value; }
	}
	
	private void OnCollisionEnter2D(Collision2D coll)
	{
		_onBlockCollision.Invoke();
	}
	private void OnTriggerEnter2D(Collider2D other)
	{
		ChangeSpeedAt(1, true);
		_onFlagTrigger.Invoke();
	}

	private void ToPlayerStartPosition()
	{
		AngleRad = _isClockwise ? PlayerStartPosition : -PlayerStartPosition;
		Move(AngleRad);
	}
	private void MovePlayer()
	{
		_speedometer.Working();
		AngleRad = GamePhysics.ChangeAngleRadAround(_land, AngleRad, _speedometer.LinearSpeed, _borderOffset, _isClockwise);
		Move(AngleRad);
	}

	private bool _running;
	private void Update()
	{
		if (_running)
		{
			if (Input.GetMouseButtonDown(0))
				_onUserTiped.Invoke();
			MovePlayer();
		}
	}
//	private IEnumerator OnGameRuning()
//	{
////		while (true)
////		{
////			if (Input.GetMouseButtonDown(0))
////				_onUserTiped.Invoke();
////			MovePlayer();
////			yield return new WaitForEndOfFrame();
////		}
////		
////		//		if (Mathf.FloorToInt(AngleRad * Mathf.Rad2Deg) % 10 == 0)
////		//			GameManager.Instance.Distanse++;
//		yield return null;
//	}
	private void Move(float angleRad)
	{
		GamePhysics.MoveAround(UIRectTransform, _land, angleRad, UIRectTransform.sizeDelta.y / 2f + _borderOffset, true);
	}
	private void ChangeSpeedAt(float linearSpeedOffset, bool isSmoothlyChanging)
	{
		_speedometer.ChangeSpeedAt(linearSpeedOffset, isSmoothlyChanging);
	}
	private void ChangeSpeedTo(float linearSpeedOffset, bool isSmoothlyChanging)
	{
		_speedometer.ChangeSpeedTo(linearSpeedOffset, isSmoothlyChanging);
	}

	public void OnGamePlayeStateChanged(GamePlayState state)
	{
		switch (state)
		{
			case GamePlayState.Start:
				_speedometer.On();
				_running = false;
				//StopCoroutine(OnGameRuning());
				ToPlayerStartPosition();
				break;
			case GamePlayState.Running:
				if(!_speedometer.IsTurnOn)
					_speedometer.On();
				//StartCoroutine(OnGameRuning());
				_running = true;
				break;
			case GamePlayState.GameOver:
				_speedometer.Off();
				_running = false;
				//StopCoroutine(OnGameRuning());
				break;
			case GamePlayState.Pause:
				//StopCoroutine(OnGameRuning());
				_running = false;
				break;
			default:
				//StopCoroutine(OnGameRuning());
				break;
		}
	}

	[Serializable] public class AngleChanged : UnityEvent<float> { }
	[Serializable] public class UserTiped : UnityEvent { }
	[Serializable] public class Collisioned : UnityEvent { }
	[Serializable] public class Triggered : UnityEvent { }
}
