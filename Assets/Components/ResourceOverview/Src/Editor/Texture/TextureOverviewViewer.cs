using EditorCommon;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace ResourceFormat
{
    public class TextureOverviewViewer
    {
        public TextureOverviewViewer(EditorWindow hostWindow)
        {
            m_data = new List<object>[(int)TextureOverviewMode.Count];
            m_flag = new bool[(int)TextureOverviewMode.Count];
            for (int i = 0; i < (int)TextureOverviewMode.Count; ++i)
            {
                m_flag[i] = false;
                m_data[i] = new List<object>();
            }
            m_dataTable = new TableView(hostWindow, typeof(TextureOverviewData));

            TextureOverviewData.SwitchDataTableMode(m_mode, m_dataTable);

            m_showTable = new TableView(hostWindow, typeof(TextureInfo));
            m_showTable.AddColumn("Path", "Path", 0.8f, TextAnchor.MiddleLeft);
            m_showTable.AddColumn("MemSize", "Memory", 0.2f, TextAnchor.MiddleCenter, "<fmt_bytes>");

            m_dataTable.OnSelected += OnDataSelected;
            m_showTable.OnSelected += OnInfoSelected;
        }

        public void OnDataSelected(object selected, int col)
        {
            TextureOverviewData overViewData = selected as TextureOverviewData;
            if (overViewData == null)
                return;
            if (m_showTable != null)
            {
                m_showTable.RefreshData(overViewData.GetObjects());
            }
        }

        public void OnInfoSelected(object selected, int col)
        {
            TextureInfo texInfo = selected as TextureInfo;
            if (texInfo == null)
                return;
            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(texInfo.Path, typeof(UnityEngine.Object));
            EditorGUIUtility.PingObject(obj);
            Selection.activeObject = obj;
        }

        public void SwitchMode(int mode)
        {
            m_mode = (TextureOverviewMode)mode;
            if (!m_flag[mode] && m_texInfoList != null)
            {
                m_flag[mode] = true;

                List<object> data = m_data[mode];
                for (int i = 0; i < m_texInfoList.Count; ++i)
                { 
                    TextureInfo texInfo = m_texInfoList[i];

                    EditorUtility.DisplayProgressBar("刷新数据", System.IO.Path.GetFileName(texInfo.Path), (i * 1.0f) / m_texInfoList.Count);
                    bool find = false;
                    for (int j = 0; j < data.Count; ++j)
                    {
                        TextureOverviewData overViewData = data[j] as TextureOverviewData;
                        if (overViewData.IsMatch(texInfo))
                        {
                            find = true;
                            overViewData.AddObject(texInfo);
                            break;
                        } 
                    }

                    if (!find)
                    {
                        TextureOverviewData overViewData = TextureOverviewData.CreateNew(m_mode, texInfo);
                        overViewData.AddObject(texInfo);
                        data.Add(overViewData);
                    }
                }

                EditorUtility.ClearProgressBar();
            }

            if (m_dataTable != null)
            {
                TextureOverviewData.SwitchDataTableMode(m_mode, m_dataTable);
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

            m_texInfoList = TextureInfo.GetTextureInfoByDirectory("Assets/" + m_rootPath);
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
                    OverviewTableConst.TextureModeName, OverviewTableConst.TextureModeName.Length, TableStyles.ToolbarButton);
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

        protected TextureOverviewMode m_mode = TextureOverviewMode.ReadWrite;
        protected string m_rootPath = string.Empty;
        protected TableView m_dataTable;
        protected TableView m_showTable;
        protected List<object>[] m_data;
        protected List<TextureInfo> m_texInfoList;
        protected bool[] m_flag;
    }
}