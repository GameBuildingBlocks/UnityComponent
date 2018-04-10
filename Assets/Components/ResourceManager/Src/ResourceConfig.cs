using UnityEngine;
using CommonComponent;

namespace GameResource
{
    public static class ResourceConfig
    {
        public static readonly long BUNDLE_TIME_OUT = 10000; //ms
        public static readonly int MAX_CACHE_BUNDLE_SIZE = 20;
        public static readonly int MAX_POOL_COUNT = 1024;
        public static readonly string BUNDLE_SUFFIX = "assetbundle";
        public static readonly string PATH_DIR_NAME = "res";

        public static string FormatPath(int bundleId, bool export)
        {
            if (export)
            {
                return string.Format("{0}/{1}/{2}.{3}", PathTool.GetPersistentDataPath(), PATH_DIR_NAME, bundleId, BUNDLE_SUFFIX);
            }
            else
            {
#if UNITY_EDITOR
                return string.Format("{0}/{1}/{2}.{3}", Application.streamingAssetsPath, PATH_DIR_NAME, bundleId, BUNDLE_SUFFIX);
#elif UNITY_ANDROID
                return string.Format("{0}!assets/{1}/{2}.{3}", Application.dataPath, PATH_DIR_NAME, bundleId, BUNDLE_SUFFIX);
#else
                return string.Format("{0}/{1}/{2}.{3}", Application.streamingAssetsPath, PATH_DIR_NAME, bundleId, BUNDLE_SUFFIX);
#endif
            }
        }

        public static string FormatPath(string name, bool export)
        {
            if (export)
            {
                return string.Format("{0}/{1}/{2}", PathTool.GetPersistentDataPath(), PATH_DIR_NAME, name);
            }
            else
            {
#if UNITY_EDITOR
                return string.Format("{0}/{1}/{2}", Application.streamingAssetsPath, PATH_DIR_NAME, name);
#elif UNITY_ANDROID
                return string.Format("{0}!assets/{1}/{2}", Application.dataPath, PATH_DIR_NAME, name);
#else
                return string.Format("{0}/{1}/{2}", Application.streamingAssetsPath, PATH_DIR_NAME, name);
#endif
            }
        }

        public static long GetBackLoadingTime(BackLoadingPriority backLoadingPriority)
        {
            switch (backLoadingPriority)
            {
                case BackLoadingPriority.Low:
                    return 1;
                case BackLoadingPriority.BelowNormal:
                    return 5;
                case BackLoadingPriority.Normal:
                    return 10;
                case BackLoadingPriority.High:
                    return 100;
                default:
                    return 10;
            }
        }
    }

}