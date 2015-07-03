using UnityEngine;

public class BoolNegationValueConverter : ValueConverterBase<bool, bool>
{
	[SerializeField] private GameObject[] _positive;
	[SerializeField] private GameObject[] _negative;

	public override void Conver(bool value)
	{
		Value = !value;

		if (_positive != null)
			for (int i = 0; i < _positive.Length; i++)
				if(_positive[i] != null)
					_positive[i].SetActive(value);
		if (_negative != null)
			for (int i = 0; i < _negative.Length; i++)
				if(_negative[i] != null)
					_negative[i].SetActive(!value);
	}

	public override void ConverBack(bool value)
	{
		Conver(value);
	}
}