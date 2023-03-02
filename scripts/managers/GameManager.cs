using Godot;
using Godot.Collections;
using GodotUtilities;

namespace Managers;

public partial class GameManager : Node
{
    [Node]
    private ResourcePreloader _resourcePreloader;

    public static GameManager Instance { get; private set; }

    public static Node Root { get; private set; }
    public static Node2D SceneRoot { get; private set; }

    public static float Time { get; private set; } = 0f;
    public static uint FrameCount { get; private set; } = 0;

    public static float Delta { get; private set; } = 0f;
    public static float PhysicsDelta { get; private set; } = 0f;

    public static World2D GlobalWorld { get; private set; }
    public static PhysicsDirectSpaceState2D GlobalPhysicsSpace => GlobalWorld.DirectSpaceState;

    [Export] public bool Debug { get; set; } = false;

    #region Game Events

    [Signal]
    public delegate void OnDebugModeChangedEventHandler(bool debugMode);

    #endregion

    // public override void _Notification(long what)
    // {
    //     if (what == NotificationSceneInstantiated)
    //     {
    //         Instance = this;
    //     }
    // }

    public override void _Notification(int what)
    {
        if (what == NotificationSceneInstantiated)
        {
            Instance = this;
        }
    }

    public override void _EnterTree()
    {
        ProcessPriority = -1;
    }

    public override void _Ready()
    {
        Root = GetTree().Root;
        SceneRoot = GetTree().CurrentScene as Node2D;
    }

    public override void _Process(double delta)
    {
        Delta = (float)delta;
        Time += Delta;

        FrameCount++;

        if (Input.IsActionJustPressed("btn_toggle_debug"))
        {
            Debug = !Debug;
            EmitSignal(SignalName.OnDebugModeChanged, Debug);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        GlobalWorld = SceneRoot.GetWorld2D();
        PhysicsDelta = (float)delta;
    }

    #region Physics Helpers

    public static bool Raycast(Vector2 from, Vector2 to, uint mask)
    {
        PhysicsRayQueryParameters2D query = PhysicsRayQueryParameters2D.Create(from, to, mask, null);
        Dictionary res = GlobalPhysicsSpace.IntersectRay(query);

        return res.Count > 0;
    }

    #endregion

    #region FX Helpers

    public static Node2D SpawnPixelatedFX(PackedScene fx, Vector2 position, bool root = true)
    {
        var fxInstance = fx.Instantiate() as Node2D;

        if (root)
            Root.AddChild(fxInstance);
        else
            Instance.AddChild(fxInstance);

        fxInstance.GlobalPosition = position;
        RenderingManager.Instance.TryAddNodeToLayer(fxInstance);

        return fxInstance;
    }

    #endregion
}
