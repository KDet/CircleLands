using UnityEngine;

public class WheelsGenerator : ObstacleGeneratorBase 
{
	[SerializeField][Range(0f, 100f)] private float _minWheelSize = 50f;
	[SerializeField][Range(0f, 100f)] private float _maxWheelSize = 80f;
	[SerializeField] private float _landRadiusOffset = -8f;
	
	private float _minObstacleLengthDeg;
	protected override float MinObstacleLengthDeg 
	{
		get 
		{ 
			return _minObstacleLengthDeg == 0f 
			? (_minObstacleLengthDeg = GamePhysics.LengthDeg(_minWheelSize, _landRadius)) 
				: _minObstacleLengthDeg; 
		}
	}
	private float _maxObstacleLengthDeg;
	protected override float MaxObstacleLengthDeg 
	{
		get 
		{ 
			return _maxObstacleLengthDeg == 0f 
			? (_maxObstacleLengthDeg = GamePhysics.LengthDeg(_maxWheelSize, _landRadius)) 
				: _maxObstacleLengthDeg; 
		}
	}

	protected override ObstacleBaseView AddObstacle(bool isUpperBlock, float length, float angleDeg, bool isClockwise, float landRadius, Vector2 landCenter)
	{
		//RectTransform etalon = isUpperBlock ? _obstacleView.UIRectTransform : _obstacleLower.UIRectTransform;
		RectTransform instanse = Instantiate(_obstacleView.UIRectTransform);
		instanse.anchoredPosition = Vector2.zero;
		WheelView wheelView = instanse.GetComponent<WheelView>();

		float size = GamePhysics.DegLength(length * 360f, _landRadius);
		wheelView.Init(size, angleDeg, isClockwise, isUpperBlock, landRadius, landCenter);
		
		GamePhysics.MoveAround(instanse, _landRadius + _landRadiusOffset, Vector2.zero, (angleDeg + 90f) * Mathf.Deg2Rad, false);
		
		instanse.SetParent(_root, false);
		return wheelView;
	}
}
