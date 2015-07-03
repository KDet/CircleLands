using UnityEngine;
using UnityEngine.UI;

public class SpikeView : ObstacleBaseView
{
	public override float AngleDeg 
	{
		get { return base.AngleDeg; }
		protected set 
		{
			UIRectTransform.localRotation = Quaternion.AngleAxis(value, Vector3.forward);
			//_lowerObstacle.rectTransform.localRotation = Quaternion.AngleAxis(value, Vector3.forward);
		}
	}
	public override void Init (float length, float angleDeg, bool isClockwise, bool isUpperBlock, float landRadius, Vector2 landCenter)
	{
		base.Init (length, angleDeg, isClockwise, isUpperBlock, landRadius, landCenter);

		for (int i = 0; i < _upperObstacle.rectTransform.childCount; i++)
			_upperObstacle.rectTransform.GetChild(i).GetComponent<Image>().fillAmount = length;
		
		for (int i = 0; i < _lowerObstacle.rectTransform.childCount; i++)
			_lowerObstacle.rectTransform.GetChild(i).GetComponent<Image>().fillAmount = length;
	}
}
