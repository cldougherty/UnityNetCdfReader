using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using System.IO;
using System.Text;

public class AdobeColorTableConverterEditorWindow : EditorWindow
{
    Button _LoadActFilesButton;
    Label _ActFilesLoadedLabel;
    Button _SaveActFileAsTextureButton;

    Color32[] colors;


    [MenuItem("Tools/AdobeColorTableConverterEditorWindow")]
    public static void ShowExample()
    {
        AdobeColorTableConverterEditorWindow wnd = GetWindow<AdobeColorTableConverterEditorWindow>();
        wnd.titleContent = new GUIContent("AdobeColorTableConverterEditorWindow");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // VisualElements objects can contain other VisualElement following a tree hierarchy.
        //VisualElement label = new Label("Hello World! From C#");
        //root.Add(label);

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Tools/AdobeColorTableConverter/AdobeColorTableConverterEditorWindow.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Tools/AdobeColorTableConverter/AdobeColorTableConverterEditorWindow.uss");
       // VisualElement labelWithStyle = new Label("Hello World! With Style");
        //labelWithStyle.styleSheets.Add(styleSheet);
        //root.Add(labelWithStyle);
        InitFields();
    }

    void InitFields()
    {
        _LoadActFilesButton = rootVisualElement.Q<Button>("LoadActFilesButton");
        _LoadActFilesButton.clickable.clicked += LoadActFilesButtonClicked;
        _SaveActFileAsTextureButton = rootVisualElement.Q<Button>("SaveActFileAsTextureButton");
        _SaveActFileAsTextureButton.clickable.clicked += SaveActFileAsTextureButtonClicked;
        _ActFilesLoadedLabel = rootVisualElement.Q<Label>("ActFilesLoadedLabel");

    }
    private void LoadActFilesButtonClicked()
    {
        ColorRampConversionTools.ConvertFolderOfActFilesToTextures();
        //colors = ColorRampConversionTools.LoadColorsFromActFile();
    }
    private void SaveActFileAsTextureButtonClicked()
    {
       Texture2D tex = ColorRampConversionTools.SaveColorArrayAsColorRamp(colors);
        Co2Tools.SaveTextureToAssetsFolder(tex);
    }

   
}