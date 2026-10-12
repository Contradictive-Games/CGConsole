using UnityEngine;

namespace CG.Console
{
    internal static class Extensions
    {
        internal static string GetLogColorByLogType(this ConsoleWindowSettings settings, LogType type)
        {
            return type switch
            {
                LogType.Log => ColorUtility.ToHtmlStringRGBA(settings.NormalColor),
                LogType.Warning => ColorUtility.ToHtmlStringRGBA(settings.WarningColor),
                LogType.Error => ColorUtility.ToHtmlStringRGBA(settings.ErrorColor),
                LogType.Exception => ColorUtility.ToHtmlStringRGBA(settings.ExceptionColor),
                LogType.Assert => ColorUtility.ToHtmlStringRGBA(settings.AssertColor),
                _ => ColorUtility.ToHtmlStringRGBA(settings.NormalColor)
            };
        }

        internal static LogType ToLogType(this ResponseType type) => type switch
        {
            ResponseType.Success => LogType.Assert,
            ResponseType.Info => LogType.Log,
            ResponseType.Invalid => LogType.Warning,
            _ => LogType.Error
        };
    }
}