using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace ResourceFormat {
    public enum FormatMode {
        Texture,
        Model,
        Animation,
    }

    public class ViewerMgr {

        public ViewerMgr(EditorWindow host) {
            int curMode = EditorPrefs.GetInt(TableConst.CurrentMode);
            m_currentMode = (FormatMode)(curMode);

            m_modes = new Dictionary<FormatMode, BaseViewer>() 
            {
                { FormatMode.Texture, new TextureViewer(host)},
                { FormatMode.Model, new ModelViewer(host)},
                { FormatMode.Animation, new AnimationViewer(host)},
            };
        }

        public BaseViewer GetCurrentMode() {
            BaseViewer view;
            if (!m_modes.TryGetValue(m_currentMode, out view)) {
                return null;
            }
            return view;
        }

        public void SwitchTo(FormatMode newMode) {
            if (m_currentMode == newMode) {
                return;
            }

            BaseViewer preViewer = GetCurrentMode();
            if (preViewer != null) {
                preViewer.OnLeave();
            }

            m_currentMode = newMode;

            BaseViewer curViewer = GetCurrentMode();
            if (curViewer != null) {
                curViewer.OnLeave();
            }

            EditorPrefs.SetInt(TableConst.CurrentMode, (int)(m_currentMode));
        }

        public void OnGUI(Rect rect) {
            GUILayout.BeginHorizontal(TableStyles.Toolbar);
            GUILayout.Label("Mode: ", GUILayout.MaxWidth(60));
            int selMode = GUILayout.SelectionGrid((int)m_currentMode,
                TableConst.Modes, TableConst.Modes.Length, TableStyles.ToolbarButton);
            if (selMode != (int)m_currentMode) {
                SwitchTo((FormatMode)selMode);
            }
            GUILayout.EndHorizontal();

            float yOffset = TableConst.TopBarHeight;
            Rect viewRect = new Rect(0, yOffset, rect.width, rect.height - yOffset);
            BaseViewer viewTable = GetCurrentMode();
            if (viewTable != null) {
                viewTable.Draw(viewRect);
            }
        }

        private FormatMode m_currentMode = FormatMode.Texture;
        private Dictionary<FormatMode, BaseViewer> m_modes = null;
    }

}