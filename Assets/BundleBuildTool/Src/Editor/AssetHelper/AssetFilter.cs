using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

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
        public static List<UnityEngine.Object> FilterObjectByType(UnityEngine.Object[] assetsAtPath, BundleType bundleType, string assetPath)
        {
            List<UnityEngine.Object> ret = new List<UnityEngine.Object>();
            switch (bundleType)
            {
            case BundleType.FBX:
                foreach (UnityEngine.Object obj in assetsAtPath)
                {
                    if (obj == null)
                        continue;
                    Type type = obj.GetType();
                    if (type == typeof(AnimationClip) && obj.name != EditorConst.EDITOR_ANICLIP_NAME)
                    {
                        ret.Add(obj);
                    }
                    else
                    {
                        ret.Add(obj);
                    }
                }
                break;
            case BundleType.Controller:
                foreach (UnityEngine.Object obj in assetsAtPath)
                {

                    if (obj == null)
                        continue;
                    string typeName = obj.GetType().ToString();
                    if (typeName.Contains("AnimatorStateMachine") || typeName.Contains("AnimatorStateTransition") ||
                        typeName.Contains("AnimatorState") || typeName.Contains("AnimatorTransition") ||
                        typeName.Contains("BlendTree"))
                        continue;
                    ret.Add(obj);
                }

                break;
            default:
                ret.AddRange(assetsAtPath);
                break;
            }
            if (ret.Count == 0)
            {
                ret.AddRange(assetsAtPath);
            }

            for (int i = 0; i < m_filterList.Count; ++i)
            {
                ret = m_filterList[i](ret.ToArray(), bundleType, assetPath);
            }

            return ret;
        }

        private static List<FilterDelegate> m_filterList = new List<FilterDelegate>();
    }
}