using UnityEngine;
using UnityEngine.UI;

public class PriceValueConverter : ValueConverterBase<int, string>
{
	[SerializeField] private Text _destination;
	public override void Conver(int value)
	{
		string val = Convert(value);
		Value = val;
		_destination.text = val;
	}

	private static string Convert(int price)
	{
		return price <= 0 ? Language.Get("Free") : string.Format("{0} {1}", price, Language.Get("Flags"));
	}
}
