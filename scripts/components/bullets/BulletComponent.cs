using System.Collections.Generic;
using Game.Components.Guns;
using Game.Extensions;
using Godot;
using GodotUtilities;
using Managers;

namespace Game.Components.Bullets;

public abstract partial class BulletComponent : CharacterBody2D
{
    public CharacterBody2D Shooter { get; private set; }
    public GunDefinition GunDefinition { get; private set; }
    public Vector2 FireDirection { get; private set; }

    [Node]
    public FactionComponent FactionComponent { get; protected set; }

    public List<BulletTravelDefinition> TravelDefinitions { get; private set; }

    private float _distanceTraveled;
    private float _timeAlive;

    private Vector2 _lastAppliedVelocity;
    private Vector2 _additionalVelocity;

    public void Init(CharacterBody2D shooter, GunDefinition gunDefinition, Vector2 fireDirection)
    {
        Shooter = shooter;
        GunDefinition = gunDefinition;
        FireDirection = fireDirection;

        TravelDefinitions = new List<BulletTravelDefinition>();
    }

    public void AddTravelDefinition(BulletTravelDefinition travelDefinition)
    {
        TravelDefinitions.Add(travelDefinition);
    }

    public void AddTravelDefinitions(IEnumerable<BulletTravelDefinition> travelDefinitions)
    {
        TravelDefinitions.AddRange(travelDefinitions);
    }

    public void AddTravelDefinitions(params BulletTravelDefinition[] travelDefinitions)
    {
        TravelDefinitions.AddRange(travelDefinitions);
    }

    public override void _EnterTree()
    {
        this.WireNodes();
    }

    public override void _PhysicsProcess(double delta)
    {
        var prevVel = Velocity;

        Velocity = Vector2.Zero;
        _additionalVelocity = Vector2.Zero;
        foreach (var travelDefinition in TravelDefinitions)
        {
            var args = new BulletTravelDefinition.BulletTravelArgs
            {
                Bullet = this,
                DistanceTraveled = _distanceTraveled,
                CurrentVelocity = prevVel,
                FireDirection = FireDirection
            };

            var result = travelDefinition.ComputeTravel(args);

            Velocity += result.ComputedVelocity;
            _additionalVelocity += result.AdditionalVelocity;
        }

        Velocity += _additionalVelocity;
        _lastAppliedVelocity = Velocity;
        MoveAndSlide();
        if (GetSlideCollisionCount() > 0)
            HandleCollision();

        Rotation = Velocity.Angle();

        _timeAlive += GameManager.PhysicsDelta;

        // TODO(calco): Maybe make this use time alive??
        _distanceTraveled += GunDefinition.BulletSpeed * GameManager.PhysicsDelta;
        // _distanceTraveled = _timeAlive;

        if (GunDefinition.BulletRange > 0 && _distanceTraveled > GunDefinition.BulletRange)
            HandleDeath();

        if (GunDefinition.BulletLifetime > 0 && _timeAlive > GunDefinition.BulletLifetime)
            HandleDeath();
    }

    private void HandleCollision()
    {
        bool shouldDie = false;
        int cnt = GetSlideCollisionCount();
        for (int i = 0; i < cnt; i++)
        {
            var coll = GetSlideCollision(i);
            if (coll.GetCollider() is not Node2D other)
                continue;

            if (other.TryGetComponent<FactionComponent>(out var factionComponent))
            {
                if (factionComponent.FactionType != FactionComponent.FactionType)
                {
                    shouldDie = true;
                    break;
                }

                this.AddCollisionExceptionWith(other);
            }
            else
            {
                shouldDie = true;
                break;
            }
        }

        if (shouldDie)
        {
            HandleDeath();
        }
        else
        {
            Velocity = _lastAppliedVelocity - Velocity;
            MoveAndSlide();
        }
    }

    private void HandleDeath()
    {
        QueueFree();
    }
}