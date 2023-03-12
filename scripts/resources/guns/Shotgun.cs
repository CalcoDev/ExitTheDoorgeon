using Game.Components.Guns;
using Game.Resources.Bullets.TravelDefinitions;

namespace Game.Resources.Guns;

public partial class Shotgun : GunComponent
{
    public override void _Ready()
    {
        Definition = new Definitions.ShotgunDefinition();
        BulletTravelDefinitions.Add(new LinearBulletTravelDefinition());
        base._Ready();
    }
}