using System;
using UnityEngine;
using UnityEngine.Rendering;

public class Easing
{
    const float c1 = 1.70158f;
    const float c2 = c1 * 1.525f;
    const float c3 = c1 + 1;
    const float c4 = (2 * Mathf.PI) / 3;
    const float c5 = (2 * Mathf.PI) / 4.5f;
    const float n1 = 7.5625f;
    const float d1 = 2.75f;
    public static float Linear(float t)
    {
        return t;
    }
    public static float Linear(float t, bool b)
    {
        if (b)
        {
            return Mathf.PingPong(Linear(t), 0.5f);
        }
        else
        {
            return t;
        }

    }
    public class Sine
    {
        public static float In(float t)
        {
            return 1 - Mathf.Cos((t * Mathf.PI) / 2);
        }
        public static float Out(float t)
        {
            return Mathf.Sin((t * Mathf.PI) / 2);
        }
        public static float InOut(float t)
        {
            return -(Mathf.Cos(Mathf.PI * t) - 1) / 2;
        }
        public static float InOut(float t, bool b)
        {
            if (b)
            {
                return Mathf.PingPong(InOut(t), 0.5f);
            }
            else
            {
                return InOut(t);
            }
        }
    }
    public class Quadratic
    {
        public static float In(float t)
        {
            return t * t;
        }
        public static float Out(float t)
        {
            return 1 - (1 - t) * (1 - t);
        }
        public static float InOut(float t)
        {
            return t < 0.5 ? 2 * t * t : 1 - Mathf.Pow(-2 * t + 2, 2) / 2;
        }
        public static float InOut(float t, bool b)
        {
            if (b)
            {
                return Mathf.PingPong(InOut(t), 0.5f);
            }
            else
            {
                return InOut(t);
            }
        }
    };
    public class Elastic
    {
        public static float In(float t)
        {
            return t == 0 ? 0 : t == 1 ? 1 : -Mathf.Pow(2, 10 * t - 10) * Mathf.Sin((t * 10 - 10.75f) * c4);
        }
        public static float Out(float t)
        {
            return t == 0 ? 0 : t == 1 ? 1 : Mathf.Pow(2, -10 * t) *
                Mathf.Sin((t * 10 - 0.75f) * c4) + 1;
        }
        public static float InOut(float t)
        {
            return t == 0 ? 0 : t == 1 ? 1 : t < 0.5f ? -(Mathf.Pow(2, 20 * t - 10)
                * Mathf.Sin((20 * t - 11.125f) * c5)) / 2 : (Mathf.Pow(2, -20 * t + 10)
                * Mathf.Sin((20 * t - 11.125f) * c5)) / 2 + 1;
        }
        public static float InOut(float t, bool b)
        {
            if (b)
            {
                return Mathf.PingPong(InOut(t), 0.5f);
            }
            else
            {
                return InOut(t);
            }
        }
    }
    public class Bounce
    {

        public static float In(float t)
        {
            return 1 - Out(1 - t);
        }
        public static float Out(float t)
        {
            if (t < 1 / d1)
            {
                return n1 * t * t;
            }
            else if (t < 2 / d1)
            {
                return n1 * (t -= 1.5f / d1) * t + 0.75f;
            }
            else if (t < 2.5 / d1)
            {
                return n1 * (t -= 2.25f / d1) * t + 0.9375f;
            }
            else
            {
                return n1 * (t -= 2.625f / d1) * t + 0.984375f;
            }
        }
        public static float InOut(float t)
        {
            return t < 0.5 ? (1 - Out(1 - 2 * t)) / 2 : (1 + Out(2 * t - 1)) / 2;
        }
        public static float InOut(float t, bool b)
        {
            if (b)
            {
                return Mathf.PingPong(InOut(t), 0.5f);
            }
            else
            {
                return InOut(t);
            }
        }
    }

    public class Back
    {
        public static float In(float t)
        {
            return c3 * t * t * t - c1 * t * t;
        }
        public static float Out(float t)
        {
            return 1 + c3 * Mathf.Pow(t - 1, 3) + c1 * Mathf.Pow(t - 1, 2);
        }
        public static float InOut(float t)
        {
            return t < 0.5 ? (Mathf.Pow(2 * t, 2) * ((c2 + 1) * 2 * t - c2)) / 2 : (Mathf.Pow(2 * t - 2, 2)
                * ((c2 + 1) * (t * 2 - 2) + c2) + 2) / 2;
        }
        public static float InOut(float t, bool b)
        {
            if (b)
            {
                return Mathf.PingPong(InOut(t), 0.5f);
            }
            else
            {
                return InOut(t);
            }
        }
    }


}