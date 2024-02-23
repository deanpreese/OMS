using System.Security.Cryptography;

namespace OMS.Relay.Services;

public class RelayPlatformOrderIDGen : IRelayPlatformOrderIDGen
{
    private RandomNumberGenerator rng;
    
    public RelayPlatformOrderIDGen()
    {
        rng = RandomNumberGenerator.Create();
    }

    public int GetNextOrderID()
    {
        var salt = new byte[4];
        rng.GetBytes(salt);
        return BitConverter.ToInt32(salt, 0);
    }
}
