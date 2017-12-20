using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using CommonComponent;

namespace GameResource
{
    internal class ResourceLoadRequest : RequestBase<ResourceData>
    {
        private readonly System.Type m_type = default(System.Type);
        private AssetBundle m_assetBundle = null;
        private AssetBundleRequest m_assetBundleRequest = null;
        private ResourceRequest m_resourceRequest = null;
        private LinkedList<ResourceLoadHandle> m_loadList = new LinkedList<ResourceLoadHandle>();

        public System.Type type
        {
            get { return m_type; }
        }

        public ResourceLoadRequest(ulong id, System.Type type, int priority)
            : base(id)
        {
            m_type = type;
            m_priority = priority;
        }

        public override bool TryLoad()
        {
            if (ResourceMainfest.IsAssetBundleResource(Id))
            {
                m_assetBundle = AssetBundleManager.Instance.Load(Id);
            }

            if (m_assetBundle != null)
            {
                string name = ResourceMainfest.GetPathName(Id);
                if (!string.IsNullOrEmpty(name))
                {
                    m_assetBundleRequest = m_assetBundle.LoadAssetAsync(name, m_type);
                    if (m_assetBundleRequest != null)
                    {
                        m_assetBundleRequest.priority = m_priority;
                    }
                }
                if (m_assetBundleRequest == null)
                {
                    return false;
                }
            }
            else
            {
                string path = ResourceMainfest.GetHashPath(Id);
                if (!string.IsNullOrEmpty(path))
                {
                    m_resourceRequest = Resources.LoadAsync(path, m_type);
                    if (m_resourceRequest != null)
                    {
                        m_resourceRequest.priority = m_priority;
                    }
                }
                if (m_resourceRequest == null)
                {
                    return false;
                }
            }

            return base.TryLoad();
        }

        public override bool IsDone()
        {
            if (m_assetBundleRequest != null)
            {
                return m_assetBundleRequest.isDone;
            }

            if (m_resourceRequest != null)
            {
                return m_resourceRequest.isDone;
            }

            return base.IsDone();
        }

        public override bool LoadDone()
        {
            Object asset = null;
            if (m_assetBundleRequest != null)
            {
                asset = m_assetBundleRequest.asset;
            }
            else if (m_resourceRequest != null)
            {
                asset = m_resourceRequest.asset;
            }

            if (asset != null)
            {
                m_data = new ResourceData(Id, m_type, asset);
            }

            foreach (var loadHandle in m_loadList)
            {
                if (loadHandle.isInterrupt)
                {
                    continue;
                }

                loadHandle.isDone = true;
                loadHandle.asset = asset;
                if (m_data != null)
                {
                    m_data.AddRef();
                }
            }
            m_loadList.Clear();
            m_loadList = null;

            m_assetBundleRequest = null;
            m_resourceRequest = null;
            if (m_assetBundle != null)
            {
                AssetBundleManager.Instance.Unload(Id);
                m_assetBundle = null;
            }

            return base.LoadDone();
        }

        public void AddLoadHandle(ResourceLoadHandle loadHandle)
        {
            if (loadHandle != null)
            {
                m_loadList.AddLast(loadHandle);
            }
        }
    }
}