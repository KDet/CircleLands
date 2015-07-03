// Localization pacakge by Mike Hergaarden - M2H.nl
// DOCUMENTATION: http://www.m2h.nl/files/LocalizationPackage.pdf
// Thank you for buying this package!

using UnityEngine;

public class LangDemo : MonoBehaviour {


    private Vector2 scrollView;
    public GUISkin mySkin;

	void OnGUI () {
        GUI.skin = mySkin;

        GUILayout.BeginArea(new Rect(0,0,Screen.width, Screen.height));
        
        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.BeginVertical();

        GUILayout.Label( Language.Get("MAIN_WELCOME"), GUILayout.Width(300));
        GUILayout.Label( Language.Get("MAIN_INTRO"));
        GUILayout.Space(20);

        //An example of dynamic content inside your translations
        float progress = (int)((Time.time % 4) * 25);
        //specify you own 'variable' in your language. i.e.: {X}
        GUILayout.Label(   Language.Get("MAIN_PROGRESSBAR").Replace("{X}", progress+"")   );

        //Show language list
        GUILayout.Label( Language.Get("MAIN_SELECT_LANGUAGE"));
        scrollView = GUILayout.BeginScrollView(scrollView, GUILayout.Height(100));
        foreach (string lang in Language.GetLanguages())
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            if(GUILayout.Button(lang, GUILayout.Width(50))){
                Language.SwitchLanguage(lang);
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();

        GUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();

        GUILayout.EndArea();



		GUILayout.Label( Language.Get("TEST_CHARACTERS"));
		           
        //SHOW TEXT FROM A DIFFERENT SHEET:
        //GUILayout.Label(Language.Get("EXTRA", "Sheet2"));
	}

    //You can add the following method to -any- of your scripts to get an event when language ahs changed
    void ChangedLanguage(LanguageCode code)
    {
        Debug.Log("DEMO We switched to: " + code);
        mySkin.font = (Font)Language.GetAsset("font");
	Debug.Log("DEMO Font: "+mySkin.font);
    }


}