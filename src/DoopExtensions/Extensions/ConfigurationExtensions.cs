using Microsoft.Extensions.Configuration;
using System;

namespace DoopExtensions.Extensions
{
    public static class ConfigurationExtensions
    {
        public static T SafeGetValue<T>(this IConfiguration configuration, string key, T defaultReturn = default)
        {
            try
            {
                var sectionExists = configuration.GetSection(key);
                return sectionExists.Exists() ? configuration.StrictGetValue<T>(key) : defaultReturn;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException($"Error in safely get config parameters: {ex.InnerException}");
            }
        }

        public static T StrictGetValue<T>(this IConfiguration configuration, string key)
        {
            try
            {
                var sectionExists = configuration.GetSection(key);

                if (sectionExists.Exists())
                {
                    return configuration.GetValue<T>(key);
                }
                else
                {
                    throw new ArgumentNullException($"{key} is not a valid key in the configuration file.");
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException($"Error in safely get config parameters for key {key}: {ex.InnerException}");
            }
        }

        // string shortcuts as those are generally the most common
        public static string StrictGetValue(this IConfiguration configuration, string key)
        {
            return configuration.StrictGetValue<string>(key);
        }

        public static string SafeGetValue(this IConfiguration configuration, string key)
        {
            return configuration.SafeGetValue(key, default(string));
        }
    }
}