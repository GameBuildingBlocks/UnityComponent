using EditorCommon;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace ResourceFormat
{
    public class ModelOverviewViewer
    {
        public ModelOverviewViewer(EditorWindow hostWindow)
        {
            m_data = new List<object>[(int)ModelOverviewMode.Count];
            m_flag = new bool[(int)ModelOverviewMode.Count];
            for (int i = 0; i < (int)ModelOverviewMode.Count; ++i)
            {
                m_flag[i] = false;
                m_data[i] = new List<object>();
            }
            m_dataTable = new TableView(hostWindow, typeof(ModelOverviewData));

            ModelOverviewData.SwitchDataTableMode(m_mode, m_dataTable);

            m_showTable = new TableView(hostWindow, typeof(ModelInfo));
            m_showTable.AddColumn("Path", "Path", 0.7f, TextAnchor.MiddleLeft);
            m_showTable.AddColumn("MemSize", "Memory", 0.1f, TextAnchor.MiddleCenter, "<fmt_bytes>");
            m_showTable.AddColumn("vertexCount", "VertexCount", 0.1f);
            m_showTable.AddColumn("triangleCount", "TriangleCount", 0.1f);

            m_dataTable.OnSelected += OnDataSelected;
            m_showTable.OnSelected += OnInfoSelected;
        }

        public void OnDataSelected(object selected, int col)
        {
            ModelOverviewData overViewData = selected as ModelOverviewData;
            if (overViewData == null)
                return;
            if (m_showTable != null)
            {
                m_showTable.RefreshData(overViewData.GetObjects());
            }
        }

        public void OnInfoSelected(object selected, int col)
        {
            ModelInfo modelInfo = selected as ModelInfo;
            if (modelInfo == null)
                return;
            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(modelInfo.Path, typeof(UnityEngine.Object));
            EditorGUIUtility.PingObject(obj);
            Selection.activeObject = obj;
        }

        public void SwitchMode(int mode)
        {
            m_mode = (ModelOverviewMode)mode;
            if (!m_flag[mode] && m_modelInfoList != null)
            {
                m_flag[mode] = true;

                List<object> data = m_data[mode];
                for (int i = 0; i < m_modelInfoList.Count; ++i)
                {
                    ModelInfo modelInfo = m_modelInfoList[i];

                    EditorUtility.DisplayProgressBar("刷新数据",
                        System.IO.Path.GetFileName(modelInfo.Path), (i * 1.0f) / m_modelInfoList.Count);
                    bool find = false;
                    for (int j = 0; j < data.Count; ++j)
                    {
                        ModelOverviewData overViewData = data[j] as ModelOverviewData;
                        if (overViewData.IsMatch(modelInfo))
                        {
                            find = true;
                            overViewData.AddObject(modelInfo);
                            break;
                        }
                    }

                    if (!find)
                    {
                        ModelOverviewData overViewData = ModelOverviewData.CreateNew(m_mode, modelInfo);
                        overViewData.AddObject(modelInfo);
                        data.Add(overViewData);
                    }
                }

                EditorUtility.ClearProgressBar();
            }

            if (m_dataTable != null)
            {
                ModelOverviewData.SwitchDataTableMode(m_mode, m_dataTable);
                m_dataTable.RefreshData(m_data[mode]);
            }
        }

        public void RefreshData()
        {
            for (int i = 0; i < m_data.Length; ++i)
            {
                m_data[i].Clear();
                m_flag[i] = false;
            }

            m_modelInfoList = ModelInfo.GetModelInfoByDirectory("Assets/" + m_rootPath);
            SwitchMode((int)m_mode);
        }

        public void Draw(Rect r)
        {
            int border = TableConst.TableBorder;
            float split = TableConst.SplitterRatio;
            int toolbarHeight = 50;

            //GUILayout.BeginArea(r);
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal(TableStyles.Toolbar);
            {
                GUILayout.Label("RootPath: ", GUILayout.Width(100));
                string rootPath = EditorGUILayout.TextField(m_rootPath, TableStyles.TextField, GUILayout.Width(360));
                if (rootPath != m_rootPath)
                {
                    m_rootPath = rootPath;
                }
                if (GUILayout.Button("Refresh Data", TableStyles.ToolbarButton, GUILayout.MaxWidth(120)))
                {
                    RefreshData();
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(TableStyles.Toolbar);
            {
                GUILayout.Label("Mode: ", GUILayout.Width(100));
                int selAlphaMode = GUILayout.SelectionGrid((int)m_mode,
                    OverviewTableConst.ModelModeName, OverviewTableConst.ModelModeName.Length, TableStyles.ToolbarButton);
                if (selAlphaMode != (int)m_mode)
                {
                    SwitchMode(selAlphaMode);
                }
            }
            GUILayout.EndHorizontal();

            int startY = toolbarHeight + border;
            int height = (int)(r.height - startY - 5);
            if (m_dataTable != null)
            {
                m_dataTable.Draw(new Rect(
                    border, startY,
                    r.width * split - 1.5f * border,
                    height));
            }

            if (m_showTable != null)
            {
                m_showTable.Draw(new Rect(
                    r.width * split + 0.5f * border,
                    startY,
                    r.width * (1.0f - split) - 1.5f * border,
                    height));
            }

            GUILayout.EndVertical();
            //GUILayout.EndArea();
        }

        protected ModelOverviewMode m_mode = ModelOverviewMode.ReadWrite;
        protected string m_rootPath = string.Empty;
        protected TableView m_dataTable;
        protected TableView m_showTable;
        protected List<object>[] m_data;
        protected List<ModelInfo> m_modelInfoList;
        protected bool[] m_flag;
    }
}