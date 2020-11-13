using System;
using System.Linq;
using System.Reflection;
using Mcma.Logging;

namespace Mcma.Serialization
{
    public static class McmaTypes
    {
        static McmaTypes()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                AddTypesFromAssembly(assembly);

            AppDomain.CurrentDomain.AssemblyLoad += (sender, args) => AddTypesFromAssembly(args.LoadedAssembly);
        }

        private static McmaTypeRegistrations Types { get; } = new McmaTypeRegistrations();
        
        private static void AddTypesFromAssembly(Assembly assembly)
        {
            if (assembly.FullName.StartsWith("System") || assembly.FullName.StartsWith("Microsoft") || assembly.IsDynamic)
                return;

            try
            {
                foreach (var mcmaType in assembly.GetExportedTypes().Where(t => typeof(McmaObject).IsAssignableFrom(t)))
                    McmaTypes.Add(mcmaType);
            }
            catch (Exception ex)
            {
                Logger.System.Warn($"Failed to load types from assembly {assembly.FullName}.{Environment.NewLine}Exception:{Environment.NewLine}{ex}");
            }
        }
        
        public static IMcmaTypeRegistrations Add<T>() => Add(typeof(T));
        
        public static IMcmaTypeRegistrations Add(Type type) => Types.Add(type);

        public static Type FindType(string typeString)
        {
            if (typeString == null)
                return null;

            // check for match in explicitly-provided type collection
            var objectType = Types.FirstOrDefault(t => t.Name.Equals(typeString, StringComparison.OrdinalIgnoreCase));
            if (objectType == null)
            {
                // check for match in core types
                objectType = Type.GetType(typeof(McmaObject).AssemblyQualifiedName.Replace(nameof(McmaObject), typeString));
            }

            return objectType;
        }

        public static Type GetResourceType(this McmaResource resource) => resource?.Type != null ? FindType(resource.Type) : null;
    }
}