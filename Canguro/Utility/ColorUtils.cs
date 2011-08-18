using System;
using System.Collections.Generic;

namespace Canguro.Utility
{
    internal class ColorUtils
    {
        public delegate int GetColorFromRatioDelegate(float ratio);

        public static int GetColorFromDesignRatio(float ratio)
        {
            // h should be in the range of 0 - 180
            float h = 0f;
            h = (ratio < 0) ? 180f : (ratio >= 1f) ? 0 : 180f - ratio * 120f;

            // Conversion from HSV to RGB taken from http://en.wikipedia.org/wiki/HSV_color_space
            int i;
            int q, t, v = 255, a = 255 << 24;

            h /= 60f;
            i = (int)Math.Floor(h);
            h -= i;

            t = Convert.ToInt32(h * 255f);
            q = 255 - t;

            switch (i)
            {
                case 0:
                    return a | (v << 16) | (t << 8);
                case 1:
                    return a | (q << 16) | (v << 8);
                case 2:
                    return a | (v << 8) | t;
                case 3:
                    return a | (q << 8) | v;
                case 4:
                    return a | (t << 16) | v;
                default:
                    return a | (v << 16) | q;
            }
        }

        public static int GetColorFromStress(float largestStress, float stress)
        {
            // h should be in the range of 0 - 240
            float h = 120f;
            if (largestStress != 0)
                h = stress * 120f / largestStress + 120f;

            // Conversion from HSV to RGB taken from http://en.wikipedia.org/wiki/HSV_color_space
            int i;
            int q, t, v = 255, a = 255 << 24;

            h /= 60f;
            i = (int)Math.Floor(h);
            h -= i;

            t = Convert.ToInt32(h * 255f);
            q = 255 - t;

            switch (i)
            {
                case 0:
                    return a | (v << 16) | (t << 8);
                case 1:
                    return a | (q << 16) | (v << 8);
                case 2:
                    return a | (v << 8) | t;
                case 3:
                    return a | (q << 8) | v;
                case 4:
                    return a | (t << 16) | v;
                default:
                    return a | (v << 16) | q;
            }
        }

        public static int GetColorForId(int id)
        {
            int a = 255; id = Math.Abs(id);
            float h, s, b = 0.5f;
            int cycle = id / 6;
            int delta = 60 / (1 << cycle) + 60;
            h = Math.Abs((60 * id - delta) % 360);
            s = 1f - (Math.Abs(cycle * cycle) % 100) / 100f;

            return ColorFromAhsb(a, h, s, b);
        }

        public static int ColorFromAhsb(int a, float h, float s, float b)
        {

            if (0 > a || 255 < a)
            {
                throw new ArgumentOutOfRangeException("a", a, "InvalidAlpha");
            }
            if (0f > h || 360f < h)
            {
                throw new ArgumentOutOfRangeException("h", h, "InvalidHue");
            }
            if (0f > s || 1f < s)
            {
                throw new ArgumentOutOfRangeException("s", s, "InvalidSaturation");
            }
            if (0f > b || 1f < b)
            {
                throw new ArgumentOutOfRangeException("b", b, "InvalidBrightness");
            }

            if (0 == s)
            {
                int bb = (int)(b * 255);
                return (a << 24) | (bb << 16) | (bb << 8) | bb;
            }

            float fMax, fMid, fMin;
            int iSextant, iMax, iMid, iMin;

            if (0.5 < b)
            {
                fMax = b - (b * s) + s;
                fMin = b + (b * s) - s;
            }
            else
            {
                fMax = b + (b * s);
                fMin = b - (b * s);
            }

            iSextant = (int)Math.Floor(h / 60f);
            if (300f <= h)
            {
                h -= 360f;
            }
            h /= 60f;
            h -= 2f * (float)Math.Floor(((iSextant + 1f) % 6f) / 2f);
            if (0 == iSextant % 2)
            {
                fMid = h * (fMax - fMin) + fMin;
            }
            else
            {
                fMid = fMin - h * (fMax - fMin);
            }

            iMax = Convert.ToInt32(fMax * 255);
            iMid = Convert.ToInt32(fMid * 255);
            iMin = Convert.ToInt32(fMin * 255);

            switch (iSextant)
            {
                case 1:
                    return (a << 24) | (iMid << 16) | (iMax << 8) | iMin;
                case 2:
                    return (a << 24) | (iMin << 16) | (iMax << 8) | iMid;
                case 3:
                    return (a << 24) | (iMin << 16) | (iMid << 8) | iMax;
                case 4:
                    return (a << 24) | (iMid << 16) | (iMin << 8) | iMax;
                case 5:
                    return (a << 24) | (iMax << 16) | (iMin << 8) | iMid;
                default:
                    return (a << 24) | (iMax << 16) | (iMid << 8) | iMin;
            }
        }
    }
}
