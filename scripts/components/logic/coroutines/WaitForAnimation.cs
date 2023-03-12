using System;
using Godot;

namespace Game.Components.Logic.Coroutines;

public class WaitForAnimation : IYieldable
{
    public readonly AnimationPlayer AnimationPlayer;
    public readonly string AnimationName;

    public bool IsDone => _finished;
    private bool _finished = false;

    public WaitForAnimation(AnimationPlayer animationPlayer, string name)
    {
        AnimationPlayer = animationPlayer;

        AnimationPlayer.AnimationFinished += OnAnimationFinished;
        AnimationName = name;

        AnimationPlayer.Play(name);
    }

    private void OnAnimationFinished(StringName animName)
    {
        if (animName != AnimationName)
            return;

        AnimationPlayer.AnimationFinished -= OnAnimationFinished;
        _finished = true;
    }

    public void Update(float delta)
    {
    }
}