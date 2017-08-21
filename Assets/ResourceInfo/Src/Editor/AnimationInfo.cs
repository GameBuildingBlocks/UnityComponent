using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using EditorCommon;

namespace ResourceFormat
{
    public class AnimationInfo : BaseInfo
    {
        public ModelImporterAnimationType AnimationType = ModelImporterAnimationType.None;
        public ModelImporterAnimationCompression AnimationCompression = ModelImporterAnimationCompression.Off;

        public static AnimationInfo CreateAnimationInfo(string assetPath)
        {
            AnimationInfo mInfo = null;
            if (!m_dictMatInfo.TryGetValue(assetPath, out mInfo))
            {
                mInfo = new AnimationInfo();
                m_dictMatInfo.Add(assetPath, mInfo);
            }

            ModelImporter tImporter = AssetImporter.GetAtPath(assetPath) as ModelImporter;
            if (tImporter == null || tImporter.clipAnimations == null)
                return null;

            mInfo.Path = assetPath;
            mInfo.AnimationType = tImporter.animationType;
            mInfo.AnimationCompression = tImporter.animationCompression;

            mInfo.MemSize = EditorTool.CalculateAnimationSizeBytes(assetPath);

            if (++m_loadCount % 256 == 0)
            {
                Resources.UnloadUnusedAssets();
            }

            return mInfo;
        }

        private static int m_loadCount = 0;
        private static Dictionary<string, AnimationInfo> m_dictMatInfo = new Dictionary<string, AnimationInfo>();
    }
}
