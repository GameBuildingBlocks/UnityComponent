using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using EditorCommon;

namespace BundleManager
{
    public class BundleDataControl
    {
        private BundleDataControl()
        {
            m_dataList = BundleDataAccessor.ImportDatas;
            RefreshDataWithSelect();
        }

        private static BundleDataControl ms_instance = null;
        public static BundleDataControl Instance
        {
            get
            {
                if (ms_instance == null)
                {
                    ms_instance = new BundleDataControl();
                }
                return ms_instance;
            }
        }

        public void SetViewer(TableView dataTable, TableView bundleTable, TableView assetTable)
        {
            m_dataTable = dataTable;
            m_bundleTable = bundleTable;
            m_assetTable = assetTable;
            RefreshDataWithSelect();
        }

        public int Index
        {
            get { return m_index; }
        }
        public BundleImportData Data
        {
            get { return m_editorData; }
        }
        public BundleImportData SelectData
        {
            get { return m_index == -1 ? null : m_dataList[m_index]; }
        }
        public List<BundleImportData> DataList
        {
            get { return m_dataList; }
        }

        public void OnDataSelected(object selected, int col)
        {
            BundleImportData importData = selected as BundleImportData;
            if (importData == null)
                return;

            m_editorData.CopyData(importData);
            m_index = importData.Index;
            if (m_bundleTable != null)
            {
                m_bundleTable.RefreshData(BundleDataManager.GetIndexBundleList(importData.Index));
            }
        }
        public void OnBundleSelected(object selected, int col)
        {
            BundleData bundleData = selected as BundleData;
            if (bundleData == null)
                return;

            if (m_assetTable != null)
            {
                List<object> list = new List<object>();
                for (int i = 0; i < bundleData.includs.Count; ++i)
                {
                    AssetPathInfo pathInfo = AssetPathInfo.CreatePathInfo(bundleData.includs[i]);
                    if (pathInfo != null)
                    {
                        list.Add(pathInfo);
                    }
                }
                m_assetTable.RefreshData(list);
            }
        }
        public void OnAssetSelected(object selected, int col)
        {
            AssetPathInfo texInfo = selected as AssetPathInfo;
            if (texInfo == null)
                return;
            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(texInfo.Path, typeof(UnityEngine.Object));
            if (obj != null)
            {
                EditorGUIUtility.PingObject(obj);
                Selection.activeObject = obj;
            }
        }
        public void AddData()
        {
            m_editorData.Index = m_dataList.Count;
            m_dataList.Add(m_editorData);
            m_editorData = new BundleImportData();
            m_index = -1;
            BundleDataAccessor.SaveImportData();
            RefreshDataWithSelect();
        }
        public void SaveData()
        {
            if (m_index == -1)
                return;
            BundleImportData data = m_dataList[m_index];
            data.ClearObject();
            data.CopyData(m_editorData);
            OnDataSelected(data, m_index);
            BundleDataAccessor.SaveImportData();
            RefreshDataWithSelect();
        }
        public void DeleteCurrentData()
        {
            if (m_index == -1)
                return;
            m_dataList.RemoveAt(m_index);
            m_index = -1;
            m_editorData = new BundleImportData();
            BundleDataAccessor.SaveImportData();
            RefreshDataWithSelect();
        }
        public void ModifDataPriority(bool up)
        {
            if (m_index == -1)
                return;
            var temp = m_dataList[m_index];
            if (up)
            {
                if (m_index == 0)
                    return;
                m_dataList[m_index] = m_dataList[m_index - 1];
                m_dataList[m_index - 1] = temp;
            }
            else
            {
                if (m_index + 1 == m_dataList.Count)
                    return;
                m_dataList[m_index] = m_dataList[m_index + 1];
                m_dataList[m_index + 1] = temp;
            }
            BundleDataAccessor.SaveImportData();

            RefreshDataWithSelect();
            if (m_dataTable != null)
            {
                m_dataTable.SetSelected(temp);
            }
        }
        public void NewData()
        {
            m_index = -1;
            m_editorData = new BundleImportData();
        }
        public void RefreshBaseData()
        {
            List<string> list = EditorPath.GetAssetPathList(BuildConfig.ResourceRootPath);
            RefreshList(list);
        }
        public void RefreshDataByRootPath(string path)
        {
            List<string> list = EditorPath.GetAssetPathList(BuildConfig.ResourceRootPath + "/" + path);
            RefreshList(list);
        }
        private void RefreshList(List<string> list)
        {
            m_pathInfoList.Clear();
            for (int i = 0; i < list.Count; ++i)
            {
                string path = EditorPath.FormatAssetPath(list[i]);
                string name = System.IO.Path.GetFileName(path);
                EditorUtility.DisplayProgressBar("获取AssetPath数据", name, (i * 1.0f) / list.Count);
                if (EditorPath.IsMeta(path))
                    continue;
                AssetPathInfo pathInfo = AssetPathInfo.CreatePathInfo(path);
                m_pathInfoList.Add(pathInfo);
            }
            EditorUtility.ClearProgressBar();
            RefreshDataWithSelect();
        }

        public void OnDataSelectedIndex()
        {
            if (m_index == -1)
                return;
            OnDataSelected(m_dataList[m_index], m_index);
        }
        public void RefreshDataWithSelect()
        {
            for (int i = 0; i < m_dataList.Count; ++i)
            {
                m_dataList[i].ClearObject();
                m_dataList[i].Index = i;
            }

            if (m_dataTable != null)
            {
                m_dataTable.RefreshData(EditorTool.ToObjectList<BundleImportData>(m_dataList));
            }

            if (m_pathInfoList != null)
            {
                for (int i = 0; i < m_pathInfoList.Count; ++i)
                {
                    string name = System.IO.Path.GetFileName(m_pathInfoList[i].Path);
                    EditorUtility.DisplayProgressBar("更新AssetPath表数据", name, (i * 1.0f) / m_pathInfoList.Count);
                    for (int j = m_dataList.Count - 1; j >= 0; --j)
                    {
                        if (m_dataList[j].IsMatch(m_pathInfoList[i].Path))
                        {
                            m_dataList[j].AddObject(m_pathInfoList[i]);
                            break;
                        }
                    }
                }
                EditorUtility.ClearProgressBar();
            }
        }

        public void Draw()
        {
            BundleImportData data = m_editorData;

            GUILayout.BeginHorizontal(TableStyles.Toolbar);
            {
                GUILayout.Label("RootPath: ", GUILayout.Width(80));
                string rootPath = EditorGUILayout.TextField(data.RootPath, TableStyles.TextField, GUILayout.Width(360));
                if (rootPath != data.RootPath)
                {
                    data.RootPath = rootPath;
                }
                GUILayout.Label("FileName: ", GUILayout.Width(60));
                string fileName = EditorGUILayout.TextField(data.FileNameMatch, TableStyles.TextField, GUILayout.Width(150));
                if (fileName != data.FileNameMatch)
                {
                    data.FileNameMatch = fileName;
                }

                if (Index == -1)
                {
                    if (GUILayout.Button("Add Data", TableStyles.ToolbarButton, GUILayout.MaxWidth(120)))
                    {
                        AddData();
                    }
                }
                else
                {
                    if (GUILayout.Button("Save Data", TableStyles.ToolbarButton, GUILayout.MaxWidth(140)))
                    {
                        SaveData();
                    }

                    if (GUILayout.Button("Delete Current Data", TableStyles.ToolbarButton, GUILayout.MaxWidth(140)))
                    {
                        DeleteCurrentData();
                    }

                    if (GUILayout.Button("New Data", TableStyles.ToolbarButton, GUILayout.MaxWidth(120)))
                    {
                        NewData();
                    }
                }
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
        }

        public BundleImportData GetPathImportData(string path)
        {
            path = EditorPath.FormatAssetPath(path);
            path = EditorPath.NormalizePathSplash(path);
            AssetPathInfo pathInfo = AssetPathInfo.CreatePathInfo(path);
            if (pathInfo == null)
                return null;

            if (pathInfo.Index >= 0 && pathInfo.Index < m_dataList.Count)
            {
                return m_dataList[pathInfo.Index];
            }
            for (int i = m_dataList.Count - 1; i >= 0; --i)
            {
                if (m_dataList[i].IsMatch(path))
                    return m_dataList[i];
            }
            return null;
        }

        public List<string> GetPublishPackagePath()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            for (int i = 0; i < m_dataList.Count; ++i)
            {
                BundleImportData bundleImportData = m_dataList[i];
                if (bundleImportData == null || !bundleImportData.Publish)
                    continue;
                List<object> list = bundleImportData.GetObjects();
                for (int j = 0; j < list.Count; ++j)
                {
                    AssetPathInfo info = list[j] as AssetPathInfo;
                    if (info == null || dict.ContainsKey(info.Path))
                        continue;
                    dict.Add(info.Path, info.Path);
                }
            }

            return new List<string>(dict.Keys);
        }

        private TableView m_dataTable;
        private TableView m_bundleTable;
        private TableView m_assetTable;
        private int m_index = -1;
        private BundleImportData m_editorData = new BundleImportData();
        private List<BundleImportData> m_dataList = null;
        private List<AssetPathInfo> m_pathInfoList = new List<AssetPathInfo>();
    }
}