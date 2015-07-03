using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewBase : MonoBehaviour 
{
	public RectTransform UIRectTransform 
	{
		get { return transform as RectTransform; }
	}
	
	//public static void Move
	//
	//	public virtual void Scale(float scaleFactor)
	//	{
	//		UIRectTransform.localScale *= scaleFactor;
	//	}
	//
	//
	//
		public float ScaledWidth
		{
			get { return UIRectTransform.localScale.x*UIRectTransform.sizeDelta.x; }
		}
		public float ScaledHeigth
		{
			get { return UIRectTransform.localScale.y * UIRectTransform.sizeDelta.y; }
		}
}

public struct CircleCoordinate
{
	[SerializeField]
	private readonly float _radius;
	[SerializeField]
	private readonly Vector2 _center;
	
	public float Radius
	{
		get { return _radius; }
	}
	public Vector2 Center
	{
		get { return _center; }
	}
	
	public CircleCoordinate(float radius, Vector2 center)
	: this()
	{
		_radius = radius;
		_center = center;
	}
}

public struct RadiusBounds
{
	private readonly float _inner;
	private readonly float _outer;

	public float Inner 
	{
		get { return _inner; }
	} 
	public float Outer 
	{
		get { return _outer; }
	}
	public RadiusBounds(float inner, float outer) : this()
	{
		_inner = inner;
		_outer = outer;
	}
	public static RadiusBounds Create(float floor, float offset)
	{
		return new RadiusBounds(floor - offset, floor + offset);
	}
}

public struct GamePhysics
{
	public static CircleCoordinate RectTransformToCoordinate(RectTransform center, float radiusOffset)
	{
		float radius = Mathf.Max(center.localScale.x * center.sizeDelta.x / 2f, center.localScale.y * center.sizeDelta.y / 2f) + radiusOffset;
		return new CircleCoordinate(radius, center.anchoredPosition);
	}

	public static float LengthDeg(float linerLength, float radius)
	{
		return LengthRad(linerLength, radius) * Mathf.Rad2Deg;
	}
	public static float LengthRad(float linerLength, float radius)
	{
		return 2f*Mathf.Atan(linerLength / (2f * radius));
	}

	public static float DegLength(float angleDeg, float radius)
	{
		float angleRad = angleDeg * Mathf.Deg2Rad;
		return RadLength(angleRad, radius);
	}
	public static float RadLength(float angleRad, float radius)
	{
		return 2f * radius * Mathf.Tan(angleRad / 2f);
	}
	
	public static void MoveAround(RectTransform obj, float radius, Vector2 center, float angleRad, bool rotate)
	{
		float x = Mathf.Cos(angleRad) * radius + center.x;
		float y = Mathf.Sin(angleRad) * radius + center.y;
		if (rotate)
			obj.localRotation = Quaternion.AngleAxis(angleRad * Mathf.Rad2Deg - 90f, Vector3.forward);
		obj.anchoredPosition = new Vector2(x, y);
	}
	public static void MoveAround(RectTransform obj, ref CircleCoordinate coordinate, float angleRad, bool rotate)
	{
		MoveAround(obj, coordinate.Radius, coordinate.Center, angleRad, rotate);
	}
	public static void MoveAround(RectTransform obj, RectTransform center, float angleRad, float offset, bool rotate)
	{
		if (obj != null)
		{
			float radius = Mathf.Max(center.localScale.x*center.sizeDelta.x/2f, center.localScale.y*center.sizeDelta.y/2f) + offset;
			MoveAround(obj, radius, center.anchoredPosition, angleRad, rotate);
		}
		else
			Debug.LogError("GamePhysics.MoveAround obj == null");
	}
	public static float ChangeAngleRadAround(float currentAngleRad, float linearSpeed, float radius, bool isClockwise)
	{
		int direction = isClockwise ? -1 : 1;
		return currentAngleRad + direction * (linearSpeed * 2f * Mathf.PI / radius) * Time.deltaTime;
	}
	public static float ChangeAngleRadAround(RectTransform center, float currentAngleRad, float linearSpeed, float offset, bool isClockwise)
	{
		float radius = Mathf.Max(center.localScale.x * center.sizeDelta.x / 2f, center.localScale.y * center.sizeDelta.y / 2f) + offset;
		return ChangeAngleRadAround(currentAngleRad, linearSpeed, radius, isClockwise);
	}
}

public static class CoroutineExtension
{
	// для отслеживания используем словарь <название группы, количество работающих корутинов>
	static private readonly Dictionary<string, int> Runners = new Dictionary<string, int>();
	const string DefaultGroupName = "default";
	
	// MonoBehaviour нам нужен для запуска корутина в контексте вызывающего класса
	public static void ParallelCoroutinesGroup(this IEnumerator coroutine, MonoBehaviour parent, string groupName)
	{
		if (!Runners.ContainsKey(groupName))
			Runners.Add(groupName, 0);
		
		Runners[groupName]++;
		parent.StartCoroutine(DoParallel(coroutine, parent, groupName));
	}
	// MonoBehaviour нам нужен для запуска корутина в контексте вызывающего класса
	public static void ParallelCoroutines(this IEnumerator coroutine, MonoBehaviour parent)
	{
		ParallelCoroutinesGroup(coroutine, parent, DefaultGroupName);
	}
	
	static IEnumerator DoParallel(IEnumerator coroutine, MonoBehaviour parent, string groupName)
	{
		yield return parent.StartCoroutine(coroutine);
		Runners[groupName]--;
	}
	
	// эту функцию используем, что бы узнать, есть ли в группе незавершенные корутины
	public static bool GroupProcessing(string groupName)
	{
		return (Runners.ContainsKey(groupName) && Runners[groupName] > 0);
	}
	// эту функцию используем, что бы узнать, есть ли в группе незавершенные корутины
	public static bool Processing()
	{
		return GroupProcessing(DefaultGroupName);
	}
}
