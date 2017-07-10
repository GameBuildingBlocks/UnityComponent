using System.Collections.Generic;

namespace BundleManager
{
    internal class BMDataAccessor
    {
        public static List<BundleData> Datas 
        {
            get 
            {
                return m_datas; 
            }
        }

        public static List<BundleState> States
        {
            get
            {
                return m_states;
            }
        }

        public static void Refresh()
        {
            m_datas = null;
            m_states = null;
        }

        private static List<BundleData> m_datas = null;
        private static List<BundleState> m_states = null;
    }
   
}