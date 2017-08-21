using EditorCommon;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ResourceFormat
{
    public class ModelInfo : BaseInfo
    {
        public bool ReadWriteEnable = false;
        public bool OptimizeMesh = false;
        public bool ImportMaterials;
        public bool ImportAnimation;
        public ModelImporterMeshCompression MeshCompression = ModelImporterMeshCompression.Off;
        public bool bHasUV;
        public bool bHasUV2;
        public bool bHasUV3;
        public bool bHasUV4;
        public bool bHasColor;
        public bool bHasNormal;
        public bool bHasTangent;
        public int vertexCount;
        public int triangleCount;

        public static ModelInfo CreateModelInfo(string assetPath)
        {
            if (!EditorPath.IsModel(assetPath))
            {
                return null;
            }

            ModelInfo tInfo = null;
            if (!m_dictModelInfo.TryGetValue(assetPath, out tInfo))
            {
                tInfo = new ModelInfo();
                m_dictModelInfo.Add(assetPath, tInfo);
            }
            ModelImporter tImport = AssetImporter.GetAtPath(assetPath) as ModelImporter;
            Mesh mesh = AssetDatabase.LoadAssetAtPath<Mesh>(assetPath);

            if (tImport == null || mesh == null)
                return null;

            tInfo.Path = assetPath;
            tInfo.ReadWriteEnable = tImport.isReadable;
            tInfo.OptimizeMesh = tImport.optimizeMesh;
            tInfo.ImportMaterials = tImport.importMaterials;
            tInfo.ImportAnimation = tImport.importAnimation;
            tInfo.MeshCompression = tImport.meshCompression;

            tInfo.bHasUV = mesh.uv != null && mesh.uv.Length != 0;
            tInfo.bHasUV2 = mesh.uv2 != null && mesh.uv2.Length != 0;
            tInfo.bHasUV3 = mesh.uv3 != null && mesh.uv3.Length != 0;
            tInfo.bHasUV4 = mesh.uv4 != null && mesh.uv4.Length != 0;
            tInfo.bHasColor = mesh.colors != null && mesh.colors.Length != 0;
            tInfo.bHasNormal = mesh.normals != null && mesh.normals.Length != 0;
            tInfo.bHasTangent = mesh.tangents != null && mesh.tangents.Length != 0;
            tInfo.vertexCount = mesh.vertexCount;
            tInfo.triangleCount = mesh.triangles.Length / 3;

            tInfo.MemSize = EditorTool.CalculateModelSizeBytes(assetPath);

            if (m_loadCount % 256 == 0)
            {
                Resources.UnloadUnusedAssets();
            }

            return tInfo;
        }

        public static List<ModelInfo> GetModelInfoByDirectory(string dir)
        {
            List<ModelInfo> modelInfoList = new List<ModelInfo>();
            List<string> list = new List<string>();
            EditorPath.ScanDirectoryFile(dir, true, list);
            for (int i = 0; i < list.Count; ++i)
            {
                string assetPath = EditorPath.FormatAssetPath(list[i]);
                ModelInfo modelInfo = CreateModelInfo(assetPath);
                if (modelInfo != null)
                {
                    modelInfoList.Add(modelInfo);
                }
            }

            return modelInfoList;
        }

        private static int m_loadCount = 0;
        private static Dictionary<string, ModelInfo> m_dictModelInfo = new Dictionary<string, ModelInfo>();
    }
}
