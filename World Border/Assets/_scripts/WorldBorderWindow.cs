using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine;

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
        //rootVisualElement.Add(new Label("Hello Teapot"));

        IntegerField _heightField = new IntegerField(label: "Height");
        _heightField.value = 16;
        rootVisualElement.Add(_heightField);
        //_generateButton.clicked += () =>
        //{
        //    WorldBorderGenerator.Generate(10, 5);
        //};

        IntegerField _radiusField = new IntegerField(label: "Height");
        _radiusField.value = 5;
        rootVisualElement.Add(_radiusField);
        //_generateButton.clicked += () =>
        //{
        //    WorldBorderGenerator.Generate(10, 5);
        //};

        Button _generateButton = new Button() { text = "Generate" };
        rootVisualElement.Add(_generateButton);
        _generateButton.clicked += () =>
        {
            WorldBorderGenerator.Generate(_heightField.value, _radiusField.value);
        };
    }
}
