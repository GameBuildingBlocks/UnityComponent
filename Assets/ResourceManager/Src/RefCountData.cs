using System.Collections.Generic;
using System.Diagnostics;
using CommonComponent;

namespace GameResource
{
    internal class RefCountData
    {
        protected readonly ulong m_id = 0;
        private ulong m_refCount = 0;
        private long m_lastUsedTime = Stopwatch.GetTimestamp();

        protected RefCountData(ulong id)
        {
            m_id = id;
        }

        private RefCountData()
        {
        }

        public void AddRef()
        {
            m_refCount++;
            m_lastUsedTime = Stopwatch.GetTimestamp();
        }

        public void Release()
        {
            if (m_refCount == 0)
            {
                string path = ResourceMainfest.GetHashPath(m_id);
                Log.ErrorFormat("[RefCountData] RefCount Error for id={0}, path({1}).", m_id, path == null ? "Unknown" : path);
            }
            m_refCount--;
            m_lastUsedTime = Stopwatch.GetTimestamp();
        }

        public bool IsRefZero()
        {
            return m_refCount == 0;
        }

        public ulong Id
        {
            get { return m_id; }
        }

        public long ElapsedMilliseconds
        {
            get { return (Stopwatch.GetTimestamp() - m_lastUsedTime) * 1000 / Stopwatch.Frequency; }
        }

        public void MergeRefCount(RefCountData data)
        {
            m_refCount += data.m_refCount;
        }
    }
}