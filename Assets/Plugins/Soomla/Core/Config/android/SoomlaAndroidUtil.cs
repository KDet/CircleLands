using System;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Soomla
{

	public class SoomlaAndroidUtil
    {
#if UNITY_EDITOR
		private static char DSC = Path.DirectorySeparatorChar;

        public const string ERROR_NO_SDK = "no_android_sdk";
        public const string ERROR_NO_KEYSTORE = "no_android_keystore";

        private static string setupError;

        public static bool IsSetupProperly()
        {
			if (setupError == "none") {
				return true;
			}
			if (setupError == null) {
				if (!HasAndroidSDK())
				{
					setupError = ERROR_NO_SDK;
					return false;
				}
				if (!HasAndroidKeystoreFile())
				{
					setupError = ERROR_NO_KEYSTORE;
					return false;
				}

				setupError = "none";
				return true;
			}
	        return false;
        }

		private static string HomeFolderPath
		{
			get 
			{
				string homeFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
				switch(Environment.OSVersion.Platform) {
				case PlatformID.WinCE:
				case PlatformID.Win32Windows:
				case PlatformID.Win32S:
				case PlatformID.Win32NT:
					homeFolder = Directory.GetParent(homeFolder).FullName;
					break;
				default:
					break;
				}

				return homeFolder;
			}
		}

		public static string KeyStorePass
		{
			get
			{
				string keyStorePass = PlayerSettings.Android.keystorePass;
				if (string.IsNullOrEmpty(keyStorePass)) {
					keyStorePass = @"android";
				}
				return keyStorePass;
			}
		}

        public static string KeyStorePath
        {
            get
            {
				string keyStore = PlayerSettings.Android.keystoreName;
				if (string.IsNullOrEmpty(keyStore)) {
					keyStore = HomeFolderPath + DSC + @".android" + DSC + @"debug.keystore";
				}
				return keyStore;
			}
        }

        public static string SetupError
        {
            get
            {
                return setupError;
            }
        }

        public static bool HasAndroidSDK()
        {
            return EditorPrefs.HasKey("AndroidSdkRoot") && Directory.Exists(EditorPrefs.GetString("AndroidSdkRoot"));
        }

        public static bool HasAndroidKeystoreFile()
        {
		    return File.Exists(KeyStorePath);
        }



#endif
    }
}
