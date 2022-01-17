using System;
using System.Reflection;

namespace Aguacongas.IdentityServer.Extensions
{
    public static class TypeExtension
    {
        public static bool ImplementsGenericInterface(this Type type, Type interfaceType)
        {
            if (type.IsGenericType(interfaceType))
            {
                return true;
            }
            foreach (var @interface in type.GetTypeInfo().ImplementedInterfaces)
            {
                if (@interface.IsGenericType(interfaceType))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsGenericType(this Type type, Type genericType)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == genericType;
        }
    }
}
