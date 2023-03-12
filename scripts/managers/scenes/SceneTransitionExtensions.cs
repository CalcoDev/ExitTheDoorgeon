using System;

namespace Game.Managers.Scenes;

public static class SceneTransitionExtensions
{
    public static string ToName(this SceneTransition transition)
    {
        return Enum.GetName(typeof(SceneTransition), transition).ToLower();
    }
}