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
            get { return ((Entity.UriKinds)Parent.Kind & Entity.UriKinds.Cors) == Entity.UriKinds.Cors; }
            set 
            {
                if (value)
                {
                    Parent.Kind |= (int)Entity.UriKinds.Cors;
                }
                else
                {
                    Parent.Kind &= (int)~Entity.UriKinds.Cors;
                }
            }
        }

        public bool Redirect
        {
            get { return ((Entity.UriKinds)Parent.Kind & Entity.UriKinds.Redirect) == Entity.UriKinds.Redirect; }
            set
            {
                if (value)
                {
                    Parent.Kind |= (int)Entity.UriKinds.Redirect;
                }
                else
                {
                    Parent.Kind &= (int)~Entity.UriKinds.Redirect;
                }
            }
        }

        public bool PostLogout
        {
            get { return ((Entity.UriKinds)Parent.Kind & Entity.UriKinds.PostLogout) == Entity.UriKinds.PostLogout; }
            set
            {
                if (value)
                {
                    Parent.Kind |= (int)Entity.UriKinds.PostLogout;
                }
                else
                {
                    Parent.Kind &= (int)~Entity.UriKinds.PostLogout;
                }
            }
        }
    }
}
