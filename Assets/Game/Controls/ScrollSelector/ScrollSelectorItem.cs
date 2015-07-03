using UnityEngine;

public class ScrollSelectorItem : MonoBehaviour
{
	[SerializeField] private string _itemID;

	public string ItemID
	{
		get { return _itemID; }
	}
}
