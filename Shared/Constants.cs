using System;
using System.Diagnostics.Tracing;

namespace globalgamejam2024.Shared;

public static class MovementDirection
{
    public static string Up = "Up";
    public static string Down = "Down";
    public static string Left = "Left";
    public static string Right = "Right";
}
public static class PlayerAction
{
    public static string Punch = "Punch";
    public static string Dodge = "Dodge";
}

public static class GGJ
{
    public static int CurrentPunchID = 0;
    public static MobController mobController;
    public static Player player;
    public static Random random = new Random();
}

public static class GGJCollisionLayers
{
    public static uint None = 0;
    public static uint Player = 2;
    public static uint Enemy = 3;
    public static uint Bitmask(uint value)
    {
        return (uint)Math.Pow(2f, (float)(value - 1));
    }
}
