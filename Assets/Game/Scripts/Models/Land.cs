using System;
using UnityEngine;

namespace Models
{
	[Serializable]
	public class Land : ModelBase
	{
		[SerializeField] private  string _description;
		[SerializeField] private  int _score;

		public string Description
		{
			get { return _description; }
		}
		public int Score
		{
			get { return _score; }
			set { _score = value; }
		}

		public Land(string id, string name, string description, int price, bool isBought, int score) : base(id, name, price, isBought)
		{
			_score = score;
			_description = description;
		}
		public Land(): this(null, null, null, 0,false,0) { }
	}
}