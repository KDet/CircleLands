using UnityEngine;

public class WheelView : ObstacleBaseView 
{
	public override float AngleDeg 
	{
		get { return base.AngleDeg; }
		protected set 
		{
			UIRectTransform.rotation = Quaternion.AngleAxis(value, Vector3.forward);
//			_upperObstacle.rectTransform.localRotation = Quaternion.AngleAxis(value - 90f, Vector3.forward);
//			_lowerObstacle.rectTransform.localRotation = Quaternion.AngleAxis(value - 90f, Vector3.forward);
		}
	}
	public override float Length 
	{
		get 
		{
			return UIImage.rectTransform.sizeDelta.y;
		}
		protected set 
		{
			_upperObstacle.rectTransform.sizeDelta = new Vector2(value, value);
			_upperObstacle.rectTransform.position = new Vector3(0, value /2f, _upperObstacle.rectTransform.position.z);
			_lowerObstacle.rectTransform.sizeDelta = new Vector2(value, value);
			_lowerObstacle.rectTransform.position = new Vector3(0, - value /2f, _lowerObstacle.rectTransform.position.z);
		}
	}

	public override bool IsClockwise {
		get {
			return true;
		}
		protected set {
		}
	}

	public override void Init (float length, float angleDeg, bool isClockwise, bool isUpperBlock, float landRadius, Vector2 landCenter)
	{
		base.Init (length, angleDeg, isClockwise, isUpperBlock, landRadius, landCenter);

		for (int i = 0; i < _upperObstacle.rectTransform.childCount; i++)
		{
			var recTransform = _upperObstacle.rectTransform.GetChild(i) as RectTransform;
			if(recTransform != null)
				recTransform.sizeDelta = new Vector2(length, length);
		}
		for (int i = 0; i < _lowerObstacle.rectTransform.childCount; i++)
		{
			var recTransform = _lowerObstacle.rectTransform.GetChild(i) as RectTransform;
			if(recTransform != null)
				recTransform.sizeDelta = new Vector2(length, length);
		}

		SetColider(true, length/2f);
		SetColider(false, length/2f);
	}

	private void SetColider(bool isUpperBlock,  float radius)
	{
		CircleCollider2D collider = isUpperBlock ? _upperObstacle.GetComponent<CircleCollider2D>() : _lowerObstacle.GetComponent<CircleCollider2D>();
		collider.radius = radius;
	}
}
