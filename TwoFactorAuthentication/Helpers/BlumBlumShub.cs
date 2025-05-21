using System.Numerics;

namespace TwoFactorAuthentication.Helpers;
public class BlumBlumShub
{
    private BigInteger state;
    private readonly BigInteger modulus;

    // p ve q büyük asal sayılar, ikisi de 3 mod 4'e göre ≡ 3 olmalı.
    public BlumBlumShub(BigInteger seed, BigInteger p, BigInteger q)
    {
        if (p % 4 != 3 || q % 4 != 3)
            throw new ArgumentException("p ve q asal sayıları 4 modunda 3 olmalı.");

        modulus = p * q;
        state = seed % modulus;
        if (state == 0)
            state = 1;
    }

    public int NextBit()
    {
        state = (state * state) % modulus;
        return (int)(state % 2);
    }

    public int NextNumber(int bits)
    {
        int result = 0;
        for (int i = 0; i < bits; i++)
        {
            result = (result << 1) | NextBit();
        }
        return result;
    }
}