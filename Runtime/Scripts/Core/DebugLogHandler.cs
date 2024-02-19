using System;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;

namespace UnityEngine
{
    [DebuggerStepThrough]
    public class DebugLogHandler : ILogHandler
    {
        public static readonly Regex commandRegex = new(@"\[(#\d+.)?(On[a-zA-Z]+)?(.=>.)?(?<command>[a-zA-Z]{4,})\]", RegexOptions.Compiled | RegexOptions.Multiline, TimeSpan.FromSeconds(1));
        public delegate bool IsMatchDelegate(string type, string text, out bool display);
        public delegate string PrettyPrintDelegate(string text);

        #region Fields
        static ILogHandler unityLogger;
        IsMatchDelegate isMatch;
        PrettyPrintDelegate prettyPrint;
        #endregion

        #region Constructors
        public DebugLogHandler(IsMatchDelegate isMatch, PrettyPrintDelegate prettyPrint)
        {
            unityLogger ??= Debug.unityLogger.logHandler;
            Debug.unityLogger.logHandler = this;

            this.isMatch = isMatch;
            this.prettyPrint = prettyPrint;
        }
        #endregion

        #region Methods
        public static void Log(params object[] values) => Debug.Log(string.Join(" ", values));
        public static void LogWarning(params object[] values) => Debug.LogWarning(string.Join(" ", values));
        public static void LogError(params object[] values) => Debug.LogError(string.Join(" ", values));

        void ILogHandler.LogFormat(LogType logType, Object context, string format, params object[] args)
        {
            foreach (object arg in args)
            {
                if (arg is not string name) continue;

                Match typeMatch = commandRegex.Match(name);
                if (!typeMatch.Success || !typeMatch.Groups["command"].Success) continue;

                string type = typeMatch.Groups["command"].Value;
                string text = string.Format(NumberFormatInfo.InvariantInfo, format, args);

                if (!isMatch(type, text, out bool display) || !display) return;

                unityLogger?.LogFormat(logType, context, format, text);

                return;
            }

            unityLogger?.LogFormat(logType, context, format, args);
        }
        void ILogHandler.LogException(Exception exception, Object context) => unityLogger?.LogException(exception, context);
        #endregion
    }
}