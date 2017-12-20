using UnityEngine;


namespace GameResource
{
    public enum LoadingPriority
    {
        Low = 0,
        Normal,
        High,
    }

    public enum DespawnType
    {
        Destroy,
        Inactive,   // active = false
        Invisible,  // away from camera
    }

    public enum BackLoadingPriority
    {
        Low = 0,
        BelowNormal = 1,
        Normal = 2,
        High = 4,
    }

    public class ResourceHandleBase
    {
        public bool isDone = false;
        public float progress = 0.0f;
        public bool isInterrupt = false;
    }

    public class ResourceLoadHandle : ResourceHandleBase
    {
        private Object m_asset = null;
        public Object asset
        {
            get
            {
                Object result = m_asset;
                m_asset = null;
                return result;
            }

            set
            {
                m_asset = value;
            }
        }

        public void Clear()
        {
            if (m_asset != null)
            {
                ResourceManager.Unload(m_asset);
                m_asset = null;
            }
            isInterrupt = true;
            isDone = false;
        }
    }

    public class ResourceSpawnHandle : ResourceHandleBase
    {
        private GameObject m_gameObject = null;

        public GameObject gameObject
        {
            get
            {
                GameObject result = m_gameObject;
                // make sure owner for spawn(Despawn)
                m_gameObject = null;
                return result;
            }
            set
            {
                ResourceManager.Despawn(m_gameObject);
                m_gameObject = value;
            }
        }

        public bool success
        {
            set;
            get;
        }

        public int spawnID
        {
            set;
            get;
        }

        public void Clear()
        {
            isInterrupt = true;
            isDone = false;
            ResourceManager.Despawn(m_gameObject);
            m_gameObject = null;
        }
    }

    public struct BundleState
    {
        public int bundleId;
        public bool export;
        public bool preload;
        public bool canunload;
    }
}