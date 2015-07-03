using UnityEngine;

public abstract class ValueConverterBase<TFrom, TTo> : MonoBehaviour
{
	protected object Value;

	public virtual void Conver(TFrom value)
	{
		Value = default(TTo);
	}

	public virtual void ConverBack(TTo value)
	{
		Value = default(TFrom);
	}

	public virtual TTo ConvertedValue
	{
		get { return Value is TTo ? (TTo) Value : default(TTo); }
	}
}
