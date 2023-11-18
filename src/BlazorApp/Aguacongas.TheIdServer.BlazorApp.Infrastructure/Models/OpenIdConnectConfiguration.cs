using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Models;
public class OpenIdConnectConfiguration
{
    /// <summary>
    /// Gets or sets the collection of 'acr_values_supported'
    /// </summary>
    public IEnumerable<string> AcrValuesSupported { get; set; }

    /// <summary>
    /// Gets or sets the 'authorization_endpoint'.
    /// </summary>
    public string AuthorizationEndpoint { get; set; }

    /// <summary>
    /// Gets or sets the 'check_session_iframe'.
    /// </summary>
    public string CheckSessionIframe { get; set; }

    /// <summary>
    /// Gets or sets the collection of 'claims_supported'
    /// </summary>
    public IEnumerable<string> ClaimsSupported { get; set; }

    /// <summary>
    /// Gets or sets the collection of 'claims_locales_supported'
    /// </summary>
    public IEnumerable<string> ClaimsLocalesSupported { get; set; }

    /// <summary>
    /// Gets or sets the 'claims_parameter_supported'
    /// </summary>
    public bool ClaimsParameterSupported { get; set; }

    /// <summary>
    /// Gets or sets the collection of 'claim_types_supported'
    /// </summary>
    public IEnumerable<string> ClaimTypesSupported { get; set; }

    /// <summary>
    /// Gets or sets the collection of 'display_values_supported'
    /// </summary>
    public IEnumerable<string> DisplayValuesSupported { get; set; }

    /// <summary>
    /// Gets or sets the 'end_session_endpoint'.
    /// </summary>
    public string EndSessionEndpoint { get; set; }

    /// <summary>
    /// Gets or sets the 'frontchannel_logout_session_supported'.
    /// </summary>
    public string FrontchannelLogoutSessionSupported { get; set; }

    /// <summary>
    /// Gets or sets the 'frontchannel_logout_supported'.
    /// </summary>
    public string FrontchannelLogoutSupported { get; set; }

    /// <summary>
    /// Gets or sets the collection of 'grant_types_supported'
    /// </summary>
    public IEnumerable<string> GrantTypesSupported { get; set; }

    /// <summary>
    /// Boolean value specifying whether the OP supports HTTP-based logout. Default is false.
    /// </summary>
    public bool HttpLogoutSupported { get; set; }

    /// <summary>
    /// Gets or sets the collection of 'id_token_encryption_alg_values_supported'.
    /// </summary>
    public IEnumerable<string> IdTokenEncryptionAlgValuesSupported { get; set; }

    /// <summary>
    /// Gets or sets the collection of 'id_token_encryption_enc_values_supported'.
    /// </summary>
    public IEnumerable<string> IdTokenEncryptionEncValuesSupported { get; set; }

    /// <summary>
    /// Gets or sets the collection of 'id_token_signing_alg_values_supported'.
    /// </summary>
    public IEnumerable<string> IdTokenSigningAlgValuesSupported { get; set; }

    /// <summary>
    /// Gets or sets the 'introspection_endpoint'.
    /// </summary>
    public string IntrospectionEndpoint { get; set; }

    /// <summary>
    /// Gets or sets the collection of 'introspection_endpoint_auth_methods_supported'.
    /// </summary>
    public IEnumerable<string> IntrospectionEndpointAuthMethodsSupported { get; set; }

    /// <summary>
    /// Gets or sets the collection of 'introspection_endpoint_auth_signing_alg_values_supported'.
    /// </summary>
    public IEnumerable<string> IntrospectionEndpointAuthSigningAlgValuesSupported { get; set; }

    /// <summary>
    /// Gets or sets the 'issuer'.
    /// </summary>
    public string Issuer { get; set; }

    /// <summary>
    /// Gets or sets the 'jwks_uri'
    /// </summary>
    public string JwksUri { get; set; }

    /// <summary>
    /// Boolean value specifying whether the OP can pass a sid (session ID) query parameter to identify the RP session at the OP when the logout_uri is used. Dafault Value is false.
    /// </summary>
    public bool LogoutSessionSupported { get; set; }

    /// <summary>
    /// Gets or sets the 'op_policy_uri'
    /// </summary>
    public string OpPolicyUri { get; set; }

    /// <summary>
    /// Gets or sets the 'op_tos_uri'
    /// </summary>
    public string OpTosUri { get; set; }

    /// <summary>
    /// Gets or sets the 'registration_endpoint'
    /// </summary>
    public string RegistrationEndpoint { get; set; }

    /// <summary>
    /// Gets or sets the collection of 'request_object_encryption_alg_values_supported'.
    /// </summary>
    public IEnumerable<string> RequestObjectEncryptionAlgValuesSupported { get; set; }

    /// <summary>
    /// Gets or sets the collection of 'request_object_encryption_enc_values_supported'.
    /// </summary>
    public IEnumerable<string> RequestObjectEncryptionEncValuesSupported { get; set; }

    /// <summary>
    /// Gets or sets the collection of 'request_object_signing_alg_values_supported'.
    /// </summary>
    public IEnumerable<string> RequestObjectSigningAlgValuesSupported { get; set; }

    /// <summary>
    /// Gets or sets the 'request_parameter_supported'
    /// </summary>
    public bool RequestParameterSupported { get; set; }

    /// <summary>
    /// Gets or sets the 'request_uri_parameter_supported'
    /// </summary>
    public bool RequestUriParameterSupported { get; set; }

    /// <summary>
    /// Gets or sets the 'require_request_uri_registration'
    /// </summary>
    public bool RequireRequestUriRegistration { get; set; }

    /// <summary>
    /// Gets or sets the collection of 'response_modes_supported'.
    /// </summary>
    public IEnumerable<string> ResponseModesSupported { get; set; }

    /// <summary>
    /// Gets or sets the collection of 'response_types_supported'.
    /// </summary>
    public IEnumerable<string> ResponseTypesSupported { get; set; }

    /// <summary>
    /// Gets or sets the 'service_documentation'
    /// </summary>
    public string ServiceDocumentation { get; set; }

    /// <summary>
    /// Gets or sets the collection of 'scopes_supported'
    /// </summary>
    public IEnumerable<string> ScopesSupported { get; set; }

    /// <summary>
    /// Gets or sets the collection of 'subject_types_supported'.
    /// </summary>
    public IEnumerable<string> SubjectTypesSupported { get; set; }

    /// <summary>
    /// Gets or sets the 'token_endpoint'.
    /// </summary>
    public string TokenEndpoint { get; set; }

    /// <summary>
    /// Gets or sets the collection of 'token_endpoint_auth_methods_supported'.
    /// </summary>
    public IEnumerable<string> TokenEndpointAuthMethodsSupported { get; set; }

    /// <summary>
    /// Gets or sets the collection of 'token_endpoint_auth_signing_alg_values_supported'.
    /// </summary>
    public IEnumerable<string> TokenEndpointAuthSigningAlgValuesSupported { get; set; }

    /// <summary>
    /// Gets or sets the collection of 'ui_locales_supported'
    /// </summary>
    public IEnumerable<string> UILocalesSupported { get; set; }

    /// <summary>
    /// Gets or sets the 'user_info_endpoint'.
    /// </summary>
    public string UserInfoEndpoint { get; set; }

    /// <summary>
    /// Gets or sets the collection of 'userinfo_encryption_alg_values_supported'
    /// </summary>
    public IEnumerable<string> UserInfoEndpointEncryptionAlgValuesSupported { get; set; }

    /// <summary>
    /// Gets or sets the collection of 'userinfo_encryption_enc_values_supported'
    /// </summary>
    public IEnumerable<string> UserInfoEndpointEncryptionEncValuesSupported { get; set; }

    /// <summary>
    /// Gets or sets the collection of 'userinfo_signing_alg_values_supported'
    /// </summary>
    public IEnumerable<string> UserInfoEndpointSigningAlgValuesSupported { get; set; }

}

