using UnityEngine;
using UnityEngine.UI;

public class ColorNegationValueConverter : ValueConverterBase<bool, Color>
{
	[SerializeField] private Color _positive;
	[SerializeField] private Color _negative;
	[SerializeField] private Graphic _target;

	public override void Conver(bool value)
	{
		Color color = value ? _positive : _negative;
		Value = color;
		_target.color = color;
	}
}
