using System;
using System.Collections.Generic;
using Components.Logic;
using Game.Components.Bullets;
using Game.Managers.Scenes;
using Godot;
using GodotUtilities;

namespace Game.Components.Guns;

public abstract partial class GunComponent : Node2D
{
    [Export] public PackedScene BulletScene { get; set; }

    public GunDefinition Definition { get; protected set; }
    public List<BulletTravelDefinition> BulletTravelDefinitions { get; protected set; } = new List<BulletTravelDefinition>();

    public int RemainingAmmo { get; protected set; }
    public int RemainingClip { get; protected set; }

    public bool IsReloading { get; protected set; }

    [Node]
    protected TimerComponent ReloadTimer;

    [Node]
    protected TimerComponent FireTimer;

    [Node]
    protected Node2D BulletSpawnPoint;

    public bool FireTimerFinished => FireTimer.HasFinished;

    public bool HasClip => RemainingClip > 0;
    public bool HasAmmo => RemainingAmmo > 0;

    public override void _Ready()
    {
        RemainingAmmo = Definition.MaxAmmo;
        RemainingClip = Definition.MaxClipSize;
    }

    [Signal]
    public delegate void OnFireEventHandler(CharacterBody2D shooter, Vector2 direction, uint factionType);

    [Signal]
    public delegate void OnReloadStartEventHandler(int remainingClip, int remainingAmmo);

    [Signal]
    public delegate void OnReloadFinishedEventHandler(int remainingClip, int remainingAmmo);

    public override void _EnterTree()
    {
        this.WireNodes();

        ReloadTimer.OnTimeout += OnReloadTimerTimeout;
        this.OnFire += FireBullet;
    }

    public override void _ExitTree()
    {
        ReloadTimer.OnTimeout -= OnReloadTimerTimeout;
        this.OnFire -= FireBullet;
    }

    public virtual bool CanFire()
    {
        return HasClip && FireTimerFinished && !IsReloading;
    }

    public void Reload()
    {
        GD.Print("Trying to reload...");

        if (IsReloading)
            return;

        GD.Print("Started reload...");

        EmitSignal(SignalName.OnReloadStart, RemainingClip, RemainingAmmo);
        IsReloading = true;

        ReloadTimer.Start(Definition.ReloadTime);
    }

    public void TryFire(CharacterBody2D shooter, Vector2 direction, uint factionType)
    {
        if (!CanFire())
            return;

        EmitSignal(SignalName.OnFire, shooter, direction, factionType);
        RemainingClip--;
        FireTimer.Start(Definition.FireRate);
    }

    public void ForceFire(CharacterBody2D shooter, Vector2 direction, uint factionType)
    {
        EmitSignal(SignalName.OnFire, shooter, direction, factionType);
        RemainingClip--;
        FireTimer.Start(Definition.FireRate);
    }

    public void OnReloadTimerTimeout()
    {
        int ammoNeeded = Definition.MaxClipSize - RemainingClip;
        int ammoAvailable = Mathf.Min(ammoNeeded, RemainingAmmo);

        RemainingAmmo -= ammoAvailable;
        RemainingClip += ammoAvailable;

        EmitSignal(SignalName.OnReloadFinished, RemainingClip, RemainingAmmo);

        IsReloading = false;
    }

    private bool b = false;
    protected void FireBullet(CharacterBody2D shooter, Vector2 direction, uint factionType)
    {
        for (int i = 0; i < Definition.BulletCount; i++)
        {
            var bullet = BulletScene.Instantiate<BulletComponent>();

            var fanAngle = Definition.BulletFanAngle * ((i / (float)Definition.BulletCount) - 0.5f);
            var fanDir = direction.Rotated(fanAngle);

            var spread = (float)GD.RandRange(-Definition.Spread, Definition.Spread);
            var bulletDirection = fanDir.Rotated(spread);

            // TODO(calco): Refactor this into a GameModeManager.
            SceneManager.Instance.CurrentYSort.AddChild(bullet);

            bullet.Init(shooter, Definition, bulletDirection);
            bullet.AddTravelDefinitions(BulletTravelDefinitions);
            bullet.FactionComponent.FactionType = (FactionComponent.Faction)factionType;
            bullet.GlobalPosition = BulletSpawnPoint.GlobalPosition;
            bullet.GlobalRotation = bulletDirection.Angle();
            b = !b;
        }

        // TODO(calco): Apply knockback to shooter.
        var v = shooter.Velocity;
        shooter.Velocity = -direction * Definition.Knockback;
        shooter.MoveAndSlide();
        shooter.Velocity = v;
    }
}