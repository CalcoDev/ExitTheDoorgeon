using Game.Components.Bullets;

namespace Game.Resources.Bullets.TravelDefinitions;

public partial class LinearBulletTravelDefinition : BulletTravelDefinition
{
    public override BulletTravelResult ComputeTravel(BulletTravelArgs args)
    {
        return new BulletTravelResult
        {
            ComputedVelocity = args.FireDirection * args.Bullet.GunDefinition.BulletSpeed
        };
    }
}