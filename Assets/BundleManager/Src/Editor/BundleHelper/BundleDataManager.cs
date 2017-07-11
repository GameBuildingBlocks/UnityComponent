using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

namespace BundleManager 
{
    public static class BundleDataManager
    {
        public static  BundleData GetBundleData(string name)
        {
            BundleData ret = null;
            m_dataDict.TryGetValue(name, out ret);
            return ret;
        }

        static public BundleState GetBundleState(string name)
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
                if (m_dataDict.ContainsKey(parent))
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
            BMDataAccessor.Datas.Add(newBundle);

            BundleState newState = new BundleState();
            newState.bundleID = name;
            newState.version = 1;
            m_stateDict.Add(name, newState);
            BMDataAccessor.States.Add(newState);

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

            return true;
        }
        public static bool AddPathToBundle(string path, string bundleName, long size)
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
            //string path = AssetDatabase.GUIDToAssetPath(guid);

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

        /**
         * Remove asset from bundle's include list by guid.
         */
        public static void RemoveAssetFromBundle(string guid, string bundleName)
        {
            
        }
        public static string GetIndexBundleName(int index)
        {
            return "Index_" + index.ToString();
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
            return BundleTypeTool.GetBundleID(type, m_typeCount[type] - 1).ToString();
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

        private static void Init()
        {
            BMDataAccessor.Refresh();
            Clear();

            foreach (BundleState bundleState in BMDataAccessor.States)
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

            foreach (BundleData bundleData in BMDataAccessor.Datas)
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
            m_dataDict.Clear();
            m_stateDict.Clear();
        }

        private static Dictionary<BundleType, int> m_typeCount = new Dictionary<BundleType, int>();
        private static Dictionary<string, string> m_guidToBundle = new Dictionary<string, string>();
        private static Dictionary<string, BundleData> m_dataDict = new Dictionary<string, BundleData>();
        private static Dictionary<string, BundleState> m_stateDict = new Dictionary<string, BundleState>();
    }
}