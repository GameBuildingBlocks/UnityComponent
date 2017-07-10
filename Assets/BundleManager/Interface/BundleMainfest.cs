using System;
using System.IO;
using System.Collections.Generic;

namespace BundleManager
{
    public class BundleMainfest
    {
        public bool IsBundleResource(ulong hashID)
        {
            return true;
        }
        public int GetBundleIdByPathHash(ulong hashID)
        {
            return -1;
        }

        public BundleState GetBundleState(int bundleID)
        {
            return new BundleState();
        }

        public bool GetBundleDepencies(int bundleID, List<int> retDepenList)
        {
            return true;
        }

        public bool LoadMainfest(byte[] buffer)
        {
            return true;
        }

        public bool LoadBundleState(byte[] buffer)
        {
            return true;
        }

        public bool GetPreLoadBundle(List<int> retPreloadList)
        {
            return true;
        }

    }

#if UNITY_EDITOR
    public class BundleMainfestEditor
    {
        public void AddPathWithBundleName(string path, string bundleName)
        {
            if (!m_pathToBundleName.ContainsKey(path))
            {
                m_pathToBundleName.Add(path, bundleName);
            }
            else
            {
                m_pathToBundleName[path] = bundleName;
            }
        }

        public void AddBundleDepend(string bundleName, string depBundleName)
        {
            List<string> list = null;
            if (!m_bundleDependDict.TryGetValue(bundleName, out list))
            {
                list = new List<string>();
                m_bundleDependDict.Add(bundleName, list);
            }

            if (!list.Contains(depBundleName))
            {
                list.Add(depBundleName);
            }
        }

        public void AddBundleState(BundleState bundleState)
        {
            if (!m_bundleNameToState.ContainsKey(bundleState.bundleID))
            {
                m_bundleNameToState.Add(bundleState.bundleID, bundleState);
            }
            else
            {
                m_bundleNameToState[bundleState.bundleID] = bundleState;
            }
        }

        public void Clear()
        {
            m_bundleDependDict.Clear();
            m_pathToBundleName.Clear();
            m_bundleNameToState.Clear();
        }

        public bool SaveMainfestBytes(string path)
        {
            return true;
        }

        public bool SaveBundleStateBytes(string path)
        {
            return true;
        }

        Dictionary<string, BundleState> m_bundleNameToState = new Dictionary<string, BundleState>();
        Dictionary<string, string> m_pathToBundleName = new Dictionary<string, string>();
        Dictionary<string, List<string>> m_bundleDependDict = new Dictionary<string, List<string>>();
    }
#endif
}