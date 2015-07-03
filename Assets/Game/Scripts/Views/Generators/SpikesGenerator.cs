using UnityEngine;

public class SpikesGenerator : ObstacleGeneratorBase 
{
	[SerializeField][Range(0f, 1f)] private float _minSpikeAngle = 0.25f;
	[SerializeField][Range(0f, 1f)] private float _maxSpikeAngle = 1f;
	[SerializeField] private float _landRadiusOffset = -8f;

	private float _minObstacleLengthDeg;
	protected override float MinObstacleLengthDeg 
	{

		get { return _minObstacleLengthDeg == 0.0f 
			? (_minObstacleLengthDeg = GamePhysics.LengthDeg(_obstacleView.UIRectTransform.sizeDelta.x, _landRadius)) 
			: _minObstacleLengthDeg; }
	}
	protected override float MaxObstacleLengthDeg 
	{
		get { return MinObstacleLengthDeg; }
	}
	
	protected override ObstacleBaseView AddObstacle(bool isUpperBlock, float length, float angleDeg, bool isClockwise, float landRadius, Vector2 landCenter)
	{
		//RectTransform etalon = isUpperBlock ? _obstacleView.UIRectTransform : _obstacleLower.UIRectTransform;
		RectTransform instanse = Instantiate(_obstacleView.UIRectTransform);
		//int dir = isClockwise ? -1: 1;
		SpikeView spikeView = instanse.GetComponent<SpikeView>();
		spikeView.Init(Random.Range(_minSpikeAngle, _maxSpikeAngle) , angleDeg, isClockwise, isUpperBlock, landRadius, landCenter);

		GamePhysics.MoveAround(instanse, _landRadius + _landRadiusOffset, Vector2.zero, (angleDeg + 90f) * Mathf.Deg2Rad , false);

		instanse.SetParent(_root, false);
		return spikeView;
	}
}