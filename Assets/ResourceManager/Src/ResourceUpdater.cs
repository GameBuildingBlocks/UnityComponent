using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Profiling;
using CommonComponent;

namespace GameResource
{
    [DisallowMultipleComponent]
    public class ResourceUpdater : MonoBehaviour
    {
        public delegate void UpdaterDelegate();

        private static ResourceUpdater ms_instance = null;
        public static ResourceUpdater Instance
        {
            get
            {
                if (ms_instance != null)
                {
                    return ms_instance;
                }

                ResourceUpdater instance = ResourceUpdater.FindObjectOfType(typeof(ResourceUpdater)) as ResourceUpdater;
                if (instance == null)
                {
                    GameObject gameObject = new GameObject("UnityComponent-ResourceManager");
                    instance = gameObject.AddComponent<ResourceUpdater>();
                }

                if (instance == null)
                {
                    return null;
                }

                // load asset bundle mainfest
                ResourceMainfest.LoadMainfest();
                ms_instance = instance;
                DontDestroyOnLoad(ms_instance.gameObject);

                return ms_instance;
            }
        }

        public void RegisterUpdater(UpdaterDelegate up)
        {
            for (int i = 0; i < m_updaterDelegate.Count; ++i)
            {
                if (m_updaterDelegate[i] == up) return;
            }
            m_updaterDelegate.Add(up);
        }

        public void UnRegisterUpdater(UpdaterDelegate up)
        {
            m_updaterDelegate.Remove(up);
        }

        private List<UpdaterDelegate> m_updaterDelegate = new List<UpdaterDelegate>();

        void LateUpdate()
        {
            ResourceTimeControl.Reset();
            for (int i = 0; i < m_updaterDelegate.Count; ++i)
            {
                try
                {
                    m_updaterDelegate[i]();
                }
                catch (System.Exception ex)
                {
                    Log.Exception(ex);
                }
            }
        }
    }
}