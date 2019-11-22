using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    public class ClientUri: Entity.ClientUri
    {
        public ClientUri(Entity.ClientUri parent)
        {
            Parent = parent;
            Id = parent.Id;
            Uri = parent.Uri;
        }
        public Entity.ClientUri Parent { get; }

        public bool Cors
        {
            get { return ((Entity.UriKind)Parent.Kind & Entity.UriKind.Cors) == Entity.UriKind.Cors; }
            set 
            {
                if (value)
                {
                    Parent.Kind |= (int)Entity.UriKind.Cors;
                }
                else
                {
                    Parent.Kind &= (int)~Entity.UriKind.Cors;
                }
            }
        }

        public bool Redirect
        {
            get { return ((Entity.UriKind)Parent.Kind & Entity.UriKind.Redirect) == Entity.UriKind.Redirect; }
            set
            {
                if (value)
                {
                    Parent.Kind |= (int)Entity.UriKind.Redirect;
                }
                else
                {
                    Parent.Kind &= (int)~Entity.UriKind.Redirect;
                }
            }
        }

        public bool PostLogout
        {
            get { return ((Entity.UriKind)Parent.Kind & Entity.UriKind.PostLogout) == Entity.UriKind.PostLogout; }
            set
            {
                if (value)
                {
                    Parent.Kind |= (int)Entity.UriKind.PostLogout;
                }
                else
                {
                    Parent.Kind &= (int)~Entity.UriKind.PostLogout;
                }
            }
        }
    }
}
