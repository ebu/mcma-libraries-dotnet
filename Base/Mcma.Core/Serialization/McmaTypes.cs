using System;
using System.Linq;
using System.Reflection;
using Mcma.Logging;

namespace Mcma.Serialization
{
    /// <summary>
    /// Static registry of types of which MCMA serialization must be aware
    /// </summary>
    public static class McmaTypes
    {
        static McmaTypes()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                AddTypesFromAssembly(assembly);

            AppDomain.CurrentDomain.AssemblyLoad += (_, args) => AddTypesFromAssembly(args.LoadedAssembly);
        }

        private static McmaTypeRegistrations Types { get; } = new();
        
        private static void AddTypesFromAssembly(Assembly assembly)
        {
            if (assembly.FullName.StartsWith("System") || assembly.FullName.StartsWith("Microsoft") || assembly.IsDynamic)
                return;

            try
            {
                foreach (var mcmaType in assembly.GetExportedTypes().Where(t => typeof(McmaObject).IsAssignableFrom(t)))
                    Add(mcmaType);
            }
            catch (Exception ex)
            {
                Logger.System.Warn($"Failed to load types from assembly {assembly.FullName}.{Environment.NewLine}Exception:{Environment.NewLine}{ex}");
            }
        }
        
        /// <summary>
        /// Adds a well-known type to the registry
        /// </summary>
        /// <typeparam name="T">The type to add</typeparam>
        /// <returns></returns>
        public static IMcmaTypeRegistrations Add<T>() => Add(typeof(T));
        
        /// <summary>
        /// Adds a well-known type to the registry
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IMcmaTypeRegistrations Add(Type type) => Types.Add(type);

        /// <summary>
        /// Finds a registered type with the given name
        /// </summary>
        /// <param name="typeString">The name of the type to find. Must be an unqualified name (<see cref="Type.Name"/>), as would be found in the @type json property</param>
        /// <returns>The type with the given name, if any</returns>
        public static Type FindType(string typeString)
        {
            if (typeString == null)
                return null;

            // check for match in explicitly-provided type collection
            var objectType = Types.FirstOrDefault(t => t.Name.Equals(typeString, StringComparison.OrdinalIgnoreCase));
            if (objectType == null)
            {
                // check for match in core types
                objectType = Type.GetType(typeof(McmaObject).AssemblyQualifiedName?.Replace(nameof(McmaObject), typeString) ?? typeString);
            }

            return objectType;
        }
    }
}