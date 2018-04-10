using CommonComponent;

namespace GameResource
{
    public abstract class ResourceSingleton<T> where T : ResourceSingleton<T>, new()
    {
        private static T m_instance = default(T);
        public static T Instance
        {
            get
            {
                if (m_instance != null)
                {
                    return m_instance;
                }
                m_instance = new T();
                if (!m_instance.Init())
                {
                    Log.ErrorFormat("[ResourceSingleton] getInstance().Init() failed, type={0}",
                        m_instance.GetType().ToString()
                    );
                }
                return m_instance;
            }
        }
        protected virtual bool Init()
        {
            return true;
        }
        protected virtual bool UnInit()
        {
            m_instance = null;
            return true;
        }
    }
}