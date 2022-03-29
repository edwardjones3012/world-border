using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class WorldBorderWindow : EditorWindow
{
    private static float _defaultHeight = 12;
    private static float _defaultRadius = 5;
    private static Color _defaultNearColor = new Color(1, 0.1367925f, 0.1367925f, 1);
    private static Color _defaultFarColor = new Color(0.1933962f, 0.1933962f, 1, 1);
    private static float _defaultNearPoint = 1;
    private static float _defaultMinDistance = 2;
    private static float _defaultMaxDistance = 15;
    private static float _defaultPixelated = 0;
    private static float _defaultPixelation = 8;
    private static float _defaultSpeed = 3;
    private static float _defaultHorizontalDistortion = 0;
    private static VerticalDirection _defaultVerticalDirection = VerticalDirection.Down;
    private static HorizontalDirection _defaultHorizontalDirection = HorizontalDirection.Left;
    private static float _defaultLineWidth = 3;
    private static float _defaultCenterVertical = 1;

    [MenuItem("Tools/ World Border Editor")]
    public static void ShowEditorWindow()
    {
        EditorWindow window = GetWindow<WorldBorderWindow>();
        window.titleContent = new GUIContent("World Border");
        window.maxSize = new Vector2(500, 1000);
    }

    private void CreateFields(CreateType createType, bool save = true)
    {
        rootVisualElement.Clear();
        // HEADER IMAGE
        Image _headerImage = new Image();
        _headerImage.image = (Texture)Resources.Load("world-border-header-placeholder-6");
        _headerImage.scaleMode = ScaleMode.ScaleToFit;
        rootVisualElement.Add(_headerImage);

        // RESET BUTTON
        Button _resetButton = new Button() { text = "Reset" };
        rootVisualElement.Add(_resetButton);
        _resetButton.AddToClassList("button_top");
        _resetButton.clicked += () =>
        {
            ResetValuesToDefault();
        };

        // CURRENT RESET BUTTON
        Button _currentBorderValuesButton = new Button() { text = "Restore Saved Values" };
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
        _heightField.value = createType == CreateType.Default ? (int)_defaultHeight : (int)PlayerPrefs.GetFloat("height");
        _heightField.RegisterValueChangedCallback(OnHeightChange);
        rootVisualElement.Add(_heightField);
        VisualElementStyleSheetSet set = _heightField.styleSheets;

        // RADIUS
        IntegerField _radiusField = new IntegerField(label: "Radius");
        _radiusField.name = "radius";
        _radiusField.value = createType == CreateType.Default ? (int)_defaultRadius : (int)PlayerPrefs.GetFloat("radius");
        _radiusField.RegisterValueChangedCallback(OnRadiusChange);
        rootVisualElement.Add(_radiusField);

        Label _colorLabel = new Label(text: "COLOR");
        _colorLabel.AddToClassList("label_header");
        rootVisualElement.Add(_colorLabel);

        // NEAR COLOR
        ColorField _nearColorField = new ColorField("Near Color");
        _nearColorField.value = createType == CreateType.Default ? _defaultNearColor : ConvertToColor(PlayerPrefs.GetString("nearColor"));
        _nearColorField.name = "nearColor";
        _nearColorField.RegisterValueChangedCallback(OnNearColorChange);
        rootVisualElement.Add(_nearColorField);

        // FAR COLOR
        ColorField _farColorField = new ColorField("Far Color");
        _farColorField.value = createType == CreateType.Default ? _defaultFarColor : ConvertToColor(PlayerPrefs.GetString("farColor"));
        _farColorField.name = "farColor";
        _farColorField.RegisterValueChangedCallback(OnFarColorChange);
        rootVisualElement.Add(_farColorField);

        Label _distanceLabel = new Label(text: "DISTANCE");
        _distanceLabel.AddToClassList("label_header");
        rootVisualElement.Add(_distanceLabel);

        // NEAR POINT
        IntegerField _nearPointField = new IntegerField("Near Point");
        _nearPointField.value = createType == CreateType.Default ? (int)_defaultNearPoint : (int)PlayerPrefs.GetFloat("nearPoint");
        _nearPointField.name = "nearPoint";
        _nearPointField.RegisterValueChangedCallback(OnNearPointChange);
        rootVisualElement.Add(_nearPointField);

        // MIN DISTANCE
        IntegerField _minDistanceField = new IntegerField(label: "Min Distance");
        _minDistanceField.value = createType == CreateType.Default ? (int)_defaultMinDistance : (int)PlayerPrefs.GetFloat("minDistance");
        _minDistanceField.name = "minDistance";
        _minDistanceField.RegisterValueChangedCallback(OnMinDistanceChange);
        rootVisualElement.Add(_minDistanceField);

        // MAX DISTANCE
        IntegerField _maxDistanceField = new IntegerField(label: "Max Distance");
        _maxDistanceField.value = createType == CreateType.Default ? (int)_defaultMaxDistance : (int)PlayerPrefs.GetFloat("maxDistance");
        _maxDistanceField.name = "maxDistance";
        _maxDistanceField.RegisterValueChangedCallback(OnMaxDistanceChange);
        rootVisualElement.Add(_maxDistanceField);

        Label _patternLabel = new Label(text: "PATTERN");
        _patternLabel.AddToClassList("label_header");
        rootVisualElement.Add(_patternLabel);

        // PIXELATED
        Toggle _pixelatedToggle = new Toggle(label: "Pixelated");
        _pixelatedToggle.value = createType == CreateType.Default ? GetBoolFromFloat(_defaultPixelated) : GetBoolFromFloat(PlayerPrefs.GetFloat("pixelated"));
        _pixelatedToggle.name = "pixelated";
        _pixelatedToggle.RegisterValueChangedCallback(OnPixelatedChange);
        rootVisualElement.Add(_pixelatedToggle);

        // PIXELATION
        IntegerField _pixelationField = new IntegerField(label: "Pixelation");
        _pixelationField.value = createType == CreateType.Default ? (int)_defaultPixelation : (int)PlayerPrefs.GetFloat("pixelation");
        _pixelationField.visible = _pixelatedToggle.value;
        _pixelationField.name = "pixelation";
        _pixelationField.AddToClassList("unity-integer-field__indent");
        _pixelationField.RegisterValueChangedCallback(OnPixelationChange);

        rootVisualElement.Add(_pixelationField);

        // LINE WIDTH
        SliderInt _lineWidthField = new SliderInt(label: "Line Width", start: 1, end: 9);
        _lineWidthField.value = createType == CreateType.Default ? (int)_defaultLineWidth : (int)PlayerPrefs.GetFloat("lineWidth");
        _lineWidthField.name = "lineWidth";
        _lineWidthField.RegisterValueChangedCallback(OnLineWidthChange);
        rootVisualElement.Add(_lineWidthField);

        // HORIZONTAL DISTORTION
        IntegerField _horizontalDistortionField = new IntegerField(label: "Horizontal Distortion");
        _horizontalDistortionField.value = createType == CreateType.Default ? (int)_defaultHorizontalDistortion : (int)PlayerPrefs.GetFloat("horizontalDistortion");
        _horizontalDistortionField.name = "horizontalDistortion";
        rootVisualElement.Add(_horizontalDistortionField);
        _horizontalDistortionField.RegisterValueChangedCallback(OnHorizontalDistortionChange);

        Label _miscLabel = new Label(text: "MISC");
        _miscLabel.AddToClassList("label_header");
        rootVisualElement.Add(_miscLabel);

        // SPEED
        IntegerField _speedField = new IntegerField(label: "Speed");
        _speedField.value = createType == CreateType.Default ? (int)_defaultSpeed : (int)PlayerPrefs.GetFloat("speed");
        _speedField.name = "speed";
        _speedField.RegisterValueChangedCallback(OnSpeedChange);
        rootVisualElement.Add(_speedField);

        // could maybe use minmax field and get each end?

        // VERTICAL DIRECTION
        EnumField _verticalDirectionField = new EnumField(label: "Vertical Direction", VerticalDirection.Down);
        _verticalDirectionField.name = "verticalDirection";
        _verticalDirectionField.value = createType == CreateType.Default ? _defaultVerticalDirection : (VerticalDirection)PlayerPrefs.GetFloat("verticalDirection");
        _verticalDirectionField.RegisterValueChangedCallback(OnVerticalDirectionChange);

        rootVisualElement.Add(_verticalDirectionField);

        // HORIZONTAL DIRECTION
        EnumField _horizontalDirectionField = new EnumField(label: "Horizontal Direction", HorizontalDirection.Left);
        _horizontalDirectionField.name = "horizontalDirection";
        _horizontalDirectionField.value = createType == CreateType.Default ? _defaultHorizontalDirection : (HorizontalDirection)PlayerPrefs.GetFloat("horizontalDirection");
        _horizontalDirectionField.RegisterValueChangedCallback(OnHorizontalDirectionChange);

        rootVisualElement.Add(_horizontalDirectionField);

        // CENTER VERTICAL
        Toggle _centerVerticalToggle = new Toggle(label: "Center Vertical");
        _centerVerticalToggle.name = "centerVertical";
        _centerVerticalToggle.value = createType == CreateType.Default ? GetBoolFromFloat(_defaultCenterVertical) : GetBoolFromFloat(PlayerPrefs.GetFloat("centerVertical"));
        _centerVerticalToggle.RegisterValueChangedCallback(OnVerticalCenterChange);
        rootVisualElement.Add(_centerVerticalToggle);



        // GENERATE BUTTON
        Button _generateButton = new Button() { text = "Save" };
        rootVisualElement.Add(_generateButton);
        _generateButton.AddToClassList("button_top");
        _generateButton.clicked += () =>
        {
            if (save)
            {
                PlayerPrefs.SetFloat("height", _heightField.value);
                PlayerPrefs.SetFloat("tempHeight", _heightField.value);
                PlayerPrefs.SetFloat("radius", _radiusField.value);
                PlayerPrefs.SetFloat("tempRadius", _radiusField.value);
                PlayerPrefs.SetFloat("pixelated", _pixelatedToggle.value == false ? 0 : 1);
                PlayerPrefs.SetFloat("tempPixelated", _pixelatedToggle.value == false ? 0 : 1);

                PlayerPrefs.SetFloat("pixelation", _pixelationField.value);
                PlayerPrefs.SetFloat("tempPixelation", _pixelationField.value);

                PlayerPrefs.SetFloat("speed", _speedField.value);
                PlayerPrefs.SetFloat("tempSpeed", _speedField.value);

                PlayerPrefs.SetString("nearColor", ExtractRGBA(_nearColorField.value));
                PlayerPrefs.SetString("tempNearColor", ExtractRGBA(_nearColorField.value));

                PlayerPrefs.SetString("farColor", ExtractRGBA(_farColorField.value));
                PlayerPrefs.SetString("tempFarColor", ExtractRGBA(_farColorField.value));

                PlayerPrefs.SetFloat("minDistance", _minDistanceField.value);
                PlayerPrefs.SetFloat("tempMinDistance", _minDistanceField.value);

                PlayerPrefs.SetFloat("maxDistance", _maxDistanceField.value);
                PlayerPrefs.SetFloat("tempMaxDistance", _maxDistanceField.value);

                PlayerPrefs.SetFloat("nearPoint", _nearPointField.value);
                PlayerPrefs.SetFloat("tempNearPoint", _nearPointField.value);

                PlayerPrefs.SetFloat("lineWidth", _lineWidthField.value);
                PlayerPrefs.SetFloat("tempLineWidth", _lineWidthField.value);

                PlayerPrefs.SetFloat("horizontalDistortion", _horizontalDistortionField.value);
                PlayerPrefs.SetFloat("tempHorizontalDistortion", _horizontalDistortionField.value);

                PlayerPrefs.SetFloat("verticalDirection", (VerticalDirection)_verticalDirectionField.value == VerticalDirection.Up ? 0 : 1);
                PlayerPrefs.SetFloat("tempVerticalDirection", (VerticalDirection)_verticalDirectionField.value == VerticalDirection.Up ? 0 : 1);

                PlayerPrefs.SetFloat("horizontalDirection", (HorizontalDirection)_horizontalDistortionField.value == HorizontalDirection.Left ? 0 : 1);
                PlayerPrefs.SetFloat("tempHorizontalDirection", (HorizontalDirection)_horizontalDistortionField.value == HorizontalDirection.Left ? 0 : 1);

                PlayerPrefs.SetFloat("centerVertical", _centerVerticalToggle.value == false ? 0 : 1);
                PlayerPrefs.SetFloat("tempCenterVertical", _centerVerticalToggle.value == false ? 0 : 1);
            }

            BorderProperties borderProperties = new BorderProperties(height: _heightField.value, radius: _radiusField.value,
                pixelated: _pixelatedToggle.value, pixelation: _pixelationField.value,
                speed: _speedField.value, nearColor: _nearColorField.value, farColor: _farColorField.value,
                minDistance: _minDistanceField.value, maxDistance: _maxDistanceField.value, nearPoint: _nearPointField.value,
                lineWidth: _lineWidthField.value, horizontalDistortion: _horizontalDistortionField.value,
                verticalDirection: (VerticalDirection)_verticalDirectionField.value,
                horizontalDirection: (HorizontalDirection)_horizontalDirectionField.value,
                centerVertical: _centerVerticalToggle.value);
            WorldBorderGenerator.Generate(borderProperties);
        };
    }

    private void GenerateFromTemporary()
    {
        float height = PlayerPrefs.GetFloat("tempHeight", _defaultHeight);
        float radius = PlayerPrefs.GetFloat("tempRadius", _defaultRadius);
        float pixelated = PlayerPrefs.GetFloat("tempPixelated", _defaultPixelated);
        float pixelation = PlayerPrefs.GetFloat("tempPixelation", _defaultPixelation);
        float speed = PlayerPrefs.GetFloat("tempSpeed", _defaultSpeed);
        string nearColor = PlayerPrefs.GetString("tempNearColor", "1-0-0-0");
        string farColor = PlayerPrefs.GetString("tempFarColor", "0-0-1-0");
        float minDistance = PlayerPrefs.GetFloat("tempMinDistance", _defaultMinDistance);
        float maxDistance = PlayerPrefs.GetFloat("tempMaxDistance", _defaultMaxDistance);
        float nearPoint = PlayerPrefs.GetFloat("tempNearPoint", _defaultNearPoint);
        float lineWidth = PlayerPrefs.GetFloat("tempLineWidth", _defaultLineWidth);
        float horizontalDistortion = PlayerPrefs.GetFloat("tempHorizontalDistortion", _defaultHorizontalDistortion);
        VerticalDirection verticalDirection = (VerticalDirection)PlayerPrefs.GetFloat("tempVerticalDirection", 0);
        HorizontalDirection horizontalDirection = (HorizontalDirection)PlayerPrefs.GetFloat("tempHorizontalDirection", 0);
        float centerVertical = PlayerPrefs.GetFloat("tempCenterVertical", _defaultCenterVertical);

        BorderProperties borderProperties = new BorderProperties(height: height, radius: radius, pixelated: pixelated == 1, pixelation: (int)pixelation, speed: (int)speed, nearColor: ConvertToColor(nearColor),
            farColor: ConvertToColor(farColor), minDistance: minDistance, maxDistance: maxDistance, lineWidth: (int)lineWidth, nearPoint: (int)nearPoint,
            horizontalDistortion: (int)horizontalDistortion, verticalDirection: verticalDirection, horizontalDirection: horizontalDirection, centerVertical: centerVertical == 0 ? false : true);
        WorldBorderGenerator.Generate(borderProperties);
    }

    private bool GetBoolFromFloat(float val)
    {
        return val == 1;
    }

    private string ExtractRGBA(Color col)
    {
        string color = col.r.ToString() + "-" + col.g.ToString() + "-" + col.b.ToString() + "-" + col.a.ToString() + "-";
        return color;
    }

    private Color ConvertToColor(string rgba)
    {
        string[] splitRGBA = rgba.Split('-');
        string r = splitRGBA[0].ToString();
        float.TryParse(r, out var red);

        string g = splitRGBA[1].ToString();
        float.TryParse(g, out var green);

        string b = splitRGBA[2].ToString();
        float.TryParse(b, out var blue);

        string a = splitRGBA[3].ToString();
        float.TryParse(a, out var alpha);

        return new Color(red, green, blue, alpha);
    }

    public void CreateGUI()
    {
        StyleSheet styleSheet = (StyleSheet)EditorGUIUtility.Load("UIEditorStyleSheet.uss");
        rootVisualElement.styleSheets.Add(styleSheet);
        CreateFields(CreateType.Default);
    }

    public void ResetValuesToDefault()
    {
        CreateFields(CreateType.Default);
        UpdateAllTemporaryValues();
    }

    public void SetValuesToLastGenerated()
    {
        CreateFields(CreateType.Saved);
        UpdateAllTemporaryValues();
    }

    #region Update Fields
    void OnPixelatedChange(ChangeEvent<bool> evt)
    {
        PlayerPrefs.SetFloat("tempPixelated", evt.newValue ? 1 : 0);

        IEnumerable<VisualElement> visualElements = rootVisualElement.hierarchy.Children();
        foreach (VisualElement ve in visualElements)
        {
            if (ve.name == "pixelation")
            {
                ve.visible = evt.newValue;
            }
        }
        if (evt.newValue == true) WorldBorderGenerator.UpdateShader("Boundary Pixelated");
        else WorldBorderGenerator.UpdateShader("Boundary");
        GenerateFromTemporary();
    }

    private VisualElement FindVisualElement(string name)
    {
        IEnumerable<VisualElement> visualElements = rootVisualElement.hierarchy.Children();
        foreach (VisualElement ve in visualElements)
        {
            if (ve.name == name)
            {
                return ve;
            }
        }
        return null;
    }

    private void UpdateAllTemporaryValues()
    {
        UpdateTemporaryValueSliderInt("lineWidth", "tempLineWidth");

        UpdateTemporaryValueInteger("height", "tempHeight");
        UpdateTemporaryValueInteger("radius", "tempRadius");
        UpdateTemporaryValueInteger("nearPoint", "tempNearPoint");
        UpdateTemporaryValueInteger("minDistance", "tempMinDistance");
        UpdateTemporaryValueInteger("maxDistance", "tempMaxDistance");
        UpdateTemporaryValueInteger("pixelation", "tempPixelation");
        UpdateTemporaryValueInteger("horizontalDistortion", "tempHorizontalDistortion");
        UpdateTemporaryValueInteger("speed", "tempSpeed");

        UpdateTemporaryValueColor("nearColor", "tempNearColor");
        UpdateTemporaryValueColor("farColor", "tempFarColor");

        UpdateTemporaryValueToggle("centerVertical", "tempCenterVertical");
        UpdateTemporaryValueToggle("pixelated", "tempPixelated");

        GenerateFromTemporary();
    }

    private void UpdateTemporaryValueInteger(string name, string tempName)
    {
        VisualElement element = FindVisualElement(name);
        IntegerField field = (IntegerField)element;

        PlayerPrefs.SetFloat(tempName, field.value);
    }

    private void UpdateTemporaryValueColor(string name, string tempName)
    {
        VisualElement element = FindVisualElement(name);
        ColorField field = (ColorField)element;
        PlayerPrefs.SetString(tempName, ExtractRGBA(field.value));
    }

    private void UpdateTemporaryValueSliderInt(string name, string tempName)
    {
        VisualElement element = FindVisualElement(name);
        SliderInt field = (SliderInt)element;

        PlayerPrefs.SetFloat(tempName, field.value);
    }

    private void UpdateTemporaryValueToggle(string name, string tempName)
    {
        VisualElement element = FindVisualElement(name);
        Toggle field = (Toggle)element;

        PlayerPrefs.SetFloat(tempName, field.value ? 1:0);
    }

    void OnLineWidthChange(ChangeEvent<int> evt)
    {
        PlayerPrefs.SetFloat("tempLineWidth", evt.newValue);

        WorldBorderGenerator.UpdateFloat("_LineWidth", evt.newValue);
    }

    void OnPixelationChange(ChangeEvent<int> evt)
    {
        PlayerPrefs.SetFloat("tempPixelation", evt.newValue);

        WorldBorderGenerator.UpdateFloat("_Pixelation", evt.newValue);
    }

    void OnVerticalCenterChange(ChangeEvent<bool> evt)
    {
        PlayerPrefs.SetFloat("tempCenterVertical", evt.newValue?1:0);

        WorldBorderGenerator.UpdateCenteredPosition(evt.newValue);
    }
    void OnHeightChange(ChangeEvent<int> evt)
    {
        int value = evt.newValue;

        if (evt.newValue < 1)
        {
            value = 1;
        }

        PlayerPrefs.SetFloat("tempHeight", value);
        GenerateFromTemporary();
    }

    void OnRadiusChange(ChangeEvent<int> evt)
    {
        int value = evt.newValue;

        if (evt.newValue < 1)
        {
            value = 1;
        }
        PlayerPrefs.SetFloat("tempRadius", value);
        GenerateFromTemporary();
    }

    void OnSpeedChange(ChangeEvent<int> evt)
    {
        int value = evt.newValue;

        if (evt.newValue < 0)
        {
            value = 0;
        }
        PlayerPrefs.SetFloat("tempSpeed", value);
        GenerateFromTemporary();
    }

    void OnMinDistanceChange(ChangeEvent<int> evt)
    {
        int value = evt.newValue;

        if (evt.newValue < 1)
        {
            value = 1;
        }
        PlayerPrefs.SetFloat("tempMinDistance", value);
        GenerateFromTemporary();
    }
    void OnMaxDistanceChange(ChangeEvent<int> evt)
    {
        int value = evt.newValue;

        if (evt.newValue < 1)
        {
            value = 1;
        }
        PlayerPrefs.SetFloat("tempMaxDistance", value);
        GenerateFromTemporary();
    }
    void OnNearPointChange(ChangeEvent<int> evt)
    {
        int value = evt.newValue;

        if (evt.newValue < 1)
        {
            value = 1;
        }
        PlayerPrefs.SetFloat("tempNearPoint", value);
        GenerateFromTemporary();
    }

    void OnHorizontalDistortionChange(ChangeEvent<int> evt)
    {
        int value = evt.newValue;

        if (evt.newValue < 1)
        {
            value = 1;
        }
        PlayerPrefs.SetFloat("tempHorizontalDistortion", value);
        GenerateFromTemporary();
    }
    
    void OnVerticalDirectionChange(ChangeEvent<System.Enum> evt)
    {
        var value = (VerticalDirection)evt.newValue;
        PlayerPrefs.SetFloat("tempVerticalDirection", (float)value);
        GenerateFromTemporary();
    }

    void OnHorizontalDirectionChange(ChangeEvent<System.Enum> evt)
    {
        var value = (HorizontalDirection)evt.newValue;
        PlayerPrefs.SetFloat("tempHorizontalDirection", (float)value);
        GenerateFromTemporary();
    }

    void OnCenterVerticalChange(ChangeEvent<int> evt)
    {
        int value = evt.newValue;

        if (evt.newValue < 1)
        {
            value = 1;
        }
        PlayerPrefs.SetFloat("tempCenterVertical", value);
        GenerateFromTemporary();
    }

    void OnNearColorChange(ChangeEvent<Color> evt)
    {
        Color col = evt.newValue;
        string c = ExtractRGBA(col);
        PlayerPrefs.SetString("tempNearColor", c);
        GenerateFromTemporary();
    }

    void OnFarColorChange(ChangeEvent<Color> evt)
    {
        Color col = evt.newValue;
        string c = ExtractRGBA(col);
        PlayerPrefs.SetString("tempFarColor", c);
        GenerateFromTemporary();
    }
    #endregion
}

public enum CreateType
{
    Default,
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
