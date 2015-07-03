using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class ObstacleGeneratorBase : MonoBehaviour, IDisposable 
{
	[SerializeField] protected ObstacleBaseView _obstacleView;
	[SerializeField] protected Transform _root;
	
	private List<ObstacleBaseView> _obstacles = new List<ObstacleBaseView>();
	private int _minFreeSpaceDeg;
	private int _availableTotalSpace;
	private int _minSingleDeg;
	private int _minOpositePairDeg;

	protected float _landRadius;
	protected Vector2 _landCenter;
	
	private bool _isTurnOn;

	public List<ObstacleBaseView> Obstacles
	{
		get { return _obstacles; }
		protected set { _obstacles = value; }
	}

	protected abstract float MinObstacleLengthDeg { get; }
	protected abstract float MaxObstacleLengthDeg { get; }

	public bool IsTurnOn
	{
		get { return _isTurnOn; }
	}

	private int MaxBlockCount(int space, int minSingle, int minOp)
	{
		int pairs = space/minOp;
		int extra = space%minOp != 0 ? (space - minOp*pairs)/minSingle : 0;
		return 2*pairs + extra;
	}
	
	public virtual void Clear()
	{
		Obstacles.Clear();
		for (int i = _root.transform.childCount - 1; i >= 0; i--)
			Destroy(_root.transform.GetChild(i).gameObject);
	}
	public virtual void Next(int blockCount, bool isClockwise)
	{
		if (!IsTurnOn)
			return;
		
		Clear();
		
		blockCount = Mathf.Clamp(blockCount, 1, MaxBlockCount(_availableTotalSpace, _minSingleDeg, _minOpositePairDeg));
		int pairsCount = blockCount/2;
		int singles = blockCount%2 == 0 ? 0 : 1;
		int angleDirection = isClockwise ? -1 : 1;
		float currentAngle = angleDirection*_minFreeSpaceDeg;
		if (pairsCount > 0)
		{
			float maxSpaceAngle = Mathf.Max((_availableTotalSpace - singles * _minSingleDeg) / pairsCount, _minOpositePairDeg);
			for (int i = 0; i < pairsCount; i++)
			{
				float spaceDeg = Random.Range(_minOpositePairDeg, maxSpaceAngle);
				float angle = Mathf.Max(Random.Range(Mathf.Abs(currentAngle), Mathf.Abs(currentAngle)+ maxSpaceAngle - spaceDeg), Mathf.Abs(currentAngle));
				GenerateOpositePair(_minFreeSpaceDeg, angleDirection * angle, spaceDeg, isClockwise, true, _landRadius);
				currentAngle += angleDirection * maxSpaceAngle;
			}
		}
		if (singles > 0 && blockCount == 1)
		{
			int availableTotalSpace = blockCount == 1 ? FirstBlockMaxAngle : _availableTotalSpace;
			//currentAngle = lastObstacle == null ? currentAngle : lastObstacle.AngleDeg + lastObstacle.Length*360f + _minFreeSpaceDeg;
//			if (singles > 0)
//			{
//				var spaceDeg = Mathf.Clamp(availableTotalSpace - Mathf.Abs(currentAngle), _minFreeSpaceDeg, MaxObstacleLengthDeg);
//				if (Mathf.Abs(spaceDeg) + Mathf.Abs(currentAngle) <= _availableTotalSpace)
//					GenerateSingle(_minFreeSpaceDeg, currentAngle, spaceDeg, isClockwise, true, _landRadius);
//			}
			
			float minAngle = Mathf.Abs(currentAngle);
			float maxAngle = availableTotalSpace - _minFreeSpaceDeg;
			if (minAngle <= maxAngle)
			{
				float angle = Random.Range(minAngle, maxAngle);
				float lengthSpaceDeg = Mathf.Min(Mathf.Max(Random.Range(_minFreeSpaceDeg, availableTotalSpace - Mathf.Abs(currentAngle)), _minFreeSpaceDeg), MaxObstacleLengthDeg);
				GenerateSingle(_minFreeSpaceDeg, angleDirection*angle, lengthSpaceDeg, isClockwise, true, _landRadius);
//				var spaceDeg = Mathf.Clamp(availableTotalSpace - Mathf.Abs(currentAngle), _minFreeSpaceDeg, MaxObstacleLengthDeg);
//				if (Mathf.Abs(spaceDeg) + Mathf.Abs(angle) <= _availableTotalSpace)
//					GenerateSingle(_minFreeSpaceDeg, angle, spaceDeg, isClockwise, true, _landRadius);
				//}
			}
		}
	}

	private const int FirstBlockMaxAngle = 360 - 2 * 35;

	private void GenerateOpositePair(int minFreeSpaceDeg, float currentAngle, float spaceDeg, bool clockwise, bool isUpperBlockFirst, float landRadius)
	{
		int angleDirection = clockwise ? -1 : 1;
		float maxFreeSpaceDeg = spaceDeg - minFreeSpaceDeg - 2*MinObstacleLengthDeg;
		float angle = Random.Range(minFreeSpaceDeg, maxFreeSpaceDeg);
		float maxLengthDeg = spaceDeg - angle - minFreeSpaceDeg - MinObstacleLengthDeg;
		float lengthDeg = Mathf.Clamp(Random.Range(MinObstacleLengthDeg, maxLengthDeg), MinObstacleLengthDeg, MinObstacleLengthDeg);
		ObstacleBaseView block = AddObstacle(isUpperBlockFirst,
		                           lengthDeg/360f,
		                           currentAngle + angleDirection*angle,
		                           clockwise,
		                           landRadius,
		                           _landCenter);
		Obstacles.Add(block);
		float nextMaxFreeSpace = spaceDeg - angle - lengthDeg - MinObstacleLengthDeg;
		// (spaceDeg - 2 * _minFreeSpaceDeg - 2 * minBlockLength) / 2f;
		float nextAngle = angle + lengthDeg + Random.Range(minFreeSpaceDeg, nextMaxFreeSpace);
		float nextMaxLengthDeg = spaceDeg - nextAngle;
		block = AddObstacle(!isUpperBlockFirst,
		                 Mathf.Clamp(nextMaxLengthDeg, MinObstacleLengthDeg, MaxObstacleLengthDeg)/360f,
		                 currentAngle + angleDirection*nextAngle,
		                 clockwise,
		                 landRadius,
		                 _landCenter);
		Obstacles.Add(block);
	}
	private void GenerateSingle(int minFreeSpaceDeg, float currentAngle, float spaceDeg, bool clockwise, bool isUpperBlockFirst, float landRadius)
	{
		int angleDirection = clockwise ? -1 : 1;
		float maxFreeSpaceDeg = spaceDeg - MinObstacleLengthDeg;
		float angle = Random.Range(minFreeSpaceDeg, maxFreeSpaceDeg);
		float maxLengthDeg = spaceDeg - angle;
		float lengthDeg = Mathf.Clamp(Random.Range(MinObstacleLengthDeg, maxLengthDeg), MinObstacleLengthDeg, MaxObstacleLengthDeg);
		var block = AddObstacle(isUpperBlockFirst,
		                     Mathf.Clamp(lengthDeg, MinObstacleLengthDeg, MaxObstacleLengthDeg)/360f,
		                     currentAngle + angleDirection*angle,
		                     clockwise,
		                     landRadius,
		                     _landCenter);
		Obstacles.Add(block);
	}

	protected abstract ObstacleBaseView AddObstacle(bool isUpperBlock, float length, float angleDeg, bool isClockwise, float landRadius, Vector2 landCenter);

	public virtual void Off()
	{
		_isTurnOn = false;
		Obstacles.Clear();
		Clear();
	}
	public virtual void On(RectTransform land, float offset, float playerWidth)
	{
		_landRadius = (land.localScale.y * land.sizeDelta.y) / 2f;
		_landCenter = land.anchoredPosition;
		_minFreeSpaceDeg = Mathf.CeilToInt(GamePhysics.LengthDeg(playerWidth, _landRadius + offset));
		//2f*Mathf.Atan(playerWidth / (2f * _landRadius)) * Mathf.Rad2Deg
		_availableTotalSpace = 360 - 2*_minFreeSpaceDeg;
		_minSingleDeg = Mathf.CeilToInt(_minFreeSpaceDeg + MinObstacleLengthDeg);
		_minOpositePairDeg = _minSingleDeg*2;
		Obstacles.Clear();
		_isTurnOn = true;
	}

	public void Swipe()
	{
		if (_obstacles != null)
			for (int i = 0; i < _obstacles.Count; i++)
				_obstacles[i].SwipeBlockToLandPosition();
	}

	#region IDisposable implementation
	void IDisposable.Dispose()
	{
		Off();
	}
	#endregion
}
