using EditorCommon;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ResourceFormat
{
    public class ModelDataControl : FormatDataControl<ModelImportData>
    {
        public ModelDataControl(TableView dataTable, TableView showTable)
        {
            m_dataTable = dataTable;
            m_showTable = showTable;

            m_dataList = ConfigData.ModelSelectData;
            for (int i = 0; i < m_dataList.Count; ++i)
            {
                m_dataList[i].ClearObject();
            }

            if (m_dataTable != null)
            {
                m_dataTable.RefreshData(EditorTool.ToObjectList<ModelImportData>(m_dataList));
            }
        }

        public bool UnFormat
        {
            get { return m_showUnformatObject; }
            set { m_showUnformatObject = value; }
        }

        protected override void _RefreshList(List<string> list)
        {
            m_modelInfo = new List<ModelInfo>();
            for (int i = 0; i < list.Count; ++i)
            {
                string path = EditorPath.FormatAssetPath(list[i]);
                string name = System.IO.Path.GetFileName(path);
                EditorUtility.DisplayProgressBar("获取模型数据", name, (i * 1.0f) / list.Count);
                if (!EditorPath.IsModel(path))
                    continue;
                ModelInfo modelInfo = ModelInfo.CreateModelInfo(path);
                if (modelInfo != null)
                {
                    m_modelInfo.Add(modelInfo);
                }
            }
            EditorUtility.ClearProgressBar();
            RefreshDataWithSelect();
        }

        public override void RefreshDataWithSelect()
        {
            base.RefreshDataWithSelect();

            if (m_modelInfo != null)
            {
                for (int i = 0; i < m_modelInfo.Count; ++i)
                {
                    string name = System.IO.Path.GetFileName(m_modelInfo[i].Path);
                    EditorUtility.DisplayProgressBar("更新模型表数据", name, (i * 1.0f) / m_modelInfo.Count);
                    for (int j = m_dataList.Count - 1; j >= 0; --j)
                    {
                        if (m_dataList[j].IsMatch(m_modelInfo[i].Path))
                        {
                            m_dataList[j].AddObject(m_modelInfo[i]);
                            break;
                        }
                    }
                }
                EditorUtility.ClearProgressBar();
            }

        }
        public override void OnDataSelected(object selected, int col)
        {
            ModelImportData texSelectData = selected as ModelImportData;
            if (texSelectData == null)
                return;

            m_editorData.CopyData(texSelectData);
            m_index = texSelectData.Index;
            if (texSelectData != null)
            {
                m_showTable.RefreshData(texSelectData.GetObjects(m_showUnformatObject));
            }
        }

        private Object _FilterMesh(ModelImportData selected, Object obj)
        {
            Mesh mesh = obj as Mesh;
            if (mesh == null)
                return obj;

            if (!selected.ImportNormal)
            {
                mesh.normals = null;
            }
            if (!selected.ImportTangent)
            {
                mesh.tangents = null;
            }
            if (!selected.ImportUV2)
            {
                mesh.uv2 = null;
            }
            if (!selected.ImportUV3)
            {
                mesh.uv3 = null;
            }
            if (!selected.ImportUV4)
            {
                mesh.uv4 = null;
            }

            return mesh;
        }

        private bool m_showUnformatObject = false;
        private List<ModelInfo> m_modelInfo = new List<ModelInfo>();
    }
}
