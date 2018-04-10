using EditorCommon;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ResourceFormat
{
    public class AnimationDataControl : FormatDataControl<AnimationImportData>
    {
        public AnimationDataControl(TableView dataTable, TableView showTable)
        {
            m_dataTable = dataTable;
            m_showTable = showTable;

            m_dataList = ConfigData.AnimationImportData;
            for (int i = 0; i < m_dataList.Count; ++i)
            {
                m_dataList[i].ClearObject();
            }

            if (m_dataTable != null)
            {
                m_dataTable.RefreshData(EditorTool.ToObjectList<AnimationImportData>(m_dataList));
            }
        }

        public bool UnFormat
        {
            get { return m_showUnformatObject; }
            set { m_showUnformatObject = value; }
        }

        protected override void _RefreshList(List<string> list)
        {
            m_aniInfo = new List<AnimationInfo>();
            for (int i = 0; i < list.Count; ++i)
            {
                string path = EditorPath.FormatAssetPath(list[i]);
                string name = System.IO.Path.GetFileName(path);
                EditorUtility.DisplayProgressBar("获取动作数据", name, (i * 1.0f) / list.Count);
                if (!EditorPath.IsAnimation(path))
                    continue;
                AnimationInfo aniInfo = AnimationInfo.CreateAnimationInfo(path);
                if (aniInfo != null)
                {
                    m_aniInfo.Add(aniInfo);
                }
            }
            EditorUtility.ClearProgressBar();
            RefreshDataWithSelect();
        }

        public override void RefreshDataWithSelect()
        {
            base.RefreshDataWithSelect();

            if (m_aniInfo != null)
            {
                for (int i = 0; i < m_aniInfo.Count; ++i)
                {
                    string name = System.IO.Path.GetFileName(m_aniInfo[i].Path);
                    EditorUtility.DisplayProgressBar("更新动作表数据", name, (i * 1.0f) / m_aniInfo.Count);
                    for (int j = m_dataList.Count - 1; j >= 0; --j)
                    {
                        if (m_dataList[j].IsMatch(m_aniInfo[i].Path))
                        {
                            m_dataList[j].AddObject(m_aniInfo[i]);
                            break;
                        }
                    }
                }
                EditorUtility.ClearProgressBar();
            }

        }
        public override void OnDataSelected(object selected, int col)
        {
            AnimationImportData aniSelectData = selected as AnimationImportData;
            if (aniSelectData == null)
                return;

            m_editorData.CopyData(aniSelectData);
            m_index = aniSelectData.Index;
            ;
            if (aniSelectData != null)
            {
                m_showTable.RefreshData(aniSelectData.GetObjects(m_showUnformatObject));
            }
        }

        private bool m_showUnformatObject = false;
        private List<AnimationInfo> m_aniInfo = new List<AnimationInfo>();
    }
}
