using System.Collections;
using System.Linq;
using Game.Managers;
using Game.Managers.Scenes;
using Godot;
using Godot.Collections;
using GodotUtilities;

namespace Managers;

public partial class GameManager : Node
{
    public static GameManager Instance { get; private set; }

    [Export] public bool Debug { get; set; } = false;
    [Export] public Color ClearColour { get; set; } = new Color("#0e071b");

    #region Time

    public static float Time { get; private set; } = 0f;
    public static uint FrameCount { get; private set; } = 0;

    public static float Delta { get; private set; } = 0f;
    public static float PhysicsDelta { get; private set; } = 0f;

    #endregion

    #region Game Events

    [Signal]
    public delegate void OnDebugModeChangedEventHandler(bool debugMode);

    #endregion

    public override void _EnterTree()
    {
        Instance = this;
        this.WireNodes();

        ProcessPriority = -1;
    }

    public override void _Ready()
    {
        CoroutineManager.StartCoroutine(SetUpGame(), true, true);

        RenderingServer.SetDefaultClearColor(ClearColour);
    }

    private IEnumerator SetUpGame()
    {
        var root = GetTree().Root;
        var node = root.GetChildren().FirstOrDefault(x => x.Name.ToString().EndsWith("scene", System.StringComparison.CurrentCultureIgnoreCase));
        root.RemoveChild(node);
        yield return SceneManager.Instance.LoadNodeCoroutine(node, SceneTransition.None);
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
        PhysicsDelta = (float)delta;
    }
}
