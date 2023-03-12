using Godot;

namespace Game.Extensions;

public static class NodeExtensions
{
    public static bool TryGetComponent<T>(this Node node, out T component) where T : Node
    {
        // Check if node itself is of type T, if so return it
        if (node is T t)
        {
            component = t;
            return true;
        }

        // Check if node has a child of type T, if so return it
        int cnt = node.GetChildCount();
        for (int i = 0; i < cnt; i++)
        {
            var child = node.GetChild(i);
            if (child is T tChild)
            {
                component = tChild;
                return true;
            }
        }

        component = null;
        return false;
    }

    public static bool TryRemoveChild(this Node node, Node child)
    {
        if (node.HasNode(child.Name.ToString()))
        {
            node.RemoveChild(child);
            return true;
        }

        return false;
    }
}