using UnityEngine;

public class AngleDistanceValueConverter : ValueConverterBase<float, int>
{
	[SerializeField] private GamePlayViewModel _destination;

	public override void Conver(float value)
	{
		int val = Convert(value);
		Value = val;
		_destination.Distanse = val;
	}

	private static int Convert(float angleRad)
	{
		var angleDeg = angleRad*Mathf.Rad2Deg;
		return Mathf.Abs(Mathf.FloorToInt((angleDeg - 90f) / 10f));
	}
}