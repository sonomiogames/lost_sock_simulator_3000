using System;

[Serializable]
public class BigNumber : IComparable<BigNumber>
{
    public double mantissa;
    public int exponent;

    const double EPSILON = 1e-12;

    public BigNumber(double m, int e)
    {
        mantissa = m;
        exponent = e;
        Normalize();
    }

    public static BigNumber Zero => new BigNumber(0, 0);

    public static BigNumber FromDouble(double value)
    {
        if (Math.Abs(value) < EPSILON)
        {
            return Zero;
        }

        int exp = (int)Math.Floor(Math.Log10(Math.Abs(value)));
        double man = value / Math.Pow(10, exp);

        return new BigNumber(man, exp);
    }

    /* ******************** *
     * NORMALIZATION        *
     * ******************** */
    public void Normalize()
    {
        if (Math.Abs(mantissa) < EPSILON)
        {
            mantissa = 0;
            exponent = 0;
            return;
        }

        while (Math.Abs(mantissa) >= 10)
        {
            mantissa /= 10;
            exponent++;
        }

        while (Math.Abs(mantissa) < 1)
        {
            mantissa *= 10;
            exponent--;
        }
    }

    /* ******************** *
     * ADDITION             *
     * ******************** */
    public static BigNumber operator +(BigNumber a, BigNumber b)
    {
        if (a.mantissa == 0) return new BigNumber(b.mantissa, b.exponent);
        if (b.mantissa == 0) return new BigNumber(a.mantissa, a.exponent);

        if (a.exponent > b.exponent)
        {
            double scaled = b.mantissa * Math.Pow(10, b.exponent - a.exponent);
            return new BigNumber(a.mantissa + scaled, a.exponent);
        }
        else
        {
            double scaled = a.mantissa * Math.Pow(10, a.exponent - b.exponent);
            return new BigNumber(scaled + b.mantissa, b.exponent);
        }
    }

    public void Add(double value)
    {
        this.Add(FromDouble(value));
    }

    public void Add(BigNumber other)
    {
        var result = this + other;
        mantissa = result.mantissa;
        exponent = result.exponent;
    }

    /* ******************** *
     * SUBSTRACTION         *
     * ******************** */
    public static BigNumber operator -(BigNumber a, BigNumber b)
    {
        if (b.mantissa == 0)
        {
            return new BigNumber(a.mantissa, a.exponent);
        }

        if (a.mantissa == 0)
        {
            return Zero;
        }

        if (a < b)
        {
            return Zero;
        }

        if (a.exponent > b.exponent)
        {
            double scaled = b.mantissa * Math.Pow(10, b.exponent - a.exponent);
            return new BigNumber(a.mantissa - scaled, a.exponent);
        }
        else
        {
            double scaled = a.mantissa * Math.Pow(10, a.exponent - b.exponent);
            return new BigNumber(scaled - b.mantissa, b.exponent);
        }
    }

    public void Subtract(BigNumber other)
    {
        var result = this - other;
        mantissa = result.mantissa;
        exponent = result.exponent;
    }

    public void Subtract(double value)
    {
        Subtract(FromDouble(value));
    }

    /* ******************** *
     * MULTIPLY             *
     * ******************** */
    public static BigNumber operator *(BigNumber a, double scalar)
    {
        return new BigNumber(a.mantissa * scalar, a.exponent);
    }

    public void Multiply(double scalar)
    {
        mantissa *= scalar;
        Normalize();
    }

    /* ******************** *
     * COMPARISON           *
     * ******************** */
    public int CompareTo(BigNumber other)
    {
        if (exponent != other.exponent)
        {
            return exponent.CompareTo(other.exponent);
        }

        return mantissa.CompareTo(other.mantissa);
    }

    public static bool operator >=(BigNumber a, BigNumber b)
    {
        return a.CompareTo(b) >= 0;
    }

    public static bool operator <=(BigNumber a, BigNumber b)
    {
        return a.CompareTo(b) <= 0;
    }

    public static bool operator >(BigNumber a, BigNumber b) => a.CompareTo(b) > 0;
    public static bool operator <(BigNumber a, BigNumber b) => a.CompareTo(b) < 0;

    /* ******************** *
     * STRING               *
     * ******************** */
    public string ToStringShort()
    {
        if (mantissa == 0) return "0";

        if (exponent < 6)
        {
            double full = mantissa * Math.Pow(10, exponent);
            return Math.Floor(full).ToString();
        }

        return mantissa.ToString("F2") + "e" + exponent;
    }

    private static readonly Random _rng = new Random();

    public override string ToString() => ToString(false);

    public string ToString(bool addRandom=false)
    {
        if (mantissa == 0)
        {
            return "0";
        }

        if (exponent < 15)
        {
            double full = mantissa * Math.Pow(10, exponent);
            return Math.Floor(full).ToString("N0");
        }

        string mantissaDigits = mantissa.ToString("F15", System.Globalization.CultureInfo.InvariantCulture).Replace(".", "");
        int totalLen = exponent + 1;
        while (mantissaDigits.Length < totalLen)
        {
            mantissaDigits += addRandom ? _rng.Next(0, 10).ToString() : "0";
        }

        return AddCommas(mantissaDigits.Substring(0, totalLen));
    }

    private string AddCommas(string number)
    {
        int len = number.Length;
        int firstGroup = len % 3;

        if (firstGroup == 0)
        {
            firstGroup = 3;
        }

        string result = number.Substring(0, firstGroup);

        for (int i = firstGroup; i < len; i += 3)
        {
            result += "," + number.Substring(i, 3);
        }

        return result;
    }
}
