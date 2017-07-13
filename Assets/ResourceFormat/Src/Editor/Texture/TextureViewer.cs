using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using EditorCommon;

namespace ResourceFormat
{
    public class TextureViewer : BaseViewer
    {
        public TextureViewer(EditorWindow hostWindow)
        {
            m_dataTable = new TableView(hostWindow, typeof(TextureImportData));

            m_dataTable.AddColumn("RootPath", "PackageRootPath", 0.26f, TextAnchor.MiddleLeft);
            m_dataTable.AddColumn("FileNameMatch", "Name", 0.05f);
            m_dataTable.AddColumn("Index", "Priority", 0.04f);
            m_dataTable.AddColumn("TotalCount", "Count", 0.04f);
            m_dataTable.AddColumn("TotalMemuse", "Memory", 0.04f, TextAnchor.MiddleCenter, "<fmt_bytes>");
            m_dataTable.AddColumn("TexType", "TextureType", 0.08f);
            m_dataTable.AddColumn("ShapeType", "Shape", 0.04f);
            m_dataTable.AddColumn("AlphaMode", "AlphaMode", 0.05f);
            m_dataTable.AddColumn("AndroidFormat", "AndroidFormat", 0.08f);
            m_dataTable.AddColumn("IosFormat", "IosFormat", 0.08f);
            m_dataTable.AddColumn("MaxSize", "MaxSize", 0.04f);
            m_dataTable.AddColumn("ReadWriteEnable", "R/W", 0.04f);
            m_dataTable.AddColumn("MipmapEnable", "Mipmap", 0.04f);
            m_dataTable.AddColumn("PreBuild", "PreBuild", 0.04f);
            m_dataTable.AddColumn("ForceSet", "Force", 0.04f);
            m_dataTable.AddColumn("AlwaysMatch", "Always", 0.04f);

            m_dataTable.SetSortParams(2, false);

            m_showTable = new TableView(hostWindow, typeof(TextureInfo));

            m_showTable.AddColumn("Path", "Path", 0.45f, TextAnchor.MiddleLeft);
            m_showTable.AddColumn("MemSize", "Memory", 0.05f, TextAnchor.MiddleCenter, "<fmt_bytes>");
            m_showTable.AddColumn("ReadWriteEnable", "R/W", 0.05f);
            m_showTable.AddColumn("MipmapEnable", "Mipmap", 0.05f);
            m_showTable.AddColumn("AndroidFormat", "AndroidFormat", 0.1f);
            m_showTable.AddColumn("IosFormat", "IosFormat", 0.1f);
            m_showTable.AddColumn("ImportType", "ImportType", 0.1f);
            m_showTable.AddColumn("ImportShape", "ImportShape", 0.1f);

            m_showTable.SetSortParams(1, true);

            m_control = new TextureDataControl(m_dataTable, m_showTable);

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

            TextureImportData data = m_control.Data;

            GUILayout.BeginHorizontal(TableStyles.Toolbar);
            {
                GUILayout.Label("TextureType: ", GUILayout.Width(130));
                int selType = GUILayout.SelectionGrid(data.TextureTypeIndex,
                    TableConst.TextureType, TableConst.TextureType.Length, TableStyles.ToolbarButton);
                if (selType != data.TextureTypeIndex)
                {
                    data.TextureTypeIndex = selType;
                }

                GUILayout.Label("TextureShape: ", GUILayout.Width(130));
                int selShape = GUILayout.SelectionGrid(data.TextureShapeIndex,
                    TableConst.TextureShape, TableConst.TextureShape.Length, TableStyles.ToolbarButton);
                if (selShape != data.TextureShapeIndex)
                {
                    data.TextureShapeIndex = selShape;
                }

                GUILayout.Label("MaxSize: ", GUILayout.Width(60));
                int preIndex = ArrayUtility.IndexOf(TableConst.MaxSizeInt, data.MaxSize);
                int selIndex = EditorGUILayout.Popup(preIndex, TableConst.MaxSize, TableStyles.ToolbarButton, GUILayout.MaxWidth(60));
                if (selIndex != preIndex && selIndex >= 0 && selIndex < TableConst.MaxSizeInt.Length)
                {
                    data.MaxSize = TableConst.MaxSizeInt[selIndex];
                }

                bool readWriteEnable = GUILayout.Toggle(data.ReadWriteEnable, " ReadWriteEnable");
                if (readWriteEnable != data.ReadWriteEnable)
                {
                    data.ReadWriteEnable = readWriteEnable;
                }

                bool mipMapEnable = GUILayout.Toggle(data.MipmapEnable, " MipMapEnable");
                if (mipMapEnable != data.MipmapEnable)
                {
                    data.MipmapEnable = mipMapEnable;
                }

                bool preBuildEnable = GUILayout.Toggle(data.PreBuild, " PreBuildEnable");
                if (preBuildEnable != data.PreBuild)
                {
                    data.PreBuild = preBuildEnable;
                }

                bool forceSet = GUILayout.Toggle(data.ForceSet, " ForceSet");
                if (forceSet != data.ForceSet)
                {
                    data.ForceSet = forceSet;
                }

                bool alwaysMatch = GUILayout.Toggle(data.AlwaysMatch, " AlwayMatch");
                if (alwaysMatch != data.AlwaysMatch)
                {
                    data.AlwaysMatch = alwaysMatch;
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(TableStyles.Toolbar);
            {
                GUILayout.Label("Alphamode: ", GUILayout.Width(120));
                int selAlphaMode = GUILayout.SelectionGrid((int)data.AlphaMode,
                    TableConst.AlpaMode, TableConst.AlpaMode.Length, TableStyles.ToolbarButton);
                if (selAlphaMode != (int)data.AlphaMode)
                {
                    data.AlphaMode = (TextureAlphaMode)selAlphaMode;
                }

                GUILayout.Label("AndroidFormat: ", GUILayout.Width(120));
                int selAndroidFormat = GUILayout.SelectionGrid(data.AndroidFormatIndex,
                    TableConst.AndoridFormat, TableConst.AndoridFormat.Length, TableStyles.ToolbarButton);
                if (selAndroidFormat != data.AndroidFormatIndex)
                {
                    data.AndroidFormatIndex = selAndroidFormat;
                }

                GUILayout.Label("IosFormat: ", GUILayout.Width(120));
                int selIosFormat = GUILayout.SelectionGrid(data.IosFormatIndex,
                    TableConst.IosFormat, TableConst.IosFormat.Length, TableStyles.ToolbarButton);
                if (selIosFormat != data.IosFormatIndex)
                {
                    data.IosFormatIndex = selIosFormat;
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
                    TextureFormater.ApplyFormatToObject(m_control.SelectData);
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

        private TextureDataControl m_control;
    }
}