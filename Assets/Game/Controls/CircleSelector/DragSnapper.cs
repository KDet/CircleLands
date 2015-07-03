using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Snap a scroll rect to its children items. All self contained.
/// Note: Only supports 1 direction
/// </summary>
public class DragSnapper : UIBehaviour, IEndDragHandler, IBeginDragHandler
{
	[SerializeField] private ScrollRect _scrollRect; // the scroll rect to scroll
	[SerializeField] private SnapDirection _direction; // the direction we are scrolling
	[SerializeField] private int _itemCount; // how many items we have in our scroll rect
	[SerializeField] private AnimationCurve _curve = AnimationCurve.Linear(0f, 0f, 1f, 1f); // a curve for transitioning in order to give it a little bit of extra polish
	[SerializeField] private float _speed; // the speed in which we snap ( normalized position per second? )
	private int _itemIndex;
	[SerializeField] private DragSnapperIndexChangedEvent _indexChanged = new DragSnapperIndexChangedEvent();

	public int ItemIndex
	{
		get { return _itemIndex; }
		private set
		{
			_itemIndex = value;
			_indexChanged.Invoke(value);
		}
	}

	public void SetItemIndex(int index)
	{
		float delta = 1f / (_itemCount - 1); // percentage each item takes

		if (_direction == SnapDirection.Horizontal)
			_scrollRect.horizontalNormalizedPosition = delta * index;
		else
			_scrollRect.verticalNormalizedPosition = delta * index;
	}

//	protected override void Reset()
//	{
//		base.Reset();
//
//		if (_scrollRect == null) // if we are resetting or attaching our script, try and find a scroll rect for convenience 
//			_scrollRect = GetComponent<ScrollRect>();
//	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		StopCoroutine(SnapRect()); // if we are snapping, stop for the next input
	}
	public void OnEndDrag(PointerEventData eventData)
	{
		StartCoroutine(SnapRect()); // simply start our coroutine ( better than using update )
	}

	private IEnumerator SnapRect()
	{
		if (_scrollRect == null)
			throw new Exception("Scroll Rect can not be null");
		if (_itemCount == 0)
			throw new Exception("Item count can not be zero");

		float startNormal = _direction == SnapDirection.Horizontal
			? _scrollRect.horizontalNormalizedPosition
			: _scrollRect.verticalNormalizedPosition; // find our start position
		float delta = 1f/(_itemCount - 1); // percentage each item takes
		int target = Mathf.RoundToInt(startNormal/delta); // this finds us the closest target based on our starting point
		float endNormal = delta*target; // this finds the normalized value of our target
		float duration = Mathf.Abs((endNormal - startNormal)/_speed);
			// this calculates the time it takes based on our speed to get to our target

		float timer = 0f; // timer value of course
		while (timer < 1f) // loop until we are done
		{
			timer = Mathf.Min(1f, timer + Time.deltaTime/duration); // calculate our timer based on our speed
			float value = Mathf.Lerp(startNormal, endNormal, _curve.Evaluate(timer));
				// our value based on our animation curve, cause linear is lame

			if (_direction == SnapDirection.Horizontal) // depending on direction we set our horizontal or vertical position
				_scrollRect.horizontalNormalizedPosition = value;
			else
				_scrollRect.verticalNormalizedPosition = value;

			yield return new WaitForEndOfFrame(); // wait until next frame
		}
		ItemIndex = target;
	}
}

// The direction we are snapping in
public enum SnapDirection
{
	Horizontal,
	Vertical
}

[Serializable] public class DragSnapperIndexChangedEvent : UnityEvent<int> { }