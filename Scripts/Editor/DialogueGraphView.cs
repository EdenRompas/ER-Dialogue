using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueGraphView : GraphView
{
    public DialogueGraphView()
    {
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();
    }

    public void CreateNode()
    {
        var node = new DialogueNode();
        node.SetPosition(new Rect(Vector2.zero, new Vector2(200, 150)));
        AddElement(node);
    }

    public void CreateStartNode()
    {
        var node = new StartNode();
        node.SetPosition(new Rect(Vector2.zero, new Vector2(200, 150)));
        AddElement(node);
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        var compatiblePorts = new List<Port>();
        ports.ForEach(funcCall: (port) =>
        {
            if (startPort != port && startPort.node != port.node)
            {
                compatiblePorts.Add(port);
            }
        });

        return compatiblePorts;
    }

    public void ClearGraph()
    {
        graphElements.ForEach(RemoveElement);
    }
}