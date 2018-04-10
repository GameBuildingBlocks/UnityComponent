using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using CommonComponent;

namespace GameResource
{
    internal class ResourceSpawnRequest : RequestBase<ResourcePool>
    {
        private ResourceLoadHandle m_resourceLoadHandle = null;
        private LinkedList<ResourceSpawnHandle> m_spawnList = new LinkedList<ResourceSpawnHandle>();

        public LinkedList<ResourceSpawnHandle> spawnHandleList
        {
            get { return m_spawnList; }
        }

        public ResourceSpawnRequest(ulong pathHash, int priority)
            : base(pathHash)
        {
            m_priority = priority;
        }

        public override bool TryLoad()
        {
            m_resourceLoadHandle = ResourceDataManager.Instance.LoadAsync(Id, typeof(GameObject), m_priority);
            if (m_resourceLoadHandle == null)
                return false;

            return base.TryLoad();
        }

        public override bool IsDone()
        {
            if (m_resourceLoadHandle != null)
            {
                if (m_resourceLoadHandle.isDone)
                {
                    Object asset = m_resourceLoadHandle.asset;
                    if (asset != null)
                    {
                        m_data = new ResourcePool(Id, asset);
                    }
                    m_resourceLoadHandle = null;

                    return true;
                }
                return m_resourceLoadHandle.isDone;
            }

            return base.IsDone();
        }

        public override bool LoadDone()
        {
            foreach (var spawnHandle in m_spawnList)
            {
                if (spawnHandle.isInterrupt || spawnHandle.isDone)
                    continue;

                spawnHandle.isDone = true;
                GameObject spawnGameObject = null;
                if (m_data != null)
                {
                    m_data.Spawn(out spawnGameObject);
                }
                spawnHandle.gameObject = spawnGameObject;
                spawnHandle.success = spawnGameObject != null;
                if (spawnGameObject != null)
                {
                    spawnHandle.spawnID = spawnGameObject.GetInstanceID();
                }
                // spawn cost much time yield it
                if (ResourceTimeControl.TimeOut())
                {
                    return false;
                }
            }

            return base.LoadDone();
        }

        public void AddSpawnHandle(ResourceSpawnHandle spawnHandle)
        {
            if (spawnHandle != null)
            {
                m_spawnList.AddLast(spawnHandle);
            }
        }
    }
}