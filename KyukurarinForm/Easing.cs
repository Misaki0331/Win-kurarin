using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KyukurarinForm
{
    public class Arial
    {
        public enum Position
        {
            TopLeft,
            TopCenter,
            TopRight,
            Left,
            Center,
            Right,
            BottomLeft,
            BottomCenter,
            BottomRight

        }
        public static int GetXPos(Position type,int size)
        {
            switch (type)
            {
                case Position.TopLeft:
                case Position.Left:
                case Position.BottomLeft:
                    return 0;
                case Position.TopCenter:
                case Position.Center:
                case Position.BottomCenter:
                    return -size / 2;
                default: return -size;
            }
        }
        public static int GetYPos(Position type, int size)
        {
            switch (type)
            {
                case Position.TopLeft:
                case Position.TopCenter:
                case Position.TopRight:
                    return 0;
                case Position.Left:
                case Position.Center:
                case Position.Right:
                    return -size / 2;
                default: return -size;
            }
        }
    }
    public class Easing
    {
        public static double GetEasing(int type,double t)
        {
            switch (type)
            {
                case 0: return Linear(t, 1, 0, 1);
                case 1: return CubicOut(t, 1, 0, 1);
                case 2: return CubicIn(t, 1, 0, 1);
                case 3: return QuadIn(t, 1, 0, 1);
                case 4: return QuadOut(t, 1, 0, 1);
                case 5: return QuadInOut(t, 1, 0, 1);
                case 6: return CubicIn(t, 1, 0, 1);
                case 7: return CubicOut(t, 1, 0, 1);
                case 8: return CubicInOut(t, 1, 0, 1);
                case 9: return QuartIn(t, 1, 0, 1);
                case 10: return QuartOut(t, 1, 0, 1);
                case 11: return QuartInOut(t, 1, 0, 1);
                case 12: return QuintIn(t, 1, 0, 1);
                case 13: return QuintOut(t, 1, 0, 1);
                case 14: return QuintInOut(t, 1, 0, 1);
                case 15: return SineIn(t, 1, 0, 1);
                case 16: return SineOut(t, 1, 0, 1);
                case 17: return SineInOut(t, 1, 0, 1);
                case 18: return ExpIn(t, 1, 0, 1);
                case 19: return ExpOut(t, 1, 0, 1);
                case 20: return ExpInOut(t, 1, 0, 1);
                case 21: return CircIn(t, 1, 0, 1);
                case 22: return CircOut(t, 1, 0, 1);
                case 23: return CircInOut(t, 1, 0, 1);
                case 24: return ElasticIn(t, 1, 0, 1);
                case 25: return ElasticOut(t, 1, 0, 1);
                case 26: return ElasticInOut(t, 1, 0, 1);
                case 27: return BackIn(t, 1, 0, 1,1);
                case 28: return BackOut(t, 1, 0, 1,1);
                case 29: return BackInOut(t, 1, 0, 1,1);
                case 30: return BounceIn(t, 1, 0, 1);
                case 31: return BounceOut(t, 1, 0, 1);
                case 32: return BounceInOut(t, 1, 0, 1);
                default: return Linear(t, 1, 0, 1);
            }
        }




        public static double QuadIn(double t, double totaltime, double min, double max)
        {
            max -= min;
            t /= totaltime;
            return max * t * t + min;
        }

        public static double QuadOut(double t, double totaltime, double min, double max)
        {
            max -= min;
            t /= totaltime;
            return -max * t * (t - 2) + min;
        }

        public static double QuadInOut(double t, double totaltime, double min, double max)
        {
            max -= min;
            t /= totaltime / 2;
            if (t < 1) return max / 2 * t * t + min;

            t = t - 1;
            return -max / 2 * (t * (t - 2) - 1) + min;
        }

        public static double CubicIn(double t, double totaltime, double min, double max)
        {
            max -= min;
            t /= totaltime;
            return max * t * t * t + min;
        }

        public static double CubicOut(double t, double totaltime, double min, double max)
        {
            max -= min;
            t = t / totaltime - 1;
            return max * (t * t * t + 1) + min;
        }

        public static double CubicInOut(double t, double totaltime, double min, double max)
        {
            max -= min;
            t /= totaltime / 2;
            if (t < 1) return max / 2 * t * t * t + min;

            t = t - 2;
            return max / 2 * (t * t * t + 2) + min;
        }

        public static double QuartIn(double t, double totaltime, double min, double max)
        {
            max -= min;
            t /= totaltime;
            return max * t * t * t * t + min;
        }

        public static double QuartOut(double t, double totaltime, double min, double max)
        {
            max -= min;
            t = t / totaltime - 1;
            return -max * (t * t * t * t - 1) + min;
        }

        public static double QuartInOut(double t, double totaltime, double min, double max)
        {
            max -= min;
            t /= totaltime / 2;
            if (t < 1) return max / 2 * t * t * t * t + min;

            t = t - 2;
            return -max / 2 * (t * t * t * t - 2) + min;
        }

        public static double QuintIn(double t, double totaltime, double min, double max)
        {
            max -= min;
            t /= totaltime;
            return max * t * t * t * t * t + min;
        }

        public static double QuintOut(double t, double totaltime, double min, double max)
        {
            max -= min;
            t = t / totaltime - 1;
            return max * (t * t * t * t * t + 1) + min;
        }

        public static double QuintInOut(double t, double totaltime, double min, double max)
        {
            max -= min;
            t /= totaltime / 2;
            if (t < 1) return max / 2 * t * t * t * t * t + min;

            t = t - 2;
            return max / 2 * (t * t * t * t * t + 2) + min;
        }

        public static double SineIn(double t, double totaltime, double min, double max)
        {
            max -= min;
            return -max * Math.Cos(t * (Math.PI * 90 / 180) / totaltime) + max + min;
        }

        public static double SineOut(double t, double totaltime, double min, double max)
        {
            max -= min;
            return max * Math.Sin(t * (Math.PI * 90 / 180) / totaltime) + min;
        }

        public static double SineInOut(double t, double totaltime, double min, double max)
        {
            max -= min;
            return -max / 2 * (Math.Cos(t * Math.PI / totaltime) - 1) + min;
        }

        public static double ExpIn(double t, double totaltime, double min, double max)
        {
            max -= min;
            return t == 0.0 ? min : max * Math.Pow(2, 10 * (t / totaltime - 1)) + min;
        }

        public static double ExpOut(double t, double totaltime, double min, double max)
        {
            max -= min;
            return t == totaltime ? max + min : max * (-Math.Pow(2, -10 * t / totaltime) + 1) + min;
        }

        public static double ExpInOut(double t, double totaltime, double min, double max)
        {
            if (t == 0.0f) return min;
            if (t == totaltime) return max;
            max -= min;
            t /= totaltime / 2;

            if (t < 1) return max / 2 * Math.Pow(2, 10 * (t - 1)) + min;

            t = t - 1;
            return max / 2 * (-Math.Pow(2, -10 * t) + 2) + min;

        }

        public static double CircIn(double t, double totaltime, double min, double max)
        {
            max -= min;
            t /= totaltime;
            return -max * (Math.Sqrt(1 - t * t) - 1) + min;
        }

        public static double CircOut(double t, double totaltime, double min, double max)
        {
            max -= min;
            t = t / totaltime - 1;
            return max * Math.Sqrt(1 - t * t) + min;
        }

        public static double CircInOut(double t, double totaltime, double min, double max)
        {
            max -= min;
            t /= totaltime / 2;
            if (t < 1) return -max / 2 * (Math.Sqrt(1 - t * t) - 1) + min;

            t = t - 2;
            return max / 2 * (Math.Sqrt(1 - t * t) + 1) + min;
        }

        public static double ElasticIn(double t, double totaltime, double min, double max)
        {
            max -= min;
            t /= totaltime;

            double s = 1.70158f;
            double p = totaltime * 0.3f;
            double a = max;

            if (t == 0) return min;
            if (t == 1) return min + max;

            if (a < Math.Abs(max))
            {
                a = max;
                s = p / 4;
            }
            else
            {
                s = p / (2 * Math.PI) * Math.Asin(max / a);
            }

            t = t - 1;
            return -(a * Math.Pow(2, 10 * t) * Math.Sin((t * totaltime - s) * (2 * Math.PI) / p)) + min;
        }

        public static double ElasticOut(double t, double totaltime, double min, double max)
        {
            max -= min;
            t /= totaltime;

            double s = 1.70158f;
            double p = totaltime * 0.3f; ;
            double a = max;

            if (t == 0) return min;
            if (t == 1) return min + max;

            if (a < Math.Abs(max))
            {
                a = max;
                s = p / 4;
            }
            else
            {
                s = p / (2 * Math.PI) * Math.Asin(max / a);
            }

            return a * Math.Pow(2, -10 * t) * Math.Sin((t * totaltime - s) * (2 * Math.PI) / p) + max + min;
        }

        public static double ElasticInOut(double t, double totaltime, double min, double max)
        {
            max -= min;
            t /= totaltime / 2;

            double s = 1.70158f;
            double p = totaltime * (0.3f * 1.5f);
            double a = max;

            if (t == 0) return min;
            if (t == 2) return min + max;

            if (a < Math.Abs(max))
            {
                a = max;
                s = p / 4;
            }
            else
            {
                s = p / (2 * Math.PI) * Math.Asin(max / a);
            }

            if (t < 1)
            {
                return -0.5f * (a * Math.Pow(2, 10 * (t -= 1)) * Math.Sin((t * totaltime - s) * (2 * Math.PI) / p)) + min;
            }

            t = t - 1;
            return a * Math.Pow(2, -10 * t) * Math.Sin((t * totaltime - s) * (2 * Math.PI) / p) * 0.5f + max + min;
        }

        public static double BackIn(double t, double totaltime, double min, double max, double s)
        {
            max -= min;
            t /= totaltime;
            return max * t * t * ((s + 1) * t - s) + min;
        }

        public static double BackOut(double t, double totaltime, double min, double max, double s)
        {
            max -= min;
            t = t / totaltime - 1;
            return max * (t * t * ((s + 1) * t + s) + 1) + min;
        }

        public static double BackInOut(double t, double totaltime, double min, double max, double s)
        {
            max -= min;
            s *= 1.525f;
            t /= totaltime / 2;
            if (t < 1) return max / 2 * (t * t * ((s + 1) * t - s)) + min;

            t = t - 2;
            return max / 2 * (t * t * ((s + 1) * t + s) + 2) + min;
        }

        public static double BounceIn(double t, double totaltime, double min, double max)
        {
            max -= min;
            return max - BounceOut(totaltime - t, totaltime, 0, max) + min;
        }

        public static double BounceOut(double t, double totaltime, double min, double max)
        {
            max -= min;
            t /= totaltime;

            if (t < 1.0f / 2.75f)
            {
                return max * (7.5625f * t * t) + min;
            }
            else if (t < 2.0f / 2.75f)
            {
                t -= 1.5f / 2.75f;
                return max * (7.5625f * t * t + 0.75f) + min;
            }
            else if (t < 2.5f / 2.75f)
            {
                t -= 2.25f / 2.75f;
                return max * (7.5625f * t * t + 0.9375f) + min;
            }
            else
            {
                t -= 2.625f / 2.75f;
                return max * (7.5625f * t * t + 0.984375f) + min;
            }
        }

        public static double BounceInOut(double t, double totaltime, double min, double max)
        {
            if (t < totaltime / 2)
            {
                return BounceIn(t * 2, totaltime, 0, max - min) * 0.5f + min;
            }
            else
            {
                return BounceOut(t * 2 - totaltime, totaltime, 0, max - min) * 0.5f + min + (max - min) * 0.5f;
            }
        }

        public static double Linear(double t, double totaltime, double min, double max)
        {
            return (max - min) * t / totaltime + min;
        }

    }
}
