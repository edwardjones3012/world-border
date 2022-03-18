using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class WorldBorderWindow : EditorWindow
{
    private static float _defaultHeight = 12;
    private static float _defaultRadius = 5;
    private static Color _defaultNearColor = Color.red;
    private static Color _defaultFarColor = Color.blue;
    private static float _defaultNearPoint = 1;
    private static float _defaultMinDistance = 2;
    private static float _defaultMaxDistance = 15;
    private static bool _defaultPixelated = false;
    private static float _defaultPixelation = 8;
    private static float _defaultSpeed = 3;
    private static float _defaultHorizontalDistortion = 0;
    private static VerticalDirection _defaultVerticalDirection = VerticalDirection.Down;
    private static HorizontalDirection _defaultHorizontalDirection = HorizontalDirection.Left;
    private static float _defaultLineWidth = 12;
    private static bool _defaultCenterVertical = false;

    private static Dictionary<string, dynamic> defaults = new Dictionary<string, dynamic>()
    {
        { "_height", _defaultHeight },
        { "_radius", _defaultRadius }
    };

    [MenuItem("Tools/ World Border")]
    public static void ShowEditorWindow()
    {
        EditorWindow window = GetWindow<WorldBorderWindow>();
        window.titleContent = new GUIContent("World Border");
        window.maxSize = new Vector2(500, 1000);
    }

    private void CreateFields(CreateType createType)
    {
        rootVisualElement.Clear();
        // HEADER IMAGE
        Image _headerImage = new Image();
        _headerImage.image = (Texture)Resources.Load("world-border-header-placeholder-6");
        _headerImage.scaleMode = ScaleMode.ScaleToFit;
        rootVisualElement.Add(_headerImage);

        // RESET BUTTON
        Button _resetButton = new Button() { text = "Reset Values" };
        rootVisualElement.Add(_resetButton);
        _resetButton.clicked += () =>
        {
            ResetValuesToDefault();
        };

        // CURRENT RESET BUTTON
        Button _currentBorderValuesButton = new Button() { text = "Get Current Border Values" };
        rootVisualElement.Add(_currentBorderValuesButton);
        _currentBorderValuesButton.clicked += () =>
        {
            SetValuesToLastGenerated();
        };

        Label _sizeLabel = new Label(text: "SIZE");
        _sizeLabel.AddToClassList("label_header");
        rootVisualElement.Add(_sizeLabel);


        //HEIGHT
        IntegerField _heightField = new IntegerField(label: "Height");
        _heightField.name = "height";
        _heightField.value = createType == CreateType.Init ? (int)_defaultHeight : (int)PlayerPrefs.GetFloat("height"); 
        rootVisualElement.Add(_heightField);
        VisualElementStyleSheetSet set = _heightField.styleSheets;

        // RADIUS
        IntegerField _radiusField = new IntegerField(label: "Radius");
        _radiusField.value = createType == CreateType.Init ? (int)_defaultRadius : (int)PlayerPrefs.GetFloat("radius");
        _radiusField.name = "radius";
        rootVisualElement.Add(_radiusField);

        Label _colorLabel = new Label(text: "COLOR");
        _colorLabel.AddToClassList("label_header");
        rootVisualElement.Add(_colorLabel);

        // NEAR COLOR
        ColorField _nearColorField = new ColorField("Near Color");
        _nearColorField.value = Color.red;
        _nearColorField.name = "nearColor";
        rootVisualElement.Add(_nearColorField);

        // FAR COLOR
        ColorField _farColorField = new ColorField("Far Color");
        _farColorField.value = Color.blue;
        _farColorField.name = "farColor";
        rootVisualElement.Add(_farColorField);

        Label _distanceLabel = new Label(text: "DISTANCE");
        _distanceLabel.AddToClassList("label_header");
        rootVisualElement.Add(_distanceLabel);

        // NEAR POINT
        IntegerField _nearPointField = new IntegerField("Near Point");
        _nearPointField.value = createType == CreateType.Init ? (int)_defaultNearPoint : (int)PlayerPrefs.GetFloat("nearPoint");
        _nearPointField.name = "nearPoint";
        rootVisualElement.Add(_nearPointField);

        // MIN DISTANCE
        IntegerField _minDistanceField = new IntegerField(label: "Min Distance");
        _minDistanceField.value = 2;
        _minDistanceField.name = "minDistance";
        rootVisualElement.Add(_minDistanceField);

        // MAX DISTANCE
        IntegerField _maxDistanceField = new IntegerField(label: "Max Distance");
        _maxDistanceField.value = 15;
        _maxDistanceField.name = "maxDistance";
        rootVisualElement.Add(_maxDistanceField);

        Label _patternLabel = new Label(text: "PATTERN");
        _patternLabel.AddToClassList("label_header");
        rootVisualElement.Add(_patternLabel);

        // PIXELATED
        Toggle _pixelatedToggle = new Toggle(label: "Pixelated");
        _pixelatedToggle.value = false;
        _pixelatedToggle.name = "pixelated";
        rootVisualElement.Add(_pixelatedToggle);

        // PIXELATION
        IntegerField _pixelationField = new IntegerField(label: "Pixelation");
        _pixelationField.value = 8;
        _pixelationField.visible = _pixelatedToggle.value;
        _pixelationField.name = "pixelation";
        _pixelationField.AddToClassList("unity-integer-field__indent");
        rootVisualElement.Add(_pixelationField);
        _pixelatedToggle.RegisterValueChangedCallback(UpdatePixelationFieldVisibility);

        // LINE WIDTH
        IntegerField _lineWidthField = new IntegerField(label: "Line Width");
        _lineWidthField.value = 3;
        _lineWidthField.name = "lineWidth";
        rootVisualElement.Add(_lineWidthField);

        // HORIZONTAL DISTORTION
        IntegerField _horizontalDistortionField = new IntegerField(label: "Horizontal Distortion");
        _horizontalDistortionField.value = 0;
        _horizontalDistortionField.name = "horizontalDistortion";
        rootVisualElement.Add(_horizontalDistortionField);

        Label _miscLabel = new Label(text: "MISC");
        _miscLabel.AddToClassList("label_header");
        rootVisualElement.Add(_miscLabel);

        // SPEED
        IntegerField _speedField = new IntegerField(label: "Speed");
        _speedField.value = 3;
        _speedField.name = "speed";
        rootVisualElement.Add(_speedField);



        // could maybe use minmax field and get each end?



        // VERTICAL DIRECTION
        EnumField _verticalDirectionField = new EnumField(label: "Vertical Direction", VerticalDirection.Down);
        _verticalDirectionField.name = "verticalDirection";
        rootVisualElement.Add(_verticalDirectionField);

        // HORIZONTAL DIRECTION
        EnumField _horizontalDirectionField = new EnumField(label: "Horizontal Direction", HorizontalDirection.Left);
        _horizontalDirectionField.name = "horizontalDirection";
        rootVisualElement.Add(_horizontalDirectionField);

        // CENTER VERTICAL
        Toggle _centerVertical = new Toggle(label: "Center Vertical");
        _centerVertical.name = "centerVertical";
        _centerVertical.value = false;
        rootVisualElement.Add(_centerVertical);

        // GENERATE BUTTON
        Button _generateButton = new Button() { text = "Generate" };
        rootVisualElement.Add(_generateButton);
        _generateButton.clicked += () =>
        {
            PlayerPrefs.SetFloat("height", _heightField.value);
            PlayerPrefs.SetFloat("radius", _radiusField.value);
            PlayerPrefs.SetFloat("pixelated", _pixelatedToggle.value == false ? 0 : 1);
            PlayerPrefs.SetFloat("pixelation", _pixelationField.value);
            PlayerPrefs.SetFloat("speed", _speedField.value);
            //PlayerPrefs.SetFloat("nearColor", _nearColorField.value); 
            //PlayerPrefs.SetFloat("farColor", _farColorField.value); 
            PlayerPrefs.SetFloat("minDistance", _minDistanceField.value);
            PlayerPrefs.SetFloat("maxDistance", _maxDistanceField.value);
            PlayerPrefs.SetFloat("nearPoint", _nearPointField.value);
            PlayerPrefs.SetFloat("lineWidth", _lineWidthField.value);
            PlayerPrefs.SetFloat("horizontalDistortion", (HorizontalDirection)_horizontalDistortionField.value == HorizontalDirection.Left ? 0 : 1);
            PlayerPrefs.SetFloat("verticalDirection", (VerticalDirection)_verticalDirectionField.value == VerticalDirection.Up ? 0 : 1);
            PlayerPrefs.SetFloat("horizontalDirection", _nearPointField.value);

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

    public void CreateGUI()
    {
        StyleSheet styleSheet = (StyleSheet)EditorGUIUtility.Load("UIEditorStyleSheet.uss");
        rootVisualElement.styleSheets.Add(styleSheet);
        CreateFields(CreateType.Init);
        
    }

    public void ResetValuesToDefault()
    {
        CreateFields(CreateType.Init);
    }

    public void SetValuesToLastGenerated()
    {
        CreateFields(CreateType.Saved);
    }

    void UpdatePixelationFieldVisibility(ChangeEvent<bool> evt)
    {
        IEnumerable<VisualElement> visualElements = rootVisualElement.hierarchy.Children();
        foreach (VisualElement ve in visualElements)
        {
            if (ve.name == "pixelation")
            {
                ve.visible = evt.newValue;
            }
        }

    }
    private static EventCallback<ChangeEvent<bool>> OnPixelatedChanged;

}

public enum CreateType
{
    Init,
    Saved
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
