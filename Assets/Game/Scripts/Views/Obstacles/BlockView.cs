using UnityEngine;
using UnityEngine.UI;

public class BlockView : ObstacleBaseView 
{
	[SerializeField][Range(0, 360)] private float _coliderDegStep = 7;

	public override void Init(float length, float angleDeg, bool isClockwise, bool isUpperBlock, float landRadius, Vector2 landCenter)
	{
		base.Init(length, angleDeg, isClockwise, isUpperBlock, landRadius,landCenter);

		for (int i = 0; i < _upperObstacle.rectTransform.childCount; i++)
			_upperObstacle.rectTransform.GetChild(i).GetComponent<Image>().fillAmount = length;

		for (int i = 0; i < _lowerObstacle.rectTransform.childCount; i++)
			_lowerObstacle.rectTransform.GetChild(i).GetComponent<Image>().fillAmount = length;

		SetColider(true, landRadius);
		SetColider(false, landRadius);
	}



	private void SetColider(bool isUpperBlock,  float landRadius)
	{
		PolygonCollider2D collider = isUpperBlock ? _upperObstacle.GetComponent<PolygonCollider2D>() : _lowerObstacle.GetComponent<PolygonCollider2D>();

		float offset = Mathf.Abs(_upperObstacle.rectTransform.sizeDelta.y - _lowerObstacle.rectTransform.sizeDelta.y) / 2f;
		float innerRadius = isUpperBlock ? landRadius : landRadius - offset; 
		float outerRadius = isUpperBlock ? landRadius + offset : landRadius;

		int direction = IsClockwise ? -1: 1;
		int pointPairsCount = Mathf.CeilToInt(Length * 360f / _coliderDegStep);
		int index = 0;
		Vector2[] points = new Vector2[Mathf.Max(4, pointPairsCount*2 + 1)];
		//List<Vector2> points = new List<Vector2>(pointPairsCount);
		points[index++] = new Vector2(0, innerRadius);
		points[index++] = new Vector2(0, outerRadius);
		//points.Add(new Vector2(0, innerRadius));
		//points.Add(new Vector2(0, outerRadius));
		for (int i = 1 ; i < pointPairsCount - 1; i++)
		{
			float angle = (90 + direction * _coliderDegStep * i) * Mathf.Deg2Rad;
			points[index++] = new Vector2(Mathf.Cos(angle)*outerRadius, Mathf.Sin(angle)*outerRadius);
			//points.Add(new Vector2(Mathf.Cos(angle)*outerRadius, Mathf.Sin(angle)*outerRadius));
		}
		float angleDeg = 90f + direction * Length * 360f;
		points[index++] = new Vector2(Mathf.Cos(angleDeg * Mathf.Deg2Rad)*outerRadius, Mathf.Sin(angleDeg * Mathf.Deg2Rad)*outerRadius);
		points[index++] = new Vector2(Mathf.Cos(angleDeg * Mathf.Deg2Rad)*innerRadius, Mathf.Sin(angleDeg * Mathf.Deg2Rad)*innerRadius);
//		points.Add(new Vector2(Mathf.Cos(angleDeg * Mathf.Deg2Rad)*outerRadius, Mathf.Sin(angleDeg * Mathf.Deg2Rad)*outerRadius));
//		points.Add(new Vector2(Mathf.Cos(angleDeg * Mathf.Deg2Rad)*innerRadius, Mathf.Sin(angleDeg * Mathf.Deg2Rad)*innerRadius));

		float rest = (Length * 360f - _coliderDegStep *(pointPairsCount - 1));
		for (int i = 0; i < pointPairsCount - 1; i++)
		{
			float angle = (rest + angleDeg - direction * _coliderDegStep * i) * Mathf.Deg2Rad;
			points[index++] = new Vector2(Mathf.Cos(angle)*innerRadius, Mathf.Sin(angle)*innerRadius);
			//points.Add(new Vector2(Mathf.Cos(angle)*innerRadius, Mathf.Sin(angle)*innerRadius));
		}

		collider.points = points;
		//col.points = points.ToArray();
	}
}