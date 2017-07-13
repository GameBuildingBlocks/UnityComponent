using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using EditorCommon;

namespace BundleManager
{
    public delegate List<UnityEngine.Object> FilterDelegate(UnityEngine.Object[] assets, BundleType bundleType, string assetPath);

    public class AssetFilter
    {
        public static void ClearFilter()
        {
            m_filterList.Clear();
        }
        public static void RegisterFilter(FilterDelegate filterDelegate)
        {
            m_filterList.Add(filterDelegate);
        }
        public static List<UnityEngine.Object> FilterObjectByType(UnityEngine.Object[] assets, BundleType bundleType, string assetPath)
        {
            List<UnityEngine.Object> ret = new List<UnityEngine.Object>();
            foreach (UnityEngine.Object asset in assets)
            {
                if (FilterObject(asset, bundleType))
                {
                    ret.Add(asset);
                }
            }

            for (int i = 0; i < m_filterList.Count; ++i)
            {
                ret = m_filterList[i](ret.ToArray(), bundleType, assetPath);
            }

            return ret;
        }

        private static bool FilterObject(UnityEngine.Object asset, BundleType bundleType)
        {
            if (asset == null)
            {
                return false;
            }

            switch (bundleType)
            {
            case BundleType.FBX:
                return !(asset.GetType() == typeof(AnimationClip) && asset.name == EditorConst.EDITOR_ANICLIP_NAME);
            case BundleType.Controller:
                string typeName = asset.GetType().ToString();
                for (int i = 0 ; i < EditorConst.EDITOR_CONTROL_NAMES.Length; ++i)
                {
                    if (typeName.Contains(EditorConst.EDITOR_CONTROL_NAMES[i]))
                    {
                        return false;
                    }
                }
                return true;
            default:
                return true;
            }
        }

        private static List<FilterDelegate> m_filterList = new List<FilterDelegate>();
    }
}