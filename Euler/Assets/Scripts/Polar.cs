using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Polar
{
    public float m_az;      // in radians
    public float m_el;      // in radians
    public float m_rad;

    public Polar(float az, float el, float rad)
    {
        m_az = az;
        m_el = el;
        m_rad = rad;
    }

    public Polar(Vector3 cart)
    {
        m_rad = cart.magnitude;
        Vector3 cartXZ = cart;
        cartXZ.y = 0.0f;
        float dXZ = cartXZ.magnitude;
        m_az = Mathf.Atan2(cart.x, cart.z);
        m_el = Mathf.Atan2(cart.y, dXZ);
    }

    public Vector3 ToCart()
    {
        Vector3 cart = Vector3.zero;
        cart.y = m_rad * Mathf.Sin(m_el);
        float dXZ = m_rad * Mathf.Cos(m_el);
        cart.x = dXZ * Mathf.Sin(m_az);
        cart.z = dXZ * Mathf.Cos(m_az);

        return cart;
    }

    public Vector3 ToFlat(float baseRadius)
    {
        Vector3 flat = Vector3.zero;
        float r = baseRadius * 2.0f * (0.5f * Mathf.PI - m_el) / Mathf.PI;
        flat.x = r * Mathf.Sin(m_az);
        flat.z = r * Mathf.Cos(m_az);
        flat.y = m_rad - baseRadius;

        return flat;
    }

    public static float FlatteningCurve(float howFlat)
    {
        float t2 = howFlat * howFlat;
        float t3 = t2 * howFlat;
        float lerp = -2.0f * t3 + 3.0f * t2;
        return lerp;
    }

    public Vector3 ConvertTheVert(float baseRadius, float howFlat)
    {
        Vector3 cart = ToCart();
        Vector3 flat = ToFlat(baseRadius);
        float lerp = FlatteningCurve(howFlat);
        return Vector3.Lerp(cart, flat, lerp);
    }

    public Vector3 ToNorm()
    {
        Vector3 cart = Vector3.zero;
        cart.y = Mathf.Sin(m_el);
        float dXZ = Mathf.Cos(m_el);
        cart.x = dXZ * Mathf.Sin(m_az);
        cart.z = dXZ * Mathf.Cos(m_az);

        return cart;
    }

    public Vector3 NormFlat()
    {
        return Vector3.up;
    }

    public Vector3 ConvertTheNorm(float howFlat)
    {
        Vector3 cart = ToNorm();
        Vector3 flat = NormFlat();
        float lerp = FlatteningCurve(howFlat);
        return Vector3.Lerp(cart, flat, lerp);
    }

    public Vector3 ToNorth()
    {
        Vector3 cart = Vector3.zero;
        cart.y = -Mathf.Cos(m_el);
        float dXZ = Mathf.Sin(m_el);
        cart.x = dXZ * Mathf.Sin(m_az);
        cart.z = dXZ * Mathf.Cos(m_az);
        return cart;
    }

    public Vector3 NorthFlat()
    {
        Vector3 cart = Vector3.zero;
        cart.x = Mathf.Sin(m_az);
        cart.z = Mathf.Cos(m_az);
        return cart;
    }

    public Vector3 ConvertTheNorth(float howFlat)
    {
        Vector3 cart = ToNorth();
        Vector3 flat = NorthFlat();
        float lerp = FlatteningCurve(howFlat);
        return Vector3.Lerp(cart, flat, lerp);
    }
}
