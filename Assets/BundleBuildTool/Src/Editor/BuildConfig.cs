using UnityEngine;
using UnityEditor;

namespace BundleManager
{
    public static class BuildConfig
    {
        public static BuildAssetBundleOptions CompressAssetBundleOptions = BuildAssetBundleOptions.ChunkBasedCompression;
        public static BuildAssetBundleOptions DeterministicBundleOptions = BuildAssetBundleOptions.DeterministicAssetBundle;
        public static BuildOptions CompressSceneOptions = BuildOptions.CompressWithLz4;
        public static string BundleSuffix = ".assetBundle";
        public static string InterpretedRootPath = "Assets/BundleBuildTool/Bundle/";
        public static string ResourceRootPath = "Assets";
        public static string BundleDataPath = InterpretedRootPath + "BundleData.txt";
        public static string BundleStatePath = InterpretedRootPath + "BundleStatePath.txt";
        public static string BundleImportDataPath = InterpretedRootPath + "BundleImportData.txt";

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

        public static string BundleMainfestOutputPath
        {
            get
            {
                return GetBuildOutputPath("BundleMainfest");
            }
        }
        public static string BundleStateOutputPath
        {
            get
            {
                return GetBuildOutputPath("BundleState", ".bytes");
            }
        }
        public static string BundleDictOutputPath
        {
            get
            {
                return GetBuildOutputPath("BundleDict", ".bytes");
            }
        }
        public static string GetBuildOutputPath(string bundleName, string suffix = "")
        {
            if (string.IsNullOrEmpty(suffix))
            {
                suffix = BundleSuffix;
            }

            return string.Format("{0}/{1}.{2}", InterpretedOutputPath, bundleName, suffix);
        }
        public static string FormatPublishPath(string assetPath)
        {
            return assetPath;
        }
    }

    public static class BundleName
    {
        public static string BN_SHADER = "Shader";
        public static string BN_SCRIPT = "Script";
    }

    public static class TableStyles
    {
        public static GUIStyle Toolbar = "Toolbar";
        public static GUIStyle ToolbarButton = "ToolbarButton";

        public static GUIStyle TextField = "TextField";
    }
}