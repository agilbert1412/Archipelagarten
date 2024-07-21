using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx.Logging;

namespace Archipelagarten2.Utilities
{
    public static class TypeAccess
    {
        public static void Initialize()
        {
        }

        public static Type TypeByName(string name)
        {
            var type = Type.GetType(name, false);
            if ((object)type == null)
                type = AllTypes().FirstOrDefault(t => t.FullName == name);
            if ((object)type == null)
                type = AllTypes().FirstOrDefault(t => t.Name == name);
            if ((object)type == null)
                DebugLogging.LogErrorMessage("TypeAccess.TypeByName: Could not find type named " + name);
            return type;
        }

        public static IEnumerable<Type> AllTypes() => AllAssemblies().SelectMany(GetTypesFromAssembly);
        
        public static IEnumerable<Assembly> AllAssemblies() => AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.FullName.StartsWith("Microsoft.VisualStudio"));
        
        public static Type[] GetTypesFromAssembly(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                DebugLogging.LogErrorMessage($"TypeAccess.GetTypesFromAssembly: assembly {assembly} => {ex}");
                return ex.Types.Where(type => type != null).ToArray();
            }
        }
    }
}
