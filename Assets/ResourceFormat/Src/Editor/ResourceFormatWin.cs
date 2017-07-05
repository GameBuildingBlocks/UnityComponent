using UnityEditor;
using UnityEngine;

namespace ResourceFormat {
    public class ResourceFormatWin : EditorWindow {
        private ViewerMgr m_modeMgr;

        [MenuItem("Window/ResourceFormatWin")]
        static void Create() {
            ResourceFormatWin resourceInfoWin = EditorWindow.GetWindow<ResourceFormatWin>();
            resourceInfoWin.minSize = new Vector2(1000, 800);
        }

        void OnEnable() {
            if (m_modeMgr == null) {
                m_modeMgr = new ViewerMgr(this);
            }
        }

        void OnGUI() {
            if (m_modeMgr != null) {
                m_modeMgr.OnGUI(position);
            }
        }

        public ViewerMgr GetFormatModeManager() {
            if (m_modeMgr == null) {
                m_modeMgr = new ViewerMgr(this);
            }
            return m_modeMgr;
        }
    }
}