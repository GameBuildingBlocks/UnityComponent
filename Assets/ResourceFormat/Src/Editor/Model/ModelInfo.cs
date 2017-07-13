using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using EditorCommon;

namespace ResourceFormat
{
    public class ModelInfo : BaseInfo
    {
        public bool ReadWriteEnable = false;
        public bool OptimizeMesh = false;
        public bool ImportMaterials;
        public bool ImportAnimation;
        public ModelImporterMeshCompression MeshCompression = ModelImporterMeshCompression.Off;

        public static ModelInfo CreateModelInfo(string assetPath)
        {
            ModelInfo tInfo = null;
            if (!m_dictTexInfo.TryGetValue(assetPath, out tInfo))
            {
                tInfo = new ModelInfo();
                m_dictTexInfo.Add(assetPath, tInfo);
            }
            ModelImporter tImport = AssetImporter.GetAtPath(assetPath) as ModelImporter;
            if (tImport == null) return null;

            tInfo.Path = assetPath;
            tInfo.ReadWriteEnable = tImport.isReadable;
            tInfo.OptimizeMesh = tImport.optimizeMesh;
            tInfo.ImportMaterials = tImport.importMaterials;
            tInfo.ImportAnimation = tImport.importAnimation;
            tInfo.MeshCompression = tImport.meshCompression;

            tInfo.MemSize = EditorTool.CalculateModelSizeBytes(assetPath);

            if (m_loadCount % 256 == 0)
            {
                Resources.UnloadUnusedAssets();
            }

            return tInfo;
        }

        private static int m_loadCount = 0;
        private static Dictionary<string, ModelInfo> m_dictTexInfo = new Dictionary<string, ModelInfo>();
    }
}