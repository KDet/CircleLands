using System;
using Soomla;
using Soomla.Profile;
using UnityEngine;
using UnityEngine.Events;

public class SocialHolder : MonoBehaviour 
{
	public enum SocialProvider
	{
		FACEBOOK,
		GOOGLE,
		TWITTER
	};

	private bool isInit;
	[SerializeField] private SocialProvider provider = SocialProvider.FACEBOOK;
	[SerializeField] private SocialFailed _onSocialFailed = new SocialFailed();
	//private Reward exampleReward = null;//new BadgeReward("example_reward", "Example Social Reward");

	private static Provider TargetProvider(SocialProvider type)
	{
		switch (type) 
		{
		case SocialProvider.FACEBOOK:
			return Provider.FACEBOOK;
		case SocialProvider.GOOGLE:
			return Provider.GOOGLE;
		case SocialProvider.TWITTER:
			return Provider.TWITTER;
		default:
			return null;
		}
	}
		
	public void UploadScreenShot()
	{
		Provider targetProvider = TargetProvider(provider);
		SmoomlaEnshuredAction(targetProvider, ()=> {
			ProfileEvents.OnSocialActionFailed += (provider1, type, arg3, arg4) =>  _onSocialFailed.Invoke();
			SoomlaProfile.UploadCurrentScreenShot(this, targetProvider, "Circle Lands Record!", "This is proof of my achievement in Circle Lands game.", null);
		});
	}

	private void SmoomlaEnshuredAction(Provider provider, Action action)
	{
		if(!isInit)
		{
			ProfileEvents.OnSoomlaProfileInitialized += () => {
				SoomlaUtils.LogDebug ("Social", "SoomlaProfile Initialized !");
				isInit = true;
				SmoomlaLoginEnshure(action, provider);
			};
			Initialize();
		}
		else
			SmoomlaLoginEnshure(action, provider);
	}
	private void SmoomlaLoginEnshure(Action action, Provider provider)
	{
		if(!SoomlaProfile.IsLoggedIn(provider))
		{
			ProfileEvents.OnLoginFinished += (user, i) => { 
				if(action != null)
					action(); 
			};
			ProfileEvents.OnLoginFailed += (provider1, error, userPayload) => _onSocialFailed.Invoke();
			SoomlaProfile.Login(provider, null, null);
		}
		else if(action != null)
			action();
	}	
	private static void Initialize()
	{
		SoomlaProfile.Initialize();
//		#if UNITY_IPHONE
//		Handheld.SetActivityIndicatorStyle(UnityEngine.iOS.ActivityIndicatorStyle.Gray);
//		#elif UNITY_ANDROID
//		Handheld.SetActivityIndicatorStyle(AndroidActivityIndicatorStyle.Small);
//		#endif
	}
	
	private void Start() 
	{
		ProfileEvents.OnSoomlaProfileInitialized += () =>
		{
			SoomlaUtils.LogDebug("Social", "SoomlaProfile Initialized !");
			isInit = true;
		};
		//ProfileEvents.OnLoginFailed += (provider1, error, userPayload) => _onSocialFailed.Invoke();
		Initialize();
	}

	[Serializable] public class SocialFailed : UnityEvent { }
}
