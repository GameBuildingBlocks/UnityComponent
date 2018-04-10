using UnityEngine;
using UnityEngine.Profiling;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using EditorCommon;

namespace BundleManager
{
    public static class BundleAdapter
    {
        private static BundleDataControl m_dataControl = BundleDataControl.Instance;
        private static Dictionary<string, bool> m_bundleDict = new Dictionary<string, bool>();
        private static Dictionary<string, string> m_specialCache = new Dictionary<string, string>();
        private static bool m_buildAll = false;
        private static uint m_processCnt = 0;

        public static void RefreshData()
        {
            m_dataControl.RefreshBaseData();
            m_bundleDict.Clear();
            m_specialCache.Clear();
            AssetSize.Clear();
            AssetDepot.Clear();
        }

        public static void CreateBundles()
        {
            PreProcessSpecialBundle();

            List<string> list = m_dataControl.GetPublishPackagePath();
            for (int i = 0; i < list.Count; ++i)
            {
                string name = Path.GetFileName(list[i]);
                if (EditorUtility.DisplayCancelableProgressBar("Create Bundle", name, (i * 1.0f) / list.Count))
                {
                    Debug.LogWarning("[BundleAdapter] CreateBundles Stop.");
                    break;
                }
                string[] dep = AssetDepot.GetDependenciesCache(list[i]);
                for (int j = 0; j < dep.Length; ++j)
                {
                    if (string.IsNullOrEmpty(dep[j]))
                        continue;
                    ProcessSpecialResource(dep[j]);
                    BundleImportData data = m_dataControl.GetPathImportData(dep[j]);
                    if (data != null && !data.SkipData)
                    {
                        _AddPathToBundleByImportData(data, dep[j]);
                    }
                }
            }
            EditorUtility.ClearProgressBar();

            BundleDataAccessor.SaveData();
            AssetDatabase.SaveAssets();

        }
        public static void UpdateBundleBuildList()
        {
            m_buildAll = true;
        }
        public static void BuildBundles()
        {
            BundleBuildHelper.PushAssetDependencies();

            BuildSingleBundle(BundleName.BN_SCRIPT, null);

            BundleData shader = BundleDataManager.GetBundleData(BundleName.BN_SHADER);
            for (int i = 0; shader != null && i < shader.children.Count; ++i)
            {
                BundleData shaderChild = BundleDataManager.GetBundleData(shader.children[i]);
                BundleState childState = BundleDataManager.GetBundleState(shader.children[i]);
                bool result = BundleBuildHelper.BuildShaderBundle(shaderChild, childState);
                if (!result)
                {
                    Debug.LogErrorFormat("[BundleAdapter] BuildShaderBundle {0} Failed.", shader.children[i]);
                }
            }

            List<BundleImportData> dataList = m_dataControl.DataList;
            for (int i = 0; i < dataList.Count; ++i)
            {
                BundleImportData data = dataList[i];
                string parentName = BundleDataManager.GetIndexBundleName(i);
                BuildSingleBundle(parentName, data);
            }

            BundleBuildHelper.PopAssetDependencies();

            BundleDataAccessor.SaveData();
            BundleExport.ExportBundleMainfestToOutput();

            AssetDatabase.SaveAssets();
        }
        public static void ClearBundles()
        {
            BundleDataManager.RemoveAllBundle();
        }

        private static void BuildSingleBundle(string name, BundleImportData data)
        {
            BundleData bundle = BundleDataManager.GetBundleData(name);
            if (bundle == null)
                return;
            for (int i = 0; i < bundle.children.Count; ++i)
            {
                BundleData child = BundleDataManager.GetBundleData(bundle.children[i]);
                BundleState childState = BundleDataManager.GetBundleState(bundle.children[i]);
                if (child == null || child.includs.Count == 0 || childState == null)
                {
                    continue;
                }
                if (!m_buildAll && !m_bundleDict.ContainsKey(child.name))
                {
                    continue;
                }

                bool result = BundleBuildHelper.BuildSingleBundle(child, childState, data == null ? false : data.PushDependice);
                if (!result)
                {
                    Debug.LogErrorFormat("[BundleAdapter] BuildSingleBundle {0} Failed.", child.name);
                }
            }
        }
        private static void ProcessClear()
        {
            if (++m_processCnt % 512 == 0)
            {
                AssetDatabase.SaveAssets();
                Resources.UnloadUnusedAssets();
            }
        }
        private static void ProcessDependBundleList()
        {
            bool loop = true;
            while (loop)
            {
                loop = false;
                List<string> list = new List<string>(m_bundleDict.Keys);
                for (int i = 0; i < list.Count; ++i)
                {
                    bool isProcess = false;

                    if (!m_bundleDict.TryGetValue(list[i], out isProcess) || isProcess)
                    {
                        continue;
                    }

                    m_bundleDict[list[i]] = true;
                    loop = true;

                    BundleData data = BundleDataManager.GetBundleData(list[i]);
                    if (data == null)
                        continue;
                    for (int j = 0; j < data.includs.Count; ++j)
                    {
                        string[] dep = AssetDepot.GetDependenciesCache(data.includs[j]);
                        for (int k = 0; k < dep.Length; ++k)
                        {
                            string bundleName = BundleDataManager.GetPathBundleName(dep[k]);
                            if (!string.IsNullOrEmpty(bundleName))
                            {
                                _AddToDict(bundleName, m_bundleDict);
                            }
                        }
                    }
                }
            }
        }
        private static void ProcessUpdateBundleList(string bundleName)
        {
            BundleData data = BundleDataManager.GetBundleData(bundleName);
            if (data == null)
                return;

            for (int i = 0; i < data.includs.Count; ++i)
            {
                _AddToDict(data.name, m_bundleDict);
            }
        }
        private static void PreProcessSpecialBundle()
        {
            BundleData shader = BundleDataManager.GetBundleData(BundleName.BN_SHADER);
            if (shader == null)
            {
                BundleDataManager.CreateNewBundle(BundleName.BN_SHADER, "", BundleType.None, BundleLoadState.UnloadOnUnloadAsset);
                shader = BundleDataManager.GetBundleData(BundleName.BN_SHADER);
            }
            if (shader.children.Count == 0)
            {
                string name = BundleDataManager.GenBundleNameByType(BundleType.Shader);
                BundleDataManager.CreateNewBundle(name, shader.name, BundleType.Shader, BundleLoadState.Preload);
            }
            _AddToDict(shader.name, m_bundleDict);

            BundleData script = BundleDataManager.GetBundleData(BundleName.BN_SCRIPT);
            if (script == null)
            {
                BundleDataManager.CreateNewBundle(BundleName.BN_SCRIPT, "", BundleType.None, BundleLoadState.UnloadOnUnloadAsset);
                script = BundleDataManager.GetBundleData(BundleName.BN_SCRIPT);
            }

            if (script.children.Count == 0)
            {
                string name = BundleDataManager.GenBundleNameByType(BundleType.Script);
                BundleDataManager.CreateNewBundle(name, script.name, BundleType.Script, BundleLoadState.Preload);
            }
            _AddToDict(script.name, m_bundleDict);
        }

        private static void ProcessSpecialResource(string path)
        {
            if (m_specialCache.ContainsKey(path))
                return;
            m_specialCache.Add(path, path);
            if (EditorPath.IsShader(path))
            {
                if (!BundleDataManager.CheckPathInBundle(path))
                {
                    BundleData shader = BundleDataManager.GetBundleData(BundleName.BN_SHADER);
                    if (shader != null && shader.children.Count > 0)
                    {
                        BundleDataManager.AddPathToBundle(path, shader.children[0], 1024);
                    }
                }
            }
            else if (EditorPath.IsMaterial(path))
            {
                BundleData shaderBundle = BundleDataManager.GetBundleData(BundleName.BN_SHADER);
                if (shaderBundle == null)
                    return;
                UnityEngine.Object[] assetAtPath = AssetDatabase.LoadAllAssetsAtPath(path);
                foreach (var obj in assetAtPath)
                {
                    if (obj != null && obj.GetType() == typeof(Material))
                    {
                        Material mat = obj as Material;
                        if (mat != null && mat.shader != null && !string.IsNullOrEmpty(mat.shader.name))
                        {
                            if (!shaderBundle.includs.Contains(mat.shader.name))
                            {
                                shaderBundle.includs.Add(mat.shader.name);
                            }
                        }
                    }
                    if ((!(obj is GameObject)) && (!(obj is Component)))
                    {
                        Resources.UnloadAsset(obj);
                    }
                }
                ProcessClear();
            }
            else if (EditorPath.IsScript(path))
            {
                BundleData script = BundleDataManager.GetBundleData(BundleName.BN_SCRIPT);
                if (script != null && script.children.Count > 0)
                {
                    BundleDataManager.AddPathToBundle(path, script.children[0], 1024);
                }
            }
        }
        private static void _AddPathToBundleByImportData(BundleImportData data, string path)
        {
            if (data == null || string.IsNullOrEmpty(path) || BundleDataManager.CheckPathInBundle(path))
                return;

            string parentName = BundleDataManager.GetIndexBundleName(data.Index);
            string bundleName = BundleDataManager.GetPathBundleName(path);
            if (!string.IsNullOrEmpty(bundleName))
            {
                BundleData bundleData = BundleDataManager.GetBundleData(bundleName);
                if (bundleData.parent == parentName)
                {
                    return;
                }
                else
                {
                    BundleDataManager.RemovePathFromBundle(path, bundleName);
                }
            }

            _AddToDict(bundleName, m_bundleDict);

            BundleData parent = BundleDataManager.GetBundleData(parentName);
            if (parent == null)
            {
                BundleDataManager.CreateNewBundle(parentName, "", BundleType.None, BundleLoadState.UnloadOnUnloadAsset);
                parent = BundleDataManager.GetBundleData(parentName);
            }

            string name = "";
            for (int i = 0; i < parent.children.Count; ++i)
            {
                int index = parent.children.Count - i - 1;
                BundleData child = BundleDataManager.GetBundleData(parent.children[index]);

                if (BundleDataManager.IsBundleFull(child, data.LimitCount, data.LimitKBSize * 1024))
                {
                    continue;
                }

                if (data.Publish && BundleDataManager.IsNameDuplicatedAsset(child, path))
                {
                    continue;
                }

                name = child.name;
                break;
            }

            if (string.IsNullOrEmpty(name))
            {
                name = BundleDataManager.GenBundleNameByType(data.Type);
                BundleDataManager.CreateNewBundle(name, parent.name, data.Type, data.LoadState);
            }

            int size = (int)AssetSize.CalcAssetSize(path, data.Type);
            BundleDataManager.AddPathToBundle(path, name, size);
            _AddToDict(name, m_bundleDict);
        }
        private static void _AddToDict(string path, Dictionary<string, bool> dict)
        {
            if (!dict.ContainsKey(path))
            {
                dict.Add(path, false);
            }
        }

    }
}