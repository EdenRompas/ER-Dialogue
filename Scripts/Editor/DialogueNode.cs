using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;

public class DialogueNode : Node
{
    public string Guid;

    public string CharacterName { get; set; }
    public List<string> Lines { get; set; } = new List<string>();
    public Sprite CharacterSprite { get; set; }
    public int CharacterId { get; set; }

    public Port InputPort { get; private set; }
    public Port OutputPort {  get; private set; }

    private VisualElement _linesContainer;

    public DialogueNode()
    {
        Guid = System.Guid.NewGuid().ToString();

        title = "Input Name";

        var nameField = new TextField("Character");
        nameField.RegisterValueChangedCallback(evt =>
        {
            CharacterName = evt.newValue;
            title = evt.newValue;

            if (evt.newValue == "")
            {
                title = "Input Name";
            }
        });
        mainContainer.Add(nameField);

        var idField = new IntegerField("Character ID");
        idField.RegisterValueChangedCallback(evt => CharacterId = evt.newValue);
        mainContainer.Add(idField);

        var spriteField = new ObjectField("Sprite")
        {
            objectType = typeof(Sprite),
            allowSceneObjects = false
        };
        spriteField.RegisterValueChangedCallback(evt => CharacterSprite = evt.newValue as Sprite);
        mainContainer.Add(spriteField);

        if (Lines.Count == 0)
        {
            Lines.Add("");
        }

        DrawLinesList();

        AddInputPort("In");
        AddOutputPort("Out");
        RefreshExpandedState();
        RefreshPorts();
    }

    private void DrawLinesList()
    {
        _linesContainer = new VisualElement();
        _linesContainer.style.marginTop = 6;
        mainContainer.Add(_linesContainer);

        for (int i = 0; i < Lines.Count; i++)
        {
            AddLineField(i, Lines[i]);
        }

        var addButton = new Button(() =>
        {
            Lines.Add("");
            AddLineField(Lines.Count - 1, "");
        })
        {
            text = "+ Add Line"
        };
        mainContainer.Add(addButton);
    }

    private void AddLineField(int index, string value)
    {
        var container = new VisualElement();
        container.style.flexDirection = FlexDirection.Row;

        var lineField = new TextField() { value = value };
        lineField.style.flexGrow = 1;

        lineField.RegisterValueChangedCallback(evt =>
        {
            Lines[index] = evt.newValue;
        });

        var removeButton = new Button(() =>
        {
            Lines.RemoveAt(index);
            _linesContainer.Remove(container);
            RedrawLines();
        })
        {
            text = "-"
        };
        removeButton.style.marginLeft = 4;

        container.Add(lineField);
        container.Add(removeButton);
        _linesContainer.Add(container);
    }

    private void RedrawLines()
    {
        _linesContainer.Clear();
        for (int i = 0; i < Lines.Count; i++)
        {
            AddLineField(i, Lines[i]);
        }
    }

    private void AddInputPort(string portName)
    {
        InputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
        InputPort.portName = portName;
        inputContainer.Add(InputPort);
    }

    private void AddOutputPort(string portName)
    {
        OutputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
        OutputPort.portName = portName;
        outputContainer.Add(OutputPort);
    }
}