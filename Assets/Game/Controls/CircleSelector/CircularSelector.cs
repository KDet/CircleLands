using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CircularSelector : MonoBehaviour
{
	[SerializeField] [Range(0, 360)] private float _selectedItemDeg = 10f;
	[SerializeField] [Range(0, 360)] private float _itemsPositionDeg = 55f;
	[SerializeField] [Range(0, 360)] private float _itemsOffsetDeg = 30f;
	[SerializeField] private float _speed = 6f;
	
	[SerializeField] private RectTransform _center;
	[SerializeField] private float _borderOffset = 40f;
	[SerializeField] private bool _isClockwise = false;
	
	[SerializeField] private CircularSelectorItem[] _items;
	[SerializeField] private CircularSelectorItem _selectedItem;
	[SerializeField] private SelectedItemChanged _onSelectedItemChanged = new SelectedItemChanged();
	[SerializeField]private SelectedItemChanged _onSelectedItemChanging = new SelectedItemChanged();

	private bool _init;

	public void OnItemChanged(CircularSelectorItem circularSelectorItem)
	{
		if(!_init)
			return;
		string selectedItemID = _selectedItem.ID;
		if (!string.Equals(selectedItemID, circularSelectorItem.ID) && circularSelectorItem.IsSelected)
			if (!circularSelectorItem.IsAvailable)
			{
				if (!circularSelectorItem.IsMoving)
					StartCoroutine(MoveUnAvailableItem(circularSelectorItem));
			}
			else
			{
				int availableCount = CountAvailable();
				if (availableCount > 1)
					MoveItems(Items, circularSelectorItem, selectedItemID, availableCount);
			}
	}
	public void Init(CircularSelectorItem selected)
	{
		_init = false;
		//гарантує те що всі елементи посортовані по зростанню кута
		float angle = _itemsPositionDeg;
		if (_items != null)
			for (int i = 0; i < _items.Length; i++)
			{
				if (selected != null && string.Equals(_items[i].ID, selected.ID))
				{
					_items[i].MoveToAngle(_selectedItemDeg, _center, _borderOffset, _isClockwise);
					_items[i].IsSelected = true;
					_selectedItem = _items[i];
				}
				else
				{
					_items[i].MoveToAngle(angle, _center, _borderOffset, _isClockwise);
					_items[i].IsSelected = false;
					angle += _itemsOffsetDeg;
				}
			}
		_init = true;
	}

	private void MoveItems(CircularSelectorItem[] items, CircularSelectorItem circularSelectorItem, string selectedItemID, int availableCount)
	{
		if (!IsMoving(items))
			StartCoroutine(MoveItemsCoroutine(items, circularSelectorItem, selectedItemID, availableCount));
	}
	private IEnumerator MoveUnAvailableItem(CircularSelectorItem item)
	{
		var angle = item.AngleDeg;
		StartCoroutine(item.MoveToAngleCoroutine(angle - _itemsOffsetDeg/3f, 3*_speed, _center, _borderOffset, _isClockwise));
		while (item.IsMoving)
			yield return null;
		yield return StartCoroutine(item.MoveToAngleCoroutine(angle, _speed, _center, _borderOffset, _isClockwise));
	}
	private IEnumerator MoveItemsCoroutine(CircularSelectorItem[] items, CircularSelectorItem circularSelectorItem, string selectedItemID, int availableCount)
	{
		float[] angels = new float[items.Length];
		for (int i = 0; i < items.Length; i++)
			if (items[i].IsAvailable)
			{
				if (items[i].IsSelected)
					angels[i] = _selectedItemDeg;
				//CoroutineExtension.ParallelCoroutines(items[i].MoveToAngleCoroutine(_selectedItemDeg, _speed, _center, _borderOffset, _isClockwise), this);
				else if (items[i].ID == selectedItemID)
				{
					float angle = _itemsPositionDeg + (availableCount - 2)*_itemsOffsetDeg;
					angels[i] = angle;
					//CoroutineExtension.ParallelCoroutines(items[i].MoveToAngleCoroutine(angle, _speed, _center, _borderOffset, _isClockwise), this);
				}
				else if (items[i].AngleDeg > circularSelectorItem.AngleDeg)
				{
					float angle = Mathf.Clamp(items[i].AngleDeg - _itemsOffsetDeg, _itemsPositionDeg,
						_itemsPositionDeg + (availableCount - 2)*_itemsOffsetDeg);
					angels[i] = angle;
					//CoroutineExtension.ParallelCoroutines(items[i].MoveToAngleCoroutine(angle, _speed, _center, _borderOffset, _isClockwise), this);
				}
				else
					angels[i] = items[i].AngleDeg;
			}
			else
				angels[i] = items[i].AngleDeg;
		for (int i = 0; i < items.Length; i++)
			items[i].MoveToAngleCoroutine(angels[i], _speed, _center, _borderOffset, _isClockwise).ParallelCoroutines(this);
		
		_onSelectedItemChanging.Invoke(circularSelectorItem);
		
		while (CoroutineExtension.Processing())
			yield return null;
		SelectedItem = circularSelectorItem;
	}
	private static bool IsMoving(CircularSelectorItem[] items)
	{
		for (int i = 0; i < items.Length; i++)
			if (items[i].IsMoving)
				return true;
		return false;
	}
	private int CountAvailable()
	{
		int count = 0;
		if (Items != null && Items.Length != 0)
			for (int i = 0; i < Items.Length; i++)
				if (Items[i].IsAvailable)
					++count;
		return count;
	}

	public CircularSelectorItem SelectedItem
	{
		get { return _selectedItem; }
		set
		{
			_selectedItem = value;
			_onSelectedItemChanged.Invoke(value);
		}
	}
	public CircularSelectorItem[] Items
	{
		get { return _items; }
	}

	private void Start()
	{
		if (!_init)
			Init(_selectedItem);
	}
	private void OnDisable()
	{
		_init = false;
	}

	[Serializable] public class SelectedItemChanged : UnityEvent<CircularSelectorItem> { }
}
