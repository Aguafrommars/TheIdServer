// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.IdentityServer.Extensions;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Aguacongas.IdentityServer.Store
{
    public static class GetRequestExtensions
    {
        public static string ToRQL<T, TKey>(this PageRequest request, string collectionName, Expression<Func<T, TKey>> keyDefinitionExpression) where T : class
        {
            var builder = new StringBuilder("from ");

            builder.Append(collectionName);
            var where = ToWhereClause(request, keyDefinitionExpression);
            if (where != null)
            {
                builder.Append('\n');
                builder.Append(where);
            }
            return ToIncludeAndOrderByClause<T>(request, builder);
        }

        public static string ToRQL<T>(this PageRequest request, string collectionName, IEdmModel edm) where T : class
        {
            var builder = new StringBuilder("from ");

            builder.Append(collectionName);
            var where = ToWhereClause<T>(request, edm);
            if (where != null)
            {
                builder.Append('\n');
                builder.Append(where);
            }
            return ToIncludeAndOrderByClause<T>(request, builder);
        }

        public static string ToWhereClause<T, TKey>(this PageRequest request, Expression<Func<T, TKey>> keyDefinitionExpression) 
            where T: class
        {
            var filter = request?.Filter;
            if (string.IsNullOrWhiteSpace(filter))
            {
                return null;
            }

            var builder = new ODataConventionModelBuilder();
            var entitySet = builder.EntitySet<T>(typeof(T).Name);
            var entityType = entitySet.EntityType;
            entityType.HasKey(keyDefinitionExpression);
            var edm = builder.GetEdmModel();
            var parser = new ODataUriParser(edm, new Uri($"{typeof(T).Name}?$filter={filter}", UriKind.Relative));
            var filterClause = parser.ParseFilter();
            var visitor = new RavenDbQueryNodeVisitor<T>();
            filterClause.Expression.Accept(visitor);

            return visitor.Builder.ToString();
        }

        public static string ToWhereClause<T>(this PageRequest request, IEdmModel edm)
            where T : class
        {
            var filter = request?.Filter;
            if (string.IsNullOrWhiteSpace(filter))
            {
                return null;
            }

            var parser = new ODataUriParser(edm, new Uri($"{typeof(T).Name}?$filter={filter}", UriKind.Relative));
            var filterClause = parser.ParseFilter();
            var visitor = new RavenDbQueryNodeVisitor<T>();
            filterClause.Expression.Accept(visitor);

            return visitor.Builder.ToString();
        }

        public static string ToIncludeClause<T>(this GetRequest request)
        {
            var expand = request?.Expand;
            if (string.IsNullOrWhiteSpace(expand))
            {
                return null;
            }

            var type = typeof(T);
            var pathList = expand.Split(',');
            var expandList = new List<string>(pathList.Length);
            foreach (var path in pathList)
            {
                var pathBuilder = GetIncludePath(type, path);
                expandList.Add(pathBuilder.ToString());
            }

            return $"include {string.Join(',', expandList)}";
        }

        public static string ToOrderByClause(this PageRequest request)
        {
            var orderBy = request?.OrderBy;
            if (string.IsNullOrWhiteSpace(orderBy))
            {
                return null;
            }

            return $"order by {orderBy}";
        }

        private static StringBuilder GetIncludePath(Type type, string path)
        {
            var pathBuilder = new StringBuilder();
            pathBuilder.Append(path);
            var property = type.GetProperty(path);
            if (property == null)
            {
                throw new InvalidOperationException($"{path} is not a property of '{type.Name}'.");
            }

            if (property.PropertyType.ImplementsGenericInterface(typeof(ICollection<>)))
            {
                pathBuilder.Append("[].Id");
                return pathBuilder;
            }

            if (property.PropertyType.GetProperty("Id") == null)
            {
                throw new InvalidOperationException($"{path} doesn't have a property named 'Id'.");
            }

            pathBuilder.Append("Id");
            return pathBuilder;
        }

        private static string ToIncludeAndOrderByClause<T>(PageRequest request, StringBuilder builder) where T : class
        {
            var include = ToIncludeClause<T>(request);
            if (include != null)
            {
                builder.Append('\n');
                builder.Append(include);
            }
            var orderBy = ToOrderByClause(request);
            if (orderBy != null)
            {
                builder.Append('\n');
                builder.Append(orderBy);
            }

            return builder.ToString();
        }

        class RavenDbQueryNodeVisitor<TSource> : QueryNodeVisitor<TSource> where TSource : class
        {
            public StringBuilder Builder { get; } = new StringBuilder("where ");
            public override TSource Visit(BinaryOperatorNode nodeIn)
            {
                nodeIn.Left.Accept(this);

                Builder.Append(' ');
                switch(nodeIn.OperatorKind)
                {
                    case BinaryOperatorKind.Or:
                    case BinaryOperatorKind.And:
                        Builder.Append(nodeIn.OperatorKind.ToString().ToLowerInvariant());
                        break;
                    case BinaryOperatorKind.Equal:
                        Builder.Append('=');
                        break;
                    case BinaryOperatorKind.NotEqual:
                        Builder.Append("!=");
                        break;
                    case BinaryOperatorKind.GreaterThan:
                        Builder.Append('>');
                        break;
                    case BinaryOperatorKind.GreaterThanOrEqual:
                        Builder.Append(">=");
                        break;
                    case BinaryOperatorKind.LessThan:
                        Builder.Append('<');
                        break;
                    case BinaryOperatorKind.LessThanOrEqual:
                        Builder.Append("<=");
                        break;
                }
                Builder.Append(' ');

                nodeIn.Right.Accept(this);                
                return null;
            }
            public override TSource Visit(SingleValuePropertyAccessNode nodeIn)
            {
                Builder.Append(nodeIn.Property.Name);
                return null;
            }

            public override TSource Visit(ConstantNode nodeIn)
            {
                if (nodeIn.Value is DateTime || nodeIn.Value is DateTimeOffset)
                {
                    Builder.Append('\'');
                    Builder.Append(nodeIn.LiteralText);
                    Builder.Append('\'');
                    return null;
                }

                Builder.Append(nodeIn.LiteralText);
                return null;
            }

            public override TSource Visit(ConvertNode nodeIn)
            {
                nodeIn.Source.Accept(this);
                return null;
            }

            public override TSource Visit(SingleValueFunctionCallNode nodeIn)
            {
                var functionName = nodeIn.Name;
                bool isSearch = false;
                if (functionName == "contains")
                {
                    functionName = "search";
                    isSearch = true;
                }
                Builder.Append(functionName);
                Builder.Append('(');

                var visitor = new FunctionQueryNodeVisitor<TSource>(nodeIn.Parameters.Count(), isSearch);
                foreach (var node in nodeIn.Parameters)
                {
                    node.Accept(visitor);                    
                }

                Builder.Append(string.Join(',', visitor.Parameters));
                Builder.Append(')');

                return null;
            }
        }

        class FunctionQueryNodeVisitor<TSource> : QueryNodeVisitor<TSource> where TSource : class
        {
            public List<string> Parameters { get; }

            private readonly bool _isSearch;

            public FunctionQueryNodeVisitor(int paramCount, bool isSearch)
            {
                Parameters = new List<string>(paramCount);
                _isSearch = isSearch;
            }

            public override TSource Visit(SingleValuePropertyAccessNode nodeIn)
            {
                Parameters.Add(nodeIn.Property.Name);
                return null;
            }

            public override TSource Visit(ConstantNode nodeIn)
            {
                if (_isSearch)
                {
                    Parameters.Add($"'*{nodeIn.Value}*'");
                    return null;
                }

                Parameters.Add(nodeIn.LiteralText);
                return null;
            }

            public override TSource Visit(ConvertNode nodeIn)
            {
                nodeIn.Source.Accept(this);
                return null;
            }
        }
    }
}
