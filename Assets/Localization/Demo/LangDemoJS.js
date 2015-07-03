// Localization pacakge by Mike Hergaarden - M2H.nl
// DOCUMENTATION: http://www.m2h.nl/files/LocalizationPackage.pdf
// Thank you for buying this package!

/*

//TO USE THIS FILE:
//1: REMOVE THE COMMENTS (line 5 and 54)
//2: Move the PLUGINS folder to the root of your project. This will ensure Javascript can find the C# code in Language.cs


private  var scrollView : Vector2;

function OnGUI () : void {
    GUILayout.BeginArea(new Rect(0,0,Screen.width, Screen.height));
    
    GUILayout.FlexibleSpace();
    GUILayout.BeginHorizontal();
    GUILayout.FlexibleSpace();
    GUILayout.BeginVertical();

    GUILayout.Label( Language.Get("MAIN_WELCOME"), GUILayout.Width(300));
    GUILayout.Label( Language.Get("MAIN_INTRO"));
    GUILayout.Space(20);

    //An example of dynamic content inside your translations
     var progress : float= Mathf.Round((Time.time % 4) * 25);
    //specify you own 'variable' in your language. i.e.: {X}
    GUILayout.Label(   Language.Get("MAIN_PROGRESSBAR").Replace("{X}", progress+"")   );

    //Show language list
    GUILayout.Label( Language.Get("MAIN_SELECT_LANGUAGE"));
    scrollView = GUILayout.BeginScrollView(scrollView);
    for (var lang : String in Language.GetLanguages())
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

    //SHOW TEXT FROM A DIFFERENT SHEET:
    //GUILayout.Label(Language.Get("EXTRA", "Sheet2"));
}

*/
