// Project: Aguafrommars/TheIdServer
// Copyright (c) 2026 @Olivier Lefebvre
namespace Aguacongas.IdentityServer.Store
{
    /// <summary>
    /// Helper methods to build safe OData filter expressions.
    /// </summary>
    public static class ODataFilter
    {
        /// <summary>
        /// Builds an OData string literal. Single quotes are escaped by doubling them as
        /// specified by the OData specification, preventing filter injection.
        /// </summary>
        /// <param name="value">The value to escape.</param>
        /// <returns>The escaped literal.</returns>
        public static string Literal(string value)
        {
            return value == null ? "''" : $"'{value.Replace("'", "''")}'";
        }

        /// <summary>
        /// Builds a <c>contains</c> predicate for the given property and value.
        /// </summary>
        /// <param name="property">The property name.</param>
        /// <param name="value">The value to search for.</param>
        /// <returns>The predicate.</returns>
        public static string Contains(string property, string value)
        {
            return $"contains({property},{Literal(value)})";
        }
    }
}