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
        window.maxSize = new Vector2(500, 1000);
    }

    public void CreateGUI()
    {
        StyleSheet styleSheet = (StyleSheet)EditorGUIUtility.Load("UIEditorStyleSheet.uss");
        rootVisualElement.styleSheets.Add(styleSheet);

        // HEADER IMAGE
        Image _headerImage = new Image();
        _headerImage.image = (Texture)Resources.Load("world-border-header-placeholder-6");
        _headerImage.scaleMode = ScaleMode.ScaleToFit;
        rootVisualElement.Add(_headerImage);

        Label _sizeLabel = new Label(text: "SIZE");
        _sizeLabel.AddToClassList("label_header");
        rootVisualElement.Add(_sizeLabel);

        //HEIGHT
        IntegerField _heightField = new IntegerField(label: "Height");
        _heightField.value = 16;
        rootVisualElement.Add(_heightField);
        VisualElementStyleSheetSet set = _heightField.styleSheets;
        
        // RADIUS
        IntegerField _radiusField = new IntegerField(label: "Radius");
        _radiusField.value = 5;
        rootVisualElement.Add(_radiusField);

        Label _colorLabel = new Label(text: "COLOR");
        _colorLabel.AddToClassList("label_header");
        rootVisualElement.Add(_colorLabel);

        // NEAR COLOR
        ColorField _nearColorField = new ColorField("Near Color");
        _nearColorField.value = Color.red;
        rootVisualElement.Add(_nearColorField);

        // FAR COLOR
        ColorField _farColorField = new ColorField("Far Color");
        _farColorField.value = Color.blue;
        rootVisualElement.Add(_farColorField);

        Label _distanceLabel = new Label(text: "DISTANCE");
        _distanceLabel.AddToClassList("label_header");
        rootVisualElement.Add(_distanceLabel);

        // NEAR POINT
        IntegerField _nearPointField = new IntegerField("Near Point");
        _nearPointField.value = 1;
        rootVisualElement.Add(_nearPointField);

        // MIN DISTANCE
        IntegerField _minDistanceField = new IntegerField(label: "Min Distance");
        _minDistanceField.value = 2;
        rootVisualElement.Add(_minDistanceField);

        // MAX DISTANCE
        IntegerField _maxDistanceField = new IntegerField(label: "Max Distance");
        _maxDistanceField.value = 15;
        rootVisualElement.Add(_maxDistanceField);

        Label _patternLabel = new Label(text: "PATTERN");
        _patternLabel.AddToClassList("label_header");
        rootVisualElement.Add(_patternLabel);

        // PIXELATED
        Toggle _pixelatedToggle = new Toggle(label: "Pixelated");
        _pixelatedToggle.value = false;
        rootVisualElement.Add(_pixelatedToggle);

        // PIXELATION
        IntegerField _pixelationField = new IntegerField(label: "Pixelation");
        _pixelationField.value = 8;
        _pixelationField.visible = _pixelatedToggle.value;
        _pixelationField.name = "Pixelation";
        _pixelationField.AddToClassList("unity-integer-field__indent");
        rootVisualElement.Add(_pixelationField);
        _pixelatedToggle.RegisterValueChangedCallback(UpdatePixelationFieldVisibility);

        // LINE WIDTH
        IntegerField _lineWidthField = new IntegerField(label: "Line Width");
        _lineWidthField.value = 3;
        rootVisualElement.Add(_lineWidthField);

        // HORIZONTAL DISTORTION
        IntegerField _horizontalDistortionField = new IntegerField(label: "Horizontal Distortion");
        _horizontalDistortionField.value = 0;
        rootVisualElement.Add(_horizontalDistortionField);

        Label _miscLabel = new Label(text: "MISC");
        _miscLabel.AddToClassList("label_header");
        rootVisualElement.Add(_miscLabel);

        // SPEED
        IntegerField _speedField = new IntegerField(label: "Speed");
        _speedField.value = 3;
        rootVisualElement.Add(_speedField);



        // could maybe use minmax field and get each end?



        // VERTICAL DIRECTION
        EnumField _verticalDirectionField = new EnumField(label: "Vertical Direction", VerticalDirection.Down);
        rootVisualElement.Add(_verticalDirectionField);

        // HORIZONTAL DIRECTION
        EnumField _horizontalDirectionField = new EnumField(label: "Horizontal Direction", HorizontalDirection.Left);
        rootVisualElement.Add(_horizontalDirectionField);

        // CENTER VERTICAL
        Toggle _centerVertical = new Toggle(label: "Center Vertical");
        _centerVertical.value = false;
        rootVisualElement.Add(_centerVertical);

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

    void UpdatePixelationFieldVisibility(ChangeEvent<bool> evt)
    {
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
