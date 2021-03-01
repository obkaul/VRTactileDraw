using Assets.PatternDesigner.Scripts.Attributes;
using System;

namespace Assets.PatternDesigner.Scripts.Values.Resources
{
    /// <summary>
    ///     Handles resources like strings or colors.
    /// </summary>
    public static class ResourcesManager
    {
        public static string Get(Strings resource, params object[] args)
        {
            return string.Format(Get(resource), args);
        }

        public static string Get(Strings resource)
        {
            var type = resource.GetType();

            var fInfo = type.GetField(resource.ToString());

            var attribute = (StringValueAttribute)Attribute.GetCustomAttribute(fInfo, typeof(StringValueAttribute));
			
            return attribute != null ? attribute.Value : "";
        }

        public static string GetSecondsText(float seconds)
        {
            return Get(seconds < Constants.FINE_SECONDS_MAX ? Strings.SECONDS_FINE : Strings.SECONDS, seconds);
        }
    }
}