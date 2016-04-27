using System;
using HomeExam.Helpers;

namespace HomeExam
{
	public static class Utility
	{
		public static bool IsZero(this double val, double offByTolerance = 0.00001)
		{
			return Math.Abs(val) < offByTolerance;
		}

		public static int RandomSign(this Random rnd)
		{
			return rnd.Next(0, 2) * 2 - 1;
		}

		public static double RandomRange(this Random rnd, double min, double max)
		{
			return rnd.NextDouble() * (max - min) + min;
		}
	}
}
