using Godot;

namespace Game.Components.Guns;

public abstract partial class GunDefinition : RefCounted
{
    public abstract string Name { get; }
    public abstract string Description { get; }

    /// <summary>
    /// The maximum amount of ammo this gun can hold.
    /// </summary>
    public abstract int MaxAmmo { get; }

    /// <summary>
    /// The maximum amount of ammo this gun can hold in a clip.
    /// </summary>
    public abstract int MaxClipSize { get; }

    public abstract float ReloadTime { get; }
    public abstract float FireRate { get; }

    /// <summary>
    /// Amount of knockback this gun applies to the user, <b>in pixels</b>.
    /// </summary>
    public abstract float Knockback { get; }

    /// <summary>
    /// Amount of bullet spread, <b>in radians</b>.
    /// </summary>
    public abstract float Spread { get; }

    public abstract float Damage { get; }

    #region Bullet

    public abstract int BulletCount { get; }

    /// <summary>
    /// The extents of the fan of bullets, <b>in radians</b>. Will be centered around the fire direction, and bullets will be evenly distributed.
    /// </summary>
    public abstract float BulletFanAngle { get; }

    public abstract float BulletLifetime { get; }
    public abstract float BulletRange { get; }
    public abstract float BulletSpeed { get; }

    #endregion
}