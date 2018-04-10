using EditorCommon;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace ResourceFormat {
    public enum ModelOverviewMode
    {
        ReadWrite = 0,
        ImportMaterial,
        OptimizeMesh,
        MeshData,
        MeshCompress,
        VertexCount,
        TriangleCount,

        Count,
    }

    public class ModelOverviewData
    {
        public int Count;
        public int Memory;
        public bool ReadWriteEnable;
        public bool ImportMaterials;
        public bool OptimizeMesh;
        public int MeshDataID;
        public string MeshDataStr;
        public ModelImporterMeshCompression MeshCompression = ModelImporterMeshCompression.Off;
        public string VertexRangeStr;
        public string TriangleRangeStr;
        public ModelOverviewMode Mode;

        public static ModelOverviewData CreateNew(ModelOverviewMode mode, ModelInfo modelInfo)
        {
            ModelOverviewData retData = new ModelOverviewData();
            retData.Mode = mode;
            retData.ReadWriteEnable = modelInfo.ReadWriteEnable;
            retData.ImportMaterials = modelInfo.ImportMaterials;
            retData.OptimizeMesh = modelInfo.OptimizeMesh;
            retData.MeshDataID = modelInfo.GetMeshDataID();
            retData.MeshDataStr = ModelInfo.GetMeshDataStr(retData.MeshDataID);
            retData.MeshCompression = modelInfo.MeshCompression;
            retData.VertexRangeStr = modelInfo.GetVertexRangeStr();
            retData.TriangleRangeStr = modelInfo.GetTriangleRangeStr();

            return retData;
        }

        public static void SwitchDataTableMode(ModelOverviewMode mode, TableView tableView)
        {
            float leftWide = 0.4f;
            tableView.ClearColumns();
            switch (mode)
            {
            case ModelOverviewMode.ReadWrite:
                tableView.AddColumn("ReadWriteEnable", "R/W Enable", leftWide);
                break;
            case ModelOverviewMode.ImportMaterial:
                tableView.AddColumn("ImportMaterials", "ImportMaterials", leftWide);
                break;
            case ModelOverviewMode.OptimizeMesh:
                tableView.AddColumn("OptimizeMesh", "OptimizeMesh", leftWide);
                break;
            case ModelOverviewMode.MeshData:
                tableView.AddColumn("MeshDataStr", "MeshData", leftWide);
                break;
            case ModelOverviewMode.MeshCompress:
                tableView.AddColumn("MeshCompression", "MeshCompression", leftWide);
                break;
            case ModelOverviewMode.VertexCount:
                tableView.AddColumn("VertexRangeStr", "Vertex", leftWide);
                break;
            case ModelOverviewMode.TriangleCount:
                tableView.AddColumn("TriangleRangeStr", "Triangle", leftWide);
                break;
            }
            tableView.AddColumn("Count", "Count", (1.0f - leftWide) / 2.0f);
            tableView.AddColumn("Memory", "Memory", (1.0f - leftWide) / 2.0f, TextAnchor.MiddleCenter, "<fmt_bytes>");
        }

        public bool IsMatch(ModelInfo modelInfo)
        {
            switch (Mode)
            {
            case ModelOverviewMode.ReadWrite:
                return ReadWriteEnable == modelInfo.ReadWriteEnable;
            case ModelOverviewMode.ImportMaterial:
                return ImportMaterials == modelInfo.ImportMaterials;
            case ModelOverviewMode.OptimizeMesh:
                return OptimizeMesh == modelInfo.OptimizeMesh;
            case ModelOverviewMode.MeshData:
                return MeshDataID == modelInfo.GetMeshDataID();
            case ModelOverviewMode.MeshCompress:
                return MeshCompression == modelInfo.MeshCompression;
            case ModelOverviewMode.VertexCount:
                return VertexRangeStr == modelInfo.GetVertexRangeStr();
            case ModelOverviewMode.TriangleCount:
                return TriangleRangeStr == modelInfo.GetTriangleRangeStr();
            }
            return false;
        }

        public void AddObject(ModelInfo modelInfo)
        {
            Count = Count + 1;
            Memory += modelInfo.MemSize;
            m_objects.Add(modelInfo);
        }

        public List<object> GetObjects()
        {
            return m_objects;
        }

        protected List<object> m_objects = new List<object>();
    }
}