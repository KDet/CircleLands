using UnityEngine;

public class BlocksGenerator : ObstacleGeneratorBase
{
	[SerializeField] [Range(0, 180)] private float _minBlockLengthDeg = 5f;
	[SerializeField] [Range(0, 180)] private float _maxBlockLengthDeg = 35f;
	
	protected override float MinObstacleLengthDeg 
	{
		get { return _minBlockLengthDeg; }
	}
	protected override float MaxObstacleLengthDeg 
	{
		get { return _maxBlockLengthDeg; }
	}
	
	protected override ObstacleBaseView AddObstacle(bool isUpperBlock, float length, float angleDeg, bool isClockwise, float landRadius, Vector2 landCenter)
	{
		//RectTransform etalon = isUpperBlock ? _obstacleView.UIRectTransform : _obstacleLower.UIRectTransform;
		RectTransform instanse = Instantiate(_obstacleView.UIRectTransform);
		BlockView blockView = instanse.GetComponent<BlockView>();
		blockView.Init(length, angleDeg, isClockwise, isUpperBlock, landRadius,landCenter);
		
		instanse.SetParent(_root, false);
		return blockView;
	}

}