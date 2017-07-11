using UnityEngine;
using System.Collections.Generic;

namespace BundleManager
{
    public class AssetPathInfo
    {
        // Index Of BundleImportData
        public string Path = "Unknown";
        public int Index = -1;

        public static AssetPathInfo CreatePathInfo(string assetPath)
        {
            AssetPathInfo pathInfo = null;
            if (!m_dictPathInfo.TryGetValue(assetPath, out pathInfo))
            {
                pathInfo = new AssetPathInfo();
                pathInfo.Path = assetPath;
                m_dictPathInfo.Add(assetPath, pathInfo);
            }
            return pathInfo;
        }

        private static Dictionary<string, AssetPathInfo> m_dictPathInfo = new Dictionary<string, AssetPathInfo>();
    }
}