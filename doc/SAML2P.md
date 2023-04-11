# Configure a SAML 2.0 client

![SAML2P](assets/SAML2p.jpeg)

The client id is the issuer.
Add identiy scopes to select user claims.

## Encryption certificate, signature algorithm, name id format, claims mapping

Associate your client with relying party to configure an encryption certificate, a signature algorithm, the name id format and/or the claims mapping.

## ACS Artifact

Add the property *UseAcsArtifact* with value *true* to use ACS artifact.

![UseAcsArtifact](assets/UseAcsArtifact.jpeg)