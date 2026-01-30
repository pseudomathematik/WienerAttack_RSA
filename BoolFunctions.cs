using WpfApp_Wienner_Attack_BigInteger;
using System.Numerics;

namespace WpfApp_Wienner_Attack_Bool
{
	internal class BoolFunctions
	{
		public static bool HasIntegerRoot(BigInteger x)
		{
			if (x < 0) return false;
			BigInteger r = BigIntegerFunctions.BISqrt(x);
			return r >= 0 && r * r == x;
		}
		public static bool TryRecoverPQFromPhi(BigInteger N, BigInteger phiCandidate, out BigInteger p, out BigInteger q)
		{
			p = 0; q = 0;

			if (phiCandidate <= 0 || phiCandidate >= N) return false;

			BigInteger s = N - phiCandidate + 1; 
			if (s <= 0) return false;

			BigInteger D = s * s - 4 * N;
			if (D < 0) return false;
			if (!HasIntegerRoot(D)) return false;

			BigInteger r = BigIntegerFunctions.BISqrt(D);

			
			if (((s + r) & 1) != 0 || ((s - r) & 1) != 0) return false;

			p = (s + r) / 2;
			q = (s - r) / 2;

			if (p <= 1 || q <= 1) return false;
			if (p * q != N) return false;

			return true;
		}
	}
}
