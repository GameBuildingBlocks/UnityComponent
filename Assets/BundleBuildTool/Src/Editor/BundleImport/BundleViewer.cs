using UnityEditor;
using UnityEngine;

namespace BundleManager
{
    public class BundleViewer
    {
        public BundleViewer(EditorWindow hostWindow)
        {
            m_dataTable = new TableView(hostWindow, typeof(BundleImportData));

            // setup the description for content
            m_dataTable.AddColumn("RootPath", "RootPath", 0.30f, TextAnchor.MiddleLeft);
            m_dataTable.AddColumn("FileNameMatch", "Name", 0.15f);
            m_dataTable.AddColumn("Index", "Order", 0.05f);
            m_dataTable.AddColumn("Publish", "Publish", 0.05f);
            m_dataTable.AddColumn("PushDependice", "Push", 0.05f);
            m_dataTable.AddColumn("Type", "Type", 0.08f);
            m_dataTable.AddColumn("LoadState", "LoadState", 0.08f);
            m_dataTable.AddColumn("LimitCount", "LimitCount", 0.07f);
            m_dataTable.AddColumn("LimitKBSize", "LimitSize", 0.07f);
            m_dataTable.AddColumn("SkipData", "SkipData", 0.05f);
            m_dataTable.AddColumn("TotalCount", "Count", 0.05f);

            // sorting
            m_dataTable.SetSortParams(2, false);

            m_bundleTable = new TableView(hostWindow, typeof(BundleData));

            m_bundleTable.AddColumn("name", "Name", 0.30f, TextAnchor.MiddleLeft);
            m_bundleTable.AddColumn("type", "Type", 0.30f);
            m_bundleTable.AddColumn("loadState", "State", 0.30f);
            m_bundleTable.AddColumn("size", "Size", 0.10f, TextAnchor.MiddleCenter, "<fmt_bytes>");

            m_bundleTable.SetSortParams(0, false);

            m_assetTable = new TableView(hostWindow, typeof(AssetPathInfo));
            m_assetTable.AddColumn("Path", "Path", 1.00f, TextAnchor.MiddleLeft);

            m_control = BundleDataControl.Instance;
            m_control.SetViewer(m_dataTable, m_bundleTable, m_assetTable);

            // register the event-handling function
            m_dataTable.OnSelected += m_control.OnDataSelected;
            m_bundleTable.OnSelected += m_control.OnBundleSelected;
            m_assetTable.OnSelected += m_control.OnAssetSelected;
        }
        public void Draw(Rect r)
        {
            int border = 10;
            float splitH = 0.4f;
            float splitW = 0.5f;
            int toolbarHeight = 80;
            float spacePixel = 10.0f;

            //GUILayout.BeginArea(r);
            GUILayout.BeginVertical();

            m_control.Draw();

            BundleImportData data = m_control.Data;

            GUILayout.BeginHorizontal(TableStyles.Toolbar);
            {
                EditorGUILayout.LabelField("LimitCount : ");
                data.LimitCount = EditorGUILayout.IntField(data.LimitCount);

                GUILayout.Space(spacePixel);

                EditorGUILayout.LabelField("LimitKBSize : ");
                data.LimitKBSize = EditorGUILayout.IntField(data.LimitKBSize);

                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(TableStyles.Toolbar);
            {
                EditorGUILayout.LabelField("BundleType : ");
                data.Type = (BundleType)EditorGUILayout.EnumPopup(data.Type);
                GUILayout.Space(spacePixel);
                EditorGUILayout.LabelField("LoadState: ");
                data.LoadState = (BundleLoadState)EditorGUILayout.EnumPopup(data.LoadState);
                GUILayout.Space(spacePixel);
                data.Publish = GUILayout.Toggle(data.Publish, " Publish");
                GUILayout.Space(spacePixel);
                data.PushDependice = GUILayout.Toggle(data.PushDependice, " Push");
                GUILayout.Space(spacePixel);
                data.SkipData = GUILayout.Toggle(data.SkipData, " SkipData");
                GUILayout.FlexibleSpace();

            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(TableStyles.Toolbar);
            {
                if (GUILayout.Button("Move Up Priority", TableStyles.ToolbarButton, GUILayout.MaxWidth(120)))
                {
                    m_control.ModifDataPriority(true);
                }

                if (GUILayout.Button("Move Down Priority", TableStyles.ToolbarButton, GUILayout.MaxWidth(120)))
                {
                    m_control.ModifDataPriority(false);
                }

                if (GUILayout.Button("Create Bundle", TableStyles.ToolbarButton, GUILayout.MaxWidth(140)))
                {
                    m_control.RefreshBaseData();
                    BundleAdapter.CreateBundles();
                }

                if (GUILayout.Button("Build All Bundle", TableStyles.ToolbarButton, GUILayout.MaxWidth(140)))
                {
                    BundleAdapter.UpdateBundleBuildList(null);
                    BundleAdapter.BuildBundles();
                }

                if (GUILayout.Button("Clear All Bundle", TableStyles.ToolbarButton, GUILayout.MaxHeight(140)))
                {
                    BundleDataManager.RemoveAllBundle();
                }

                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();

            int startY = toolbarHeight + border;
            int height = (int)(r.height - startY - border * 2);
            if (m_dataTable != null)
            {
                m_dataTable.Draw(new Rect(border, startY,
                    r.width - border, (int)(height * splitH - border * 1.5f)));
            }

            if (m_bundleTable != null)
            {
                m_bundleTable.Draw(new Rect(border,
                    (int)(height * splitH + border * 0.5f + startY),
                    (int)(r.width * splitW) - border * 1.5f,
                    (int)(height * (1.0f - splitH) - border * 1.5f)));
            }

            if (m_assetTable != null)
            {
                m_assetTable.Draw(new Rect((int)(r.width * splitW) + border * 0.5f,
                    (int)(height * splitH + border * 0.5f + startY),
                    (int)(r.width * (1.0f - splitW)) - border * 1.5f,
                    (int)(height * (1.0f - splitH) - border * 1.5f)));
            }

            GUILayout.EndVertical();
            //GUILayout.EndArea();
        }

        private BundleDataControl m_control;
        private TableView m_dataTable;
        private TableView m_bundleTable;
        private TableView m_assetTable;
    }
}