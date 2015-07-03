using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }

	public void Localization(GameLang lang)
	{
		Language.SwitchLanguage(lang.ToString());
	}

	private void Awake()
	{
		#region Create singeltone
		if (Instance != null && Instance != this)
			Destroy(gameObject);
		Instance = this;
		DontDestroyOnLoad(gameObject);
		#endregion
	}

	public void Quit()
	{
		//#if UNITY_WEBPLAYER
		//		const string webplayerQuitURL = "http://google.com";
		//#endif
		
		//#if UNITY_EDITOR 
		//UnityEditor.EditorApplication.isPlaying = false;
		//#elif UNITY_WEBPLAYER
		//			Application.OpenURL(webplayerQuitURL);
		//#else
		StopAllCoroutines();
		Application.Quit();
		//#endif
	}
}
