using UnityEngine;
using UnityEngine.Profiling;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace BundleManager
{
    public static class BundleAdapter
    {
        private static BundleDataControl m_dataControl = BundleDataControl.Instance;
        private static Dictionary<string, long> m_pathFileSize = new Dictionary<string, long>();
        private static Dictionary<string, bool> m_bundleDict = new Dictionary<string, bool>();
        private static Dictionary<string, string> m_shaderCache = new Dictionary<string, string>();
        private static bool m_useFilter = false;
        private static uint m_processCnt = 0;

        public static void RefreshData()
        {
            m_dataControl.RefreshBaseData();
            m_bundleDict.Clear();
            m_shaderCache.Clear();
            m_pathFileSize.Clear();
        }

        public static void CreateBundles()
        {
            _PreProcessSpecialBundle();

            List<string> list = m_dataControl.GetPublishPackagePath();
            for (int i = 0; i < list.Count; ++i)
            {
                string name = Path.GetFileName(list[i]);
                if (EditorUtility.DisplayCancelableProgressBar("Create Bundle", name, (i * 1.0f) / list.Count))
                {
                    Debug.LogWarning("[BundleAdapter] CreateBundles Stop.");
                    break;
                }
                string[] dep = AssetDepend.GetDependenciesCache(list[i]);
                for (int j = 0; j < dep.Length; ++j)
                {
                    if (string.IsNullOrEmpty(dep[j]))
                        continue;
                    _ProcessSpecialResource(dep[j]);
                    BundleImportData data = m_dataControl.GetPathSelectData(dep[j]);
                    if (data != null && !data.SkipData)
                    {
                        _AddPathToBundleByPathConfig(data, dep[j]);
                    }
                }
            }
            EditorUtility.ClearProgressBar();

            BMDataAccessor.SaveData();
            AssetDatabase.SaveAssets();

        }
        public static void UpdateBundleBuildList(FilterHandler filter)
        {
            if (filter == null)
            {
                m_useFilter = false;
                return;
            }
            m_useFilter = true;
            List<BundleImportData> dataList = m_dataControl.DataList;
            for (int l = 0; l < dataList.Count; ++l)
            {
                BundleImportData data = dataList[l];
                if (data == null)
                    continue;
                if (EditorUtility.DisplayCancelableProgressBar("Update Bundle List", data.RootPath, (l * 1.0f) / dataList.Count))
                {
                    Debug.LogWarning("CreateBundles Stop.");
                    break;
                }
                string parentName = BundleDataManager.GetIndexBundleName(l);
                BundleData parent = BundleDataManager.GetBundleData(parentName);
                for (int i = 0; parent != null && i < parent.children.Count; ++i)
                {
                    _ProcessUpdateBundleList(parent.children[i], data, filter);
                }
            }
            EditorUtility.ClearProgressBar();
            _ProcessDependBundleList();
        }
        public static void BuildBundles()
        {
            //ModelDataControl dataControl = new ModelDataControl(null, null);
            //BuildHelper.SetDelegate(dataControl.FilterObject);

            BuildHelper.PushAssetDependencies();
            //BuildHelper.BuildShaderBundle();
            _BuildSingleBundle(BundleName.BN_SCRIPT, null);

            List<BundleImportData> dataList = m_dataControl.DataList;
            for (int i = 0; i < dataList.Count; ++i)
            {
                BundleImportData data = dataList[i];
                string parentName = BundleDataManager.GetIndexBundleName(i);
                _BuildSingleBundle(parentName, data);
            }
            BuildHelper.PopAssetDependencies();

            //BuildHelper.ExportBMDatasToOutput();

            AssetDatabase.SaveAssets();
        }
        public static void ClearBundles()
        {
            //BuildHelper.RemoveAllBundle();
        }

        private static void _BuildSingleBundle(string name, BundleImportData data)
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
                if (m_useFilter && !m_bundleDict.ContainsKey(child.name))
                {
                    continue;
                }

                BuildHelper.BuildSingleBundle(child, childState, data.PushDependice);
            }
        }
        private static void _ProcessClear()
        {
            if (++m_processCnt % 256 == 0)
            {
                AssetDatabase.SaveAssets();
                Resources.UnloadUnusedAssets();
            }
        }
        private static void _ProcessDependBundleList()
        {
            bool loop = true;
            while (loop)
            {
                loop = false;
                List<string> list = new List<string>(m_bundleDict.Keys);
                for (int i = 0; i < list.Count; ++i)
                {
                    bool isProcess = false;

                    if (!m_bundleDict.TryGetValue(list[i], out isProcess))
                    {
                        continue;
                    }
                    if (isProcess)
                        continue;

                    m_bundleDict[list[i]] = true;
                    loop = true;

                    BundleData data = BundleDataManager.GetBundleData(list[i]);
                    if (data == null)
                        continue;
                    for (int j = 0; j < data.includs.Count; ++j)
                    {
                        string[] dep = AssetDepend.GetDependenciesCache(data.includs[j]);
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
        private static void _ProcessUpdateBundleList(string bundleName, BundleImportData selectData, FilterHandler filter)
        {
            BundleData data = BundleDataManager.GetBundleData(bundleName);
            if (data == null)
                return;

            for (int i = 0; i < data.includs.Count; ++i)
            {
                if (filter == null || filter(data.includs[i]))
                {
                    _AddToDict(data.name, m_bundleDict);
                    continue;
                }
                if (!selectData.Publish)
                    continue;
                string[] dep = AssetDepend.GetDependenciesCache(data.includs[i]);
                for (int k = 0; k < dep.Length; ++k)
                {
                    if (!filter(dep[k]))
                    {
                        continue;
                    }
                    if (!BundleDataManager.CheckPathInBundle(dep[k]))
                    {
                        _AddToDict(data.name, m_bundleDict);
                        break;
                    }
                }
            }
        }
        private static void _PreProcessSpecialBundle()
        {
            BundleData shader = BundleDataManager.GetBundleData(BundleName.BN_SHADER);
            if (shader == null)
            {
                BundleDataManager.CreateNewBundle(BundleName.BN_SHADER, "", BundleType.None, BundleLoadState.UnLoadOnUnloadAsset);
                shader = BundleDataManager.GetBundleData(BundleName.BN_SHADER);
            }
            if (shader.children.Count == 0)
            {
                string name = BundleDataManager.GenBundleNameByType(BundleType.Shader);
                BundleDataManager.CreateNewBundle(name, shader.name, BundleType.Shader, BundleLoadState.PreLoad);
            }
            _AddToDict(shader.name, m_bundleDict);

            BundleData script = BundleDataManager.GetBundleData(BundleName.BN_SCRIPT);
            if (script == null)
            {
                BundleDataManager.CreateNewBundle(BundleName.BN_SCRIPT, "", BundleType.None, BundleLoadState.UnLoadOnUnloadAsset);
                script = BundleDataManager.GetBundleData(BundleName.BN_SCRIPT);
            }

            if (script.children.Count == 0)
            {
                string name = BundleDataManager.GenBundleNameByType(BundleType.Script);
                BundleDataManager.CreateNewBundle(name, script.name, BundleType.Script, BundleLoadState.PreLoad);
            }
            _AddToDict(script.name, m_bundleDict);
        }

        private static void _ProcessSpecialResource(string path)
        {
            if (m_shaderCache.ContainsKey(path))
                return;
            m_shaderCache.Add(path, path);
            if (PathConfig.IsShader(path))
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
            else if (PathConfig.IsMaterial(path))
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
                _ProcessClear();
            }
            else if (PathConfig.IsScript(path))
            {
                BundleData script = BundleDataManager.GetBundleData(BundleName.BN_SCRIPT);
                BundleDataManager.AddPathToBundle(path, script.children[0], 1024);
            }
        }
        private static void _AddPathToBundleByPathConfig(BundleImportData data, string path)
        {
            if (data == null || string.IsNullOrEmpty(path) || BundleDataManager.CheckPathInBundle(path))
                return;

            string parentName = BundleDataManager.GetIndexBundleName(data.Index);
            if (data.Type == BundleType.Shader)
            {
                parentName = BundleName.BN_SHADER;
            }
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
                BundleDataManager.CreateNewBundle(parentName, "", BundleType.None, BundleLoadState.UnLoadOnUnloadAsset);
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

            long size = CalcPathFileSize(path, data.Type);
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
        public static long CalcPathFileSize(string path, BundleType type)
        {
            path = PathConfig.FormatAssetPath(path);
            path = PathConfig.NormalizePathSplash(path);

            long ret = 0;
            if (m_pathFileSize.TryGetValue(path, out ret))
            {
                return ret;
            }

            UnityEngine.Object[] objs = null;

            switch (type)
            {
            case BundleType.Texture:
                objs = AssetDatabase.LoadAllAssetsAtPath(path);
                for (int i = 0; i < objs.Length; ++i)
                {
                    if (objs[i] is Texture)
                    {
#pragma warning disable 0618
                        ret += Profiler.GetRuntimeMemorySize(objs[i]);
#pragma warning restore 0618
                    }
                }
                ret /= 4;

                break;
            case BundleType.Material:
                string[] deps = AssetDepend.GetDependenciesCache(path);
                for (int i = 0; i < deps.Length; ++i)
                {
                    if (PathConfig.IsTexture(deps[i]))
                    {
                        BundleImportData data = m_dataControl.GetPathSelectData(deps[i]);
                        if (data == null || data.SkipData)
                        {
                            ret += EditorCommon.CalculateTextureSizeBytes(deps[i]) / 4;
                        }
                    }
                }
                ret += 512;
                break;
            case BundleType.FBX:
            case BundleType.Controller:
            case BundleType.Animation:
                objs = AssetDatabase.LoadAllAssetsAtPath(path);
                List<UnityEngine.Object> list = new List<Object>();
                BuildHelper.FilterObjectByType(objs, list, type, path);
                for (int i = 0; i < list.Count; ++i)
                {
#pragma warning disable 0618
                    ret += Profiler.GetRuntimeMemorySize(list[i]);
#pragma warning restore 0618
                }
                ret /= 6;
                break;
            default:
                FileInfo fileInfo = new FileInfo(path);
                ret = fileInfo.Length;
                break;
            }

            if (objs != null)
            {
                _ProcessClear();
            }

            for (int i = 0; objs != null && i < objs.Length; ++i)
            {
                if ((!(objs[i] is GameObject)) && (!(objs[i] is Component)))
                {
                    Resources.UnloadAsset(objs[i]);
                }
            }

            m_pathFileSize.Add(path, ret);
            return ret;
        }
    }
}