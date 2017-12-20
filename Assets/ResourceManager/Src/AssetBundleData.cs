using UnityEngine;
using System.IO;
using System.Collections.Generic;
using CommonComponent;

namespace GameResource
{
    internal class AssetBundleData : RefCountData
    {
        private bool m_unloadable = true;
        private AssetBundle m_assetBundle = null;

        public AssetBundleData(int bundleId)
            : base((ulong)bundleId)
        {
        }
        public bool Unloadable()
        {
            return m_unloadable && IsRefZero();
        }
        public void Unload(bool unloadAllLoadedObjects)
        {
            if (m_assetBundle != null)
            {
                m_assetBundle.Unload(unloadAllLoadedObjects);
                m_assetBundle = null;
            }
        }
        public int GetBundleId()
        {
            return (int)Id;
        }
        public AssetBundle GetAssetBundle()
        {
            return m_assetBundle;
        }
        public bool TimeOut()
        {
            return ElapsedMilliseconds > ResourceConfig.BUNDLE_TIME_OUT;
        }
        public bool Load()
        {
            BundleState bundleState = ResourceMainfest.GetBundleState((int)Id);

            if (bundleState.bundleId == -1)
            {
                Log.ErrorFormat("[AssetBundleData] bundleState not found bundleId={0}.", Id);
                return false;
            }

            m_unloadable = !bundleState.canunload && !bundleState.preload;

            string url = ResourceConfig.FormatPath((int)Id, bundleState.export);
            m_assetBundle = AssetBundle.LoadFromFile(url);

            if (m_assetBundle == null)
            {
                Log.ErrorFormat("[AssetBundleData] load asset bundle failed, url={0}", url);
            }
#if ENABLE_PROFILER
            else
            {
                m_assetBundle.name = string.Intern(Id.ToString());
            }
#endif

            return m_assetBundle != null;
        }
    }
}