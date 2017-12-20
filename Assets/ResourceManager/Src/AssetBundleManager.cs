using UnityEngine;
using System.Collections.Generic;

namespace GameResource
{
    public class AssetBundleManager : ResourceSingleton<AssetBundleManager>
    {
        #region public
        public AssetBundle Load(ulong hashID)
        {
            int bundleID = ResourceMainfest.GetBundleID(hashID);
            if (bundleID == -1)
            {
                return null;
            }

            AssetBundleData assetBundleData = _LoadAssetBundleDataWithDepend(bundleID);
            return assetBundleData.GetAssetBundle();
        }

        public void Unload(ulong hashID)
        {
            int bundleID = ResourceMainfest.GetBundleID(hashID);
            if (bundleID != -1)
            {
                _UnloadAssetBundleDataWithDepend(bundleID);
            }
        }

        public void UnloadUnusedAssets(List<ulong> resHashs)
        {
            for (int i = 0; i < resHashs.Count; ++i)
            {
                int bundleID = ResourceMainfest.GetBundleID(resHashs[i]);
                if (bundleID != -1)
                {
                    _UpdateBundleRef(bundleID, true);
                }
            }

            _UnloadUnloadingAssetBundle(false);

            for (int i = 0; i < resHashs.Count; ++i)
            {
                int bundleID = ResourceMainfest.GetBundleID(resHashs[i]);
                if (bundleID != -1)
                {
                    _UpdateBundleRef(bundleID, false);
                }
            }
        }

        public void ReloadAssetBundle()
        {
            _UnloadUnloadingAssetBundle(true);
            ResourceMainfest.ReloadMainfest();
            _LoadPreLoadBundle();
        }

        #endregion

        #region init
        protected override bool Init()
        {
            ResourceUpdater.Instance.RegisterUpdater(Update);
            _LoadPreLoadBundle();
            return base.Init();
        }
        protected override bool UnInit()
        {
            ResourceUpdater.Instance.UnRegisterUpdater(Update);
            return base.UnInit();
        }
        protected void Update()
        {
            _UpdateCacheBundeList(false);
        }
        #endregion

        #region private
        private void _LoadPreLoadBundle()
        {
            List<BundleState> preLoadList = ResourceMainfest.GetPreloadBundleList();
            for (int i = 0; i < preLoadList.Count; ++i)
            {
                _LoadAssetBundleData(preLoadList[i].bundleId);
            }
        }

        private AssetBundleData _LoadAssetBundleDataWithDepend(int bundleId)
        {
            int cur = ResourceMainfest.GetDependFirst(bundleId);
            while (cur != -1)
            {
                int depBundleId = ResourceMainfest.GetDependValue(cur);
                cur = ResourceMainfest.GetDependNext(cur);
                _LoadAssetBundleData(depBundleId);
            }
            return _LoadAssetBundleData(bundleId);
        }

        private void _UnloadAssetBundleDataWithDepend(int bundleId)
        {
            AssetBundleData bundleData = null;
            if (!m_assetBundleDict.TryGetValue(bundleId, out bundleData))
            {
                return;
            }

            int cur = ResourceMainfest.GetDependFirst(bundleId);
            while (cur != -1)
            {
                int depBundleId = ResourceMainfest.GetDependValue(cur);
                cur = ResourceMainfest.GetDependNext(cur);
                AssetBundleData depBundle = null;
                m_assetBundleDict.TryGetValue(depBundleId, out depBundle);
                if (depBundle != null)
                {
                    depBundle.Release();
                }
            }

            bundleData.Release();
            if (bundleData.Unloadable())
            {
                _UnloadBundleData(bundleData);
            }
        }

        private AssetBundleData _LoadAssetBundleData(int bundleId)
        {
            AssetBundleData ret = null;
            if (!m_assetBundleDict.TryGetValue(bundleId, out ret))
            {
                ret = new AssetBundleData(bundleId);
                ret.Load();

                m_assetBundleDict.Add(bundleId, ret);
            }
            ret.AddRef();
            return ret;
        }

        private void _UpdateCacheBundeList(bool force)
        {
            if (m_cacheUnloadBundle.Count == 0) return;

            int bundleId = m_cacheUnloadBundle.First.Value;

            AssetBundleData bundleData = null;
            m_assetBundleDict.TryGetValue(bundleId, out bundleData);

            if (bundleData == null || !bundleData.Unloadable())
            {
                m_cacheUnloadBundle.RemoveFirst();
                return;
            }

            if (force || bundleData.TimeOut())
            {
                m_cacheUnloadBundle.RemoveFirst();
                bundleData.Unload(false);
                m_assetBundleDict.Remove(bundleId);
            }
        }

        private void _UnloadUnloadingAssetBundle(bool all)
        {
            if (m_assetBundleDict.Count == 0) return;

            List<int> unloadBundleList = new List<int>(m_assetBundleDict.Count);
            using (var itor = m_assetBundleDict.GetEnumerator())
            {
                while (itor.MoveNext())
                {
                    if (!all && !itor.Current.Value.Unloadable())
                    {
                        continue;
                    }
                    unloadBundleList.Add(itor.Current.Key);
                }
            }

            for (int i = 0; i < unloadBundleList.Count; ++i)
            {
                AssetBundleData bundleData = null;
                if (m_assetBundleDict.TryGetValue(unloadBundleList[i], out bundleData))
                {
                    if (bundleData != null) bundleData.Unload(false);
                    m_assetBundleDict.Remove(unloadBundleList[i]);
                }
            }
            unloadBundleList.Clear();
        }

        private void _UpdateBundleDataRef(AssetBundleData bundleData, bool addRef)
        {
            if (bundleData != null)
            {
                if (addRef)
                {
                    bundleData.AddRef();
                }
                else
                {
                    bundleData.Release();
                }
            }
        }

        private void _UpdateBundleRef(int bundleId, bool addRef)
        {
            AssetBundleData bundleData = null;
            if (m_assetBundleDict.TryGetValue(bundleId, out bundleData))
            {
                _UpdateBundleDataRef(bundleData, addRef);
            }

            int cur = ResourceMainfest.GetDependFirst(bundleId);
            while (cur != -1)
            {
                int value = ResourceMainfest.GetDependValue(cur);
                cur = ResourceMainfest.GetDependNext(cur);
                if (m_assetBundleDict.TryGetValue(value, out bundleData))
                {
                    _UpdateBundleDataRef(bundleData, addRef);
                }
            }
        }

        private void _UnloadBundleData(AssetBundleData bundleData)
        {
            if (bundleData == null || !bundleData.Unloadable()) return;
            if (m_cacheUnloadBundle.Contains(bundleData.GetBundleId())) return;
            if (m_cacheUnloadBundle.Count >= ResourceConfig.MAX_CACHE_BUNDLE_SIZE)
            {
                bundleData.AddRef();
                _UpdateCacheBundeList(true);
                bundleData.Release();
            }
            m_cacheUnloadBundle.AddLast(bundleData.GetBundleId());
        }

        private LinkedList<int> m_cacheUnloadBundle = new LinkedList<int>();
        private Dictionary<int, AssetBundleData> m_assetBundleDict = new Dictionary<int, AssetBundleData>();
        #endregion
    }
}