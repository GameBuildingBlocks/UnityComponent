using UnityEngine;
using UnityEngine.Profiling;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace BundleManager
{
    public class BundleManagerWin : EditorWindow
    {
        //[MenuItem("Window/BundleManagerWin")]
        public static void Create()
        {
            BundleManagerWin bundleManagerWin = EditorWindow.GetWindow<BundleManagerWin>();
            bundleManagerWin.minSize = new Vector2(1000, 800);
        }

        void OnEnable()
        {
            if (m_bundleDataViewer == null)
            {
                m_bundleDataViewer = new BundleDataViewer(this);
            }
        }

        void OnGUI()
        {
            if (m_bundleDataViewer != null)
            {
                //Rect viewRect = new Rect(0, 10, position.width, position.height - 10);
                m_bundleDataViewer.Draw(position);
            }
        }

        private BundleDataViewer m_bundleDataViewer = null;
    }
}