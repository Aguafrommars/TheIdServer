namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class InputLengthRestrictions
    {
        /// <summary>
        /// Max length for client_id
        /// </summary>
        public int ClientId { get; set; }

        /// <summary>
        /// Max length for external client secrets
        /// </summary>
        public int ClientSecret { get; set; }

        /// <summary>
        /// Max length for scope
        /// </summary>
        public int Scope { get; set; }

        /// <summary>
        /// Max length for redirect_uri
        /// </summary>
        public int RedirectUri { get; set; }

        /// <summary>
        /// Max length for nonce
        /// </summary>
        public int Nonce { get; set; }

        /// <summary>
        /// Max length for ui_locale
        /// </summary>
        public int UiLocale { get; set; }

        /// <summary>
        /// Max length for login_hint
        /// </summary>
        public int LoginHint { get; set; }

        /// <summary>
        /// Max length for acr_values
        /// </summary>
        public int AcrValues { get; set; }

        /// <summary>
        /// Max length for grant_type
        /// </summary>
        public int GrantType { get; set; }

        /// <summary>
        /// Max length for username
        /// </summary>
        public int UserName { get; set; }

        /// <summary>
        /// Max length for password
        /// </summary>
        public int Password { get; set; }

        /// <summary>
        /// Max length for CSP reports
        /// </summary>
        public int CspReport { get; set; }

        /// <summary>
        /// Max length for external identity provider name
        /// </summary>
        public int IdentityProvider { get; set; }

        /// <summary>
        /// Max length for external identity provider errors
        /// </summary>
        public int ExternalError { get; set; }

        /// <summary>
        /// Max length for authorization codes
        /// </summary>
        public int AuthorizationCode { get; set; }

        /// <summary>
        /// Max length for device codes
        /// </summary>
        public int DeviceCode { get; set; }

        /// <summary>
        /// Max length for refresh tokens
        /// </summary>
        public int RefreshToken { get; set; }

        /// <summary>
        /// Max length for token handles
        /// </summary>
        public int TokenHandle { get; set; }

        /// <summary>
        /// Max length for JWTs
        /// </summary>
        public int Jwt { get; set; }

        /// <summary>
        /// Max length for binding_message
        /// </summary>
        public int BindingMessage { get; set; }

        /// <summary>
        /// Max length for user_code
        /// </summary>
        public int UserCode { get; set; }

        /// <summary>
        /// Max length for id_token_hint
        /// </summary>
        public int IdTokenHint { get; set; }

        /// <summary>
        /// Max length for login_hint_token
        /// </summary>
        public int LoginHintToken { get; set; }

        /// <summary>
        /// Max length for auth_req_id
        /// </summary>
        public int AuthenticationRequestId { get; set; }
    }
}