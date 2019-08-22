using System;
using System.Collections.Generic;
using System.Linq;

namespace BrowserCollectionViewer
{
    public class TypeConverter<BaseType>
    {
        private static Dictionary<string, Type> _types;
        private static object _lock = new object();

        public static Type FromString(string typeName)
        {
            if (_types == null) CacheTypes();

            if (_types.ContainsKey(typeName))
            {
                return _types[typeName];
            }
            else
            {
                return null;
            }
        }

        private static void CacheTypes()
        {
            lock (_lock)
            {
                if (_types == null)
                {
                    // Initialize the myTypes list.
                    var baseType = typeof(BaseType);
                    var typeAssembly = baseType.Assembly;
                    var types = typeAssembly.GetTypes().Where(t =>
                        t.IsClass &&
                        !t.IsAbstract &&
                        baseType.IsAssignableFrom(t));

                    _types = types.ToDictionary(t => t.Name);
                }
            }
        }
    }
}