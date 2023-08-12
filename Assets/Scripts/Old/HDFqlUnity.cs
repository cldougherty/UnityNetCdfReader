using AS.HDFql;
using UnityEngine;
//using System.Reflection;
//using System.Runtime.InteropServices;
[System.Serializable]
public class HDFqlUnity : MonoBehaviour // 1st eteration when project was runtime
{
    //double[,] data;
    public double[] latLegend;
    public double[] lonLegend;
    double maxValueInDataset;
    double minValueInDataset;
    public float SouthBoundingCoordinate;
    public float NorthBoundingCoordinate;
    public float WestBoundingCoordinate;
    public float EastBoundingCoordinate;

    public Texture2D datasetColorRamp;
    public Material co2HeatMapMaterial;
    public Texture2D generatedCo2Heatmap;
    public Gradient datasetGradient;
    public GameObject co2PointModel;
    public Material pointMaterial;
    [System.Serializable]
    public struct DoubleArr
    {
        public double[] array;
    }

    [System.Serializable]
    public struct DoubleDoubleArr
    {
        public DoubleArr[] array; //DONT CHANGE PLZ :v
        public readonly int width;
        public readonly int height;
        public double this[int x, int y]
        {
            get { return array[x].array[y]; }
            set { array[x].array[y] = value; }
        }

        public DoubleDoubleArr(int width, int height)
        {
            this.width = width;
            this.height = height;
            array = new DoubleArr[width];
            for (int i = 0; i < width; i++)
            {
                array[i].array = new double[height];
            }
        }
    }
    [SerializeField]
    DoubleDoubleArr xco2FromDataset;
    private Renderer _renderer;
    private MaterialPropertyBlock _propBlock;
    
    void Awake()
    {
        _propBlock = new MaterialPropertyBlock();
    }
    /// <summary>
    /// Saves a variable from the HDF and stores it in a given Unity Object
    /// </summary>
    /// <param name="varName"></param>
    /// <param name=""></param>

    void Start()//old code
    {
       HdfqlTools.UseFileFromPath();
        //CheckValidFile();
        //SetDirectoryAndFileName();
        SaveLatLongHeaderInfo();
        GetXco2FromHDF();
        GetBoundingCoordinatesFromHDF();
        SpawnLatLongObectsOnSphere();
        //MoveObjectToPosition();
        HDFql.Execute("CLOSE FILE");
        MakeTexture();
    }

    void SetDirectoryAndFileName()
    {
        string File_Directory = "C:\\Users\\Name\\Project\\";
        //HDFql.Execute("USE DIRECTORY " + "\"" + File_Directory + "\"");
        HDFql.Execute("SHOW USE DIRECTORY");
        while (HDFql.CursorNext() == HDFql.Success) // display content of default cursor
        {
            Debug.Log(HDFql.CursorGetChar());
        }
        string File_Name = "oco2_GEOS_L3CO2_month_201503_B10206Ar.nc4";
        Debug.Log("Using HDF file at: " + "\"" + File_Directory + File_Name);
        string filePath = "C:\\Users\\Name\\Project\\oco2_GEOS_L3CO2_month_201503_B10206Ar.nc4";
        string useFileCommand = "USE READONLY FILE " + filePath + ";";
        HDFql.Execute(useFileCommand);
        HDFql.Execute("SHOW USE FILE ");
        while (HDFql.CursorNext() == HDFql.Success) // display content of default cursor
        {
            Debug.Log(HDFql.CursorGetChar());
        }
    }

    void SaveLatLongHeaderInfo()
    {
        latLegend = HdfqlTools.Save1DDoubleArrayBasedOnCalculatedCount("lat");
        lonLegend =HdfqlTools.Save1DDoubleArrayBasedOnCalculatedCount("lon");
    }
    
    
    void GetBoundingCoordinatesFromHDF()
    {
        float.TryParse(HdfqlTools.GetHDFqlString("SouthBoundingCoordinate"), out SouthBoundingCoordinate);
        float.TryParse(HdfqlTools.GetHDFqlString("NorthBoundingCoordinate"), out NorthBoundingCoordinate);
        float.TryParse(HdfqlTools.GetHDFqlString("EastBoundingCoordinate"), out EastBoundingCoordinate);
        float.TryParse(HdfqlTools.GetHDFqlString("WestBoundingCoordinate"), out WestBoundingCoordinate);
    }

    void GetXco2FromHDF()
    {
        double[,] data = new double[latLegend.Length,lonLegend.Length];
        int x, y;
       HdfqlTools.SaveTransientVariableFromHdfToUnity("XCO2", data);
        HDFql.Execute("SELECT FROM XCO2 INTO MEMORY " + HDFql.VariableTransientRegister(data));
        minValueInDataset = double.MaxValue;
        maxValueInDataset = double.MinValue;
        xco2FromDataset = new DoubleDoubleArr(latLegend.Length, lonLegend.Length);
        for (x = 0; x < latLegend.Length; x++)
        {
            for (y = 0; y < lonLegend.Length; y++)
            {
                if (data[x, y] < minValueInDataset)
                    minValueInDataset = data[x, y];
                else if (data[x, y] > maxValueInDataset)
                    maxValueInDataset = data[x, y];
                xco2FromDataset[x, y] = data[x, y];
            }
        }
        Debug.Log("min: " + minValueInDataset + "max: " + maxValueInDataset);
    }

    void SpawnLatLongObectsOnSphere()
    {
        GameObject tempObject;
        Renderer tempRenderer;
        int divider =4;
        for (int x = 0; x < latLegend.Length; x++)
        {
            for (int y = 0; y < lonLegend.Length; y++)
            {
                if (x % divider == 0 && y % divider == 0)
                {
                    if (lonLegend[y] >= WestBoundingCoordinate && lonLegend[y] <= EastBoundingCoordinate && (latLegend[x] >= SouthBoundingCoordinate && latLegend[x] <= NorthBoundingCoordinate))
                    {
                        Vector3 pointPosition = CoordinateSystem.CoordinateToPoint(new Coordinate((float)lonLegend[y] * Mathf.Deg2Rad, (float)latLegend[x] * Mathf.Deg2Rad), 1);
                        Vector3 pointVector = pointPosition - Vector3.zero;
                        tempObject = Instantiate(co2PointModel, pointPosition, Quaternion.LookRotation(pointVector), transform);
                        tempRenderer = tempObject.GetComponent<Renderer>();
                        tempRenderer.material = pointMaterial;
                        tempRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                        float Xco2Clamped = Mathf.InverseLerp((float)minValueInDataset, (float)maxValueInDataset, (float)xco2FromDataset[x, y]);
                        tempRenderer.material.color = datasetGradient.Evaluate(Xco2Clamped);//for gradient
                        tempObject.transform.localScale = new Vector3(0.01f * divider, 0.01f * divider, Xco2Clamped / 2);
                        tempObject.name = (float)lonLegend[y] + " " + (float)latLegend[x] + " " + Xco2Clamped;
                    }
                }
            }
        }
    }

    void MakeTexture()
    {
        generatedCo2Heatmap = new Texture2D(lonLegend.Length, latLegend.Length);
        for (int x = 0; x < latLegend.Length; x++)
        {
            for (int y = 0; y < lonLegend.Length; y++)
            {
                float Xco2Clamped = Mathf.InverseLerp((float)minValueInDataset, (float)maxValueInDataset, (float)xco2FromDataset[x, y]);
                generatedCo2Heatmap.SetPixel(y, x, datasetGradient.Evaluate(Xco2Clamped));
            }
        }
        generatedCo2Heatmap.Apply();
        GameObject duplicatedSphere = new GameObject("CO2 Heatmap Overlay");
       MeshFilter MF = duplicatedSphere.AddComponent<MeshFilter>();
        MF.mesh = World.instance.gameObject.GetComponent<MeshFilter>().mesh;
        MeshRenderer MR = duplicatedSphere.AddComponent<MeshRenderer>();
        duplicatedSphere.GetComponent<Renderer>().material = co2HeatMapMaterial;
        duplicatedSphere.GetComponent<Renderer>().material.mainTexture = generatedCo2Heatmap;
        duplicatedSphere.GetComponent<Renderer>().material.color = new Color(1,1,1,0.8f);
        duplicatedSphere.transform.parent = World.instance.transform;
        duplicatedSphere.transform.localScale *= 1.01f;
        duplicatedSphere.transform.eulerAngles =new Vector3(0,180,0);
    }

    void oldCode()//example from HDFQL website
    {
        //HDFql.Execute("USE DIRECTORY " + "\"" + File_Directory + "\"");
        //HDFql.Execute("USE FILE " + File_Directory +  File_Name);
        //HDFql.Execute("SHOW USE FILE");
        //HDFql.Execute("USE FILE data.h5");
        // HDFql.Execute("USE DIRECTORY");
        //HDFql.CursorUse(null);
        //HDFql.Execute("SHOW ATTRIBUTE /");
        //HDFql.Execute("SHOW / LIKE **");
        //HDFql.Execute("SHOW DATASET / LIKE **");
        //SHOW LIKE ** //ALL OBJECTS RECURSIVLY FROM current group.
        //HDFql.Execute("SHOW DATASET / LIKE ** ");
        //HDFql.Execute("SELECT FROM XCO2");
        //SHOW ATTRIBUTE group2 LIKE **/1|3 //show all attributes recursively starting from group2 that contain 'i' or '3' 
        //HDFql.Execute("SELECT FROM " + File_Directory + File_Name +  "/"); // select (i.e. read) dataset "mydata" from file "myfile.h5" and populate default cursor with it

        /*HDFql.Execute("SELECT FROM XCO2");
        HDFql.Execute("SHOW ATTRIBUTE /");
        while (HDFql.CursorNext() == HDFql.Success)
        {
            Debug.Log(counter + ": " + HDFql.CursorGetDouble());
            counter++;
        }*/

        // HDFql.Execute("SHOW DATASET LIKE * */ ^temperature WHERE DATA TYPE == FLOAT");
        //HDFql.Execute("SHOW DATASET LIKE * */ ^XCO2 WHERE DATA TYPE == DOUBLE");
        /* while (HDFql.CursorNext() == HDFql.Success)
        {
            HDFql.Execute("SELECT FROM my_dataset(2, 3, 5)");
            Debug.Log(HDFql.CursorGetDouble());
            Debug.Log("Dataset found: " + HDFql.CursorGetChar());
            //System.Console.WriteLine("Dataset found: { 0}", HDFql.CursorGetChar());
            HDFql.Execute("SELECT FROM " + HDFql.CursorGetChar() + " ORDER ASC INTO UNIX FILE output.txt SPLIT 1");
        }*/
        //HDFql.Execute("SHOW DATASET LIKE * */ ^lat WHERE DATA TYPE == FLOAT");
        /*while (HDFql.CursorNext() == HDFql.Success)
        {
            Debug.Log("Dataset found: " + HDFql.CursorGetChar());
            System.Console.WriteLine("Dataset found: { 0}", HDFql.CursorGetChar());
            HDFql.Execute("SELECT FROM " + HDFql.CursorGetChar() + " ORDER ASC INTO UNIX FILE output.txt SPLIT 1");
        }*/
        //Debug.Log(HDFql.CursorGetChar());
        /*HDFql.Execute("SHOW DATASET / LIKE **");
        while (HDFql.CursorNext() == HDFql.Success)
        {
            Debug.Log(HDFql.CursorGetChar());
        }*/
        HDFql.Execute("SHOW USE DIRECTORY");
        while (HDFql.CursorNext() == HDFql.Success) // display content of default cursor
        {
            Debug.Log(HDFql.CursorGetChar());
        }
        HDFql.Execute("SHOW DIRECTORY / LIKE **");
        while (HDFql.CursorNext() == HDFql.Success) // display content of default cursor
        {
            Debug.Log(HDFql.CursorGetChar());
        }
    }
}