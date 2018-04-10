using UnityEditor;
using UnityEngine;

namespace ResourceFormat
{
    enum OverviewWinType
    {
        Texture,
        Model,
    }

    public class ResourceOverviewWin : EditorWindow
    {
        [MenuItem("UComponents/ResourceOverviewWin")]
        static void Create()
        {
            ResourceOverviewWin resourceInfoWin = EditorWindow.GetWindow<ResourceOverviewWin>();
            resourceInfoWin.minSize = new Vector2(800, 600);
        }

        void OnEnable()
        {
            if (m_texOViewer == null)
            {
                m_texOViewer = new TextureOverviewViewer(this);
                m_modelOViewer = new ModelOverviewViewer(this);
            }
        }

        void OnGUI()
        {
            if (m_texOViewer != null)
            {
                GUILayout.BeginHorizontal(TableStyles.Toolbar);
                GUILayout.Label("ResourceType: ", GUILayout.MaxWidth(100));
                int selMode = GUILayout.SelectionGrid((int)m_currentMode,
                    OverviewTableConst.OverviewModeName, OverviewTableConst.OverviewModeName.Length, TableStyles.ToolbarButton);
                if (selMode != (int)m_currentMode)
                {
                    m_currentMode = (OverviewWinType)selMode;
                }
                GUILayout.EndHorizontal();

                float yOffset = TableConst.TopBarHeight;
                Rect viewRect = new Rect(0, yOffset, position.width, position.height - yOffset);

                if (m_currentMode == OverviewWinType.Texture)
                {
                    m_texOViewer.Draw(viewRect);
                }
                else if (m_currentMode == OverviewWinType.Model)
                {
                    m_modelOViewer.Draw(viewRect);
                }
            }
        }
        private OverviewWinType m_currentMode = OverviewWinType.Texture;
        private TextureOverviewViewer m_texOViewer;
        private ModelOverviewViewer m_modelOViewer;
    }
}