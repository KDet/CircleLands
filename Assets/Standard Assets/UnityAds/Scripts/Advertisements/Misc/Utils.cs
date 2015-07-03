#if UNITY_ANDROID || UNITY_IOS

namespace UnityEngine.Advertisements {
	internal static class Utils {
    private static void Log(Advertisement.DebugLevel debugLevel, string message) {
      if((Advertisement.debugLevel & debugLevel) != Advertisement.DebugLevel.NONE) {
        Debug.Log(message);
      }
    }

    public static void LogDebug(string message) {
      Log (Advertisement.DebugLevel.DEBUG,"Debug: " + message);
    }
    
    public static void LogInfo(string message) {
      Log (Advertisement.DebugLevel.INFO, "Info:" + message);
    }
    
    public static void LogWarning(string message) {
      Log (Advertisement.DebugLevel.WARNING,"Warning:" + message);
    }
    
    public static void LogError(string message) {
      Log (Advertisement.DebugLevel.ERROR, "Error: " + message);
    }
  }
}

#endif
