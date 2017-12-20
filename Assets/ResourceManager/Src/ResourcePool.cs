using UnityEngine;
using System.Collections.Generic;
using CommonComponent;

namespace GameResource
{
    internal class ResourcePool : RefCountData
    {
        private GameObject m_asset = null;
        private LinkedList<GameObject> m_objectList = new LinkedList<GameObject>();

        public ResourcePool(ulong pathHash, Object asset)
            : base(pathHash)
        {
            m_asset = (GameObject)asset;
        }

        // return true if a cache object.
        public bool Spawn(out GameObject ret)
        {
            AddRef();
            ret = null;
            if (m_objectList.Count > 0)
            {
                ret = m_objectList.First.Value;
                m_objectList.RemoveFirst();
                return true;
            }

            if (m_asset != null)
            {
                ret = Object.Instantiate<GameObject>(m_asset);
                if (ret != null)
                {
                    ret.name = m_asset.name;
                }
            }

            return false;
        }

        public int GetObjectCount()
        {
            return m_objectList.Count;
        }

        public bool Despawn(GameObject gameObject, DespawnType despawnType)
        {
            if (gameObject == null)
                return false;

            Release();

            if (despawnType == DespawnType.Destroy)
            {
                if (Application.isPlaying)
                    GameObject.Destroy(gameObject);
                else
                    GameObject.DestroyImmediate(gameObject);

                return false;
            }

            m_objectList.AddLast(gameObject);
            return true;
        }

        public void Clear(bool unloadAsset)
        {
            foreach (var gameObject in m_objectList)
            {
                if (Application.isPlaying)
                    GameObject.Destroy(gameObject);
                else
                    GameObject.DestroyImmediate(gameObject);
            }
            m_objectList.Clear();

            if (unloadAsset)
            {
                ResourceDataManager.Instance.Unload(m_asset);
                m_asset = null;
            }
        }

        public void Merge(ResourcePool data)
        {
            Log.Assert(data.m_asset == this.m_asset);

            MergeRefCount(data);

            foreach (var gameObject in data.m_objectList)
            {
                m_objectList.AddLast(gameObject);
            }
            data.m_objectList.Clear();
            data.Clear(true);
        }
    }
}