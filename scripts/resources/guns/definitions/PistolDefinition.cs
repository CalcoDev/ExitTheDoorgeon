using Game.Components.Guns;
using Godot;

namespace Game.Resources.Guns.Definitions;

public partial class PistolDefinition : GunDefinition
{
    public override string Name => "Pistol";
    public override string Description => "A basic pistol.";

    public override int MaxAmmo => 100;
    public override int MaxClipSize => 10;

    public override float ReloadTime => 1f;
    public override float FireRate => 0.15f;

    public override float Knockback => 100f;
    public override float Spread => Mathf.Pi / 10f;

    public override float Damage => 4f;

    public override float BulletLifetime => -1f;
    public override float BulletRange => -1f;
    public override float BulletSpeed => 100f;

    public override int BulletCount => 1;
    public override float BulletFanAngle => 0f;
}