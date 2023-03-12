using Game.Components.Bullets;
using Godot;

namespace Game.Resources.Bullets.TravelDefinitions;

public partial class SineWaveBulletTravelDefinition : BulletTravelDefinition
{
    public readonly float Frequency = 1f;
    public readonly float Amplitude = 1f;
    public readonly bool Reverse = false;

    public SineWaveBulletTravelDefinition(float frequency, float amplitude, bool reverse = false)
    {
        Frequency = frequency;
        Amplitude = amplitude;
        Reverse = reverse;
    }

    public override BulletTravelResult ComputeTravel(BulletTravelArgs args)
    {
        var s = Mathf.Sin(args.DistanceTraveled * Mathf.Pi * Frequency) * Amplitude;
        var d = args.FireDirection.Rotated(Mathf.Pi / 2f * (Reverse ? -1f : 1f));

        return new BulletTravelResult
        {
            AdditionalVelocity = d * s
        };
    }
}