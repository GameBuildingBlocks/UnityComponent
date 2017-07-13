using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using EditorCommon;

namespace BundleManager
{
    public static class BundleExport
    {
        public static void ExportBundleMainfestToOutput()
        {
            EditorTool.CreateDirectory(BuildConfig.InterpretedOutputPath);

            ExportBundleDictToOutput();

            string[] list = { BuildConfig.BundleMainfestOutputPath};
            uint crc = 0;
            bool success = BundleBuildHelper.BuildAssetBundle(list, BuildConfig.MainfestOutputPath, out crc, BundleType.TextAsset);
            if (!success)
            {
                Debug.LogErrorFormat("[BundleExport] BuildAssetBundle {0} Failed.", BuildConfig.BundleMainfestOutputPath);
            }
        }

        public static void ExportBundleDictToOutput()
        {
            EditorTool.CreateDirectory(BuildConfig.InterpretedOutputPath);

            BundleDataControl dataControl = BundleDataControl.Instance;
            BundleMainfest bundleMainfest = new BundleMainfest();
            BundleData[] bundleData = BundleDataAccessor.Datas.ToArray();

            Dictionary<string, string> dict = new Dictionary<string, string>();

            for (int i = 0; i < bundleData.Length; ++i)
            {
                for (int j = 0; j < bundleData[i].includs.Count; ++j)
                {
                    string path = bundleData[i].includs[j];
                    if (string.IsNullOrEmpty(path))
                        continue;

                    if (!dict.ContainsKey(path))
                    {
                        dict.Add(path, bundleData[i].name);
                    }
                    else
                    {
                        Debug.LogWarningFormat("[BundleExport] Path to bundle name have same path {0} : {1} _ {2}", path, bundleData[i].name, dict[path]);
                    }

                    BundleImportData data = dataControl.GetPathImportData(path);
                    if (data == null || !data.Publish || !path.StartsWith("Assets", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    string bundlePath = path; // format path to load path!!!
                    bundleMainfest.AddPathToBundle(bundlePath, bundleData[i].name);
                }
            }

            for (int i = 0; i < bundleData.Length; ++i)
            {
                for (int j = 0; j < bundleData[i].includs.Count; ++j)
                {
                    string[] dep = AssetDepot.GetDependenciesCache(bundleData[i].includs[j]);
                    for (int k = 0; k < dep.Length; ++k)
                    {
                        if (EditorPath.IsScript(dep[k]) || EditorPath.IsShader(dep[k]))
                            continue;

                        string bundleName = null;
                        dict.TryGetValue(EditorPath.NormalizePathSplash(dep[k]), out bundleName);
                        if (string.IsNullOrEmpty(bundleName) || bundleName == bundleData[i].name)
                            continue;

                        BundleState childBuildState = BundleDataManager.GetBundleState(bundleName);
                        if (childBuildState.loadState == BundleLoadState.Preload || childBuildState.size == -1)
                            continue;

                        bundleMainfest.AddBundleDepend(bundleData[i].name, bundleName);
                    }
                }
            }

            List<BundleState> stateList = new List<BundleState>(BundleDataAccessor.States);
            bundleMainfest.AddBundleState(stateList);

            bundleMainfest.SaveBytes(BuildConfig.BundleMainfestOutputPath);

            AssetDatabase.ImportAsset(BuildConfig.BundleMainfestOutputPath, ImportAssetOptions.ForceSynchronousImport);
        }
    }
}