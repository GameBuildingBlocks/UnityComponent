using System.IO;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEditor;
using System.Collections.Generic;

namespace BundleManager
{
    public class AssetSize
    {
        public static long CalcAssetSize(string path, BundleType type)
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

                break;
            case BundleType.Material:
                string[] deps = AssetDepend.GetDependenciesCache(path);
                for (int i = 0; i < deps.Length; ++i)
                {
                    if (PathConfig.IsTexture(deps[i]))
                    {
                        BundleImportData data = BundleDataControl.Instance.GetPathImportData(deps[i]);
                        if (data == null || data.SkipData)
                        {
                            ret += EditorCommon.CalculateTextureSizeBytes(deps[i]);
                        }
                    }
                }
                ret += 512;
                break;
            case BundleType.FBX:
            case BundleType.Controller:
            case BundleType.Animation:
                objs = AssetDatabase.LoadAllAssetsAtPath(path);
                List<UnityEngine.Object> list = AssetFilter.FilterObjectByType(objs, type, path);
                for (int i = 0; i < list.Count; ++i)
                {
#pragma warning disable 0618
                    ret += Profiler.GetRuntimeMemorySize(list[i]);
#pragma warning restore 0618
                }
                break;
            default:
                FileInfo fileInfo = new FileInfo(path);
                ret = fileInfo.Length;
                break;
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
        public static void Clear()
        {
            m_pathFileSize.Clear();
        }

        private static Dictionary<string, long> m_pathFileSize = new Dictionary<string, long>();
    }
}