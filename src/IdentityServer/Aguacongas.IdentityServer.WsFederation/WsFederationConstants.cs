// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
namespace Duende.IdentityServer.WsFederation
{
    /// <summary>
    /// Ws-Federation constants
    /// </summary>
    public static class WsFederationConstants
    {
        /// <summary>
        /// 
        /// </summary>
        public static class SamlNameIdentifierFormats
        {
            /// <summary>
            /// The email address string
            /// </summary>
            public static readonly string EmailAddressString = "urn:oasis:names:tc:SAML:1.1:nameid-format:emailAddress";
            /// <summary>
            /// The encrypted string
            /// </summary>
            public static readonly string EncryptedString = "urn:oasis:names:tc:SAML:2.0:nameid-format:encrypted";
            /// <summary>
            /// The entity string
            /// </summary>
            public static readonly string EntityString = "urn:oasis:names:tc:SAML:2.0:nameid-format:entity";
            /// <summary>
            /// The kerberos string
            /// </summary>
            public static readonly string KerberosString = "urn:oasis:names:tc:SAML:2.0:nameid-format:kerberos";
            /// <summary>
            /// The persistent string
            /// </summary>
            public static readonly string PersistentString = "urn:oasis:names:tc:SAML:2.0:nameid-format:persistent";
            /// <summary>
            /// The transient string
            /// </summary>
            public static readonly string TransientString = "urn:oasis:names:tc:SAML:2.0:nameid-format:transient";
            /// <summary>
            /// The unspecified string
            /// </summary>
            public static readonly string UnspecifiedString = "urn:oasis:names:tc:SAML:1.1:nameid-format:unspecified";
            /// <summary>
            /// The windows domain qualified name string
            /// </summary>
            public static readonly string WindowsDomainQualifiedNameString = "urn:oasis:names:tc:SAML:1.1:nameid-format:WindowsDomainQualifiedName";
            /// <summary>
            /// The X509 subject name string
            /// </summary>
            public static readonly string X509SubjectNameString = "urn:oasis:names:tc:SAML:1.1:nameid-format:X509SubjectName";
        }

        /// <summary>
        /// 
        /// </summary>
        public static class TokenTypes
        {
            /// <summary>
            /// The json web token
            /// </summary>
            public static readonly string JsonWebToken = "urn:ietf:params:oauth:token-type:jwt";
            /// <summary>
            /// The kerberos
            /// </summary>
            public static readonly string Kerberos = "http://schemas.microsoft.com/ws/2006/05/identitymodel/tokens/Kerberos";
            /// <summary>
            /// The oasis WSS saml11 token profile11
            /// </summary>
            public static readonly string OasisWssSaml11TokenProfile11 = "http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV1.1";
            /// <summary>
            /// The oasis WSS saml2 token profile11
            /// </summary>
            public static readonly string OasisWssSaml2TokenProfile11 = "http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV2.0";
            /// <summary>
            /// The RSA
            /// </summary>
            public static readonly string Rsa = "http://schemas.microsoft.com/ws/2006/05/identitymodel/tokens/Rsa";
            /// <summary>
            /// The saml11 token profile11
            /// </summary>
            public const string Saml11TokenProfile11 = "urn:oasis:names:tc:SAML:1.0:assertion";
            /// <summary>
            /// The saml2 token profile11
            /// </summary>
            public const string Saml2TokenProfile11 = "urn:oasis:names:tc:SAML:2.0:assertion";
            /// <summary>
            /// The simple web token
            /// </summary>
            public static readonly string SimpleWebToken = "http://schemas.xmlsoap.org/ws/2009/11/swt-token-profile-1.0";
            /// <summary>
            /// The user name
            /// </summary>
            public static readonly string UserName = "http://schemas.microsoft.com/ws/2006/05/identitymodel/tokens/UserName";
            /// <summary>
            /// The X509 certificate
            /// </summary>
            public static readonly string X509Certificate = "http://schemas.microsoft.com/ws/2006/05/identitymodel/tokens/X509Certificate";
        }

        /// <summary>
        /// 
        /// </summary>
        public static class Attributes
        {
            /// <summary>
            /// The protocol support enumeration
            /// </summary>
            public static readonly string ProtocolSupportEnumeration = "protocolSupportEnumeration";
            /// <summary>
            /// The context
            /// </summary>
            public static readonly string Context = "Context";
            /// <summary>
            /// The token types offered
            /// </summary>
            public static readonly string TokenTypesOffered = "TokenTypesOffered";
            /// <summary>
            /// The token type
            /// </summary>
            public static readonly string TokenType = "TokenType";
            /// <summary>
            /// The URI
            /// </summary>
            public static readonly string Uri = "Uri";
        }

        /// <summary>
        /// 
        /// </summary>
        public static class Elements
        {
            /// <summary>
            /// The security token service endpoint
            /// </summary>
            public static readonly string SecurityTokenServiceEndpoint = "SecurityTokenServiceEndpoint";
            /// <summary>
            /// The request security token response collection
            /// </summary>
            public static readonly string RequestSecurityTokenResponseCollection = "RequestSecurityTokenResponseCollection";
            /// <summary>
            /// The passive requestor endpoint
            /// </summary>
            public static readonly string PassiveRequestorEndpoint = "PassiveRequestorEndpoint";
        }

        /// <summary>
        /// The XMLNS
        /// </summary>
        public static readonly string Xmlns = "xmlns";


        /// <summary>
        /// The wreply
        /// </summary>
        public static readonly string Wreply = "wreply";


        /// <summary>
        /// 
        /// </summary>
        public static class Prefixes
        {
            /// <summary>
            /// The xsi
            /// </summary>
            public static readonly string Xsi = "xsi";
        }
    }
}
