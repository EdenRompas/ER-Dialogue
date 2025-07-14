using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class StartNode : Node
{
    public string Guid;
    public Port OutputPort { get; private set; }

    public StartNode()
    {
        mainContainer.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 1f);

        Guid = System.Guid.NewGuid().ToString();

        title = "Start";

        capabilities &= ~Capabilities.Deletable;

        OutputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
        OutputPort.portName = "Start";
        outputContainer.Add(OutputPort);

        RefreshExpandedState();
        RefreshPorts();
    }
}