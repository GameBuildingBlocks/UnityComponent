using EditorCommon;
using System.Collections.Generic;
using UnityEditor;

namespace ResourceFormat
{
    public class TextureDataControl : FormatDataControl<TextureImportData>
    {
        public TextureDataControl(TableView dataTable, TableView showTable)
        {
            m_dataTable = dataTable;
            m_showTable = showTable;

            m_dataList = ConfigData.TextureImportData;
            base.RefreshDataWithSelect();
        }

        public bool UnFormat
        {
            get { return m_showUnformatObject; }
            set { m_showUnformatObject = value; }
        }

        protected override void _RefreshList(List<string> list)
        {
            m_texInfoList = new List<TextureInfo>();
            for (int i = 0; i < list.Count; ++i)
            {
                string path = EditorPath.FormatAssetPath(list[i]);
                string name = System.IO.Path.GetFileName(path);
                EditorUtility.DisplayProgressBar("获取贴图数据", name, (i * 1.0f) / list.Count);
                if (!EditorPath.IsTexture(path))
                    continue;
                TextureInfo texInfo = TextureInfo.CreateTextureInfo(path);
                if (texInfo != null)
                {
                    m_texInfoList.Add(texInfo);
                }
            }
            EditorUtility.ClearProgressBar();

            RefreshDataWithSelect();
        }
        public override void RefreshDataWithSelect()
        {
            base.RefreshDataWithSelect();
            if (m_texInfoList != null)
            {
                for (int i = 0; i < m_texInfoList.Count; ++i)
                {
                    string name = System.IO.Path.GetFileName(m_texInfoList[i].Path);
                    EditorUtility.DisplayProgressBar("更新贴图表数据", name, (i * 1.0f) / m_texInfoList.Count);
                    for (int j = m_dataList.Count - 1; j >= 0; --j)
                    {
                        if (m_dataList[j].IsMatch(m_texInfoList[i].Path))
                        {
                            m_dataList[j].AddObject(m_texInfoList[i]);
                            break;
                        }
                    }
                }
                EditorUtility.ClearProgressBar();
            }

        }
        public override void OnDataSelected(object selected, int col)
        {
            TextureImportData texSelectData = selected as TextureImportData;
            if (texSelectData == null)
                return;

            m_editorData.CopyData(texSelectData);
            m_index = texSelectData.Index;
            ;
            if (texSelectData != null)
            {
                m_showTable.RefreshData(texSelectData.GetObjects(m_showUnformatObject));
            }
        }

        private bool m_showUnformatObject = false;
        private List<TextureInfo> m_texInfoList = new List<TextureInfo>();
    }
}
