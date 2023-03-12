using Godot;

namespace Game.Components.Bullets;

// TODO(calco): Maybe make this a Node?
public abstract partial class BulletTravelDefinition : RefCounted
{
    public class BulletTravelArgs
    {
        public BulletComponent Bullet { get; set; }
        public float DistanceTraveled { get; set; }

        public Vector2 FireDirection { get; set; }
        public Vector2 CurrentVelocity { get; set; }
    }

    public class BulletTravelResult
    {
        public Vector2 ComputedVelocity { get; set; }
        public Vector2 AdditionalVelocity { get; set; }
    }

    public abstract BulletTravelResult ComputeTravel(BulletTravelArgs args);
}