using UnityEngine;
using System.Collections.Generic;
using CommonComponent;

namespace GameResource
{
    public class ResourceDataManager : ResourceSingleton<ResourceDataManager>
    {
        public Object Load(ulong pathHash, System.Type type)
        {
            ResourceData resourceData = _GetResourceData(pathHash);
            if (resourceData != null)
            {
                Log.Assert(resourceData.type == type);
                resourceData.AddRef();
                return resourceData.asset;
            }

            Object asset = null;

            if (ResourceMainfest.IsAssetBundleResource(pathHash))
            {
                AssetBundle assetBundle = AssetBundleManager.Instance.Load(pathHash);
                string name = ResourceMainfest.GetPathName(pathHash);
                if (assetBundle != null && !string.IsNullOrEmpty(name))
                {
                    asset = assetBundle.LoadAsset(name, type);
                }
                AssetBundleManager.Instance.Unload(pathHash);
            }

            if (asset == null)
            {
                string path = ResourceMainfest.GetHashPath(pathHash);
                if (!string.IsNullOrEmpty(path))
                {
                    asset = Resources.Load(path, type);
                }
            }

            if (asset != null)
            {
                resourceData = new ResourceData(pathHash, type, asset);
                resourceData.AddRef();
                _AddResourceData(resourceData);
            }

            return asset;
        }

        public ResourceLoadHandle LoadAsync(ulong pathHash, System.Type type, int priority)
        {
            ResourceLoadHandle loadHandle = new ResourceLoadHandle();

            ResourceData resourceData = _GetResourceData(pathHash);
            if (resourceData == null)
            {
                ResourceLoadRequest request = m_loader.TryGetRequest(pathHash);
                if (request == null)
                {
                    request = new ResourceLoadRequest(pathHash, type, priority);
                    request.OnLoadDone += _OnRequestDone;
                    m_loader.PushRequst(request);
                }
                else
                {
                    Log.Assert(request.type == type);
                }
                request.AddLoadHandle(loadHandle);
            }
            else
            {
                Log.Assert(resourceData.type == type);
                resourceData.AddRef();
                loadHandle.isDone = true;
                loadHandle.asset = resourceData.asset;
            }

            return loadHandle;
        }

        public void Unload(Object asset)
        {
            if (asset == null)
                return;

            ulong pathHash = 0;
            if (!m_assetID2PathHash.TryGetValue(asset.GetInstanceID(), out pathHash))
            {
                Log.ErrorFormat("[ResourceDataManager]Failed to unload unknown asset({0})", asset.name);
                return;
            }

            ResourceData data = _GetResourceData(pathHash);
            if (data == null)
            {
                Log.ErrorFormat("[ResourceDataManager]Failed to get resource data for asset({0})", asset.name);
                return;
            }

            data.Release();
        }

        public List<ulong> GetResourcePathList()
        {
            m_tempPathList.Clear();
            foreach (var node in m_pathHash2Data)
            {
                m_tempPathList.Add(node.Key);
            }
            return m_tempPathList;
        }

        public void UnloadUnusedAssets()
        {
            List<ulong> unLoadList = new List<ulong>();
            foreach (var itor in m_pathHash2Data)
            {
                ResourceData resourceData = itor.Value;
                if (resourceData == null || resourceData.IsRefZero())
                {
                    unLoadList.Add(itor.Key);
                }
            }

            for (int i = 0; i < unLoadList.Count; ++i)
            {
                m_pathHash2Data.Remove(unLoadList[i]);
            }
        }

        protected override bool Init()
        {
            ResourceUpdater.Instance.RegisterUpdater(m_loader.Update);
            return base.Init();
        }

        protected override bool UnInit()
        {
            ResourceUpdater.Instance.UnRegisterUpdater(m_loader.Update);
            m_loader = null;
            return base.UnInit();
        }

        private ResourceData _GetResourceData(ulong pathHash)
        {
            ResourceData data = null;
            if (!m_pathHash2Data.TryGetValue(pathHash, out data))
            {
                return null;
            }
            return data;
        }

        private void _AddResourceData(ResourceData resourceData)
        {
            ResourceData existedData = null;

            if (m_pathHash2Data.TryGetValue(resourceData.Id, out existedData))
            {
                // LoadAsync first and Load before LoadAsync finished
                existedData.MergeRefCount(resourceData);
                return;
            }

            m_pathHash2Data.Add(resourceData.Id, resourceData);

            int insId = resourceData.asset.GetInstanceID();
            if (m_assetID2PathHash.ContainsKey(insId))
            {
                m_assetID2PathHash[insId] = resourceData.Id;
            }
            else
            {
                m_assetID2PathHash.Add(insId, resourceData.Id);
            }
        }

        private void _OnRequestDone(RequestBase<ResourceData> requestBase)
        {
            ResourceLoadRequest request = requestBase as ResourceLoadRequest;
            if (request == null)
            {
                Log.Error("[ResourceDataManager]Invalid request.");
                return;
            }

            ResourceData resourceData = request.asset as ResourceData;
            if (resourceData == null)
            {
                Log.Error("[ResourceDataManager]Invalid resource data.");
                return;
            }

            _AddResourceData(resourceData);
        }

        private List<ulong> m_tempPathList = new List<ulong>();
        private Dictionary<int, ulong> m_assetID2PathHash = new Dictionary<int, ulong>();
        private Dictionary<ulong, ResourceData> m_pathHash2Data = new Dictionary<ulong, ResourceData>();
        private RequestLoader<ResourceLoadRequest, ResourceData> m_loader = new RequestLoader<ResourceLoadRequest, ResourceData>();
    }
}