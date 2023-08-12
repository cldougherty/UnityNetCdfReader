using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using AS.HDFql;
using System.IO;
using System.Linq;
using Unity.EditorCoroutines.Editor;
using System.Threading.Tasks;
using System.Text;

public class Co2ToolsEditorWindow : EditorWindow
{
    public List<HdfDatabaseSet> datasetSets = new List<HdfDatabaseSet>();
    public HdfDatabaseSet currentDataset;

    [SerializeField]
    VisualTreeAsset _tree;
    TextField _dataSetFilePathTextField;
    Button _GetHdfFileButton;

    FloatField _SouthBoundingCoordinateFloatField;
    FloatField _NorthBoundingCoordinateFloatField;
    FloatField _WestBoundingCoordinateFloatField;
    FloatField _EastBoundingCoordinateFloatField;
    Button _GetBoundingCoordinatesButton;

    Button _ReadXco2FromHDFButton;

    Label _LongitudeCountLabel;
    Label _LatitudeCountLabel;
    Button _GetLatLonDimensionsButton;
    SliderInt _heatmapResolution;
    Label _datasetFileStatusLabel;
    Button _MakeAndApplyTextureButton;
    ObjectField _WorldMaterialField;
    ObjectField _EarthShellGameObjectField;
    GameObject EarthShell;

    ObjectField _HeatmapObjectField;

    Button _Make3dHeatmapButton;
    ObjectField _PointMaterialField;

    Button _DisplayCurrentFileInfoButton;

    FloatField _CustomMinValueFloatValue;
    FloatField _CustomMaxValueFloatValue;

    ListView m_leftPane;
    [SerializeField] private int m_SelectedIndex = -1;
    private VisualElement m_RightPane;

    public List<string> currentFilesFromFolder;

    [MenuItem("Tools/Co2Tools")]
    public static void ShowEditor()
    {
        var window = GetWindow<Co2ToolsEditorWindow>();
        window.titleContent = new GUIContent(text: "Co2ToolsEditorWindow");
    }

    private void OnEnable()
    {
        //SceneView.duringSceneGui += OnSceneGUI;
    }
    void OnDestroy()
    {
        if (currentDataset != null)
        {
            HDFql.Execute("CLOSE ALL FILE");
        }
        Debug.Log("Destroyed...");
    }

    private void CreateGUI()
    {
        _tree.CloneTree(rootVisualElement);
        InitFields();
        RegisterCallbackOnFields();
        CreateSplitList();
    }
    private void OnGUI()
    {
        if (currentDataset != null)
        {
            _GetBoundingCoordinatesButton.SetEnabled(true);
        }
    }


    [SerializeField]
    VisualTreeAsset m_ItemAsset;

    [SerializeField]
    VisualTreeAsset m_EditorAsset;

    void InitFields()
    {
        _dataSetFilePathTextField = rootVisualElement.Q<TextField>(name: "datasetFilePath");
        _GetHdfFileButton = rootVisualElement.Q<Button>(name: "UseHdfFileFromPathButton");
        _GetHdfFileButton.clickable.clicked += OpenDatabaseButtonPressed;
        _datasetFileStatusLabel = rootVisualElement.Q<Label>(name: "datasetFileStatusLabel");
        _SouthBoundingCoordinateFloatField = rootVisualElement.Q<FloatField>("SouthBoundingCoordinateFloatField");
        _NorthBoundingCoordinateFloatField = rootVisualElement.Q<FloatField>("NorthBoundingCoordinateFloatField");
        _WestBoundingCoordinateFloatField = rootVisualElement.Q<FloatField>("WestBoundingCoordinateFloatField");
        _EastBoundingCoordinateFloatField = rootVisualElement.Q<FloatField>("EastBoundingCoordinateFloatField");
        _LongitudeCountLabel = rootVisualElement.Q<Label>("LongitudeCountLabel");
        _LatitudeCountLabel = rootVisualElement.Q<Label>("LatitudeCountLabel");
        _GetBoundingCoordinatesButton = rootVisualElement.Q<Button>("GetBoundingCoordinatesButton");
        _GetBoundingCoordinatesButton.clickable.clicked += BoundingCoordinatesButtonPressed;
        _GetBoundingCoordinatesButton.SetEnabled(false);
        _heatmapResolution = rootVisualElement.Q<SliderInt>("HeatmapResolutionSlider");

        _WorldMaterialField = rootVisualElement.Q<ObjectField>("WorldMaterialField");
        if (_WorldMaterialField.value == null)
        {
            _WorldMaterialField.value = AssetDatabase.LoadAssetAtPath("Assets/Tools/HdfVisualizer/Materials/EarthOverlayMaterial.mat", typeof(Material));
        }

        _EarthShellGameObjectField = rootVisualElement.Q<ObjectField>("EarthShellGameObjectField");
        if (_EarthShellGameObjectField.value == null)
        {
            _EarthShellGameObjectField.value = AssetDatabase.LoadAssetAtPath("Assets/Tools/HdfVisualizer/Prefabs/WorldMeshFilter.fbx", typeof(GameObject));
            EarthShell = (GameObject)_EarthShellGameObjectField.value;
        }
        _HeatmapObjectField = rootVisualElement.Q<ObjectField>("HeatmapObjectField");
        if (_HeatmapObjectField.value == null)
        {
            _HeatmapObjectField.value = AssetDatabase.LoadAssetAtPath("Assets/Tools/HdfVisualizer/Prefabs/CubeOptimized.fbx", typeof(GameObject));
        }

        _Make3dHeatmapButton = rootVisualElement.Q<Button>("Make3dHeatmapButton");
        _Make3dHeatmapButton.clickable.clicked += Make3dHeatmapButtonClicked;

        _PointMaterialField = rootVisualElement.Q<ObjectField>("PointMaterialField");
        if (_PointMaterialField.value == null)
        {
            _PointMaterialField.value = AssetDatabase.LoadAssetAtPath("Assets/Tools/HdfVisualizer/Materials/pointMaterial.mat", typeof(Material));
        }

        _DisplayCurrentFileInfoButton = rootVisualElement.Q<Button>(name: "DisplayCurrentFileInfoButton");
        _DisplayCurrentFileInfoButton.clickable.clicked += DisplayCurrentFileInfoButtonPressed;

        _CustomMinValueFloatValue = rootVisualElement.Q<FloatField>("_CustomMinValueFloatValue");
        _CustomMaxValueFloatValue = rootVisualElement.Q<FloatField>("_CustomMaxValueFloatValue");

    }

    private void DisplayCurrentFileInfoButtonPressed()
    {
        if (HdfqlTools.CheckValidFile() == true)
        {
            m_RightPane.Clear();
            Label label = new Label();
            List<string> tempAttributes = HdfqlTools.ShowAttributes("/");
            StringBuilder stringBuilder = new StringBuilder();
            label.text = "";
            for (int i = 0; i < tempAttributes.Count; i++)
            {
                string rowValue = HdfqlTools.GetHdfAttributeValueBasedOnType("", tempAttributes[i]);
                if (!string.IsNullOrEmpty(rowValue))
                {
                    stringBuilder.AppendLine(tempAttributes[i] + ": " + rowValue);
                }
                Debug.Log(rowValue);
            }
            string output = stringBuilder.ToString();
            Debug.Log(output.Length);
            label.text = output;
            m_RightPane.Add(label);
        }
    }

    public void Make3dHeatmapButtonClicked()
    {
        Debug.Log("todo: implement again per variable");
        /*if (currentDataset != null)
        {
            if (_PlotVisualizationChoiceRadioGroup.value == 0) //gradient
                Co2Tools.SetUpSpawnLoop(ref currentDataset, gradient: currentDataset.datasetGradient);
            if (_PlotVisualizationChoiceRadioGroup.value == 1)
                Co2Tools.SetUpSpawnLoop(ref currentDataset, colorRamp: currentDataset.datasetColorRamp);
        }*/
    }

    public void RegisterCallbackOnFields()//makes sure that fields update the c# value when changed
    {
        _heatmapResolution.RegisterCallback<ChangeEvent<int>>((evt) =>
        {
            _heatmapResolution.value = evt.newValue;
            if (currentDataset != null)
            {
                currentDataset.heatMapResolution = (int)evt.newValue;
            }
        });

        _WorldMaterialField.RegisterCallback<ChangeEvent<UnityEngine.Object>>((evt) =>
        {
            _WorldMaterialField.value = evt.newValue;
            if (currentDataset != null)
            {
                currentDataset.co2HeatMapMaterial = (Material)evt.newValue;
            }
        });
        _PointMaterialField.RegisterCallback<ChangeEvent<UnityEngine.Object>>((evt) =>
        {
            _PointMaterialField.value = evt.newValue;
            if (currentDataset != null)
            {
                currentDataset.pointMaterial = (Material)evt.newValue;
            }
        });

        _EarthShellGameObjectField.RegisterCallback<ChangeEvent<UnityEngine.Object>>((evt) =>
        {
            _EarthShellGameObjectField.value = evt.newValue;
            if (currentDataset != null)
            {
                EarthShell = (GameObject)evt.newValue;
            }
        });
        _HeatmapObjectField.RegisterCallback<ChangeEvent<UnityEngine.Object>>((evt) =>
        {
            _HeatmapObjectField.value = evt.newValue;
            if (currentDataset != null)
            {
                currentDataset.co2PointModel = (GameObject)evt.newValue;
            }
        });
    }
    public void MakeAndApplyTextureButtonPressed()
    {
        Debug.Log("use per variable instead");
        //var texture = Co2Tools.SaveTextureToAssetsFolder(currentDataset.generatedHeatmap);
        //Debug.Log(texture.name);
        //Co2Tools.CreateWorldShell(EarthShell, texture, currentDataset.co2HeatMapMaterial);
    }
    //Co2Tools.ApplyTextureToMaterial(currentDataset.generatedCo2Heatmap, currentDataset.co2HeatMapMaterial);

    private void ReadXco2FromHDFButtonPressed()
    {
    }
    private void SaveVariableArrayUsingName(string variableName, VariableToPlot plot)
    {
        if (currentDataset != null)
        {
            if (HdfqlTools.CheckDataType(variableName) == HDFql.Float)
            {//for now i convert everything to doubles since the texture is the thing that is important, also i could not figure out generics
                Debug.Log(HdfqlTools.CheckDataType(variableName + "/_FillValue"));

                float missingVal = -9999;
                if (HdfqlTools.CheckDataType(variableName + "/_FillValue") == HDFql.Float)
                    float.TryParse(HdfqlTools.GetHdfAttributeValueBasedOnType(variableName, "_FillValue"), out missingVal);
                plot.dataset2dArray = new double[currentDataset.latLegend.Length, currentDataset.lonLegend.Length];
                float[,,] testFloatArray = new float[2,currentDataset.latLegend.Length, currentDataset.lonLegend.Length];
                HdfqlTools.SaveTransientVariableFromHdfToUnity(variableName, testFloatArray);
                //plot.dataset2dArray = (double)testFloatArray;
                for (int x = 0; x < testFloatArray.GetLength(1); x++)
                {
                    for (int y = 0; y < testFloatArray.GetLength(2); y++)
                    {
                        if (testFloatArray[0, x, y] == missingVal)
                        {
                            plot.dataset2dArray[x, y] = Double.PositiveInfinity;
                        }
                        else
                        {
                            plot.dataset2dArray[x, y] = testFloatArray[0, x, y];
                        }
                    }
                }
            }
            
            else if (HdfqlTools.CheckDataType(variableName) == HDFql.Double)
            {
                plot.dataset2dArray = new double[currentDataset.latLegend.Length, currentDataset.lonLegend.Length];
                HdfqlTools.SaveTransientVariableFromHdfToUnity(variableName, plot.dataset2dArray);
            }
            else if (HdfqlTools.CheckDataType(variableName) == HDFql.Smallint)
            {
                short missingVal = -9999;
                if (HdfqlTools.CheckDataType(variableName + "/_FillValue") == HDFql.Smallint)
                   short.TryParse(HdfqlTools.GetHdfAttributeValueBasedOnType(variableName, "_FillValue"), out missingVal);
                Debug.Log(missingVal);
                Debug.Log(currentDataset.latLegend.Length + " " + currentDataset.lonLegend.Length);
                if (currentDataset.latLegend.Length > 10000 || currentDataset.lonLegend.Length > 10000)
                {
                    plot.dataset2dArray = new double[currentDataset.latLegend.Length/4, currentDataset.lonLegend.Length/4];
                    Debug.Log("test array");
                    Debug.Log("Max Texture size for unity/gpu: " + SystemInfo.maxTextureSize);
                    short[,,] testShortArray = new short[1,currentDataset.latLegend.Length, currentDataset.lonLegend.Length];
                    HdfqlTools.SaveTransientVariableFromHdfToUnity(variableName, testShortArray);
                    Debug.Log(testShortArray[0,0, 0]);
                    Debug.Log(currentDataset.latLegend.Length / 4 + " " + currentDataset.lonLegend.Length / 4);
                    
                    int dim1 = testShortArray.GetLength(1);
                    int dim2 = testShortArray.GetLength(2);
                    Debug.Log(dim1 + " " + dim2);
                    for (int x = 0; x <dim1-4; x+=4)
                    {
                        Debug.Log(x);
                        for (int y = 0; y < dim2; y+=4)
                        {
                            if (testShortArray[0,x, y] == missingVal)
                            {
                                plot.dataset2dArray[x/4, y/4] = Double.PositiveInfinity;
                            }
                            else
                            {
                                plot.dataset2dArray[x/4, y/4] = testShortArray[0,x, y];
                            }
                        }
                    }
                }
            }

            else
            {
                Debug.LogWarning("unsupported data type for 2d array, only floats,doubles,smallints");
            }
            Co2Tools.GetMinMaxFrom2dArray(ref plot.dataset2dArray, plot.missingValue, out plot.minValueInDataset, out plot.maxValueInDataset);
        }
    }

    void OpenDatabaseButtonPressed()
    {
        datasetSets.Clear();
        string folderpath = EditorUtility.OpenFolderPanel("Load data", "", "");
        if (folderpath.Length > 0)
        {
            currentFilesFromFolder = Directory.GetFiles(folderpath).ToList<string>();
            for (int i = currentFilesFromFolder.Count - 1; i >= 0; i--)
            {
                currentFilesFromFolder[i] = currentFilesFromFolder[i].Replace("\\", "/");
                if (!currentFilesFromFolder[i].EndsWith(".nc") && !currentFilesFromFolder[i].EndsWith(".nc4") && !currentFilesFromFolder[i].EndsWith(".h5"))
                {
                    //currentFilesFromFolder.IndexOf(file)
                    currentFilesFromFolder.Remove(currentFilesFromFolder[i]);
                }
            }
            foreach (string file in currentFilesFromFolder)
            {
                if (file.EndsWith(".nc") || file.EndsWith(".nc4") || file.EndsWith(".h5"))
                {
                    string curPath = file;
                    if (curPath.Contains(':'))
                    {
                        curPath = curPath.Split(':')[1];
                        //filePathAfterSystemDir;
                    }
                    if (curPath.Length != 0)
                    {
                        _dataSetFilePathTextField.value = curPath;
                    }
                    bool openFile = HdfqlTools.UseFileFromPath(curPath); // opens the file
                    if (openFile == true) //
                    {
                        _datasetFileStatusLabel.text = "File Status: " + datasetSets.Count() + " Files loaded";
                        HdfDatabaseSet curFile = new HdfDatabaseSet();
                        //curFile = datasets[0];
                        curFile.path = curPath;
                        int dirSeperatorIndex = curPath.LastIndexOf('/');//after last dir
                        curFile.fileName = curPath[(dirSeperatorIndex + 1)..];
                        int fileExtSeperatorIndex = curFile.fileName.LastIndexOf('.');//File extension
                        curFile.fileName = curFile.fileName[..fileExtSeperatorIndex];
                        curFile.title = "";
                        if (HdfqlTools.CheckIfExists("ATTRIBUTE", "title"))
                        {
                            curFile.title = HdfqlTools.GetHdfAttributeValueBasedOnType("", "title");
                        }
                        else if (HdfqlTools.CheckIfExists("ATTRIBUTE", "Title"))
                        {
                            curFile.title = HdfqlTools.GetHdfAttributeValueBasedOnType("", "Title");
                        }
                        curFile.shortName = "";
                        if (HdfqlTools.CheckIfExists("ATTRIBUTE", "ShortName"))//todo: also check for short_name
                        {
                            curFile.shortName = HdfqlTools.GetHdfAttributeValueBasedOnType("", "ShortName");
                        }
                        else if (HdfqlTools.CheckIfExists("ATTRIBUTE", "shortName"))
                        {
                            curFile.shortName = HdfqlTools.GetHdfAttributeValueBasedOnType("", "shortName");
                        }
                        else if (HdfqlTools.CheckIfExists("ATTRIBUTE", "short_name"))
                        {
                            curFile.shortName = HdfqlTools.GetHdfAttributeValueBasedOnType("", "short_name");
                        }
                        curFile.longName = "";
                        if (HdfqlTools.CheckIfExists("ATTRIBUTE", "LongName"))
                        {
                            curFile.longName = HdfqlTools.GetHdfAttributeValueBasedOnType("", "LongName");
                        }
                        else if (HdfqlTools.CheckIfExists("ATTRIBUTE", "longName"))
                        {
                            curFile.longName = HdfqlTools.GetHdfAttributeValueBasedOnType("", "longName");
                        }
                        else if (HdfqlTools.CheckIfExists("ATTRIBUTE", "long_name"))
                        {
                            curFile.longName = HdfqlTools.GetHdfAttributeValueBasedOnType("", "long_name");
                        }

                        if (_WorldMaterialField.value != null)
                            curFile.co2HeatMapMaterial = (Material)_WorldMaterialField.value;
                        curFile.co2PointModel = (GameObject)_HeatmapObjectField.value;
                        curFile.pointMaterial = (Material)_PointMaterialField.value;
                        curFile.heatMapResolution = (int)_heatmapResolution.value;
                        if (datasetSets.Count == 0)
                        {
                            currentDataset = curFile;
                            AddToSplitList();
                        }
                        datasetSets.Add(curFile);
                        HDFql.Execute("CLOSE FILE");
                    }
                    else
                    {
                        _datasetFileStatusLabel.text = "File Status: error";
                    }
                }
            }
            OpenFirstFileInList();
        }
    }
    void OpenFirstFileInList()
    {
        if (currentFilesFromFolder.Count > 0)
        {
            string firstFile = currentFilesFromFolder[0]; //todo: fix error if empty
            if (firstFile.Contains(':'))
            {
                firstFile = firstFile.Split(':')[1];
            }
            bool openFirstFile = HdfqlTools.UseFileFromPath(firstFile); //make sure we open first file to get variables from later.
            GetCfConventions();//TEST
        }
    }
    private void LatLonDimensionsButtonPressed()
    {
        Co2Tools.SaveLatLongHeaderInfo(out currentDataset.latLegend, out currentDataset.lonLegend);
        _LatitudeCountLabel.text = "Lat Count:" + currentDataset.latLegend.Length;
        _LongitudeCountLabel.text = "Long Count:" + currentDataset.lonLegend.Length;
    }
    void BoundingCoordinatesButtonPressed()
    {
        if (currentDataset != null)
        {
            currentDataset.bounds = Co2Tools.GetBoundingCoordinatesFromHDF();
        }
    }
    public TwoPaneSplitView splitView;
    public void CreateSplitList()
    {
        // Create a two-pane view with the left pane being fixed with
        splitView = new TwoPaneSplitView(0, 250, TwoPaneSplitViewOrientation.Horizontal);
        // Add the panel to the visual tree by adding it as a child to the root element
        rootVisualElement.Add(splitView);
        // A TwoPaneSplitView always needs exactly two child elements
        m_leftPane = new ListView();
        splitView.Add(m_leftPane);
        m_RightPane = new ScrollView(ScrollViewMode.VerticalAndHorizontal);
        splitView.Add(m_RightPane);
    }

    void AddToSplitList()
    {
        List<string> datasets = HdfqlTools.ReturnAllDatasets();
        // Initialize the list view with all sprites' names
        m_leftPane.makeItem = () => new Label();
        // leftPane.bindItem = (item, index) => { (item as Label).text = allObjects[index].name; };
        m_leftPane.bindItem = (item, index) => { (item as Label).text = datasets[index]; };
        //leftPane.itemsSource = allObjects;
        m_leftPane.itemsSource = datasets;
        // React to the user's selection
        m_leftPane.onSelectionChange += OnSpriteSelectionChange;
        // Restore the selection index from before the hot reload
        m_leftPane.selectedIndex = m_SelectedIndex;
        // Store the selection index when the selection changes
        m_leftPane.onSelectionChange += (items) => { m_SelectedIndex = m_leftPane.selectedIndex; };
       splitView.fixedPaneInitialDimension = 399;//repainting does not work for me so change size to force update.
    }

    private void OnSpriteSelectionChange(IEnumerable<object> selectedItems)
    {
        m_RightPane.Clear();// Clear all previous content from the pane
        var selectedString = selectedItems.First() as string;
        if (selectedString == null)
            return;
        List<int> dimList = HdfqlTools.GetDimensionsFromVariable(selectedString);
        Label label = new Label();
        for (int i = 0; i < dimList.Count; i++)
        {
            label.text += "\n Dimension " + i + ": " + dimList[i];
        }
        Button button = new Button();
        button.text = "Create Plot from " + selectedString;

        RadioButtonGroup plotchoice = new RadioButtonGroup("Options", new List<string> { "Gradient", "Colormap" });
        plotchoice.value = 1;
        plotchoice.RegisterValueChangedCallback(evt => Debug.Log(evt.newValue));
        m_RightPane.Add(plotchoice);
        ObjectField _Colorramp = new ObjectField();
        _Colorramp.objectType = typeof(UnityEngine.Texture2D);
        GradientField _GradientField = new GradientField();
        Gradient grad = new Gradient();
        //_heatmapResolution.value = evt.newValue;
        grad.colorKeys = new GradientColorKey[]
        {
        new GradientColorKey(new Color(0,0.008405924f,1,1), 0),
        new GradientColorKey(new Color(0.3647059f,0.69803923f,1,1), 0.2f),
        new GradientColorKey(new Color(0.8941177f,0.9960785f, 0.68235296f,1), 0.4f),
        new GradientColorKey(new Color(1f,0.9215687f, 0.011764707f,1), 0.5f),
        new GradientColorKey(new Color(1,0.9215687f, 0.011764707f,1), 0.6f),
        new GradientColorKey(new Color(1,0.21568629f, 0,1), 0.8f),
        new GradientColorKey(new Color(1,0,0,1), 1f)
        };

        List<string> tempAttributes = HdfqlTools.ShowAttributes(selectedString + "/");
        for (int i = 0; i < tempAttributes.Count; i++)
        {
            Label labelAtt = new Label();
            labelAtt.text += tempAttributes[i] + ": " + HdfqlTools.GetHdfAttributeValueBasedOnType(selectedString, tempAttributes[i]);
            m_RightPane.Add(labelAtt);
        }

        _GradientField.RegisterCallback<ChangeEvent<Gradient>>((evt) =>
        {
            grad = _GradientField.value;
        });
        _Colorramp.value = AssetDatabase.LoadAssetAtPath("Assets/Textures/co2Table.png", typeof(Texture2D));
        _GradientField.value = grad;
        m_RightPane.Add(label);
        m_RightPane.Add(_Colorramp);
        m_RightPane.Add(_GradientField);
        button.RegisterCallback<ClickEvent>(e => PlotVariableButtonHandlerAsync(selectedString, plotchoice.value, colorRamp: (Texture2D)_Colorramp.value, gradient: grad));
        m_RightPane.Add(button);
    }

    private async Task PlotVariableButtonHandlerAsync(string variableNameToPlot, int plotChoiceValue, Texture2D colorRamp = null, Gradient gradient = null)
    {
        HdfSet myTestSet = CreateScriptableObjectFromDatabaseSet(ref currentDataset);
        string path = EditorUtility.SaveFolderPanel("Save textures to folder", "", "");
        HDFql.Execute("CLOSE FILE");
        for (int i = 0; i < currentFilesFromFolder.Count; i++)
        {
            string currentFile = currentFilesFromFolder[i];
            if (currentFile.Contains(':'))
            {
                currentFile = currentFile.Split(':')[1];
            }
            bool openFile = HdfqlTools.UseFileFromPath(currentFile); //actually opens file
            if (openFile)
            {
                VariableToPlot plot = new VariableToPlot();
                plot.name = variableNameToPlot;
                if (HdfqlTools.CheckIfExists("ATTRIBUTE", "/" + "_FillValue"))
                {
                    if (HdfqlTools.CheckDataType(variableNameToPlot + "/" + "_FillValue") == HDFql.Double)
                        plot.missingValue = Double.Parse(HdfqlTools.GetHdfAttributeValueBasedOnType(variableNameToPlot, "_FillValue"));
                    else if (HdfqlTools.CheckDataType(variableNameToPlot + "/" + "_FillValue") == HDFql.Float)
                        plot.missingValue = float.Parse(HdfqlTools.GetHdfAttributeValueBasedOnType(variableNameToPlot, "_FillValue"));
                    else if (HdfqlTools.CheckDataType(variableNameToPlot + "/" + "_FillValue") == HDFql.Smallint)
                        plot.missingValue = short.Parse(HdfqlTools.GetHdfAttributeValueBasedOnType(variableNameToPlot, "_FillValue"));
                }
                plot.longName = HdfqlTools.GetHdfAttributeValueBasedOnType(variableNameToPlot, "long_name");
                plot.units = HdfqlTools.GetHdfAttributeValueBasedOnType(variableNameToPlot, "units");
                if (HdfqlTools.CheckIfExists(variableNameToPlot, "/" + "cell_methods"))
                {
                    plot.cellMethods = HdfqlTools.GetHdfAttributeValueBasedOnType(variableNameToPlot, "cell_methods");
                }
                if (i == 0)
                {
                    Co2Tools.SaveLatLongHeaderInfo(out currentDataset.latLegend, out currentDataset.lonLegend);
                    currentDataset.bounds = Co2Tools.GetBoundingCoordinatesFromHDF();
                }
                SaveVariableArrayUsingName(variableNameToPlot, plot); //saves the 2d array to a double in plot
                if (plotChoiceValue == 0)//gradient
                {
                    plot.generatedHeatmap = Co2Tools.MakeTexture(ref currentDataset, plot, gradientValue: gradient);
                    plot.gradient = gradient;
                }
                else if (plotChoiceValue == 1)//colorramp
                {
                    if (_CustomMinValueFloatValue.value != Mathf.Infinity && _CustomMaxValueFloatValue.value != Mathf.Infinity)
                    {
                        plot.customMax = _CustomMaxValueFloatValue.value;
                        plot.customMin = _CustomMinValueFloatValue.value;
                        plot.generatedHeatmap = Co2Tools.MakeTexture(ref currentDataset, plot, colorRampValue: colorRamp, customMin: (double)_CustomMinValueFloatValue.value, customMax: (double)_CustomMaxValueFloatValue.value);
    }
                    else if (_CustomMinValueFloatValue.value != Mathf.Infinity)
                    {
                        plot.customMin = _CustomMinValueFloatValue.value;
                        plot.generatedHeatmap = Co2Tools.MakeTexture(ref currentDataset, plot, colorRampValue: colorRamp, customMin: (double)_CustomMinValueFloatValue.value);
                    }
                    else if (_CustomMaxValueFloatValue.value != Mathf.Infinity)
                    {
                        plot.customMax = _CustomMaxValueFloatValue.value;
                        plot.generatedHeatmap = Co2Tools.MakeTexture(ref currentDataset, plot, colorRampValue: colorRamp, customMax: (double)_CustomMaxValueFloatValue.value);
                    }
                    else
                    {
                        plot.generatedHeatmap = Co2Tools.MakeTexture(ref currentDataset, plot, colorRampValue: colorRamp);
                    }
                        plot.colorramp = colorRamp;
                }
                Texture2D texture = Co2Tools.SaveTexturesToAssetsFolder(plot.generatedHeatmap, path, datasetSets[i].fileName, plot /*,currentDataset*/);
                plot.generatedHeatmap = texture;
                // Co2Tools.CreateWorldShell(EarthShell, currentDataset.generatedCo2Heatmap, currentDataset.co2HeatMapMaterial);
                //if (texture)
                  //  Co2Tools.CreateWorldShell(EarthShell, texture, currentDataset.co2HeatMapMaterial);

                datasetSets[i].variablesToPlot.Add(plot);

                HDFql.Execute("CLOSE FILE");
            }
        }
        //myTestSet.hdfFiles = datasetSets;
        for (int i = 0; i < datasetSets.Count; i++)
        {
            myTestSet.hdfFiles.Add(datasetSets[i]);
        }
        AssetDatabase.Refresh();
        OpenFirstFileInList();
        EditorUtility.SetDirty(myTestSet);
        //AssetDatabase.SaveAssets();
        AssetDatabase.SaveAssetIfDirty(myTestSet);
    }

    public void GetCfConventions()// many climateForcast Conventions exist, only look for a few for now
    {
        float dx;
        if (HdfqlTools.CheckIfExists("ATTRIBUTE", "DX"))
        {
            float.TryParse((HdfqlTools.GetHdfAttributeValueBasedOnType("", "DX")), out dx);
            currentDataset.DX = dx;
            Debug.Log(dx);
        }

        if (HdfqlTools.CheckIfExists("ATTRIBUTE", "DY"))
        {
            float dy;
            float.TryParse((HdfqlTools.GetHdfAttributeValueBasedOnType("", "DY")), out dy);
            currentDataset.DY = dy;
            Debug.Log(dy);
        }

        float SOUTH_WEST_CORNER_LAT = 0;
        if (HdfqlTools.CheckIfExists("ATTRIBUTE", "SOUTH_WEST_CORNER_LAT"))
        {
            float.TryParse((HdfqlTools.GetHdfAttributeValueBasedOnType("", "SOUTH_WEST_CORNER_LAT")), out SOUTH_WEST_CORNER_LAT);
            //SOUTH_WEST_CORNER_LAT = float.Parse(HdfqlTools.GetHdfAttributeValueBasedOnType("", "SOUTH_WEST_CORNER_LAT"));
            Debug.Log(SOUTH_WEST_CORNER_LAT);
        }
        float SOUTH_WEST_CORNER_LON = 0;
        if (HdfqlTools.CheckIfExists("ATTRIBUTE", "SOUTH_WEST_CORNER_LON"))
        {
            
            float.TryParse((HdfqlTools.GetHdfAttributeValueBasedOnType("", "SOUTH_WEST_CORNER_LON")), out SOUTH_WEST_CORNER_LON);
            Debug.Log(SOUTH_WEST_CORNER_LON);
        }
        currentDataset.SOUTH_WEST_CORNER_LAT = SOUTH_WEST_CORNER_LAT;
        currentDataset.SOUTH_WEST_CORNER_LON = SOUTH_WEST_CORNER_LON;
    }

    HdfSet CreateScriptableObjectFromDatabaseSet(ref HdfDatabaseSet file)
    {
        HdfSet example = ScriptableObject.CreateInstance<HdfSet>();
        //example.hdfFiles
        // path has to start at "Assets"
        //Assets/Tools/HdfVisualizer/
        string chosenName;
        if (file.longName.Length > 0)
            chosenName = file.longName;
        else if (file.title.Length > 0)
            chosenName = file.title;
        else if (file.shortName.Length > 0)
            chosenName = file.shortName;
        else if (file.fileName.Length > 0)
            chosenName = file.fileName;
        else
        {
            Debug.LogWarning("cant choose name for scriptable object, no title/name");
            chosenName = "DefaultName";
        }
        string path = "Assets/Tools/HdfVisualizer/" +chosenName + ".asset";

        bool assetExists = AssetDatabase.GetMainAssetTypeAtPath(path) != null;

        Debug.Log("scriptable object already exists?" + assetExists);
        if (assetExists)
        {
            Debug.LogWarning("scriptable Object:  already exists");
            HdfSet existingObject = AssetDatabase.LoadAssetAtPath(path, typeof(HdfSet)) as HdfSet;
            return existingObject;
        }
        else
        {
            AssetDatabase.CreateAsset(example, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = example;
        }
        return example;
    }
}