using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

namespace BundleManager 
{
    public static class BundleDataManager
    {
        public static BundleData GetBundleData(string name)
        {
            BundleData ret = null;
            m_dataDict.TryGetValue(name, out ret);
            return ret;
        }
        public static BundleState GetBundleState(string name)
        {
            BundleState ret = null;
            m_stateDict.TryGetValue(name, out ret);
            return ret;
        }
        public static bool CreateNewBundle(string name, string parent, BundleType type, BundleLoadState loadState)
        {
            if (m_dataDict.ContainsKey(name))
            {
                return false;
            }

            BundleData newBundle = new BundleData();
            newBundle.name = name;
            newBundle.type = type;
            newBundle.loadState = loadState;

            if (!string.IsNullOrEmpty(parent))
            {
                if (!m_dataDict.ContainsKey(parent))
                {
                    return false;
                }
                else
                {
                    m_dataDict[parent].children.Add(name);
                }

                newBundle.parent = parent;
            }

            m_dataDict.Add(name, newBundle);
            BundleDataAccessor.Datas.Add(newBundle);

            BundleState newState = new BundleState();
            newState.bundleID = name;
            newState.version = 1;
            m_stateDict.Add(name, newState);
            BundleDataAccessor.States.Add(newState);

            return true;
        }
        public static bool CanBundleParentTo(string child, string newParent)
        {
            if (child == newParent)
            {
                return false;
            }

            if (string.IsNullOrEmpty(newParent))
            {
                return true;
            }

            if (!m_dataDict.ContainsKey(child) || !m_dataDict.ContainsKey(newParent))
            {
                return false;
            }

            string tarParent = m_dataDict[newParent].parent;
            while (m_dataDict.ContainsKey(tarParent))
            {
                if (tarParent == child)
                    return false;
                tarParent = m_dataDict[tarParent].parent;
            }

            return true;
        }
        public static bool SetParent(string child, string newParent)
        {
            if (!CanBundleParentTo(child, newParent))
            {
                return false;
            }

            BundleData childBundle = m_dataDict[child];
            if (m_dataDict.ContainsKey(childBundle.parent))
            {
                m_dataDict[childBundle.parent].children.Remove(child);
            }
            childBundle.parent = newParent;

            if (!string.IsNullOrEmpty(newParent))
            {
                BundleData newParentBundle = m_dataDict[newParent];
                newParentBundle.children.Add(child);
                newParentBundle.children.Sort();
            }

            return true;
        }
        public static bool CanAddPathToBundle(string path, string bundleName)
        {
            if (!File.Exists(path))
            {
                return false;
            }

            string guid = AssetDatabase.AssetPathToGUID(path);
            if (m_guidToBundle.ContainsKey(guid))
            {
                return false;
            }

            if (!m_dataDict.ContainsKey(bundleName))
            {
                return false;
            }

            return true;
        }
        public static bool AddPathToBundle(string path, string bundleName, int size)
        {
            if (!CanAddPathToBundle(path, bundleName))
            {
                return false;
            }

            BundleData bundle = m_dataDict[bundleName];
            string guid = AssetDatabase.AssetPathToGUID(path);

            bundle.includs.Add(path);
            m_guidToBundle.Add(guid, bundleName);
            bundle.size += size;

            return true;
        }
        public static void RemovePathFromBundle(string path, string bundleName)
        {
            BundleData bundle = GetBundleData(bundleName);
            if (bundle == null)
            {
                return;
            }
            string guid = AssetDatabase.AssetPathToGUID(path);

            if (m_guidToBundle.ContainsKey(guid))
            {
                m_guidToBundle.Remove(path);
            }

            for (int i = 0; i < bundle.includs.Count; ++i)
            {
                string nGuid = AssetDatabase.AssetPathToGUID(bundle.includs[i]);
                if (nGuid == guid)
                {
                    bundle.includs.RemoveAt(i);
                    break;
                }
            }
        }
        public static bool RemoveBundle(string bundleName)
        {
            BundleData bundle = GetBundleData(bundleName);
            if (bundle == null)
            {
                return false;
            }
            BundleDataAccessor.Datas.Remove(bundle);
            m_dataDict.Remove(bundleName);
            BundleDataAccessor.States.Remove(m_stateDict[bundleName]);
            m_stateDict.Remove(bundleName);

            if (!string.IsNullOrEmpty(bundle.parent) && m_dataDict.ContainsKey(bundle.parent))
            {
                m_dataDict[bundle.parent].children.Remove(bundleName);
            }

            for (int i = 0; i < bundle.includs.Count; ++i)
            {
                string nGuid = AssetDatabase.AssetPathToGUID(bundle.includs[i]);
                m_guidToBundle.Remove(nGuid);
            }

            return true;
        }
        public static void RemoveAllBundle()
        {
            BundleDataAccessor.Datas.Clear();
            BundleDataAccessor.States.Clear();
            Clear();
            BundleDataAccessor.SaveData();
        }
        public static string GetIndexBundleName(int index)
        {
            return "Index_" + index.ToString();
        }
        public static List<object> GetIndexBundleList(int index)
        {
            List<object> list = new List<object>();
            string parentBundleName = GetIndexBundleName(index);
            BundleData bundleData = GetBundleData(parentBundleName);
            if (bundleData == null)
            {
                return list;
            }
            for (int i = 0; i < bundleData.children.Count; ++i)
            {
                BundleData child = GetBundleData(bundleData.children[i]);
                if (child != null)
                {
                    list.Add(child);
                }
            }
            return list;
        }
        public static string GenBundleNameByType(BundleType type)
        {
            if (!m_typeCount.ContainsKey(type))
            {
                m_typeCount.Add(type, 1);
            }
            else
            {
                ++m_typeCount[type];
            }
            return string.Format("{0}_{1}", type, m_typeCount[type] - 1);
        }
        public static string GetPathBundleName(string assetPath)
        {
            string guid = AssetDatabase.AssetPathToGUID(assetPath);
            string ret = string.Empty;
            if (m_guidToBundle.TryGetValue(guid, out ret))
            {
                return ret;
            }
            return string.Empty;
        }
        public static bool CheckPathInBundle(string path)
        {
            string guid = AssetDatabase.AssetPathToGUID(path);
            return m_guidToBundle.ContainsKey(guid);
        }
        public static bool IsBundleFull(BundleData bundle, int maxCount, int memSize)
        {
            if (memSize > 0 && bundle.size > memSize)
                return true;
            if (maxCount > 0 && bundle.includs.Count >= maxCount)
                return true;
            return false;
        }
        public static bool IsNameDuplicatedAsset(BundleData bundle, string newAsset)
        {
            string newName = Path.GetFileNameWithoutExtension(newAsset).ToUpperInvariant();
            foreach (string path in bundle.includs)
            {
                string file = Path.GetFileNameWithoutExtension(path).ToUpperInvariant();
                if (file == newName)
                    return true;
            }

            return false;
        }

        public static void Init()
        {
            BundleDataAccessor.Refresh();
            Clear();

            foreach (BundleState bundleState in BundleDataAccessor.States)
            {
                if (!m_stateDict.ContainsKey(bundleState.bundleID))
                {
                    m_stateDict.Add(bundleState.bundleID, bundleState);
                }
                else
                {
                    Debug.LogError("[BundleDataManager] Bundle State Conflict " + bundleState.bundleID);
                }
            }

            foreach (BundleData bundleData in BundleDataAccessor.Datas)
            {
                if (!m_dataDict.ContainsKey(bundleData.name))
                {
                    m_dataDict.Add(bundleData.name, bundleData);
                }
                else
                {
                    Debug.LogError("[BundleDataManager] Bundle Data Conflict " + bundleData.name);
                }

                BundleType type = bundleData.type;
                if (!m_typeCount.ContainsKey(type))
                {
                    m_typeCount.Add(type, 1);
                }
                else
                {
                    ++m_typeCount[type];
                }

                for (int i = 0; i < bundleData.includs.Count; ++i)
                {
                    string guid = AssetDatabase.AssetPathToGUID(bundleData.includs[i]);
                    if (!m_guidToBundle.ContainsKey(guid))
                    {
                        m_guidToBundle.Add(guid, bundleData.name);
                    }
                    else
                    {
                        Debug.LogError("[BundleDataManager] Include Path Conflict " + bundleData.includs[i]);
                    }
                }
            }
        }

        private static void Clear()
        {
            m_typeCount.Clear();
            m_guidToBundle.Clear();
            m_dataDict.Clear();
            m_stateDict.Clear();
        }

        private static Dictionary<BundleType, int> m_typeCount = new Dictionary<BundleType, int>();
        private static Dictionary<string, string> m_guidToBundle = new Dictionary<string, string>();
        private static Dictionary<string, BundleData> m_dataDict = new Dictionary<string, BundleData>();
        private static Dictionary<string, BundleState> m_stateDict = new Dictionary<string, BundleState>();
    }
}