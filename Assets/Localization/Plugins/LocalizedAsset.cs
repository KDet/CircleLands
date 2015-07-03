// Localization pacakge by Mike Hergaarden - M2H.nl
// DOCUMENTATION: http://www.m2h.nl/files/LocalizationPackage.pdf
// Thank you for buying this package!

using UnityEngine;

public class LocalizedAsset : MonoBehaviour
{
    public Object localizeTarget;

    public void Awake()
    {
        LocalizeAsset(localizeTarget);
    }

    public void LocalizeAsset()
    {
        LocalizeAsset(localizeTarget);
    }




    //Only overrides current asset if a translation is available
    public static void LocalizeAsset(Object target)
    {
        if (target == null)
        {
            Debug.LogError("LocalizedAsset target is null");
            return;
        }


        if (target.GetType() == typeof(GUITexture))
        {
            GUITexture gT = (GUITexture)target;
            if (gT.texture != null)
            {
                Texture text = (Texture)Language.GetAsset(gT.texture.name);
                if (text != null)
                    gT.texture = text;
            }
        }
        else if (target.GetType() == typeof(Material))
        {
            Material mainT = (Material)target;
            if (mainT.mainTexture != null)
            {
                Texture text = (Texture)Language.GetAsset(mainT.mainTexture.name);
                if (text != null)
                    mainT.mainTexture = text;
            }
        }
        else if (target.GetType() == typeof(MeshRenderer))
        {
            MeshRenderer mainT = (MeshRenderer)target;
            if (mainT.material.mainTexture != null)
            {
                Texture text = (Texture)Language.GetAsset(mainT.material.mainTexture.name);
                if (text != null)
                    mainT.material.mainTexture = text;
            }
        }
        else
        {
            Debug.LogError("Could not localize this object type: " + target.GetType());
        }
    }

}