using System;
using System.Linq;
using System.Reflection;

namespace Aguacongas.IdentityServer.Extensions
{
    public static class TypeExtension
    {
        public static bool ImplementsGenericInterface(this Type type, Type interfaceType)
        => type.IsGenericType(interfaceType) || type.GetTypeInfo().ImplementedInterfaces.Any(info => info.IsGenericType(interfaceType));
        public static bool IsGenericType(this Type type, Type genericType)
        => type.IsGenericType && type.GetGenericTypeDefinition() == genericType;
    }
}
