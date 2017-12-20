using UnityEngine;
using System.Diagnostics;

namespace GameResource
{
    public static class ResourceTimeControl
    {
        private static Stopwatch m_stopWatch = Stopwatch.StartNew();
        private static BackLoadingPriority m_backLoadingPriority = BackLoadingPriority.Normal;
        private static long m_timeOut = 10;

        // the time use main render thread
        public static void SetBackLoadingPriority(BackLoadingPriority priority)
        {
            if (m_backLoadingPriority != priority)
            {
                m_backLoadingPriority = priority;
                m_timeOut = ResourceConfig.GetBackLoadingTime(m_backLoadingPriority);
            }
        }

        public static BackLoadingPriority GetBackLoadingPriority()
        {
            return m_backLoadingPriority;
        }

        public static void Reset()
        {
            m_stopWatch.Reset();
            m_stopWatch.Start();
        }

        public static bool TimeOut()
        {
            if (m_stopWatch.ElapsedMilliseconds > m_timeOut)
            {
                return true;
            }
            return false;
        }

        public static long ElapsedMilliseconds()
        {
            return m_stopWatch.ElapsedMilliseconds;
        }
    }
}