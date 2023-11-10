namespace Aguacongas.TheIdServer.Identity.BcryptPasswordHasher;

public class BcryptPasswordHasherOptions
{
    public int WorkFactor { get; set; } = 11;
    public byte HashPrefix { get; set; } = 0xBC;
}
