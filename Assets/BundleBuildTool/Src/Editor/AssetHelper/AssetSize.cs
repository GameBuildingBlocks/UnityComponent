using System.IO;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEditor;
using System.Collections.Generic;
using EditorCommon;

namespace BundleManager
{
    public class AssetSize
    {
        public static long CalcAssetSize(string assetPath, BundleType type)
        {
            assetPath = EditorPath.FormatAssetPath(assetPath);
            assetPath = EditorPath.NormalizePathSplash(assetPath);

            long ret = 0;
            if (m_pathFileSize.TryGetValue(assetPath, out ret))
            {
                return ret;
            }

            UnityEngine.Object[] assets = null;

            switch (type)
            {
            case BundleType.Texture:
                assets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
                for (int i = 0; i < assets.Length; ++i)
                {
                    if (assets[i] is Texture)
                    {
                        ret += EditorTool.GetRuntimeMemorySize(assets[i]);
                    }
                }

                break;
            case BundleType.Material:
                string[] deps = AssetDepot.GetDependenciesCache(assetPath);
                for (int i = 0; i < deps.Length; ++i)
                {
                    if (EditorPath.IsTexture(deps[i]))
                    {
                        BundleImportData data = BundleDataControl.Instance.GetPathImportData(deps[i]);
                        if (data == null || data.SkipData)
                        {
                            ret += EditorTool.CalculateTextureSizeBytes(deps[i]);
                        }
                    }
                }
                ret += 512;
                break;
            case BundleType.FBX:
            case BundleType.Controller:
            case BundleType.Animation:
                assets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
                List<UnityEngine.Object> list = AssetFilter.FilterObjectByType(assets, type, assetPath);
                for (int i = 0; i < list.Count; ++i)
                {
                    ret += EditorTool.GetRuntimeMemorySize(list[i]);
                }
                break;
            default:
                FileInfo fileInfo = new FileInfo(assetPath);
                ret = fileInfo.Length;
                break;
            }

            for (int i = 0; assets != null && i < assets.Length; ++i)
            {
                if ((!(assets[i] is GameObject)) && (!(assets[i] is Component)))
                {
                    Resources.UnloadAsset(assets[i]);
                }
            }

            m_pathFileSize.Add(assetPath, ret);
            return ret;
        }
        public static void Clear()
        {
            m_pathFileSize.Clear();
        }

        private static Dictionary<string, long> m_pathFileSize = new Dictionary<string, long>();
    }
}