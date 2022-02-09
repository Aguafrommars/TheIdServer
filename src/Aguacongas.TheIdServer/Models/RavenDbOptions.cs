// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre

namespace Aguacongas.TheIdServer.Models
{
    public class RavenDbOptions
    {
        public string[] Urls { get; set; }

        public string Database { get; set; }

        public string CertificatePath { get; set; }

        public string CertificatePassword { get; set; }
    }
}
