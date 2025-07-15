using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;

public class DialogueNode : Node
{
    public string Guid { get; set; }

    public string CharacterName { get; set; }
    public List<string> Lines { get; set; } = new List<string>();
    public Sprite CharacterSprite { get; set; }
    public bool IsShowIcon { get; set; }
    public IconPosition IconPosition { get; set; }

    public Port InputPort { get; private set; }
    public Port OutputPort {  get; private set; }

    private TextField _characterName;
    private Toggle _isShowIconToggle;
    private ObjectField _characterSpriteField;
    private EnumField _iconPositionField;

    private VisualElement _linesContainer;

    public DialogueNode()
    {
        mainContainer.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 1f);
        style.minWidth = 256;

        Guid = System.Guid.NewGuid().ToString();

        title = "Name";

        _characterName = new TextField("Name");
        _characterName.RegisterValueChangedCallback(evt =>
        {
            CharacterName = evt.newValue;
            title = evt.newValue;

            if (evt.newValue == "")
            {
                title = "Input Name";
            }
        });
        mainContainer.Add(_characterName);

        _isShowIconToggle = new Toggle("Is Show Icon");
        _isShowIconToggle.RegisterValueChangedCallback(evt =>
        {
            IsShowIcon = evt.newValue;
            UpdateIconVisibility();
        });
        mainContainer.Add(_isShowIconToggle);

        _characterSpriteField = new ObjectField("Character Sprite")
        {
            objectType = typeof(Sprite),
            allowSceneObjects = false
        };
        _characterSpriteField.RegisterValueChangedCallback(evt => CharacterSprite = evt.newValue as Sprite);
        mainContainer.Add(_characterSpriteField);

        var iconPositionField = new EnumField("Icon Position", IconPosition.Left);
        iconPositionField.Init(IconPosition.Left);

        iconPositionField.RegisterValueChangedCallback(evt =>
        {
            IconPosition = (IconPosition)evt.newValue;
        });
        mainContainer.Add(iconPositionField);

        iconPositionField.style.display = DisplayStyle.None;
        _iconPositionField = iconPositionField;

        UpdateIconVisibility();

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

    public void InitializeNodeData(string characterName, bool isShowIcon, IconPosition iconPosition, Sprite characterSprite, List<string> lines)
    {
        CharacterName = characterName;
        IsShowIcon = isShowIcon;
        IconPosition = iconPosition;
        CharacterSprite = characterSprite;
        Lines = new List<string>(lines);

        title = string.IsNullOrEmpty(characterName) ? "Input Name" : characterName;

        _characterName.SetValueWithoutNotify(characterName);
        _isShowIconToggle?.SetValueWithoutNotify(IsShowIcon);
        _characterSpriteField?.SetValueWithoutNotify(CharacterSprite);
        _iconPositionField?.SetValueWithoutNotify(IconPosition);

        UpdateIconVisibility();
        RedrawLines();
    }

    private void UpdateIconVisibility()
    {
        var display = IsShowIcon ? DisplayStyle.Flex : DisplayStyle.None;

        _characterSpriteField.style.display = display;
        _iconPositionField.style.display = display;

        _characterSpriteField.SetValueWithoutNotify(null);
        _iconPositionField.SetValueWithoutNotify(IconPosition.Left);
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