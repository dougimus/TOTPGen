using System;
using System.Globalization;
using System.Security.Cryptography;

namespace TOTPGen
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTimeOffset now = new DateTimeOffset(DateTime.UtcNow);
            long unixtime = now.ToUnixTimeSeconds();
            int timesteps = (int)Math.Floor((decimal)unixtime / 30);

            string hexsteps = timesteps.ToString("X16");

            byte[] hexbyte = new byte[8];
            char[] hexstep = hexsteps.ToCharArray();
            int steps = 0;
            for(int i=0;i<hexbyte.Length;i++)
            {
                hexbyte[i] = byte.Parse(hexstep[steps].ToString() + hexstep[steps+1].ToString(),NumberStyles.HexNumber);
                steps += 2;
            }

            byte[] sharedsecret = Base32.FromBase32String(args[0]);

            HMACSHA1 hasher = new HMACSHA1(sharedsecret);

            byte[] hash = hasher.ComputeHash(hexbyte);

            byte last = hash[19];
            
            int offset = int.Parse((last & 0x0f).ToString("X2"), NumberStyles.HexNumber);

            Console.WriteLine(long.Parse((hash[offset] & 0x7f).ToString("X2") + (hash[offset + 1] & 0xff).ToString("X2") + (hash[offset + 2] & 0xff).ToString("X2") + (hash[offset + 3] & 0xff).ToString("X2"),NumberStyles.HexNumber) % Math.Pow(10,6));
        }
    }
}
