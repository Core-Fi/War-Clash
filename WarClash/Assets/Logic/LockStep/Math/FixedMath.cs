//=======================================================================
// Copyright (c) 2015 John Pan
// Distributed under the MIT License.
// (See accompanying file LICENSE or copy at
// http://opensource.org/licenses/MIT)
//=======================================================================

#define HIGH_ACCURACY

using UnityEngine;
using System.Collections;
using FastCollections;
using System;
using System.Collections.Generic;

namespace Lockstep
{
	public static class FixedMath
	{

		#region Meta

		public const int SHIFT_AMOUNT = 16;
		public const long One = 1 << SHIFT_AMOUNT;
		public const long Half = One / 2;
		public const float OneF = (float)One;
		public const double OneD = (double)One;
		public const long Pi = (355 * One) / 113;
		public const long TwoPi = Pi * 2;
		public const long MaxFixedNumber = long.MaxValue >> SHIFT_AMOUNT;
		public const long TenDegrees = FixedMath.One * 1736 / 10000;
		public const long Epsilon = 1 << (SHIFT_AMOUNT - 10);

		#endregion

		#region Constructors

		/// <summary>
		/// Create a fixed point number from an integer.
		/// </summary>
		/// <param name="integer">Integer.</param>
		public static long Create(long integer)
		{
			return integer << SHIFT_AMOUNT;
		}

		public static long Create(float singleFloat)
		{
			return (long)((double)singleFloat * One);
		}

		/// <summary>
		/// Create a fixed point number from a double.
		/// </summary>
		/// <param name="doubleFloat">Double float.</param>
		public static long Create(double doubleFloat)
		{
			return (long)(doubleFloat * One);
		}

		/// <summary>
		/// Create a fixed point number from a fraction.
		/// </summary>
		/// <param name="whole">Whole.</param>
		/// <param name="fraction">Fraction.</param>
		public static long Create(long Numerator, long Denominator)
		{
			return (Numerator << SHIFT_AMOUNT) / Denominator;
		}

		/// <summary>
		/// Tries to parse string into fixed point number.
		/// </summary>
		/// <returns><c>true</c>, if parse was tried, <c>false</c> otherwise.</returns>
		/// <param name="s">S.</param>
		/// <param name="result">Result.</param>
		public static bool TryParse(string s, out long result)
		{
			string[] NewValues = s.Split('.');
			if (NewValues.Length <= 2)
			{
				long Whole;
				if (long.TryParse(NewValues[0], out Whole))
				{
					if (NewValues.Length == 1)
					{
						result = Whole << SHIFT_AMOUNT;
						return true;
					}
					else
					{
						long Numerator;
						if (long.TryParse(NewValues[1], out Numerator))
						{
							int fractionDigits = NewValues[1].Length;
							long Denominator = 1;
							for (int i = 0; i < fractionDigits; i++)
							{
								Denominator *= 10;
							}
							result = (Whole << SHIFT_AMOUNT) + FixedMath.Create(Numerator, Denominator);
							return true;
						}
					}
				}
			}
			result = 0;
			return false;
		}

		#endregion

		#region Math

		/// <summary>
		/// Addition.
		/// </summary>
		/// <param name="f1">f1.</param>
		/// <param name="f2">f2.</param>
		public static long Add(this long f1, long f2)
		{
			return f1 + f2;
		}
        public static long Add(this long f1, int f2)
        {
            return f1 + Create(f2);
        }
        /// <summary>
        /// Subtraction.
        /// </summary>
        /// <param name="f1">f1.</param>
        /// <param name="f2">f2.</param>
        public static long Sub(this long f1, long f2)
		{
			return f1 - f2;
		}

        public static long Sub(this long f1, int f2)
        {
            return f1 - Create(f2);
        }

        /// <summary>
        /// Multiplication.
        /// </summary>
        /// <param name="f1">f1.</param>
        /// <param name="f2">f2.</param>
        public static long Mul(this long f1, long f2)
        {
            f1 = (f1*f2) >> SHIFT_AMOUNT;
            return f1;
		}

		public static long Mul(this long f1, int intr)
		{
			return (f1 * intr);
		}

		/// <summary>
		/// Division.
		/// </summary>
		/// <param name="f1">f1.</param>
		/// <param name="f2">f2.</param>
		public static long Div(this long f1, long f2)
		{
			return (f1 << SHIFT_AMOUNT) / f2;
		}

        /// <summary>
		/// Division.
		/// </summary>
		/// <param name="f1">f1.</param>
		/// <param name="f2">f2.</param>
		public static long Div(this long f1, int f2)
        {
            return (f1) / f2;
        }

        /// <summary>
        /// Modulo.
        /// </summary>
        /// <param name="f1">f1.</param>
        /// <param name="f2">f2.</param>
        public static long Remainder(this long f1, long f2)
		{
			return f1 % f2;
		}

		public static long Mod(this long f1, long f2)
		{
			long f = f1 % f2;
			return f;
		}

        //public static float sqrt7(long x)
        //{

        //    unsigned int i = *(unsigned int*) &x;
        //    // adjust bias
        //    i += 127 << 23;
        //    // approximation of square root
        //    i >>= 1;
        //    return *(float*)&i;
        //}

        /// <summary>
        /// Square root.
        /// </summary>
        /// <param name="f1">f1.</param>
        static long n, n1;
        public static long Sqrt(long f1)
		{
			if (f1 == 0)
				return 0;
			n = (f1 >> 1) + 1;
			n1 = (n + (f1 / n)) >> 1;
			while (n1 < n)
			{
				n = n1;
				n1 = (n + (f1 / n)) >> 1;
			}
			return n << (SHIFT_AMOUNT / 2);
		}


		public static long Abs(this long f1)
		{
			return f1 < 0 ? -f1 : f1;
		}

		public static bool AbsMoreThan(this long f1, long f2)
		{
			if (f1 < 0)
			{
				return -f1 > f2;
			}
			else {
				return f1 > f2;
			}
		}

		public static bool AbsLessThan(this long f1, long f2)
		{
			if (f1 < 0)
			{
				return -f1 < f2;
			}
			else {
				return f1 < f2;
			}
		}

		#endregion

		#region Helpful

		/// <summary>
		/// Truncate the specified fixed-point number.
		/// </summary>
		/// <param name="f1">F1.</param>
		public static long Truncate(long f1)
		{
			return ((f1) >> SHIFT_AMOUNT) << SHIFT_AMOUNT;
		}

		/// <summary>
		/// Round the specified fixed point number.
		/// </summary>
		/// <param name="f1">F1.</param>
		public static long Round(long f1)
		{
			return ((f1 + FixedMath.Half - 1) >> SHIFT_AMOUNT) << SHIFT_AMOUNT;
		}



		/// <summary>
		/// Ceil the specified fixed point number.
		/// </summary>
		/// <param name="f1">F1.</param>
		public static long Ceil(long f1)
		{
			return ((f1 + One - 1) >> SHIFT_AMOUNT) << SHIFT_AMOUNT;
		}

		public static long Floor(long f1)
		{
			return ((f1) >> SHIFT_AMOUNT) << SHIFT_AMOUNT;
		}

		public static long Lerp(long from, long to, long t)
		{
			if (t >= One)
				return to;
			else if (t <= 0)
				return from;
			return (to * t + from * (One - t)) >> SHIFT_AMOUNT;
		}

		public static long Min(this long f1, long f2)
		{
			return f1 <= f2 ? f1 : f2;
		}
		public static long Min(long f1, long f2, long f3, long f4)
		{
			f1 = Min(f1, f2);
			f3 = Min(f3, f4);
			return Min(f1, f3);
		}

		public static long Max(this long f1, long f2)
		{
			return f1 >= f2 ? f1 : f2;
		}
		public static long Max(long f1, long f2, long f3, long f4)
		{
			f1 = Max(f1, f2);
			f3 = Max(f3, f4);
			return Max(f1, f3);
		}
		public static long Clamp(this long f1, long min, long max)
		{
			if (f1 < min) return min;
			if (f1 > max) return max;
			return f1;
		}
		public static double ToFormattedDouble(this long f1)
		{
			return Math.Round(FixedMath.ToDouble(f1), 2, MidpointRounding.AwayFromZero);
		}

		public static bool MoreThanEpsilon(this long f1)
		{
			return f1 > Epsilon || f1 < Epsilon;
		}

		public static long MoveTowards(long from, long to, long maxAmount)
		{
			if (from < to)
			{
				from += maxAmount;
				if (from > to)
					from = to;
			}
			else if (from > to)
			{
				from -= maxAmount;
				if (from < to)
					from = to;
			}
			return from;
		}

		public static long Normalized(this long f1, long range)
		{
			while (f1 < 0)
				f1 += range;
			if (f1 >= range)
				f1 = f1 % range;
			return f1;
		}
        public static int Normalized(this int f1, int range)
        {
            while (f1 < 0)
                f1 += range;
            if (f1 >= range)
                f1 = f1 % range;
            return f1;
        }
        #endregion

        #region Convert
        public static int Sign(this long f1)
		{
			if (f1 > 0)
				return 1;
			else if (f1 == 0)
				return 0;
			else
				return -1;
		}

		public static int ToInt(this long f1)
		{
			return (int)(f1 >> SHIFT_AMOUNT);
		}

	    public static long ToLong(this int f1)
	    {
	        return FixedMath.Create(f1);
	    }
	    public static long ToLong(this float f1)
	    {
	        return FixedMath.Create(f1);
	    }
        public static int RoundToInt(this long f1)
		{
			return (int)((f1 + Half - 1) >> SHIFT_AMOUNT);
		}

		public static int CeilToInt(this long f1)
		{
			return (int)((f1 + One - 1) >> SHIFT_AMOUNT);
		}
        public static int FloorToInt(this long f1)
        {
            return (int)((f1) >> SHIFT_AMOUNT);
        }
        /// <summary>
        /// Convert to double.
        /// </summary>
        /// <returns>The double.</returns>
        /// <param name="f1">f1.</param>
        public static double ToDouble(this long f1)
		{
			return (f1 / OneD);
		}
        public static long IntHundredToLong(this int v)
        {
            return v * FixedMath.One / 100;
        }
        public static int LongToIntHundred(this long v)
        {
            return (v * 100).ToInt();
        }
        /// <summary>
        /// Convert to float.
        /// </summary>
        /// <returns>The float.</returns>
        /// <param name="f1">f1.</param>
        public static float ToFloat(this long f1)
		{
			return (float)(f1 / OneD);
		}

		public static float ToPreciseFloat(this long f1)
		{
			return (float)ToDouble(f1);
		}

		/// <summary>
		/// Converts to string.
		/// </summary>
		/// <returns>The string.</returns>
		/// <param name="f1">f1.</param>

		public static string GetString(this long f1)
		{
			return (System.Math.Round((f1) / OneD, 4, System.MidpointRounding.AwayFromZero)).ToString();
		}



		#endregion

		public static class Trig
		{
           
		   
            private static long[] SinDic = new long[]
            {
                0,1143,2287,3429,4571,5711,6850,7986,9120,10252,11380,12504,13625,14742,15854,16961,18064,19160,20251,21336,22414,23486,24550,25606,26655,27696,28729,29752,30767,31772,32768,33753,34728,35693,36647,37589,38521,39440,40347,41243,42125,42995,43852,44695,45525,46340,47142,47929,48702,49460,50203,50931,51643,52339,53019,53683,54331,54963,55577,56175,56755,57319,57864,58393,58903,59395,59870,60326,60763,61183,61583,61965,62328,62672,62997,63302,63589,63856,64103,64331,64540,64729,64898,65047,65176,65286,65376,65446,65496,65526,65536,65526,65496,65446,65376,65286,65176,65047,64898,64729,64540,64331,64103,63856,63589,63302,62997,62672,62328,61965,61583,61183,60763,60326,59870,59395,58903,58393,57864,57319,56755,56175,55577,54963,54331,53683,53019,52339,51643,50931,50203,49460,48702,47929,47142,46340,45525,44695,43852,42995,42125,41243,40347,39440,38521,37589,36647,35693,34728,33753,32768,31772,30767,29752,28729,27696,26655,25606,24550,23486,22414,21336,20251,19160,18064,16961,15854,14742,13625,12504,11380,10252,9120,7986,6850,5711,4571,3429,2287,1143,0,-1143,-2287,-3429,-4571,-5711,-6850,-7986,-9120,-10252,-11380,-12504,-13625,-14742,-15854,-16961,-18064,-19160,-20251,-21336,-22414,-23485,-24550,-25606,-26655,-27696,-28729,-29752,-30767,-31772,-32767,-33753,-34728,-35693,-36647,-37589,-38521,-39440,-40347,-41243,-42125,-42995,-43852,-44695,-45525,-46340,-47142,-47929,-48702,-49460,-50203,-50931,-51643,-52339,-53019,-53683,-54331,-54963,-55577,-56175,-56755,-57319,-57864,-58393,-58903,-59395,-59870,-60326,-60763,-61183,-61583,-61965,-62328,-62672,-62997,-63302,-63589,-63856,-64103,-64331,-64540,-64729,-64898,-65047,-65176,-65286,-65376,-65446,-65496,-65526,-65536,-65526,-65496,-65446,-65376,-65286,-65176,-65047,-64898,-64729,-64540,-64331,-64103,-63856,-63589,-63302,-62997,-62672,-62328,-61965,-61583,-61183,-60763,-60326,-59870,-59395,-58903,-58393,-57864,-57319,-56755,-56175,-55577,-54963,-54331,-53683,-53019,-52339,-51643,-50931,-50203,-49460,-48702,-47929,-47142,-46340,-45525,-44695,-43852,-42995,-42125,-41243,-40347,-39440,-38521,-37589,-36647,-35693,-34728,-33753,-32768,-31772,-30767,-29752,-28729,-27696,-26655,-25606,-24550,-23486,-22414,-21336,-20251,-19160,-18064,-16961,-15854,-14742,-13625,-12504,-11380,-10252,-9120,-7986,-6850,-5711,-4571,-3429,-2287,-1143

            };

            private static long[] CosDic = new long[] 
            {
                65536,65526,65496,65446,65376,65286,65176,65047,64898,64729,64540,64331,64103,63856,63589,63302,62997,62672,62328,61965,61583,61183,60763,60326,59870,59395,58903,58393,57864,57319,56755,56175,55577,54963,54331,53683,53019,52339,51643,50931,50203,49460,48702,47929,47142,46340,45525,44695,43852,42995,42125,41243,40347,39440,38521,37589,36647,35693,34728,33753,32767,31772,30767,29752,28729,27696,26655,25606,24550,23486,22414,21336,20251,19160,18064,16961,15854,14742,13625,12504,11380,10252,9120,7986,6850,5711,4571,3429,2287,1143,0,-1143,-2287,-3429,-4571,-5711,-6850,-7986,-9120,-10252,-11380,-12504,-13625,-14742,-15854,-16961,-18064,-19160,-20251,-21336,-22414,-23486,-24550,-25606,-26655,-27696,-28729,-29752,-30767,-31772,-32768,-33753,-34728,-35693,-36647,-37589,-38521,-39440,-40347,-41243,-42125,-42995,-43852,-44695,-45525,-46340,-47142,-47930,-48702,-49460,-50203,-50931,-51643,-52339,-53019,-53683,-54331,-54963,-55577,-56175,-56755,-57319,-57864,-58393,-58903,-59395,-59870,-60326,-60763,-61183,-61583,-61965,-62328,-62672,-62997,-63302,-63589,-63856,-64103,-64331,-64540,-64729,-64898,-65047,-65176,-65286,-65376,-65446,-65496,-65526,-65536,-65526,-65496,-65446,-65376,-65286,-65176,-65047,-64898,-64729,-64540,-64331,-64103,-63856,-63589,-63302,-62997,-62672,-62328,-61965,-61583,-61183,-60763,-60326,-59870,-59395,-58903,-58393,-57864,-57319,-56755,-56175,-55577,-54963,-54331,-53683,-53019,-52339,-51643,-50931,-50203,-49460,-48702,-47930,-47142,-46340,-45525,-44695,-43852,-42995,-42125,-41243,-40347,-39440,-38521,-37589,-36647,-35693,-34728,-33753,-32767,-31772,-30767,-29752,-28729,-27696,-26655,-25606,-24550,-23485,-22414,-21336,-20251,-19160,-18064,-16961,-15854,-14742,-13625,-12504,-11380,-10252,-9120,-7986,-6850,-5711,-4571,-3429,-2287,-1143,0,1143,2287,3429,4571,5711,6850,7986,9120,10252,11380,12504,13625,14742,15854,16961,18064,19160,20251,21336,22414,23485,24550,25606,26655,27696,28729,29752,30767,31772,32767,33753,34728,35693,36647,37589,38521,39440,40347,41243,42125,42995,43852,44695,45525,46340,47142,47930,48702,49460,50203,50931,51643,52339,53019,53683,54331,54963,55577,56175,56755,57319,57864,58393,58903,59395,59870,60326,60763,61183,61583,61965,62328,62672,62997,63302,63589,63856,64103,64331,64540,64729,64898,65047,65176,65286,65376,65446,65496,65526
            };
            public static long Sin(long theta)
			{
                //Taylor series cuz easy
                //TODO: Profiling
                //Note: Max 4 multiplications before overflow
			    var degree = theta / FixedMath.Pi * FixedMath.One * 180;
			    return SinDegree(degree);
			    //New strategy... wrap theta around pi/4 for best accuracy. Mirror and flip based on quadrant.
			    /*
                bool mirror = false;
                bool flip = false;
                int quadrant = ((theta).Div(FixedMath.Pi / 2)).ToInt();
                switch (quadrant)
                {
                    case 0:
                        break;
                    case 1:
                        mirror = true;
                        break;
                    case 2:
                        flip = true;
                        break;
                    case 3:
                        mirror = true;
                        flip = true;
                        break;
                }
                theta = theta.Normalized(FixedMath.Pi / 2);
                if (mirror)
                {
                    theta = FixedMath.Pi / 2 - theta;
                }

                long thetaSquared = theta.Mul(theta);

                long result = theta;
                const int shift = FixedMath.SHIFT_AMOUNT;
                //2 shifts for 2 multiplications but there's a division so only 1 shift
                long n = (theta * theta * theta) >> (shift * 1);
                const long Factorial3 = 3 * 2 * FixedMath.One;
                result -= n / Factorial3;

                n *= thetaSquared;
                n >>= shift;
                const long Factorial5 = Factorial3 * 4 * 5;
                result += (n / Factorial5);

                n *= thetaSquared;
                n >>= shift;
                const long Factorial7 = Factorial5 * 6 * 7;
                result -= n / Factorial7;

#if false && HIGH_ACCURACY
                //Required or there'll be .07 inaccuracy
                n *= thetaSquared;
                n >>= shift;
                const long Factorial9 = Factorial7 * 8 * 9;
                result += n / Factorial9;
#endif


                if (flip)
                {
                    result *= -1;
                }
                return result;
                */
			}
			public static long Cos(long theta)
			{
			    var degree = theta / FixedMath.Pi * FixedMath.One * 180;
			    return CosDegree(degree);
			    /*
                long sin = Sin(theta);
                int factor = 1;
                if (theta > FixedMath.Pi/2 && theta < FixedMath.Pi*3/2)
                {
                    factor = -1;
                }
                return FixedMath.Sqrt(FixedMath.One - (sin.Mul(sin))) * (factor);
                */
			}
            public static long CosDegree(int degree)
            {
                var d = degree.Normalized(360);
                return CosDic[d];
            }
            public static long SinDegree(int degree)
            {
                var d = degree.Normalized(360);
                return SinDic[d];
            }
            public static long CosDegree(long degree)
		    {
                var d = degree.RoundToInt().Normalized(360);
                return CosDic[d];
            }
		    public static long SinDegree(long degree)
		    {
                var d = degree.RoundToInt().Normalized(360);
                return SinDic[d];
            }
            public static long SinToCos(long sin)
			{
				return Sqrt(FixedMath.One - (sin.Mul(sin)));
			}
			public static long Tan(long theta)
			{
				return Sin(theta).Div(Cos(theta));
			}
		}
	}
}