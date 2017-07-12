using UnityEngine;
using UnityEngine.Profiling;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace BundleManager
{
    public class BundleBuildWin : EditorWindow
    {
        [MenuItem("Window/BundleBuildWin")]
        public static void Create()
        {
            BundleBuildWin bundleManagerWin = EditorWindow.GetWindow<BundleBuildWin>();
            bundleManagerWin.minSize = new Vector2(1000, 800);
        }

        void OnEnable()
        {
            if (m_bundleDataViewer == null)
            {
                BundleDataManager.Init();
                m_bundleDataViewer = new BundleViewer(this);
            }
        }

        void OnGUI()
        {
            if (m_bundleDataViewer != null)
            {
                Rect viewRect = new Rect(0, 10, position.width, position.height - 10);
                m_bundleDataViewer.Draw(viewRect);
            }
        }

        private BundleViewer m_bundleDataViewer = null;
    }
}