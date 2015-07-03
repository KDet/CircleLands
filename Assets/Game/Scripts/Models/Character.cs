using System;
using UnityEngine;

namespace Models
{
	[Serializable]
	public class Character : ModelBase
	{
		[SerializeField] private  string _description;

		public string Description
		{
			get { return _description; }
		}
		
//		public string PriceConvert()
//		{
//			return _price == 0 ? "Free" : _price.ToString();
//		}

		public Character(string id, string name, string description, int price, bool isBought) : base(id, name, price, isBought)
		{
			_description = description;
		}
		public Character(): this(null, null, null, 0, false) { }
	}

}