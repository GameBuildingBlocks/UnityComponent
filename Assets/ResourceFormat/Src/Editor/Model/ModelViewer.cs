using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using EditorCommon;

namespace ResourceFormat
{
    public class ModelViewer : BaseViewer
    {
        private ModelDataControl m_control;
        public ModelViewer(EditorWindow hostWindow)
        {
            // m_hostWindow = hostWindow;

            // create the table with a specified object type
            m_dataTable = new TableView(hostWindow, typeof(ModelImportData));

            // setup the description for content
            m_dataTable.AddColumn("RootPath", "PackageRootPath", 0.2f, TextAnchor.MiddleLeft);
            m_dataTable.AddColumn("FileNameMatch", "Name", 0.05f);
            m_dataTable.AddColumn("Index", "Priority", 0.05f);
            m_dataTable.AddColumn("TotalCount", "Count", 0.05f);
            m_dataTable.AddColumn("TotalMemuse", "Memory", 0.05f, TextAnchor.MiddleCenter, "<fmt_bytes>");
            m_dataTable.AddColumn("ReadWriteEnable", "R/W", 0.05f);
            m_dataTable.AddColumn("ImportUV2", "uv2", 0.05f);
            m_dataTable.AddColumn("ImportUV3", "uv3", 0.05f);
            m_dataTable.AddColumn("ImportUV4", "uv4", 0.05f);
            m_dataTable.AddColumn("ImportNormal", "normal", 0.05f);
            m_dataTable.AddColumn("ImportTangent", "tangent", 0.05f);
            m_dataTable.AddColumn("MeshCompression", "MeshCompress", 0.075f);
            m_dataTable.AddColumn("OptimizeMesh", "OptimizeMesh", 0.075f);
            m_dataTable.AddColumn("ImportAnimation", "ImportAnimation", 0.075f);
            m_dataTable.AddColumn("ImportMaterials", "ImportMaterials", 0.075f);

            // sorting
            m_dataTable.SetSortParams(2, false);

            m_showTable = new TableView(hostWindow, typeof(ModelInfo));

            m_showTable.AddColumn("Path", "Path", 0.4f, TextAnchor.MiddleLeft);
            m_showTable.AddColumn("MemSize", "Memory", 0.1f, TextAnchor.MiddleCenter, "<fmt_bytes>");
            m_showTable.AddColumn("MeshCompression", "MeshCompress", 0.1f);
            m_showTable.AddColumn("ReadWriteEnable", "R/W", 0.1f);
            m_showTable.AddColumn("OptimizeMesh", "OptimizeMesh", 0.1f);
            m_showTable.AddColumn("ImportAnimation", "ImportAnimation", 0.1f);
            m_showTable.AddColumn("ImportMaterials", "ImportMaterials", 0.1f);

            m_showTable.SetSortParams(1, true);

            m_control = new ModelDataControl(m_dataTable, m_showTable);

            // register the event-handling function
            m_dataTable.OnSelected += m_control.OnDataSelected;
            m_showTable.OnSelected += m_control.OnInfoSelected;
        }
        public override void Draw(Rect r)
        {
            int border = TableConst.TableBorder;
            float split = TableConst.SplitterRatio;
            int toolbarHeight = 80;

            GUILayout.BeginArea(r);
            GUILayout.BeginVertical();

            m_control.Draw();

            ModelImportData data = m_control.Data;

            GUILayout.BeginHorizontal(TableStyles.Toolbar);
            {
                GUILayout.Label("MeshCompression: ", GUILayout.Width(130));
                int meshCompress = GUILayout.SelectionGrid((int)data.MeshCompression,
                    TableConst.MeshCompression, TableConst.MeshCompression.Length, TableStyles.ToolbarButton);
                if (meshCompress != (int)data.MeshCompression)
                {
                    data.MeshCompression = (ModelImporterMeshCompression)(meshCompress);
                }

                bool readWriteEnable = GUILayout.Toggle(data.ReadWriteEnable, " ReadWriteEnable");
                if (readWriteEnable != data.ReadWriteEnable)
                {
                    data.ReadWriteEnable = readWriteEnable;
                }
                bool optimizeMesh = GUILayout.Toggle(data.OptimizeMesh, " OptimizeMesh");
                if (optimizeMesh != data.OptimizeMesh)
                {
                    data.OptimizeMesh = optimizeMesh;
                }
                bool importMaterials = GUILayout.Toggle(data.ImportMaterials, " ImportMaterials");
                if (importMaterials != data.ImportMaterials)
                {
                    data.ImportMaterials = importMaterials;
                }
                bool importAnimation = GUILayout.Toggle(data.ImportAnimation, " ImportAnimation");
                if (importAnimation != data.ImportAnimation)
                {
                    data.ImportAnimation = importAnimation;
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(TableStyles.Toolbar);
            {
                bool importUV2 = GUILayout.Toggle(data.ImportUV2, " ImportUV2");
                if (importUV2 != data.ImportUV2)
                {
                    data.ImportUV2 = importUV2;
                }
                bool importUV3 = GUILayout.Toggle(data.ImportUV3, " ImportUV3");
                if (importUV3 != data.ImportUV3)
                {
                    data.ImportUV3 = importUV3;
                }
                bool importUV4 = GUILayout.Toggle(data.ImportUV4, " ImportUV4");
                if (importUV4 != data.ImportUV4)
                {
                    data.ImportUV4 = importUV4;
                }
                bool importNormal = GUILayout.Toggle(data.ImportNormal, " ImportNormal");
                if (importNormal != data.ImportNormal)
                {
                    data.ImportNormal = importNormal;
                }
                bool importTangent = GUILayout.Toggle(data.ImportTangent, " ImportTangent");
                if (importTangent != data.ImportTangent)
                {
                    data.ImportTangent = importTangent;
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(TableStyles.Toolbar);
            {
                bool showUnFormatObject = GUILayout.Toggle(m_control.UnFormat, " Show Unformat Data");
                if (showUnFormatObject != m_control.UnFormat)
                {
                    m_control.UnFormat = showUnFormatObject;
                    m_control.OnDataSelectedIndex();
                }

                if (GUILayout.Button("Move Up Priority", TableStyles.ToolbarButton, GUILayout.MaxWidth(120)))
                {
                    m_control.ModifDataPriority(true);
                }

                if (GUILayout.Button("Move Down Priority", TableStyles.ToolbarButton, GUILayout.MaxWidth(120)))
                {
                    m_control.ModifDataPriority(false);
                }

                if (GUILayout.Button("Apply Select Format", TableStyles.ToolbarButton, GUILayout.MaxWidth(160)))
                {
                    ModelFormater.ApplyFormatToObject(m_control.SelectData);
                }

                if (GUILayout.Button("Refresh Select Data", TableStyles.ToolbarButton, GUILayout.MaxWidth(140)))
                {
                    if (m_control.Index != -1)
                    {
                        m_control.RefreshDataByRootPath(m_control.SelectData.RootPath);
                    }
                }

                if (GUILayout.Button("Refresh All Data", TableStyles.ToolbarButton, GUILayout.MaxWidth(140)))
                {
                    m_control.RefreshBaseData();
                }

                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();

            int startY = toolbarHeight + border;
            int height = (int)(r.height - startY - border * 2);
            if (m_dataTable != null)
            {
                m_dataTable.Draw(new Rect(border, startY, r.width - 2 * border, (int)(height * split - border * 1.5f)));
            }

            if (m_showTable != null)
            {
                m_showTable.Draw(new Rect(border, (int)(height * split + border * 0.5f + startY), r.width - 2 * border, (int)(height * (1.0f - split) - border * 1.5f)));
            }

            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
    }
}