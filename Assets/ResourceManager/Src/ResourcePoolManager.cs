using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CommonComponent;

namespace GameResource
{
    public class ResourcePoolManager : ResourceSingleton<ResourcePoolManager>
    {
        #region public
        public GameObject Spawn(ulong pathHash)
        {
            ResourcePool resourcePool = _GetResourcePool(pathHash);
            if (resourcePool == null)
            {
                Object asset = ResourceDataManager.Instance.Load(pathHash, typeof(GameObject));
                if (asset != null)
                {
                    resourcePool = new ResourcePool(pathHash, asset);
                    resourcePool = _AddResourcePool(resourcePool);
                }
            }

            GameObject gameObject = null;
            if (resourcePool != null)
            {
                if (resourcePool.Spawn(out gameObject))
                {
                    --m_deSpawnCount;
                }
            }
            if (gameObject != null)
                _UpdateGameObjectState(gameObject, pathHash);

            return gameObject;
        }

        public void Despawn(GameObject gameObject, DespawnType despawnType)
        {
            if (gameObject == null)
                return;

            int spawnID = gameObject.GetInstanceID();

            ulong pathHash = 0;
            if (!m_spawnID2PathHash.TryGetValue(spawnID, out pathHash))
            {
                if (Application.isPlaying)
                    GameObject.Destroy(gameObject);
                else
                    GameObject.DestroyImmediate(gameObject);

                Log.ErrorFormat("[ResourcePoolManager]Failed to despawn unkown gameObject({0})", gameObject.name);
                return;
            }

            m_spawnID2PathHash.Remove(spawnID);

            ResourcePool resourcePool = _GetResourcePool(pathHash);
            if (resourcePool == null)
            {
                if (Application.isPlaying)
                    GameObject.Destroy(gameObject);
                else
                    GameObject.DestroyImmediate(gameObject);
                Log.ErrorFormat("[ResourcePoolManager]Failed to find resource pool for gameObject({0})", gameObject.name);

                return;
            }

            _OnCacheFull();

            if (despawnType != DespawnType.Destroy)
            {
                Transform parent = despawnType == DespawnType.Invisible ? m_invisibleParent : m_inactiveParent;
                gameObject.transform.SetParent(parent, false);

            }

            if (resourcePool.Despawn(gameObject, despawnType))
            {
                ++m_deSpawnCount;
                _AddCacheValue(resourcePool);
            }
        }

        public ResourceSpawnHandle SpawnAsync(ulong pathHash, int priority)
        {
            ResourceSpawnHandle spawnHandle = new ResourceSpawnHandle();

            ResourceSpawnRequest request = m_loader.TryGetRequest(pathHash);
            if (request == null)
            {
                request = new ResourceSpawnRequest(pathHash, priority);
                request.OnLoadDone += _OnRequestDone;
                m_loader.PushRequst(request);
            }
            request.AddSpawnHandle(spawnHandle);

            return spawnHandle;
        }

        public void UnloadUnusedPool()
        {
            foreach (var iterator in m_pathHash2Pool)
            {
                ResourcePool resourcePool = iterator.Value;
                if (resourcePool == null || resourcePool.IsRefZero())
                {
                    m_clearList.Add(iterator.Key);
                }
            }

            for (int i = 0; i < m_clearList.Count; ++i)
            {
                _ClearResourcePool(m_clearList[i]);
            }
            m_clearList.Clear();
        }

        public void ClearPool()
        {
            foreach (var iterator in m_pathHash2Pool)
            {
                m_clearList.Add(iterator.Key);
            }

            for (int i = 0; i < m_clearList.Count; ++i)
                _ClearResourcePool(m_clearList[i]);
            m_clearList.Clear();
        }
        #endregion

        #region init
        protected override bool Init()
        {
            GameObject inactiveGameObj = new GameObject("UnityComponent-InactivePool");
            inactiveGameObj.SetActive(false);
            m_inactiveParent = inactiveGameObj.transform;
            Object.DontDestroyOnLoad(inactiveGameObj);

            GameObject invisibleGameObj = new GameObject("UnityComponent-InvisiblePool");
            invisibleGameObj.transform.position = new Vector3(-6666, -88888, -6666);
            m_invisibleParent = invisibleGameObj.transform;
            Object.DontDestroyOnLoad(invisibleGameObj);

            m_loader = new RequestLoader<ResourceSpawnRequest, ResourcePool>();
            ResourceUpdater.Instance.RegisterUpdater(m_loader.Update);

            return base.Init();
        }
        protected override bool UnInit()
        {
            ResourceUpdater.Instance.UnRegisterUpdater(m_loader.Update);
            if (Application.isPlaying)
            {
                GameObject.Destroy(m_inactiveParent);
                GameObject.Destroy(m_invisibleParent);
            }
            else
            {
                GameObject.DestroyImmediate(m_invisibleParent);
                GameObject.DestroyImmediate(m_inactiveParent);
            }
            return base.UnInit();
        }
        #endregion

        #region private
        private void _UpdateGameObjectState(GameObject gameObject, ulong pathHash)
        {
            int spawnID = gameObject.GetInstanceID();
            m_spawnID2PathHash[spawnID] = pathHash;

            gameObject.transform.parent = null;
        }

        private ResourcePool _GetResourcePool(ulong pathHash)
        {
            ResourcePool resourcePool = null;
            if (!m_pathHash2Pool.TryGetValue(pathHash, out resourcePool))
                return null;
            return resourcePool;
        }

        private ResourcePool _AddResourcePool(ResourcePool resourcePool)
        {
            ResourcePool existedPool = null;

            if (m_pathHash2Pool.TryGetValue(resourcePool.Id, out existedPool))
            {
                m_deSpawnCount += resourcePool.GetObjectCount();
                existedPool.Merge(resourcePool);
                return existedPool;
            }

            m_pathHash2Pool.Add(resourcePool.Id, resourcePool);

            return resourcePool;
        }

        private void _ClearResourcePool(ulong pathHash)
        {
            ResourcePool resourcePool = null;
            if (!m_pathHash2Pool.TryGetValue(pathHash, out resourcePool))
            {
                return;
            }

            m_pathHash2Pool.Remove(pathHash);
            if (resourcePool != null)
            {
                int count = resourcePool.GetObjectCount();
                if (count > 0)
                {
                    m_deSpawnCount -= count;
                    _RemoveCacheValue(resourcePool.Id);
                }
                resourcePool.Clear(true);
            }
        }

        private void _OnRequestDone(RequestBase<ResourcePool> requestBase)
        {
            ResourceSpawnRequest request = requestBase as ResourceSpawnRequest;
            if (request == null)
            {
                Log.Error("[ResourcePoolManager]Invalid request.");
                return;
            }

            ResourcePool resourcePool = request.asset as ResourcePool;
            if (resourcePool == null)
            {
                Log.Error("[ResourcePoolManager]Invalid resource pool.");
                return;
            }

            _AddResourcePool(resourcePool);

            foreach (var spawnHandle in request.spawnHandleList)
            {
                if (!spawnHandle.success)
                    continue;

                int spawnID = spawnHandle.spawnID;
                if (m_spawnID2PathHash.ContainsKey(spawnID))
                {
                    GameObject gameObject = spawnHandle.gameObject;
                    string gameObjectName = gameObject != null ? gameObject.name : "Unknown";
                    Log.ErrorFormat("[ResourcePoolManager]Duplicated spawn id for gameObject({0})", gameObjectName);
                    if (Application.isPlaying)
                        GameObject.Destroy(gameObject);
                    else
                        GameObject.DestroyImmediate(gameObject);
                    continue;
                }

                m_spawnID2PathHash[spawnID] = resourcePool.Id;
            }
        }

        private Transform m_inactiveParent = null;
        private Transform m_invisibleParent = null;
        private List<ulong> m_clearList = new List<ulong>();
        private Dictionary<int, ulong> m_spawnID2PathHash = new Dictionary<int, ulong>();
        private Dictionary<ulong, ResourcePool> m_pathHash2Pool = new Dictionary<ulong, ResourcePool>();
        private RequestLoader<ResourceSpawnRequest, ResourcePool> m_loader = null;
        #endregion

        #region lru cache
        private void _AddCacheValue(ResourcePool resourcePool)
        {
            LinkedListNode<ResourcePool> lnode;
            if (m_cacheMap.TryGetValue(resourcePool.Id, out lnode))
            {
                m_lruList.Remove(lnode);
                m_cacheMap.Remove(resourcePool.Id);
            }

            LinkedListNode<ResourcePool> node = new LinkedListNode<ResourcePool>(resourcePool);
            m_lruList.AddLast(node);
            m_cacheMap.Add(resourcePool.Id, node);
        }

        private void _RemoveCacheValue(ulong pathHash)
        {
            LinkedListNode<ResourcePool> lnode;
            if (m_cacheMap.TryGetValue(pathHash, out lnode))
            {
                m_lruList.Remove(lnode);
                m_cacheMap.Remove(pathHash);
            }
        }

        private ResourcePool _GetCacheFirst()
        {
            if (m_lruList.Count > 0)
            {
                return m_lruList.First.Value;
            }
            else
            {
                return null;
            }
        }

        private void _OnCacheFull()
        {
            if (m_deSpawnCount > ResourceConfig.MAX_POOL_COUNT)
            {
                ResourcePool first = _GetCacheFirst();
                if (first != null)
                {
                    m_deSpawnCount -= first.GetObjectCount();
                    first.Clear(false);
                    _RemoveCacheValue(first.Id);
                }
            }
        }

        private Dictionary<ulong, LinkedListNode<ResourcePool>> m_cacheMap = new Dictionary<ulong, LinkedListNode<ResourcePool>>();
        private LinkedList<ResourcePool> m_lruList = new LinkedList<ResourcePool>();
        private int m_deSpawnCount = 0;
        #endregion
    }
}