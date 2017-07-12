using System.Collections.Generic;

namespace BundleManager
{
    internal static class BundleDataAccessor
    {
        public static List<BundleData> Datas 
        {
            get 
            {
                if (m_datas == null)
                {
                    m_datas = EditorCommon.LoadJsonData<List<BundleData>>(BuildConfig.BundleDataPath);
                }
                if (m_datas == null)
                {
                    m_datas = new List<BundleData>();
                }
                return m_datas; 
            }
        }

        public static List<BundleState> States
        {
            get
            {
                if (m_states == null)
                {
                    m_states = EditorCommon.LoadJsonData<List<BundleState>>(BuildConfig.BundleStatePath);
                }
                if (m_states == null)
                {
                    m_states = new List<BundleState>();
                }
                return m_states;
            }
        }

        public static List<BundleImportData> ImportDatas
        {
            get
            {
                if (m_importDatas == null)
                {
                    m_importDatas = EditorCommon.LoadJsonData<List<BundleImportData>>(BuildConfig.BundleImportDataPath);
                }
                if (m_importDatas == null)
                {
                    m_importDatas = new List<BundleImportData>();
                }
                return m_importDatas;
            }
        }

        public static void Refresh()
        {
            m_datas = null;
            m_states = null;
        }

        public static void SaveData()
        {
            if (m_datas != null)
            {
                EditorCommon.SaveJsonData<List<BundleData>>(m_datas, BuildConfig.BundleDataPath);
            }

            if (m_states != null)
            {
                EditorCommon.SaveJsonData<List<BundleState>>(m_states, BuildConfig.BundleStatePath);
            }
        }

        public static void SaveImportData()
        {
            if (m_importDatas != null)
            {
                EditorCommon.SaveJsonData<List<BundleImportData>>(m_importDatas, BuildConfig.BundleImportDataPath);
            }
        }

        private static List<BundleData> m_datas = null;
        private static List<BundleState> m_states = null;
        private static List<BundleImportData> m_importDatas = null;
    }
   
}