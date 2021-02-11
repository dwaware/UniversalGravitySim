using System;

public class Constants
{
    public const float DEG_PER_RAD = 57.29578f;
    public const float RAD_PER_DEG = 0.0174533f;
    public const double TWO_PI = 2 * Math.PI;

    public const float G = 39.47842f;
    // Universal Gravitational Constant (2*Pi)^2 in units of AU^3 MSol^-1 YEAR^-2

    public const int G_SCALE = 10000; // 10 second orbit at 100 pixels radius = 1AU  ( one year full year: 1 / 996000000f; )
    // Scale factor to alter the speed of orbiting objects
    // Anywhere G appears, we multiply G by this factor

    public const int MASS_SOL = 1;
    // units:  solar masses

    public const float MASS_EARTH = 3e-6f;
    // units:  solar masses, i.e. MassSun * 3x10^-6 = MassEarth
}