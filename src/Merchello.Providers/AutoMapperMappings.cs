using System;
using System.Linq;
using AutoMapper;

namespace Merchello.Providers
{
    /// <summary>
    /// Creates AutoMapper mappings - used in <see cref="UmbracoApplicationEvents"/>
    /// </summary>
    public static class AutoMapperMappings
    {
        /// <summary>
        /// The create mappings.
        /// </summary>
        public static void CreateMappings()
        {
            // BH: Replaced with Automapper profiles
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Where(p => p.FullName.StartsWith("Merchello.Providers.Payment"))) {
                // BH: Yeah I know I'm magic-stringing, deal with it
                foreach (var type in assembly.GetExportedTypes().Where(p => p.BaseType == typeof(Profile))) {
                    Mapper.AddProfile((Profile)Activator.CreateInstance(type));
                }
            }
        }
    }
}