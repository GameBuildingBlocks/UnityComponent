using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ResourceFormat
{
    public static class ModelFormater
    {
        public static void ApplyFormatToObject(ModelImportData data)
        {
            if (data == null)
                return;

            List<object> unFortmatObject = data.GetObjects(true);

            for (int i = 0; i < unFortmatObject.Count; ++i)
            {
                ModelInfo texInfo = unFortmatObject[i] as ModelInfo;
                string name = System.IO.Path.GetFileName(texInfo.Path);
                if (EditorUtility.DisplayCancelableProgressBar("设置模型格式", name, (i * 1.0f) / unFortmatObject.Count))
                {
                    Debug.LogWarning("[ModelFormater]ApplyFormatObject Stop.");
                    break;
                }
                if (texInfo == null)
                    continue;
                ModelImporter tImporter = AssetImporter.GetAtPath(texInfo.Path) as ModelImporter;
                if (tImporter == null)
                    continue;
                bool needImport = false;
                if (tImporter.isReadable != data.ReadWriteEnable)
                {
                    tImporter.isReadable = data.ReadWriteEnable;
                    needImport = true;
                }
                if (tImporter.optimizeMesh != data.OptimizeMesh)
                {
                    tImporter.optimizeMesh = data.OptimizeMesh;
                    needImport = true;
                }
                if (tImporter.importMaterials != data.ImportMaterials)
                {
                    tImporter.importMaterials = data.ImportMaterials;
                    needImport = true;
                }
                if (tImporter.importAnimation != data.ImportAnimation)
                {
                    tImporter.importAnimation = data.ImportAnimation;
                    needImport = true;
                }
                if (tImporter.meshCompression != data.MeshCompression)
                {
                    tImporter.meshCompression = data.MeshCompression;
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
                ModelInfo texInfo = unFortmatObject[i] as ModelInfo;
                string name = System.IO.Path.GetFileName(texInfo.Path);
                if (EditorUtility.DisplayCancelableProgressBar("更新模型数据", name, (i * 1.0f) / unFortmatObject.Count))
                {
                    Debug.LogWarning("[ModelFormater]Refresh Model Info Stop.");
                    break;
                }
                ModelInfo.CreateModelInfo(texInfo.Path);
            }
            EditorUtility.ClearProgressBar();
        }
    }
}
