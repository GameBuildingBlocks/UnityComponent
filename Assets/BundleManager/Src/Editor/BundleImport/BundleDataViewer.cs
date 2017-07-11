using UnityEditor;
using UnityEngine;

namespace BundleManager
{
    public class BundleDataViewer
    {
        public BundleDataViewer(EditorWindow hostWindow)
        {
            m_dataTable = new TableView(hostWindow, typeof(BundleImportData));

            // setup the description for content
            m_dataTable.AddColumn("RootPath", "RootPath", 0.30f, TextAnchor.MiddleLeft);
            m_dataTable.AddColumn("FileNameMatch", "Name", 0.15f);
            m_dataTable.AddColumn("Index", "Priority", 0.05f);
            m_dataTable.AddColumn("Publish", "Publish", 0.05f);
            m_dataTable.AddColumn("PushDependice", "Push", 0.05f);
            m_dataTable.AddColumn("Type", "Type", 0.10f);
            m_dataTable.AddColumn("LoadState", "LoadState", 0.10f);
            m_dataTable.AddColumn("LimitCount", "LimitCount", 0.05f);
            m_dataTable.AddColumn("LimitKBSize", "LimitKBSize", 0.05f);
            m_dataTable.AddColumn("SkipData", "SkipData", 0.05f);
            m_dataTable.AddColumn("TotalCount", "Count", 0.05f);

            // sorting
            m_dataTable.SetSortParams(2, false);

            m_showTable = new TableView(hostWindow, typeof(AssetPathInfo));

            m_showTable.AddColumn("Path", "Path", 1.00f, TextAnchor.MiddleLeft);

            //m_showTable.SetSortParams(1, true);

            m_control = BundleDataControl.Instance;
            m_control.SetViewer(m_dataTable, m_showTable);

            // register the event-handling function
            m_dataTable.OnSelected += m_control.OnDataSelected;
            m_showTable.OnSelected += m_control.OnInfoSelected;
        }
        public void Draw(Rect r) {
            int border = 10;
            float split = 0.8f;
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
                data.LoadState = (BundleLoadState)EditorGUILayout.EnumPopup(data.LoadState, TableStyles.ToolbarButton);
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
                if (GUILayout.Button("Move Up Priority", TableStyles.ToolbarButton, GUILayout.MaxWidth(120))) {
                    m_control.ModifDataPriority(true);
                }

                if (GUILayout.Button("Move Down Priority", TableStyles.ToolbarButton, GUILayout.MaxWidth(120))) {
                    m_control.ModifDataPriority(false);
                }

                if (GUILayout.Button("Refresh Path Data", TableStyles.ToolbarButton, GUILayout.MaxWidth(140))) {
                    m_control.RefreshBaseData();
                }

                if (GUILayout.Button("Create Bundle", TableStyles.ToolbarButton, GUILayout.MaxWidth(140))) {
                    BundleAdapter.CreateBundles();
                }

//                 if (GUILayout.Button("Update By PackageDiff", TableStyles.ToolbarButton, GUILayout.MaxWidth(140))) {
//                     PackagesDiff.LoadData();
//                     BundleAdapter.UpdateBundleBuildList(PackagesDiff.IsNeedUpdatePath);
//                 }

                if (GUILayout.Button("Build Bundle", TableStyles.ToolbarButton, GUILayout.MaxWidth(140))) {
                    BundleAdapter.BuildBundles();
                }

                if (GUILayout.Button("Build Bundle All", TableStyles.ToolbarButton, GUILayout.MaxWidth(140))) {
                    //BundleAdapter.UpdateBundleBuildList(null);
                    //BundleAdapter.BuildBundles();
                }

                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();

            int startY = toolbarHeight + border;
            int height = (int)(r.height - startY - border * 2);
            if (m_dataTable != null) {
                m_dataTable.Draw(new Rect(border, startY, r.width - border, (int)(height * split - border * 1.5f)));
            }

            if (m_showTable != null) {
                m_showTable.Draw(new Rect(border, (int)(height * split + border * 0.5f + startY), r.width - border, (int)(height * (1.0f - split) - border * 1.5f)));
            }

            GUILayout.EndVertical();
            //GUILayout.EndArea();
        }

        private BundleDataControl m_control;
        private TableView m_dataTable;
        private TableView m_showTable;
    }
}