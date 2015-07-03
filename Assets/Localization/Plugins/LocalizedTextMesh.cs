// Localization pacakge by Mike Hergaarden - M2H.nl
// DOCUMENTATION: http://www.m2h.nl/files/LocalizationPackage.pdf
// Thank you for buying this package!

//
// Thanks to Thom Denick for creating this addition!
//

using UnityEngine;

[RequireComponent(typeof(TextMesh))]

public class LocalizedTextMesh : MonoBehaviour
{
    public string keyValue;

    public void Awake()
    {
        LocalizeTextMesh(keyValue);
    }

    //Only overrides current text if a translation is available
    public void LocalizeTextMesh(string keyValue)
    {
        if (keyValue == null)
        {
            Debug.LogError("Please set the KeyValue that should be used for this TextMesh ("+name+")");
            return;
        }

        TextMesh textMesh = gameObject.GetComponent<TextMesh>();
        textMesh.text = Language.Get(keyValue);
    }

}