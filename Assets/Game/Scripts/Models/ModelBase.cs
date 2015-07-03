using System;
using UnityEngine;

namespace Models
{
	[Serializable]
	public abstract class ModelBase
	{
		[SerializeField] private string _ID;
		[SerializeField] private string _name;
		[SerializeField] private int _price;
		[SerializeField] private bool _isBought;

		public string ID
		{
			get { return _ID; }
		}
		public string Name
		{
			get { return _name; }
		}
		public int Price
		{
			get { return _price; }
			set { _price = value; }
		}

		public bool IsBought
		{
			get { return _isBought; }
			set { _isBought = value; }
		}

		protected ModelBase(string id, string name, int price, bool isBought)
		{
			_ID = id;
			_name = name;
			_price = price;
			_isBought = isBought;
		}

		public override string ToString()
		{
			return _ID;
		}
	}

	//	[Serializable]
	//	public class Tutorial
	//	{
	//		[SerializeField] private readonly string _tutorialID;
	//		[SerializeField] private Dictionary<string, string> _tooltips;
	//
	//		public Dictionary<string, string> Tooltips
	//		{
	//			get { return _tooltips; }
	//		}
	//
	//		public string TutorialID
	//		{
	//			get { return _tutorialID; }
	//		}
	//
	//	}
}