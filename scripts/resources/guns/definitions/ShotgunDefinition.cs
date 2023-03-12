using Game.Components.Guns;
using Godot;

namespace Game.Resources.Guns.Definitions;

public partial class ShotgunDefinition : GunDefinition
{
    public override string Name => "Shotgun";
    public override string Description => "A basic shotgun.";

    public override int MaxAmmo => 100;
    public override int MaxClipSize => 10;

    public override float ReloadTime => 1.5f;
    public override float FireRate => 0.35f;

    public override float Knockback => 350f;
    public override float Spread => Mathf.Pi / 16f;

    public override float Damage => 4f;

    public override float BulletLifetime => -1f;
    public override float BulletRange => -1f;
    public override float BulletSpeed => 160f;

    public override int BulletCount => 4;
    public override float BulletFanAngle => Mathf.Pi / 18f;
}