using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ResourceFormat
{
    public static class AnimationFormater
    {
        public static void ApplyFormatToObject(AnimationImportData data)
        {
            if (data == null) return;

            List<object> unFortmatObject = data.GetObjects(true);

            for (int i = 0; i < unFortmatObject.Count; ++i)
            {
                AnimationInfo texInfo = unFortmatObject[i] as AnimationInfo;
                string name = System.IO.Path.GetFileName(texInfo.Path);
                if (EditorUtility.DisplayCancelableProgressBar("设置动作格式", name, (i * 1.0f) / unFortmatObject.Count))
                {
                    Debug.LogWarning("[AnimationFormater]ApplyFormatToObject Stop.");
                    break;
                }
                if (texInfo == null) continue;
                ModelImporter tImporter = AssetImporter.GetAtPath(texInfo.Path) as ModelImporter;
                if (tImporter == null) continue;

                bool needImport = false;

                if (data.AnimationType != tImporter.animationType)
                {
                    tImporter.animationType = data.AnimationType;
                    needImport = true;
                }

                if (data.AnimationCompression != tImporter.animationCompression)
                {
                    tImporter.animationCompression = data.AnimationCompression;
                    needImport = true;
                }

                if (needImport)
                {
                    tImporter.SaveAndReimport();
                }
            }
            EditorUtility.ClearProgressBar();

            for (int i = 0; i < unFortmatObject.Count; ++i)
            {
                AnimationInfo texInfo = unFortmatObject[i] as AnimationInfo;
                string name = System.IO.Path.GetFileName(texInfo.Path);
                EditorUtility.DisplayProgressBar("更新动作数据", name, (i * 1.0f) / unFortmatObject.Count);
                AnimationInfo.CreateAnimationInfo(texInfo.Path);
            }
            EditorUtility.ClearProgressBar();
        }
    }
}