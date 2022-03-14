using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldBorderWindow : EditorWindow
{
    [MenuItem("Tools/ World Border")]
    public static void ShowEditorWindow()
    {
        EditorWindow window = GetWindow<WorldBorderWindow>();
        window.titleContent = new GUIContent("World Border");
    }

    public void CreateGUI()
    {
        // HEADER IMAGE
        Image _headerImage = new Image();
        _headerImage.image = (Texture)Resources.Load("world-border-header-placeholder-6");
        _headerImage.scaleMode = ScaleMode.ScaleToFit;
        rootVisualElement.Add(_headerImage);

        //HEIGHT
        IntegerField _heightField = new IntegerField(label: "Height");
        _heightField.value = 16;
        rootVisualElement.Add(_heightField);

        // RADIUS
        IntegerField _radiusField = new IntegerField(label: "Radius");
        _radiusField.value = 5;
        rootVisualElement.Add(_radiusField);

        // PIXELATED
        Toggle _pixelatedToggle = new Toggle(label: "Pixelated");
        _pixelatedToggle.value = false;
        rootVisualElement.Add(_pixelatedToggle);

        // PIXELATION
        IntegerField _pixelationField = new IntegerField(label: "Pixelation");
        _pixelationField.value = 8;
        _pixelationField.isReadOnly = !_pixelatedToggle.value;
        _pixelationField.visible = _pixelatedToggle.value;
        _pixelationField.name = "Pixelation";
        rootVisualElement.Add(_pixelationField);
        _pixelatedToggle.RegisterValueChangedCallback(UpdatePixelationFieldVisibility);

        // SPEED
        IntegerField _speedField = new IntegerField(label: "Speed");
        _speedField.value = 3;
        rootVisualElement.Add(_speedField);

        // NEAR POINT
        IntegerField _nearPointField = new IntegerField("Near Point");
        _nearPointField.value = 1;
        rootVisualElement.Add(_nearPointField);

        // NEAR COLOR
        ColorField _nearColorField = new ColorField("Near Color");
        _nearColorField.value = Color.red;
        rootVisualElement.Add(_nearColorField);

        // FAR COLOR
        ColorField _farColorField = new ColorField("Far Color");
        _farColorField.value = Color.blue;
        rootVisualElement.Add(_farColorField);

        // could maybe use minmax field and get each end?

        // MIN DISTANCE
        IntegerField _minDistanceField = new IntegerField(label: "Min Distance");
        _minDistanceField.value = 2;
        rootVisualElement.Add(_minDistanceField);

        // MAX DISTANCE
        IntegerField _maxDistanceField = new IntegerField(label: "Max Distance");
        _maxDistanceField.value = 15;
        rootVisualElement.Add(_maxDistanceField);

        // LINE WIDTH
        IntegerField _lineWidthField = new IntegerField(label: "Line Width");
        _lineWidthField.value = 3;
        rootVisualElement.Add(_lineWidthField);

        // HORIZONTAL DISTORTION
        IntegerField _horizontalDistortionField = new IntegerField(label: "Horizontal Distortion");
        _horizontalDistortionField.value = 0;
        rootVisualElement.Add(_horizontalDistortionField);

        // VERTICAL DIRECTION
        EnumField _verticalDirectionField = new EnumField(label: "Vertical Direction", VerticalDirection.Down);
        rootVisualElement.Add(_verticalDirectionField);

        // HORIZONTAL DIRECTION
        EnumField _horizontalDirectionField = new EnumField(label: "Horizontal Direction", HorizontalDirection.Left);
        rootVisualElement.Add(_horizontalDirectionField);

        // GENERATE BUTTON
        Button _generateButton = new Button() { text = "Generate" };
        rootVisualElement.Add(_generateButton);
        _generateButton.clicked += () =>
        {
            BorderProperties borderProperties = new BorderProperties(height: _heightField.value, radius: _radiusField.value,
                pixelated: _pixelatedToggle.value, pixelation: _pixelationField.value,
                speed: _speedField.value, nearColor: _nearColorField.value, farColor: _farColorField.value,
                minDistance: _minDistanceField.value, maxDistance: _maxDistanceField.value, nearPoint: _nearPointField.value,
                lineWidth: _lineWidthField.value, horizontalDistortion: _horizontalDistortionField.value,
                verticalDirection: (VerticalDirection)_verticalDirectionField.value, 
                horizontalDirection: (HorizontalDirection)_horizontalDirectionField.value);
            WorldBorderGenerator.Generate(borderProperties);
        };
    }

    //public void UpdatePixelationFieldVisibility(MouseDownEvent evnt, bool type)
    //{
    // (x) => 
    //}
    void UpdatePixelationFieldVisibility(ChangeEvent<bool> evt)
    {
        Debug.Log(evt.newValue);
        IEnumerable<VisualElement> visualElements = rootVisualElement.hierarchy.Children();
        foreach (VisualElement ve in visualElements)
        {
            if (ve.name == "Pixelation")
            {
                ve.visible = evt.newValue;
            }
        }

    }
    private static EventCallback<ChangeEvent<bool>> OnPixelatedChanged;

}

public enum VerticalDirection
{
    Up,
    Down,
}

public enum HorizontalDirection
{
    Left,
    Right,
}
