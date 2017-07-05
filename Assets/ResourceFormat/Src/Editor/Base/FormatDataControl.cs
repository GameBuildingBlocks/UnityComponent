using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace ResourceFormat {
    public class FormatDataControl<T> where T : ImportData, new() {
        public int Index {
            get { return m_index; }
            //set { m_index = value; }
        }
        public T Data {
            get { return m_editorData; }
            //set { m_editorData = value; }
        }
        public T SelectData {
            get { return m_index == -1 ? null : m_dataList[m_index]; }
        }
        public List<T> DataList {
            get { return m_dataList; }
        }

        public virtual void OnDataSelected(object selected, int col) {
            ImportData importData = selected as ImportData;
            if (importData == null) return;

            m_editorData.CopyData(importData);
            m_index = importData.Index;
            if (importData != null && m_showTable != null) {
                m_showTable.RefreshData(importData.GetObjects());
            }
        }
        public virtual void OnInfoSelected(object selected, int col) {
            BaseInfo texInfo = selected as BaseInfo;
            if (texInfo == null) return;
            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(texInfo.Path, typeof(UnityEngine.Object));
            EditorGUIUtility.PingObject(obj);
            Selection.activeObject = obj;
        }
        public virtual void AddData() {
            m_editorData.Index = m_dataList.Count;
            m_dataList.Add(m_editorData);
            m_editorData = new T();
            m_index = -1;
            ConfigData.SaveData();
            RefreshDataWithSelect();
        }
        public virtual void SaveData() {
            if (m_index == -1) return;
            T data = m_dataList[m_index];
            data.ClearObject();
            data.CopyData(m_editorData);
            OnDataSelected(data, m_index);
            ConfigData.SaveData();
            RefreshDataWithSelect();
        }
        public virtual void DeleteCurrentData() {
            if (m_index == -1) return;
            m_dataList.RemoveAt(m_index);
            m_index = -1;
            m_editorData = new T();
            ConfigData.SaveData();
            RefreshDataWithSelect();
        }
        public virtual void ModifDataPriority(bool up) {
            if (m_index == -1) return;
            var temp = m_dataList[m_index];
            if (up) {
                if (m_index == 0) return;
                m_dataList[m_index] = m_dataList[m_index - 1];
                m_dataList[m_index - 1] = temp;
            } else {
                if (m_index + 1 == m_dataList.Count) return;
                m_dataList[m_index] = m_dataList[m_index + 1];
                m_dataList[m_index + 1] = temp;
            }
            ConfigData.SaveData();

            RefreshDataWithSelect();
            if (m_dataTable != null) {
                m_dataTable.SetSelected(temp);
            }
        }
        public virtual void NewData() {
            m_index = -1;
            m_editorData = new T();
        }
        public virtual void OnDataSelectedIndex() {
            if (m_index == -1) return;
            OnDataSelected(m_dataList[m_index], m_index);
        }
        public virtual void RefreshDataWithSelect() {
            for (int i = 0; i < m_dataList.Count; ++i) {
                m_dataList[i].ClearObject();
                m_dataList[i].Index = i;
            }

            if (m_dataTable != null) {
                m_dataTable.RefreshData(EditorCommon.ToObjectList<T>(m_dataList));
            }
        }

        public virtual void Draw() {
            T data = m_editorData;

            GUILayout.BeginHorizontal(TableStyles.Toolbar);
            {
                GUILayout.Label("RootPath: ", GUILayout.Width(80));
                string rootPath = EditorGUILayout.TextField(data.RootPath, TableStyles.TextField, GUILayout.Width(360));
                if (rootPath != data.RootPath) {
                    data.RootPath = rootPath;
                }
                GUILayout.Label("FileName: ", GUILayout.Width(60));
                string fileName = EditorGUILayout.TextField(data.FileNameMatch, TableStyles.TextField, GUILayout.Width(150));
                if (fileName != data.FileNameMatch) {
                    data.FileNameMatch = fileName;
                }

                if (Index == -1) {
                    if (GUILayout.Button("Add Data", TableStyles.ToolbarButton, GUILayout.MaxWidth(120))) {
                        AddData();
                    }
                } else {
                    if (GUILayout.Button("Save Data", TableStyles.ToolbarButton, GUILayout.MaxWidth(140))) {
                        SaveData();
                    }

                    if (GUILayout.Button("Delete Current Data", TableStyles.ToolbarButton, GUILayout.MaxWidth(140))) {
                        DeleteCurrentData();
                    }

                    if (GUILayout.Button("New Data", TableStyles.ToolbarButton, GUILayout.MaxWidth(120))) {
                        NewData();
                    }
                }
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
        }

        protected TableView m_dataTable;
        protected TableView m_showTable;

        protected int m_index = -1;
        protected T m_editorData = new T();
        protected List<T> m_dataList = null;
    }
}