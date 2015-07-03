using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Events;

public class MonetizationHolder : MonoBehaviour 
{
	[SerializeField] private bool _testMode = true;

	[SerializeField] private string _rewardedVideoZoneID = "rewardedVideoZone";
	[SerializeField] private string _defaultVideoZoneID = "defaultZones";

	[SerializeField] private string _androidGameID = "45985";
	[SerializeField] private string _iOSGameID = "45984";

	[SerializeField] private AdvertisementFinished _gameContinueAdFinished = new AdvertisementFinished();
	[SerializeField] private AdvertisementFinished _extraFlagsAdFinished = new AdvertisementFinished();
	[SerializeField] private AdvertisementFailed _advertisementFailed = new AdvertisementFailed();
	
	private void Awake()
	{
		Advertisement.Initialize(AppID, _testMode);
	}
	private string AppID
	{
		get 
		{
#if UNITY_IOS
			return _iOSGameID;
#elif UNITY_ANDROID
			return _androidGameID;
#elif !UNITY_IOS && !UNITY_ANDROID
			return string.Empty;
#endif
		}
	}


	public void ShowAdvertisement(string zone, AdReward reward)
	{
		if(!Advertisement.isInitialized)
		{
			Advertisement.Initialize(AppID, _testMode);
			StartCoroutine(ShowAdWhenReady(zone, reward));
		}
		ShowAd(zone, reward);

	}
	public void ShowAdvertisement(AdReward reward)
	{
		ShowAdvertisement(_rewardedVideoZoneID, reward);
	}
	public void ShowAdvertisement(int reward)
	{
		if (reward == (int)AdReward.Flags || reward == (int)AdReward.GameContinue)
			ShowAdvertisement(_rewardedVideoZoneID, (AdReward)reward);
	}
	private void ShowAd(string zone, AdReward reward)
	{
		#if UNITY_EDITOR
		StartCoroutine(WaitForAd ());
		#endif
		
		zone = string.IsNullOrEmpty(zone) ? null : zone;
		
		ShowOptions options = new ShowOptions ();
		options.resultCallback = result =>
		{
			switch (result)
			{
				case ShowResult.Finished:
					Debug.Log("Advertisement Finished");
					switch (reward)
					{
						case AdReward.GameContinue:
							_gameContinueAdFinished.Invoke("Advertisement Finished", reward);
							break;
						case AdReward.Flags:
							_extraFlagsAdFinished.Invoke("Advertisement Finished", reward);
							break;
					}
					break;
				case ShowResult.Skipped:
					Debug.Log("Advertisement Skipped");
					break;
				case ShowResult.Failed:
					_advertisementFailed.Invoke();
					Debug.Log("Advertisement Failed");
					break;
			}
		};
		
		if (Advertisement.isReady (zone))
			Advertisement.Show (zone, options);
	}

	IEnumerator ShowAdWhenReady(string zone, AdReward reward)
	{
		while (!Advertisement.isReady())
			yield return null;

		ShowAd(zone, reward);
	}
	
	private static IEnumerator WaitForAd()
	{
		float currentTimeScale = Time.timeScale;
		Time.timeScale = 0f;
		yield return null;
		
		while (Advertisement.isShowing)
			yield return null;
		
		Time.timeScale = currentTimeScale;
	}

	[Serializable] public class AdvertisementFinished : UnityEvent<string, AdReward> { }
	[Serializable] public class AdvertisementFailed : UnityEvent { }

	[Serializable]
	public enum AdReward
	{
		None = 0,
		GameContinue,
		Flags
	}
}