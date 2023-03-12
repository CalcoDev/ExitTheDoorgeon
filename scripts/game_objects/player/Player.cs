using System;
using System.Collections.Generic;
using Components.Logic;
using Game.Components;
using Game.Components.Guns;
using Game.Extensions;
using Godot;
using Godot.Collections;
using GodotUtilities;
using Managers;
using Utils;

namespace Game.GameObjects;

public partial class Player : CharacterBody2D
{
    #region Input

    private struct Key
    {
        public bool Pressed;
        public bool Down;
        public bool Released;
    }

    private struct PlayerInput
    {
        public Vector2 Move;
        public Vector2 LastNonZeroMove;

        public Vector2 Aim;

        public Key FirePrimary;
        public Key FireSecondary;

        public Key SwapGuns;
        public Key ReloadPrimary;
    }

    private static void UpdateKey(ref Key key, string k)
    {
        key.Pressed = Input.IsActionJustPressed(k);
        key.Down = Input.IsActionPressed(k);
        key.Released = Input.IsActionJustReleased(k);
    }

    #endregion

    [ExportGroup("Constants")]
    [ExportSubgroup("Movement")]
    [Export] public float MoveSpeed { get; set; } = 100f;
    [Export] public float Acceleration { get; set; } = 100f;

    [ExportSubgroup("Gun Inventory")]
    [Export] public GunComponent PrimaryGun { get; set; }
    [Export] public GunComponent SecondaryGun { get; set; }

    [Export] public Array<GunComponent> GunInventory { get; set; } = new Array<GunComponent>();

    [Export] private Node2D _rightGunPos;
    [Export] private Node2D _leftGunPos;

    [ExportGroup("Variables")]

    // State Machine
    [Node]
    private StateMachineComponent _stateMachine;

    public const int StNormal = 0;
    public const int StRoll = 1;

    // Input
    private PlayerInput _input;

    // Sprite
    [Node]
    private AnimatedSprite2D _sprite;
    private bool _facingRight = true;

    // Faction
    [Node]
    private FactionComponent _factionComponent;

    #region Lifecycle

    public override void _EnterTree()
    {
        this.WireNodes();
    }

    public override void _Ready()
    {
        _stateMachine.Init(2, 0);
        _stateMachine.SetCallbacks(StNormal, UpdateNormal, EnterNormal, ExitNormal);

        if (PrimaryGun != null)
            GunInventory.Add(PrimaryGun);
        if (SecondaryGun != null)
            GunInventory.Add(SecondaryGun);
    }

    public override void _PhysicsProcess(double delta)
    {
        _input.Move.X = Input.GetAxis("axis_horizontal_negative", "axis_horizontal_positive");
        _input.Move.Y = Input.GetAxis("axis_vertical_negative", "axis_vertical_positive");

        if (_input.Move.LengthSquared() > 0.01f)
            _input.LastNonZeroMove = _input.Move;

        var mousePos = GetViewport().GetMousePosition();
        _input.Aim = (mousePos - GlobalPosition).Normalized();

        UpdateKey(ref _input.FirePrimary, "btn_fire_primary");
        UpdateKey(ref _input.FireSecondary, "btn_fire_secondary");

        UpdateKey(ref _input.SwapGuns, "btn_swap_guns");
        UpdateKey(ref _input.ReloadPrimary, "btn_reload_primary");

        int newSt = _stateMachine.Update();
        _stateMachine.SetState(newSt);
    }

    #endregion

    #region State Machine

    private void EnterNormal()
    {
    }

    private int UpdateNormal()
    {
        var targetVel = _input.Move.Normalized() * MoveSpeed;
        Velocity = Calc.ApproachV(Velocity, targetVel, Acceleration);
        MoveAndSlide();

        // if (_input.Move.LengthSquared() > 0.01f)
        //     _sprite.Play($"walk_{_input.Aim.Vector2ToDirString(0.2f)}");
        // else
        const float epsilon = 0.2f;
        _sprite.Play($"walk_{_input.Aim.Vector2ToDirString(epsilon)}");

        // Figure out if to face left or right based on the aim direction
        var d = _input.Aim.Dot(Vector2.Right);
        if (Calc.FloatEquals(d, 1f, epsilon * 2f))
            CallDeferred(nameof(FaceRight));
        else if (Calc.FloatEquals(d, -1f, epsilon * 2f))
            CallDeferred(nameof(FaceLeft));

        if (_input.SwapGuns.Pressed)
        {
            (SecondaryGun, PrimaryGun) = (PrimaryGun, SecondaryGun);
            CallDeferred(nameof(UpdateGunPositions));
        }

        if (PrimaryGun != null)
        {
            if (_input.FirePrimary.Pressed)
            {
                if (!PrimaryGun.HasClip && PrimaryGun.HasAmmo && !PrimaryGun.IsReloading)
                    PrimaryGun.Reload();

                PrimaryGun.TryFire(this, _input.Aim, (uint)_factionComponent.FactionType);
            }

            if (_input.ReloadPrimary.Pressed)
                PrimaryGun?.Reload();

            PrimaryGun.Rotation = _input.Aim.Angle();

            var scl = (PrimaryGun.Rotation > Mathf.Pi / 2 || PrimaryGun.Rotation < -Mathf.Pi / 2) ? -1 : 1;
            PrimaryGun.Scale = new Vector2(1, scl);
        }

        if (SecondaryGun != null)
        {
            if (_input.FireSecondary.Pressed)
            {
                if (!SecondaryGun.HasClip && SecondaryGun.HasAmmo && !SecondaryGun.IsReloading)
                    SecondaryGun.Reload();

                SecondaryGun.TryFire(this, _input.Aim, (uint)_factionComponent.FactionType);
            }

            SecondaryGun.Rotation = _input.Aim.Angle();

            var scl = (SecondaryGun.Rotation > Mathf.Pi / 2 || SecondaryGun.Rotation < -Mathf.Pi / 2) ? -1 : 1;
            SecondaryGun.Scale = new Vector2(1, scl);
        }

        return StNormal;
    }

    private void ExitNormal()
    {
    }

    #endregion

    #region Helpers

    private void FaceLeft()
    {
        if (!_facingRight)
            return;
        UpdateGunPositions();
    }

    private void FaceRight()
    {
        if (_facingRight)
            return;
        UpdateGunPositions();
    }

    private void UpdateGunPositions()
    {
        if (PrimaryGun != null)
        {
            _rightGunPos.TryRemoveChild(PrimaryGun);
            _leftGunPos.TryRemoveChild(PrimaryGun);

            if (_facingRight)
                _rightGunPos.AddChild(PrimaryGun);
            else
                _leftGunPos.AddChild(PrimaryGun);
        }

        if (SecondaryGun != null)
        {
            _rightGunPos.TryRemoveChild(SecondaryGun);
            _leftGunPos.TryRemoveChild(SecondaryGun);

            if (_facingRight)
                _leftGunPos.AddChild(SecondaryGun);
            else
                _rightGunPos.AddChild(SecondaryGun);
        }
    }

    #endregion
}