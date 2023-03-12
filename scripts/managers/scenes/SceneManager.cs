using System;
using System.Collections;
using Game.Components.Logic.Coroutines;
using Godot;
using GodotUtilities;

namespace Game.Managers.Scenes;

public partial class SceneManager : CanvasLayer
{
    public static SceneManager Instance { get; private set; }

    public bool IsTransitioning { get; private set; }

    // TODO(calco): Refactor this into a GameModeManager.
    public Node2D SceneContainer => _sceneContainer;
    public Node CurrentScene { get; private set; }
    public Node CurrentYSort { get; private set; }

    [Node]
    private AnimationPlayer _animator;

    private Node2D _sceneContainer;
    private bool _sceneLoadDefferedCheck = false;

    public override void _EnterTree()
    {
        this.WireNodes();

        if (Instance == null)
            Instance = this;
        else
            GD.PrintErr("SceneManager already exists!");
    }

    public override void _Ready()
    {
        CallDeferred(nameof(CreateSceneContainer));
    }

    private void CreateSceneContainer()
    {
        _sceneContainer = new Node2D();
        _sceneContainer.Name = "SceneContainer";
        AddChild(_sceneContainer);
    }

    public void LoadScenePath(string scenePath, SceneTransition transitionType = SceneTransition.Slide)
    {
        if (IsTransitioning)
            return;

        var scene = GD.Load<PackedScene>(scenePath);
        var node = scene.Instantiate();
        CoroutineManager.StartCoroutine(LoadNodeCoroutine(node, transitionType), true, true);
    }

    public void LoadScene(PackedScene scene, SceneTransition transitionType = SceneTransition.Slide)
    {
        if (IsTransitioning)
            return;

        var node = scene.Instantiate();
        CoroutineManager.StartCoroutine(LoadNodeCoroutine(node, transitionType), true, true);
    }

    public void LoadNode(Node node, SceneTransition transitionType = SceneTransition.Slide)
    {
        if (IsTransitioning)
            return;

        CoroutineManager.StartCoroutine(LoadNodeCoroutine(node, transitionType), true, true);
    }

    public IEnumerator LoadNodeCoroutine(Node node, SceneTransition transitionType = SceneTransition.Slide)
    {
        GD.Print("Loading scene: " + node.Name);

        if (IsTransitioning)
        {
            GD.PrintErr("Scene is already transitioning!");
            yield break;
        }

        IsTransitioning = true;

        if (transitionType != SceneTransition.None)
            yield return new WaitForAnimation(_animator, transitionType.ToName() + "_in");

        _sceneLoadDefferedCheck = false;
        CallDeferred(nameof(HandleSceneLoad), node);
        yield return new WaitForCondition(() => _sceneLoadDefferedCheck);

        if (transitionType != SceneTransition.None)
            yield return new WaitForAnimation(_animator, transitionType.ToName() + "_out");

        IsTransitioning = false;
    }

    private void HandleSceneLoad(Node node)
    {
        foreach (var c in _sceneContainer.GetChildren())
            c.Free();

        _sceneContainer.AddChild(node);
        _sceneLoadDefferedCheck = true;

        CurrentScene = node;
        CurrentYSort = node.GetNodeOrNull("YSort");
    }
}