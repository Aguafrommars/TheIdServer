using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    public class ClientUrl
    {
        public ClientUrl(Entity.ClientUri uri)
        {
            ClientUri = uri;
        }
        public Entity.ClientUri ClientUri { get; }

        public string Id => ClientUri.Id;
        public string Uri
        {
            get { return ClientUri.Uri; }
            set { ClientUri.Uri = value; }
        }

        public bool Cors
        {
            get { return ((Entity.UriKind)ClientUri.Kind & Entity.UriKind.Cors) == Entity.UriKind.Cors; }
            set 
            {
                if (value)
                {
                    ClientUri.Kind |= (int)Entity.UriKind.Cors;
                }
                else
                {
                    ClientUri.Kind &= (int)~Entity.UriKind.Cors;
                }
            }
        }

        public bool Redirect
        {
            get { return ((Entity.UriKind)ClientUri.Kind & Entity.UriKind.Redirect) == Entity.UriKind.Redirect; }
            set
            {
                if (value)
                {
                    ClientUri.Kind |= (int)Entity.UriKind.Redirect;
                }
                else
                {
                    ClientUri.Kind &= (int)~Entity.UriKind.Redirect;
                }
            }
        }

        public bool PostLogout
        {
            get { return ((Entity.UriKind)ClientUri.Kind & Entity.UriKind.PostLogout) == Entity.UriKind.PostLogout; }
            set
            {
                if (value)
                {
                    ClientUri.Kind |= (int)Entity.UriKind.PostLogout;
                }
                else
                {
                    ClientUri.Kind &= (int)~Entity.UriKind.PostLogout;
                }
            }
        }
    }
}
