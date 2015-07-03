using UnityEngine;
using UnityEngine.UI;

public class DistanseValueConverter : ValueConverterBase<int, string>
{
	[SerializeField] private Text _destination;

	public override void Conver(int value)
	{
		string val = Convert(value);
		Value = val;
		_destination.text = val;
	}
	private static string Convert(int distance)
	{
		return string.Format("{0}", distance);
	}
}
