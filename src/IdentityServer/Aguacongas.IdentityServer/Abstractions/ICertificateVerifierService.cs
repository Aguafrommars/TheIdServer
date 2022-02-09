// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Abstractions
{
    public interface ICertificateVerifierService
    {
        Task<IEnumerable<string>> VerifyAsync(Stream certificateContent);
    }
}