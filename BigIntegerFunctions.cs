using System;
using System.Collections.Generic;
using System.Numerics;

namespace WpfApp_Wienner_Attack_BigInteger
{
	internal class BigIntegerFunctions
	{
		public static BigInteger BISqrt(BigInteger n)
		{
			if (n < 0) return -1;
			BigInteger r = (BigInteger)Math.Sqrt((double)n);
			while (r * r > n) r--;
			while ((r + 1) * (r + 1) <= n) r++;
			return r;
		}

		public static List<BigInteger> ContinuedFraction(BigInteger a, BigInteger b)
		{
			var list = new List<BigInteger>();
			while (b != 0)
			{
				list.Add(a / b);
				var t = a % b;
				a = b;
				b = t;
			}
			return list;
		}
		public static List<(BigInteger k, BigInteger d)> Convergents(List<BigInteger> cf)
		{
			var res = new List<(BigInteger k, BigInteger d)>();
			BigInteger k0 = 0, d0 = 1;
			BigInteger k1 = 1, d1 = 0;

			foreach (var ai in cf)
			{
				BigInteger k = ai * k1 + k0;
				BigInteger d = ai * d1 + d0;
				res.Add((k, d));
				k0 = k1; d0 = d1;
				k1 = k; d1 = d;
			}
			return res;
		}

		public static BigInteger ModInverse(BigInteger a, BigInteger m)
		{
			BigInteger t = 0, newT = 1;
			BigInteger r = m, newR = a;

			while (newR != 0)
			{
				BigInteger q = r / newR;
				(t, newT) = (newT, t - q * newT);
				(r, newR) = (newR, r - q * newR);
			}
			if (r > 1) throw new Exception("a is not invertible mod m");
			if (t < 0) t += m;
			return t;
		}
	}
}
