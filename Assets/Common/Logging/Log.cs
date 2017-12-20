using UnityEngine;
using System;
using System.Diagnostics;

namespace CommonComponent
{
    public enum LogLevel
    {
        Error,
        Warning,
        Info,
    }

    public static class Log
    {
        public static LogLevel LogLevel = LogLevel.Info;

        [Conditional("ENABLE_LOG")]
        public static void Info(object msg)
        {
            if (LogLevel >= LogLevel.Info)
            {
                UnityEngine.Debug.Log(msg);
            }
        }

        [Conditional("ENABLE_LOG")]
        public static void Info(object msg, UnityEngine.Object context)
        {
            if (LogLevel >= LogLevel.Info)
            {
                UnityEngine.Debug.Log(msg, context);
            }
        }

        [Conditional("ENABLE_LOG")]
        public static void InfoFormat(string format, params object[] args)
        {
            if (LogLevel >= LogLevel.Info)
            {
                UnityEngine.Debug.LogFormat(format, args);
            }
        }

        [Conditional("ENABLE_LOG")]
        public static void InfoFormat(UnityEngine.Object context, string format, params object[] args)
        {
            if (LogLevel >= LogLevel.Info)
            {
                UnityEngine.Debug.LogFormat(context, format, args);
            }
        }

        [Conditional("ENABLE_LOG")]
        public static void Warning(object msg)
        {
            if (LogLevel >= LogLevel.Warning)
            {
                UnityEngine.Debug.LogWarning(msg);
            }
        }

        [Conditional("ENABLE_LOG")]
        public static void Warning(object msg, UnityEngine.Object context)
        {
            if (LogLevel >= LogLevel.Info)
            {
                UnityEngine.Debug.LogWarning(msg, context);
            }
        }

        [Conditional("ENABLE_LOG")]
        public static void WarningFormat(string format, params object[] args)
        {
            if (LogLevel >= LogLevel.Warning)
            {
                UnityEngine.Debug.LogWarningFormat(format, args);
            }
        }

        [Conditional("ENABLE_LOG")]
        public static void WarningFormat(UnityEngine.Object context, string format, params object[] args)
        {
            if (LogLevel >= LogLevel.Warning)
            {
                UnityEngine.Debug.LogWarningFormat(context, format, args);
            }
        }

        [Conditional("ENABLE_LOG")]
        public static void Error(object msg)
        {
            if (LogLevel >= LogLevel.Error)
            {
                UnityEngine.Debug.LogError(msg);
            }
        }

        [Conditional("ENABLE_LOG")]
        public static void Error(object msg, UnityEngine.Object context)
        {
            if (LogLevel >= LogLevel.Error)
            {
                UnityEngine.Debug.LogError(msg, context);
            }
        }

        [Conditional("ENABLE_LOG")]
        public static void ErrorFormat(string format, params object[] args)
        {
            if (LogLevel >= LogLevel.Warning)
            {
                UnityEngine.Debug.LogErrorFormat(format, args);
            }
        }

        [Conditional("ENABLE_LOG")]
        public static void ErrorFormat(UnityEngine.Object context, string format, params object[] args)
        {
            if (LogLevel >= LogLevel.Warning)
            {
                UnityEngine.Debug.LogErrorFormat(context, format, args);
            }
        }

        [Conditional("ENABLE_LOG")]
        public static void Exception(Exception ex)
        {
            if (LogLevel >= LogLevel.Error)
            {
                UnityEngine.Debug.LogException(ex);
            }
        }

        [Conditional("ENABLE_LOG")]
        public static void Exception(Exception exception, UnityEngine.Object context)
        {
            if (LogLevel >= LogLevel.Error)
            {
                UnityEngine.Debug.LogException(exception, context);
            }
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void Assert(bool condition)
        {
            if (LogLevel >= LogLevel.Error)
            {
                UnityEngine.Debug.Assert(condition);
            }
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void Assert(bool condition, UnityEngine.Object context)
        {
            if (LogLevel >= LogLevel.Error)
            {
                UnityEngine.Debug.Assert(condition, context);
            }
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void Assert(bool condition, object message)
        {
            if (LogLevel >= LogLevel.Error)
            {
                UnityEngine.Debug.Assert(condition, message);
            }
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void Assert(bool condition, string message)
        {
            if (LogLevel >= LogLevel.Error)
            {
                UnityEngine.Debug.Assert(condition, message);
            }
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void Assert(bool condition, object message, UnityEngine.Object context)
        {
            if (LogLevel >= LogLevel.Error)
            {
                UnityEngine.Debug.Assert(condition, message, context);
            }
        }
        [Conditional("UNITY_ASSERTIONS")]
        public static void Assert(bool condition, string message, UnityEngine.Object context)
        {
            if (LogLevel >= LogLevel.Error)
            {
                UnityEngine.Debug.Assert(condition, message, context);
            }
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void AssertFormat(bool condition, string format, params object[] args)
        {
            if (LogLevel >= LogLevel.Error)
            {
                UnityEngine.Debug.AssertFormat(condition, format, args);
            }
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void AssertFormat(bool condition, UnityEngine.Object context, string format, params object[] args)
        {
            if (LogLevel >= LogLevel.Error)
            {
                UnityEngine.Debug.AssertFormat(condition, context, format, args);
            }
        }

    }
}