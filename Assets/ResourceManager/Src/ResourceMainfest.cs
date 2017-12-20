using UnityEngine;
using System.IO;
using System.Collections.Generic;
using CommonComponent;

namespace GameResource
{
    public static class ResourceMainfest
    {
        public static ulong GetPathHash(string path)
        {
            ulong hashId = ResourceHash.FNVHashForPath(path);

            if (IsAssetBundleResource(hashId) && Application.isPlaying)
            {
                return hashId;
            }

            if (!m_hashToPath.ContainsKey(hashId))
            {
                //string uniPath = ResourceHash.GetUniquePath(path);
                m_hashToPath.Add(hashId, path);
            }

            return hashId;
        }
        public static string GetHashPath(ulong hashId)
        {
            string path = null;

            if (!m_hashToPath.TryGetValue(hashId, out path))
            {
                //Log.ErrorFormat("[ResourceMainfest] hashId:{0} not found.", hashId);
            }

            return path;
        }

        public static bool IsAssetBundleResource(ulong hashId)
        {
            return GetBundleID(hashId) != -1;
        }
        public static List<BundleState> GetPreloadBundleList()
        {
            return m_preLoadBundle;
        }
        public static BundleState GetBundleState(int bundleId)
        {
            BundleState ret;
            if (!m_bundleState.TryGetValue(bundleId, out ret))
            {
                ret.bundleId = -1;
                return ret;
            }
            return ret;
        }
        public static int GetBundleID(ulong hashId)
        {
            _Data data;
            if (m_hashToData.TryGetValue(hashId, out data))
            {
                return data.bundleID;
            }
            return -1;
        }
        public static int GetDependFirst(int bundleId)
        {
            return m_dependData.GetFirstID(bundleId);
        }
        public static int GetDependValue(int id)
        {
            int retValue = -1;
            if (m_dependData.GetValueByID(id, out retValue))
            {
                return retValue;
            }
            return -1;
        }
        public static int GetDependNext(int id)
        {
            return m_dependData.GetNextID(id);
        }
        public static string GetPathName(ulong hashId)
        {
            _Data data;
            if (m_hashToData.TryGetValue(hashId, out data))
            {
                return data.name;
            }
            return null;
        }
        public static void LoadMainfest()
        {
            if (m_loaded) return;
            ReloadMainfest();
        }
        public static void ReloadMainfest()
        {
            m_loaded = true;
            // load bundle data and add data to container [m_hashToData]
            // include path to bundle data & bundle depend data [m_dependData]
            // load bundle status data like BundleStates [m_bundleState|m_preLoadBundle]
        }

        private static bool m_loaded = false;
        private struct _Data
        {
            public string name;
            public int bundleID;
        }
        private static Dictionary<ulong, string> m_hashToPath = new Dictionary<ulong, string>();
        private static Dictionary<ulong, _Data> m_hashToData = new Dictionary<ulong, _Data>();
        private static Dictionary<int, BundleState> m_bundleState = new Dictionary<int, BundleState>();
        private static GraphLinkList<int, int> m_dependData = new GraphLinkList<int, int>();
        private static List<BundleState> m_preLoadBundle = new List<BundleState>();
    }
}