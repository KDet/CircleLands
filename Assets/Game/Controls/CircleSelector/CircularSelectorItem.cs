using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof (Toggle))]
public class CircularSelectorItem : OpenCloseToggle
{
	[SerializeField] private string _itemID;

	private float _angleDeg;
	private bool _isMoving;
	
	public string ID
	{
		get { return _itemID; }
	}
	public float AngleDeg
	{
		get { return _angleDeg; }
	}
	public bool IsSelected
	{
		get { return UIToggle.isOn; }
		set
		{
//			ToggleGroup group = UIToggle.group;
//			if (group)
//				group.SetAllTogglesOff();
			UIToggle.isOn = value;
		}
	}
	public bool IsMoving
	{
		get { return _isMoving; }
	}

	public IEnumerator MoveToAngleCoroutine(float angleDeg, float speed, RectTransform land, float borderOffset, bool isClockwise)
	{
		_isMoving = true;
		// делаем переход от текущей позиции к новой
		while (Math.Abs(_angleDeg - angleDeg) > float.Epsilon)
		{
			// тут меняем текущую позицию с учетом скорости и прошедшего с последнего фрейма времени
			// и ждем следующего фрейма
			_angleDeg = Mathf.Lerp(_angleDeg, angleDeg, speed*Time.deltaTime);
			_angleDeg = Mathf.Abs(angleDeg - _angleDeg) <= speed*Time.deltaTime ? angleDeg : _angleDeg;
			GamePhysics.MoveAround(transform as RectTransform, land, ((isClockwise ? -1 : 1)*_angleDeg + 90f)*Mathf.Deg2Rad,
				borderOffset, true);
			yield return null;
		}
		_isMoving = false;

	}
	public void MoveToAngle(float angleDeg, RectTransform land, float borderOffset, bool isClockwise)
	{
		GamePhysics.MoveAround(transform as RectTransform, land, ((isClockwise ? -1 : 1)*angleDeg + 90f)*Mathf.Deg2Rad, borderOffset, true);
		_angleDeg = angleDeg;
	}
}