using System.Security.Cryptography;

using OMS.Infrastructure.Interfaces;

namespace OMS.Infrastructure.Services.Common;

public class PlatformOrderIDGen : IPlatformOrderIDGen
{
    private RandomNumberGenerator rng;

    public PlatformOrderIDGen()
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
