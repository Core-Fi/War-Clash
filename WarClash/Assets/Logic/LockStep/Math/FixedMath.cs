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

		/// <summary>
		/// Convert to double.
		/// </summary>
		/// <returns>The double.</returns>
		/// <param name="f1">f1.</param>
		public static double ToDouble(this long f1)
		{
			return (f1 / OneD);
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
            private static Dictionary<long, long> SinDic = new Dictionary<long, long>
            {
                {0,0},{65536,1143},{131072,2287},{196608,3429},{262144,4571},{327680,5711},{393216,6850},{458752,7986},{524288,9120},{589824,10252},{655360,11380},{720896,12504},{786432,13625},{851968,14742},{917504,15854},{983040,16961},{1048576,18064},{1114112,19160},{1179648,20251},{1245184,21336},{1310720,22414},{1376256,23486},{1441792,24550},{1507328,25606},{1572864,26655},{1638400,27696},{1703936,28729},{1769472,29752},{1835008,30767},{1900544,31772},{1966080,32768},{2031616,33753},{2097152,34728},{2162688,35693},{2228224,36647},{2293760,37589},{2359296,38521},{2424832,39440},{2490368,40347},{2555904,41243},{2621440,42125},{2686976,42995},{2752512,43852},{2818048,44695},{2883584,45525},{2949120,46340},{3014656,47142},{3080192,47929},{3145728,48702},{3211264,49460},{3276800,50203},{3342336,50931},{3407872,51643},{3473408,52339},{3538944,53019},{3604480,53683},{3670016,54331},{3735552,54963},{3801088,55577},{3866624,56175},{3932160,56755},{3997696,57319},{4063232,57864},{4128768,58393},{4194304,58903},{4259840,59395},{4325376,59870},{4390912,60326},{4456448,60763},{4521984,61183},{4587520,61583},{4653056,61965},{4718592,62328},{4784128,62672},{4849664,62997},{4915200,63302},{4980736,63589},{5046272,63856},{5111808,64103},{5177344,64331},{5242880,64540},{5308416,64729},{5373952,64898},{5439488,65047},{5505024,65176},{5570560,65286},{5636096,65376},{5701632,65446},{5767168,65496},{5832704,65526},{5898240,65536},{5963776,65526},{6029312,65496},{6094848,65446},{6160384,65376},{6225920,65286},{6291456,65176},{6356992,65047},{6422528,64898},{6488064,64729},{6553600,64540},{6619136,64331},{6684672,64103},{6750208,63856},{6815744,63589},{6881280,63302},{6946816,62997},{7012352,62672},{7077888,62328},{7143424,61965},{7208960,61583},{7274496,61183},{7340032,60763},{7405568,60326},{7471104,59870},{7536640,59395},{7602176,58903},{7667712,58393},{7733248,57864},{7798784,57319},{7864320,56755},{7929856,56175},{7995392,55577},{8060928,54963},{8126464,54331},{8192000,53683},{8257536,53019},{8323072,52339},{8388608,51643},{8454144,50931},{8519680,50203},{8585216,49460},{8650752,48702},{8716288,47929},{8781824,47142},{8847360,46340},{8912896,45525},{8978432,44695},{9043968,43852},{9109504,42995},{9175040,42125},{9240576,41243},{9306112,40347},{9371648,39440},{9437184,38521},{9502720,37589},{9568256,36647},{9633792,35693},{9699328,34728},{9764864,33753},{9830400,32768},{9895936,31772},{9961472,30767},{10027008,29752},{10092544,28729},{10158080,27696},{10223616,26655},{10289152,25606},{10354688,24550},{10420224,23486},{10485760,22414},{10551296,21336},{10616832,20251},{10682368,19160},{10747904,18064},{10813440,16961},{10878976,15854},{10944512,14742},{11010048,13625},{11075584,12504},{11141120,11380},{11206656,10252},{11272192,9120},{11337728,7986},{11403264,6850},{11468800,5711},{11534336,4571},{11599872,3429},{11665408,2287},{11730944,1143},{11796480,0},{11862016,-1143},{11927552,-2287},{11993088,-3429},{12058624,-4571},{12124160,-5711},{12189696,-6850},{12255232,-7986},{12320768,-9120},{12386304,-10252},{12451840,-11380},{12517376,-12504},{12582912,-13625},{12648448,-14742},{12713984,-15854},{12779520,-16961},{12845056,-18064},{12910592,-19160},{12976128,-20251},{13041664,-21336},{13107200,-22414},{13172736,-23485},{13238272,-24550},{13303808,-25606},{13369344,-26655},{13434880,-27696},{13500416,-28729},{13565952,-29752},{13631488,-30767},{13697024,-31772},{13762560,-32767},{13828096,-33753},{13893632,-34728},{13959168,-35693},{14024704,-36647},{14090240,-37589},{14155776,-38521},{14221312,-39440},{14286848,-40347},{14352384,-41243},{14417920,-42125},{14483456,-42995},{14548992,-43852},{14614528,-44695},{14680064,-45525},{14745600,-46340},{14811136,-47142},{14876672,-47929},{14942208,-48702},{15007744,-49460},{15073280,-50203},{15138816,-50931},{15204352,-51643},{15269888,-52339},{15335424,-53019},{15400960,-53683},{15466496,-54331},{15532032,-54963},{15597568,-55577},{15663104,-56175},{15728640,-56755},{15794176,-57319},{15859712,-57864},{15925248,-58393},{15990784,-58903},{16056320,-59395},{16121856,-59870},{16187392,-60326},{16252928,-60763},{16318464,-61183},{16384000,-61583},{16449536,-61965},{16515072,-62328},{16580608,-62672},{16646144,-62997},{16711680,-63302},{16777216,-63589},{16842752,-63856},{16908288,-64103},{16973824,-64331},{17039360,-64540},{17104896,-64729},{17170432,-64898},{17235968,-65047},{17301504,-65176},{17367040,-65286},{17432576,-65376},{17498112,-65446},{17563648,-65496},{17629184,-65526},{17694720,-65536},{17760256,-65526},{17825792,-65496},{17891328,-65446},{17956864,-65376},{18022400,-65286},{18087936,-65176},{18153472,-65047},{18219008,-64898},{18284544,-64729},{18350080,-64540},{18415616,-64331},{18481152,-64103},{18546688,-63856},{18612224,-63589},{18677760,-63302},{18743296,-62997},{18808832,-62672},{18874368,-62328},{18939904,-61965},{19005440,-61583},{19070976,-61183},{19136512,-60763},{19202048,-60326},{19267584,-59870},{19333120,-59395},{19398656,-58903},{19464192,-58393},{19529728,-57864},{19595264,-57319},{19660800,-56755},{19726336,-56175},{19791872,-55577},{19857408,-54963},{19922944,-54331},{19988480,-53683},{20054016,-53019},{20119552,-52339},{20185088,-51643},{20250624,-50931},{20316160,-50203},{20381696,-49460},{20447232,-48702},{20512768,-47929},{20578304,-47142},{20643840,-46340},{20709376,-45525},{20774912,-44695},{20840448,-43852},{20905984,-42995},{20971520,-42125},{21037056,-41243},{21102592,-40347},{21168128,-39440},{21233664,-38521},{21299200,-37589},{21364736,-36647},{21430272,-35693},{21495808,-34728},{21561344,-33753},{21626880,-32768},{21692416,-31772},{21757952,-30767},{21823488,-29752},{21889024,-28729},{21954560,-27696},{22020096,-26655},{22085632,-25606},{22151168,-24550},{22216704,-23486},{22282240,-22414},{22347776,-21336},{22413312,-20251},{22478848,-19160},{22544384,-18064},{22609920,-16961},{22675456,-15854},{22740992,-14742},{22806528,-13625},{22872064,-12504},{22937600,-11380},{23003136,-10252},{23068672,-9120},{23134208,-7986},{23199744,-6850},{23265280,-5711},{23330816,-4571},{23396352,-3429},{23461888,-2287},{23527424,-1143},{23592960,0},
            };

		    private static Dictionary<long, long> CosDic = new Dictionary<long, long>
		    {
                {0,65536},{65536,65526},{131072,65496},{196608,65446},{262144,65376},{327680,65286},{393216,65176},{458752,65047},{524288,64898},{589824,64729},{655360,64540},{720896,64331},{786432,64103},{851968,63856},{917504,63589},{983040,63302},{1048576,62997},{1114112,62672},{1179648,62328},{1245184,61965},{1310720,61583},{1376256,61183},{1441792,60763},{1507328,60326},{1572864,59870},{1638400,59395},{1703936,58903},{1769472,58393},{1835008,57864},{1900544,57319},{1966080,56755},{2031616,56175},{2097152,55577},{2162688,54963},{2228224,54331},{2293760,53683},{2359296,53019},{2424832,52339},{2490368,51643},{2555904,50931},{2621440,50203},{2686976,49460},{2752512,48702},{2818048,47929},{2883584,47142},{2949120,46340},{3014656,45525},{3080192,44695},{3145728,43852},{3211264,42995},{3276800,42125},{3342336,41243},{3407872,40347},{3473408,39440},{3538944,38521},{3604480,37589},{3670016,36647},{3735552,35693},{3801088,34728},{3866624,33753},{3932160,32767},{3997696,31772},{4063232,30767},{4128768,29752},{4194304,28729},{4259840,27696},{4325376,26655},{4390912,25606},{4456448,24550},{4521984,23486},{4587520,22414},{4653056,21336},{4718592,20251},{4784128,19160},{4849664,18064},{4915200,16961},{4980736,15854},{5046272,14742},{5111808,13625},{5177344,12504},{5242880,11380},{5308416,10252},{5373952,9120},{5439488,7986},{5505024,6850},{5570560,5711},{5636096,4571},{5701632,3429},{5767168,2287},{5832704,1143},{5898240,0},{5963776,-1143},{6029312,-2287},{6094848,-3429},{6160384,-4571},{6225920,-5711},{6291456,-6850},{6356992,-7986},{6422528,-9120},{6488064,-10252},{6553600,-11380},{6619136,-12504},{6684672,-13625},{6750208,-14742},{6815744,-15854},{6881280,-16961},{6946816,-18064},{7012352,-19160},{7077888,-20251},{7143424,-21336},{7208960,-22414},{7274496,-23486},{7340032,-24550},{7405568,-25606},{7471104,-26655},{7536640,-27696},{7602176,-28729},{7667712,-29752},{7733248,-30767},{7798784,-31772},{7864320,-32768},{7929856,-33753},{7995392,-34728},{8060928,-35693},{8126464,-36647},{8192000,-37589},{8257536,-38521},{8323072,-39440},{8388608,-40347},{8454144,-41243},{8519680,-42125},{8585216,-42995},{8650752,-43852},{8716288,-44695},{8781824,-45525},{8847360,-46340},{8912896,-47142},{8978432,-47930},{9043968,-48702},{9109504,-49460},{9175040,-50203},{9240576,-50931},{9306112,-51643},{9371648,-52339},{9437184,-53019},{9502720,-53683},{9568256,-54331},{9633792,-54963},{9699328,-55577},{9764864,-56175},{9830400,-56755},{9895936,-57319},{9961472,-57864},{10027008,-58393},{10092544,-58903},{10158080,-59395},{10223616,-59870},{10289152,-60326},{10354688,-60763},{10420224,-61183},{10485760,-61583},{10551296,-61965},{10616832,-62328},{10682368,-62672},{10747904,-62997},{10813440,-63302},{10878976,-63589},{10944512,-63856},{11010048,-64103},{11075584,-64331},{11141120,-64540},{11206656,-64729},{11272192,-64898},{11337728,-65047},{11403264,-65176},{11468800,-65286},{11534336,-65376},{11599872,-65446},{11665408,-65496},{11730944,-65526},{11796480,-65536},{11862016,-65526},{11927552,-65496},{11993088,-65446},{12058624,-65376},{12124160,-65286},{12189696,-65176},{12255232,-65047},{12320768,-64898},{12386304,-64729},{12451840,-64540},{12517376,-64331},{12582912,-64103},{12648448,-63856},{12713984,-63589},{12779520,-63302},{12845056,-62997},{12910592,-62672},{12976128,-62328},{13041664,-61965},{13107200,-61583},{13172736,-61183},{13238272,-60763},{13303808,-60326},{13369344,-59870},{13434880,-59395},{13500416,-58903},{13565952,-58393},{13631488,-57864},{13697024,-57319},{13762560,-56755},{13828096,-56175},{13893632,-55577},{13959168,-54963},{14024704,-54331},{14090240,-53683},{14155776,-53019},{14221312,-52339},{14286848,-51643},{14352384,-50931},{14417920,-50203},{14483456,-49460},{14548992,-48702},{14614528,-47930},{14680064,-47142},{14745600,-46340},{14811136,-45525},{14876672,-44695},{14942208,-43852},{15007744,-42995},{15073280,-42125},{15138816,-41243},{15204352,-40347},{15269888,-39440},{15335424,-38521},{15400960,-37589},{15466496,-36647},{15532032,-35693},{15597568,-34728},{15663104,-33753},{15728640,-32767},{15794176,-31772},{15859712,-30767},{15925248,-29752},{15990784,-28729},{16056320,-27696},{16121856,-26655},{16187392,-25606},{16252928,-24550},{16318464,-23485},{16384000,-22414},{16449536,-21336},{16515072,-20251},{16580608,-19160},{16646144,-18064},{16711680,-16961},{16777216,-15854},{16842752,-14742},{16908288,-13625},{16973824,-12504},{17039360,-11380},{17104896,-10252},{17170432,-9120},{17235968,-7986},{17301504,-6850},{17367040,-5711},{17432576,-4571},{17498112,-3429},{17563648,-2287},{17629184,-1143},{17694720,0},{17760256,1143},{17825792,2287},{17891328,3429},{17956864,4571},{18022400,5711},{18087936,6850},{18153472,7986},{18219008,9120},{18284544,10252},{18350080,11380},{18415616,12504},{18481152,13625},{18546688,14742},{18612224,15854},{18677760,16961},{18743296,18064},{18808832,19160},{18874368,20251},{18939904,21336},{19005440,22414},{19070976,23485},{19136512,24550},{19202048,25606},{19267584,26655},{19333120,27696},{19398656,28729},{19464192,29752},{19529728,30767},{19595264,31772},{19660800,32767},{19726336,33753},{19791872,34728},{19857408,35693},{19922944,36647},{19988480,37589},{20054016,38521},{20119552,39440},{20185088,40347},{20250624,41243},{20316160,42125},{20381696,42995},{20447232,43852},{20512768,44695},{20578304,45525},{20643840,46340},{20709376,47142},{20774912,47930},{20840448,48702},{20905984,49460},{20971520,50203},{21037056,50931},{21102592,51643},{21168128,52339},{21233664,53019},{21299200,53683},{21364736,54331},{21430272,54963},{21495808,55577},{21561344,56175},{21626880,56755},{21692416,57319},{21757952,57864},{21823488,58393},{21889024,58903},{21954560,59395},{22020096,59870},{22085632,60326},{22151168,60763},{22216704,61183},{22282240,61583},{22347776,61965},{22413312,62328},{22478848,62672},{22544384,62997},{22609920,63302},{22675456,63589},{22740992,63856},{22806528,64103},{22872064,64331},{22937600,64540},{23003136,64729},{23068672,64898},{23134208,65047},{23199744,65176},{23265280,65286},{23330816,65376},{23396352,65446},{23461888,65496},{23527424,65526},{23592960,65536},
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

		    public static long CosDegree(long degree)
		    {
		        degree = (degree.RoundToInt()) << SHIFT_AMOUNT;
		        degree = degree.Normalized(FixedMath.One * 360);
		        return CosDic[degree];
            }
		    public static long SinDegree(long degree)
		    {
		        degree = (degree.RoundToInt()) << SHIFT_AMOUNT;
                degree = degree.Normalized(FixedMath.One * 360);
		        return SinDic[degree];
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