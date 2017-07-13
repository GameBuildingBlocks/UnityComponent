using System.Collections.Generic;
using EditorCommon;

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
                    m_datas = EditorTool.LoadJsonData<List<BundleData>>(BuildConfig.BundleDataPath);
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
                    m_states = EditorTool.LoadJsonData<List<BundleState>>(BuildConfig.BundleStatePath);
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
                    m_importDatas = EditorTool.LoadJsonData<List<BundleImportData>>(BuildConfig.BundleImportDataPath);
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
                EditorTool.SaveJsonData<List<BundleData>>(m_datas, BuildConfig.BundleDataPath);
            }

            if (m_states != null)
            {
                EditorTool.SaveJsonData<List<BundleState>>(m_states, BuildConfig.BundleStatePath);
            }
        }

        public static void SaveImportData()
        {
            if (m_importDatas != null)
            {
                EditorTool.SaveJsonData<List<BundleImportData>>(m_importDatas, BuildConfig.BundleImportDataPath);
            }
        }

        private static List<BundleData> m_datas = null;
        private static List<BundleState> m_states = null;
        private static List<BundleImportData> m_importDatas = null;
    }
   
}