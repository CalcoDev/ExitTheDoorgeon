using Game.Components.Guns;
using Game.Resources.Bullets.TravelDefinitions;

namespace Game.Resources.Guns;

public partial class Pistol : GunComponent
{
    public override void _Ready()
    {
        Definition = new Definitions.PistolDefinition();
        BulletTravelDefinitions.Add(new LinearBulletTravelDefinition());
        base._Ready();
    }
}