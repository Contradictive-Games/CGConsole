using UnityEngine;

namespace CG.Console
{
    internal static class Extensions
    {
        public static string GetLogColorByLogType(this ConsoleWindowSettings settings, LogType type)
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
    }
}