using UnityEngine;
using UnityEngine.Events;

public class AnimationEvents : MonoBehaviour {

	public void Disable()
	{
		gameObject.SetActive(false);
		_onDisable.Invoke();
	}
	public void Enable(GamePlayState condition)
	{
		gameObject.SetActive(condition == GamePlayState.Start);
	}
	public void Enable(int cash)
	{
		if(Repository.Storage.Cash != cash)
			gameObject.SetActive(true);
	}
	public void Enable()
	{
		gameObject.SetActive(true);
	}

	[SerializeField] private UnityEvent _onDisable = new UnityEvent();
}
