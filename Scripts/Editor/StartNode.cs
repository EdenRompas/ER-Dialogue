using UnityEditor.Experimental.GraphView;

public class StartNode : Node
{
    public Port OutputPort { get; private set; }
    public string Guid;

    public StartNode()
    {
        title = "Start";

        capabilities &= ~Capabilities.Deletable;

        Guid = System.Guid.NewGuid().ToString();

        OutputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
        OutputPort.portName = "Start";
        outputContainer.Add(OutputPort);

        RefreshExpandedState();
        RefreshPorts();
    }
}