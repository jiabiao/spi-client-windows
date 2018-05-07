using System;
using System.Numerics;

namespace SPIClient
{
    /// <summary>
    /// This class implements the Diffie-Hellman algorithm using BigIntegers.
    /// It can do the 3 main things:
    /// 1. Generate a random Private Key for you.
    /// 2. Generate your Public Key based on your Private Key.
    /// 3. Generate the Secret given their Public Key and your Private Key
    /// p and g are the shared constants for the algorithm, aka primeP and primeG.
    /// </summary>
    public static class DiffieHellman
    {

        /// <summary>
        /// Generates a random Private Key that you can use.
        /// </summary>
        /// <param name="p"></param>
        /// <returns>Random Private Key</returns>
        public static BigInteger RandomPrivateKey(BigInteger p)
        {
            var max = p - 1;
            var randBigInt = RandomHelper.RandomBigIntMethod2(max);

            // The above could give us 0 or 1, but we need min 2. So quick, albeit slightly biasing, cheat below.
            var min = new BigInteger(2);
            if (randBigInt < min)
                randBigInt = min;
            
            return randBigInt;
        }

        /// <summary>
        /// Calculates the Public Key from a Private Key.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="g"></param>
        /// <param name="privateKey"></param>
        /// <returns>Public Key</returns>
        public static BigInteger PublicKey(BigInteger p, BigInteger g, BigInteger privateKey)
        {
            // A = g**a mod p
            return BigInteger.ModPow(g, privateKey, p);
        }

        /// <summary>
        /// Calculates the shared secret given their Public Key (A) and your Private Key (b)
        /// </summary>
        /// <param name="p"></param>
        /// <param name="theirPublicKey"></param>
        /// <param name="yourPrivateKey"></param>
        /// <returns></returns>
        public static BigInteger Secret(BigInteger p, BigInteger theirPublicKey, BigInteger yourPrivateKey)
        {
            // s = A**b mod p
            return BigInteger.ModPow(theirPublicKey, yourPrivateKey, p);
        }
        
    }

    public static class RandomHelper
    {
        private static readonly Random RandomGen = new Random();
        
        public static BigInteger RandomBigIntMethod1(BigInteger max)
        {
            // The below code is a technique to generate random positive big integer up to max.
            // You can use different techniques to generate such random number, but be careful
            // about performance. Generating big random numbers could be processing intensive.
            var bytes = max.ToByteArray();
            BigInteger randBigInt;
            do
            {
                RandomGen.NextBytes(bytes);
                //C# may give us negative bytes so we need to force sign to be positive
                bytes[bytes.Length - 1] &= (byte) 0x7F;
                randBigInt = new BigInteger(bytes);
            } while (randBigInt >= max);
            return randBigInt;
        }
        
        public static BigInteger RandomBigIntMethod2(BigInteger max)
        {
            var maxIntString = max.ToString(); // this is our maximum number represented as a string, example "2468"
            string randomIntString = ""; // we will build our random number as a string
            
            bool below = false; // this indicates whether we've gone below a significant digit yet.
            
            // we go through the digits from most sihgnificant to least significant.
            foreach (var thisDigit in maxIntString)
            {
                int maxDigit;
                if (!below)
                {
                    // we need to pick a digit that is equal or smaller than this one.
                    maxDigit = int.Parse(thisDigit.ToString());
                }
                else
                {
                    // we've already gone below. So we can pick any digit from 0-9.
                    maxDigit = 9;
                }
                
                var rDigit = RandomGen.Next(0, 10);
                if (rDigit > maxDigit)
                    rDigit = 0;
                randomIntString += rDigit.ToString();
                if (rDigit < maxDigit) {
                    // We've picked a digit smaller than the corresponding one in the maximum.
                    // So from now on, any digit is good.
                    below = true;
                }
            }
            return BigInteger.Parse(randomIntString);
        }
        
        
    }
}