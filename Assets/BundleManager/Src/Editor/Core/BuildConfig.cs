using UnityEngine;
using UnityEditor;

namespace BundleManager
{
    public class BuildConfig
    {
        public static BuildAssetBundleOptions CompressAssetBundleOptions = BuildAssetBundleOptions.ChunkBasedCompression;
        public static BuildAssetBundleOptions DeterministicBundleOptions = BuildAssetBundleOptions.DeterministicAssetBundle;
        public static BuildOptions CompressSceneOptions = BuildOptions.CompressWithLz4;
        public static string BundleSuffix = ".bytes";
        public static string InterpretedRootPath = "Bundle/";

        public static BuildAssetBundleOptions CurrentBuildAssetOpts
        {
            get
            {
#pragma warning disable 0618
                return CompressAssetBundleOptions | DeterministicBundleOptions | BuildAssetBundleOptions.CollectDependencies;
#pragma warning restore 0618
            }
        }
        public static BuildOptions CurrentBuildSceneOpts
        {
            get
            {
                return CompressSceneOptions;
            }
        }
        public static string InterpretedOutputPath
        {
            get
            {
#if UNITY_ANDROID
                return InterpretedRootPath + EditorConst.PlatformAndroid;
#elif UNITY_IOS
                return InterpretedRootPath + EditorConst.PlatformIos;
#else
                return InterpretedRootPath + EditorConst.PlatformStandalones;
#endif
            }
        }
        public static BuildTarget UnityBuildTarget
        {
            get
            {
#if UNITY_ANDROID
                return BuildTarget.Android;
#elif UNITY_IOS
                return BuildTarget.iOS;
#else
                return BuildTarget.StandaloneWindows64;
#endif
            }
        }
    }
}