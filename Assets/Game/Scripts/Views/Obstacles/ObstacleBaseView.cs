using UnityEngine;
using UnityEngine.UI;

public abstract class ObstacleBaseView : ViewBase 
{
	[SerializeField] protected Image _upperObstacle;
	[SerializeField] protected Image _lowerObstacle;

	private Image _image;
	
	protected bool _isUpperBlock;
	protected float _landRadius;
	protected Vector2 _landCenter;
	
	public Image UIImage 
	{
		get { return _isUpperBlock ? _upperObstacle : _lowerObstacle; }
	}
	public virtual float AngleDeg 
	{
		get { return UIImage.rectTransform.localRotation.eulerAngles.z; }
		protected set 
		{
			_upperObstacle.rectTransform.localRotation = Quaternion.AngleAxis(value, Vector3.forward);
			_lowerObstacle.rectTransform.localRotation = Quaternion.AngleAxis(value, Vector3.forward);
		}
	}
	public virtual float Length 
	{
		get { return UIImage.fillAmount; }
		protected set 
		{ 
			_upperObstacle.fillAmount = value;
			_lowerObstacle.fillAmount = value;
		}
	}
	public virtual bool IsClockwise 
	{
		get { return UIImage.fillClockwise; }
		protected set 
		{ 
			_upperObstacle.fillClockwise = value; 
			_lowerObstacle.fillClockwise = value; 
		}
	}
	
	public bool IsUpperBlock
	{
		get { return _isUpperBlock; }
	}

	public virtual void Init(float length, float angleDeg, bool isClockwise, bool isUpperBlock, float landRadius, Vector2 landCenter)
	{
		Length = length;
		AngleDeg = angleDeg;
		IsClockwise = isClockwise;
		_isUpperBlock = isUpperBlock;
		_landRadius = landRadius;
		_landCenter = landCenter;

		SpikeImageSwitch(isUpperBlock);
	}
	public virtual void SwipeBlockToLandPosition()
	{
		_isUpperBlock = !_isUpperBlock;
		SpikeImageSwitch(_isUpperBlock);
	}
	
	protected void SpikeImageSwitch(bool isUpper)
	{
		_upperObstacle.gameObject.SetActive(isUpper);
		_lowerObstacle.gameObject.SetActive(!isUpper);
	}
}
