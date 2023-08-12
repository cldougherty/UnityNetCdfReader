using AS.HDFql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public static class Co2Tools
{
    public static DataSetTypes.BoundingCoordinates GetBoundingCoordinatesFromHDF()
    {
        DataSetTypes.BoundingCoordinates bounds = new DataSetTypes.BoundingCoordinates();
        bounds.SouthBoundingCoordinate = -90f;//default values
        bounds.NorthBoundingCoordinate = 90f;
        bounds.WestBoundingCoordinate = -180f;
        bounds.EastBoundingCoordinate = 179.375f;

        float tempFloat = Mathf.Infinity;
        if (HdfqlTools.CheckIfExists("ATTRIBUTE", "SouthBoundingCoordinate"))
        float.TryParse(HdfqlTools.GetHDFqlString("SouthBoundingCoordinate"), out tempFloat);
        if (tempFloat != Mathf.Infinity)
        bounds.SouthBoundingCoordinate = (double)tempFloat;
        if (HdfqlTools.CheckIfExists("ATTRIBUTE", "NorthBoundingCoordinate"))
            float.TryParse(HdfqlTools.GetHDFqlString("NorthBoundingCoordinate"), out tempFloat);
        if (tempFloat != Mathf.Infinity)
            bounds.NorthBoundingCoordinate = (double)tempFloat;
        if (HdfqlTools.CheckIfExists("ATTRIBUTE", "EastBoundingCoordinate"))
            float.TryParse(HdfqlTools.GetHDFqlString("EastBoundingCoordinate"), out tempFloat);
        if (tempFloat != Mathf.Infinity)
            bounds.EastBoundingCoordinate = (double)tempFloat;
        if (HdfqlTools.CheckIfExists("ATTRIBUTE", "WestBoundingCoordinate"))
            float.TryParse(HdfqlTools.GetHDFqlString("WestBoundingCoordinate"), out tempFloat);
        if (tempFloat != Mathf.Infinity)
            bounds.WestBoundingCoordinate = (double)tempFloat;

        if (HdfqlTools.CheckIfExists("ATTRIBUTE", "geospatial_lat_min"))
            float.TryParse(HdfqlTools.GetHdfAttributeValueBasedOnType("", "geospatial_lat_min"), out tempFloat);
        if (tempFloat != Mathf.Infinity)
            bounds.SouthBoundingCoordinate = (double)tempFloat;
        if (HdfqlTools.CheckIfExists("ATTRIBUTE", "geospatial_lat_max"))
            float.TryParse(HdfqlTools.GetHdfAttributeValueBasedOnType("","geospatial_lat_max"), out tempFloat);
        if (tempFloat != Mathf.Infinity)
            bounds.NorthBoundingCoordinate = (double)tempFloat;

        if (HdfqlTools.CheckIfExists("ATTRIBUTE", "geospatial_lon_min"))
            float.TryParse(HdfqlTools.GetHdfAttributeValueBasedOnType("", "geospatial_lon_min"), out tempFloat);
        if (tempFloat != Mathf.Infinity)
            bounds.WestBoundingCoordinate = (double)tempFloat;
        
        if (HdfqlTools.CheckIfExists("ATTRIBUTE", "geospatial_lon_max"))
            float.TryParse(HdfqlTools.GetHdfAttributeValueBasedOnType("", "geospatial_lon_max"), out tempFloat);
        if (tempFloat != Mathf.Infinity)
            bounds.EastBoundingCoordinate = (double)tempFloat;
        Debug.Log("geospatial_lat_min " + bounds.SouthBoundingCoordinate);
        return bounds;
    }

    public static void SaveLatLongHeaderInfo(out double[] latLegendOut, out double[] lonLegendOut)
    {
        if (HdfqlTools.CheckIfExists("DATASET", "lat"))
        {
            if (HdfqlTools.CheckDataType("lat") == HDFql.Double)
            {
                latLegendOut = HdfqlTools.Save1DDoubleArrayBasedOnCalculatedCount("lat");
                Debug.Log("latLegendOut: " + latLegendOut.Length + " 1st value: " + latLegendOut[0]);
                lonLegendOut = HdfqlTools.Save1DDoubleArrayBasedOnCalculatedCount("lon");
            }
            else if (HdfqlTools.CheckDataType("lat") == HDFql.Float)
            {
                float[] tempLatArray = HdfqlTools.Save1DFloatArrayBasedOnCalculatedCount("lat");
                latLegendOut = Convert1dFloatArrayToDouble(ref tempLatArray);
                float[] tempLonArray = HdfqlTools.Save1DFloatArrayBasedOnCalculatedCount("lon");
                lonLegendOut = Convert1dFloatArrayToDouble(ref tempLonArray);
                Debug.Log("latLegendOut: " + latLegendOut.Length + " 1st value: " + latLegendOut[0]);
            }
            else
            { 
                int theType = HdfqlTools.CheckDataType("lat");
                Debug.LogError(theType + " cant save lat long header info, not a normal float or double");
                latLegendOut = new double[1];
                lonLegendOut = new double[1];
            }
        }
        else if (HdfqlTools.CheckIfExists("DATASET", "Latitude"))
        {
            if (HdfqlTools.CheckDataType("Latitude") == HDFql.Double)
            {
                latLegendOut = HdfqlTools.Save1DDoubleArrayBasedOnCalculatedCount("Latitude");
                Debug.Log("latLegendOut: " + latLegendOut.Length + " 1st value: " + latLegendOut[0]);
                lonLegendOut = HdfqlTools.Save1DDoubleArrayBasedOnCalculatedCount("Longitude");
            }
            else if (HdfqlTools.CheckDataType("Latitude") == HDFql.Float)
            {
                float[] tempArray = HdfqlTools.Save1DFloatArrayBasedOnCalculatedCount("Latitude");
                latLegendOut = Convert1dFloatArrayToDouble(ref tempArray);
                Debug.Log("latLegendOut: " + latLegendOut.Length + " 1st value: " + latLegendOut[0]);
                tempArray = HdfqlTools.Save1DFloatArrayBasedOnCalculatedCount("Longitude");
                lonLegendOut = Convert1dFloatArrayToDouble(ref tempArray);
                Debug.Log("lonLegendOut: " + lonLegendOut.Length + " 1st value: " + lonLegendOut[0]);
            }
            else
            { //delete later
                int theType = HdfqlTools.CheckDataType("Latitude");
                Debug.LogError(theType + " cant save lat long header info, not a normal float or double");
                latLegendOut = new double[1];
                lonLegendOut = new double[1];
            }
        }
        else
        {
            Debug.LogError("cant save lat long header info");
            latLegendOut = new double[1];
            lonLegendOut = new double[1];
        }
    }

    public static double[] Convert1dFloatArrayToDouble(ref float[] array)
    {
        double[] tempDoubleArray = new double[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            tempDoubleArray[i] = array[i];
        }
        return tempDoubleArray;
    }
    public static void GetMinMaxFrom2dArray(ref double[,] array, double missingValue, out double min, out double max)
    {
        min = double.MaxValue;
        max = double.MinValue;

        int divider =1;
        if (array.GetLength(0) > 10000 || array.GetLength(1) > 10000)
            divider = 5;
        for (int x = 0; x < array.GetLength(0)/divider; x+=divider)
        {
            for (int y = 0; y < array.GetLength(1)/divider; y+= divider)
            {
                if(array[x,y] != double.PositiveInfinity)
                {
                    if (array[x, y] < min)
                        min = array[x, y];
                    else if (array[x, y] > max)
                    {
                        max = array[x, y];
                    }
                }
            }
        }
        Debug.Log("min value: " + min);
        Debug.Log("max value: " + max);
    }

    public static Texture2D MakeTexture(ref HdfDatabaseSet cd,VariableToPlot plot, Texture2D colorRampValue = null, Gradient gradientValue = null, double customMin = Double.PositiveInfinity, double customMax = Double.PositiveInfinity)
    {
        Color TransparentColor = new Color(0, 0, 0, 0);
        int numOfEmptyPixelsBottom = 0; //125;   160 is the total points on the y axis. -80 to 80

        //usually datasets with this will define the lower left of a rectangle  and the upper right of the rectangle is determined by how many values are in the 2d array
        if (cd.SOUTH_WEST_CORNER_LAT != 0 && cd.SOUTH_WEST_CORNER_LON != 0)//method 1
        {
            float a = Math.Abs(-90 - cd.SOUTH_WEST_CORNER_LAT);//how much of the map is missing
            float b = (1 / cd.DY);//DY is the resolution and we are trying to see how many values go into a single longitude/lat
            numOfEmptyPixelsBottom = Math.Abs((int)(a * b));
            //Debug.Log("generating texture. finding " + numOfEmptyPixelsBottom + " pixels to fill in");
        }
        int numOfEmptyPixelsTop = 0;
        //method 2
        //if dataset does not cover part of the bottom/top of the world
        if ((cd.bounds.NorthBoundingCoordinate > 0 && cd.bounds.NorthBoundingCoordinate < 89) && (cd.bounds.SouthBoundingCoordinate > -89 && cd.bounds.SouthBoundingCoordinate < 0))
        {
            Debug.Log(Math.Ceiling(cd.bounds.NorthBoundingCoordinate));
            int halfOfAllLatvalues = (cd.latLegend.Length / 2);
            int totalYWholeNumberValues = (int)Math.Abs(Math.Ceiling(cd.bounds.NorthBoundingCoordinate)) + (int)Math.Abs(Math.Floor(cd.bounds.SouthBoundingCoordinate));
            int missingResolution = 180 - totalYWholeNumberValues;
            numOfEmptyPixelsBottom = (halfOfAllLatvalues / (totalYWholeNumberValues / 2)) * (missingResolution / 2); //125;   160 is the total points on the y axis. -80 to 80

            Debug.Log(halfOfAllLatvalues + " " + totalYWholeNumberValues + " " + missingResolution);
            numOfEmptyPixelsTop = numOfEmptyPixelsBottom;
        }

        Texture2D generatedHeatmap;
        if (cd.lonLegend.Length > 10000)
        generatedHeatmap = new Texture2D(cd.lonLegend.Length/4, cd.latLegend.Length/4 + numOfEmptyPixelsBottom + numOfEmptyPixelsTop);
        else
        generatedHeatmap = new Texture2D(cd.lonLegend.Length, cd.latLegend.Length + numOfEmptyPixelsBottom + numOfEmptyPixelsTop);
        Debug.Log(generatedHeatmap.width +" " + generatedHeatmap.height + " " + (generatedHeatmap.height - numOfEmptyPixelsBottom - numOfEmptyPixelsTop));
        if ((cd.SOUTH_WEST_CORNER_LAT != 0 && cd.SOUTH_WEST_CORNER_LON != 0) || (cd.bounds.NorthBoundingCoordinate > 0 && cd.bounds.NorthBoundingCoordinate < 89)) //method 1
        {
            //Debug.Log("filling in bottom pixels");
            for (int x = 0; x < cd.lonLegend.Length; x++)
            {
                for (int y = 0; y < numOfEmptyPixelsBottom; y++)
                {
                    generatedHeatmap.SetPixel(x, y, TransparentColor);
                }
            }
        }
        if (gradientValue != null)
        {
            //Debug.Log("lat length: " + cd.latLegend.Length);
            //Debug.Log("lon length: " + cd.lonLegend.Length);
        }
        double Xco2Clamped = 0;
        for (int y = 0; y < (generatedHeatmap.height - numOfEmptyPixelsBottom - numOfEmptyPixelsTop); y++)
        {
            for (int x = 0; x < generatedHeatmap.width; x++)
            {
                if(customMin != double.PositiveInfinity && customMax != double.PositiveInfinity)
                        Xco2Clamped = InverseLerp(customMin, customMax, plot.dataset2dArray[y, x]);
                else if (customMin != double.PositiveInfinity)
                    Xco2Clamped = InverseLerp(customMin, plot.maxValueInDataset, plot.dataset2dArray[y, x]);
                else if (customMax != double.PositiveInfinity)
                    Xco2Clamped = InverseLerp(plot.maxValueInDataset,customMax, plot.dataset2dArray[y, x]);
                else
                    Xco2Clamped = InverseLerp(plot.minValueInDataset, plot.maxValueInDataset, plot.dataset2dArray[y, x]);

                    if (plot.dataset2dArray[y, x] != Double.PositiveInfinity)
                    {
                        if (gradientValue != null)
                        {
                            generatedHeatmap.SetPixel(x, y + numOfEmptyPixelsBottom, gradientValue.Evaluate((float)Xco2Clamped));
                        }
                        else if (colorRampValue != null)
                            generatedHeatmap.SetPixel(x, y + numOfEmptyPixelsBottom, colorRampValue.GetPixel((int)(Xco2Clamped * colorRampValue.width), 0));//for colorramp
                    }
                    else
                    {
                    //Debug.Log("missing pixel");
                        generatedHeatmap.SetPixel(x, y + numOfEmptyPixelsBottom, TransparentColor);
                    }
            }
        }
        //fill top empty pixels
        if (cd.SOUTH_WEST_CORNER_LAT != 0 && cd.SOUTH_WEST_CORNER_LON != 0)
        {
            Debug.Log("making texture with cd.SOUTH_WEST_CORNER_LAT");
            for (int x = 0; x < cd.lonLegend.Length; x++) //method 2 
            {
                for (int y = cd.latLegend.Length; y < numOfEmptyPixelsTop; y++)
                {
                    generatedHeatmap.SetPixel(x, y, TransparentColor);
                }
            }
        }

        //Debug.Log("Texture Finished");
        generatedHeatmap.Apply();

        if (cd.bounds.EastBoundingCoordinate > 180)//method 2 - assume we are going from 0- 360 instead of -180 to 180
        {//essentialy whatever was on the right half of the image is is now on the left
            Color[] pixels = generatedHeatmap.GetPixels();
            // Create a new array to store the shifted pixels
            Color[] shiftedPixels = new Color[pixels.Length];
            int shiftAmount = (int)(180 * (2160 / 360));
            // Iterate over the pixels and shift the x-coordinate
            int oldIndex;
            int newIndex;

            for (int i = 0; i < generatedHeatmap.height; i++)
            {
                for (int j = 0; j < generatedHeatmap.width; j++)
                {
                    oldIndex = i * generatedHeatmap.width + j;
                    newIndex = i * generatedHeatmap.width + (j + shiftAmount) % generatedHeatmap.width;
                    shiftedPixels[newIndex] = pixels[oldIndex];
                }
            }
            // Update the texture with the shifted pixels
            generatedHeatmap.SetPixels(shiftedPixels);

            generatedHeatmap.Apply();
        }
        return generatedHeatmap;
    }
    public static Texture2D SaveTextureToAssetsFolder(Texture2D texture, VariableToPlot plot = null, HdfDatabaseSet cd = null, int fileNumber = -1)
    {
        string fileName = "file";
        if (cd != null && plot != null)
            fileName = cd.fileName + "_" + plot.name;// +"_"+ fileNumber.ToString();
        string path = EditorUtility.SaveFilePanelInProject("Save Texture", fileName, "png", "");
        if (path.Length > 0)
        {
            byte[] pngData = texture.EncodeToPNG();
            if (pngData != null)
                File.WriteAllBytes(path, pngData);
            else
                Debug.Log("Could not convert " + texture.name + " to png. Skipping saving texture.");
            Debug.Log(path);
            AssetDatabase.Refresh();
            Texture2D CreatedTextureFromAssets = (Texture2D)AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D));
            return CreatedTextureFromAssets;
        }
        else
        {
            Debug.LogError("something went wrong when creating texture");
            return null;
        }
    }

    public static Texture2D SaveTexturesToAssetsFolder(Texture2D texture,string folderPath,string file, VariableToPlot plot = null, HdfDatabaseSet cd = null, int fileNumber = -1)
    {
        string fileName = "file";
        if (/*cd != null &&*/ plot != null)
            fileName = /*cd.fileName +*/file + "_" + plot.name;
            byte[] pngData = texture.EncodeToPNG();
            if (pngData != null)
                File.WriteAllBytes(folderPath  + "/" + fileName + ".png", pngData);
            else
                Debug.Log("Could not convert " + texture.name + " to png. Skipping saving texture.");
            AssetDatabase.Refresh();
        string test = FindDiff(Application.dataPath, folderPath + "/" + fileName + ".png");
            Texture2D CreatedTextureFromAssets = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/" + test, typeof(Texture2D));
            return CreatedTextureFromAssets;
    }

    private static string FindDiff(string s1, string s2)
    {
        string shorterOne;
        string longerOne;

        if (string.IsNullOrEmpty(s1))
            return s2;
        if (string.IsNullOrEmpty(s2))
            return s1;

        if (s1.Length > s2.Length)
        {
            shorterOne = s2;
            longerOne = s1;
        }
        else
        {
            shorterOne = s1;
            longerOne = s2;
        }

        string strDiff = string.Empty;

        string RegexString = "(" + shorterOne + ")";
        Regex regex = new Regex(RegexString, RegexOptions.IgnoreCase);
        string[] parts = regex.Split(longerOne);
        foreach (string part in parts)
        {
            if (part != shorterOne)
            {
                strDiff += part;
            }
        }

        return strDiff;
    }


    public static Texture2D SaveTextureToFolder(Texture2D texture, string path)
    {
        if (path.Length > 0)
        {
            byte[] pngData = texture.EncodeToPNG();
            if (pngData != null)
                File.WriteAllBytes(path, pngData);
            else
                Debug.Log("Could not convert " + texture.name + " to png. Skipping saving texture.");
            Debug.Log(path);
            AssetDatabase.Refresh();
            Texture2D CreatedTextureFromAssets = (Texture2D)AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D));
            return CreatedTextureFromAssets;
        }
        else
        {
            Debug.LogError("something went wrong when creating texture");
            return null;
        }
    }

    public static void SetUpSpawnLoop(ref HdfDatabaseSet cd, VariableToPlot plot, Texture2D colorRamp = null, Gradient gradient = null)
    {
        GameObject tempObject;
        Renderer tempRenderer;
        for (int x = 0; x < cd.latLegend.Length; x++)
        {
            for (int y = 0; y < cd.lonLegend.Length; y++)
            {
                if (x % cd.heatMapResolution == 0 && y % cd.heatMapResolution == 0)
                {
                    if (cd.lonLegend[y] >= cd.bounds.WestBoundingCoordinate && cd.lonLegend[y] <= cd.bounds.EastBoundingCoordinate && (cd.latLegend[x] >= cd.bounds.SouthBoundingCoordinate && cd.latLegend[x] <= cd.bounds.NorthBoundingCoordinate))
                    {
                       tempObject = SpawnAndPlaceDataPoint(x, y, ref cd);
                       tempRenderer = SetDataPointRenderSettings(ref cd, ref tempObject);
                        double Xco2Clamped = InverseLerp(plot.minValueInDataset, plot.maxValueInDataset, plot.dataset2dArray[x, y]);
                        if (gradient != null)
                        {
                            Debug.Log(cd.pointMaterial);
                            tempRenderer.material.color = gradient.Evaluate((float)Xco2Clamped);//for gradient
                        }
                        else if (colorRamp != null)
                        {
                            tempRenderer.sharedMaterial.color = colorRamp.GetPixel((int)(Xco2Clamped * colorRamp.width), 0);//for colorramp
                        }
                        tempObject.transform.localScale = new Vector3(0.01f * cd.heatMapResolution, 0.01f * cd.heatMapResolution, (float)Xco2Clamped / 2);
                        tempObject.name = (float)cd.lonLegend[y] + " " + (float)cd.latLegend[x] + " " + Xco2Clamped;
                        plot.HeatMapGameObjects.Add(tempObject);
                    }
                }
            }
        }
        Debug.Log(plot.HeatMapGameObjects.Count);
    }
    public static GameObject SpawnAndPlaceDataPoint(int x,int y, ref HdfDatabaseSet cd)
    {
        GameObject tempObject;
        Vector3 pointPosition = CoordinateSystem.CoordinateToPoint(new Coordinate((float)cd.lonLegend[y] * Mathf.Deg2Rad, (float)cd.latLegend[x] * Mathf.Deg2Rad), 1);
        Vector3 pointVector = pointPosition - Vector3.zero;
        tempObject = MonoBehaviour.Instantiate(cd.co2PointModel, pointPosition, Quaternion.LookRotation(pointVector) );
        return tempObject;
    }
    public static Renderer SetDataPointRenderSettings(ref HdfDatabaseSet cd, ref GameObject obj)
    {
        Renderer tempRenderer;
        tempRenderer = obj.GetComponent<Renderer>();
        tempRenderer.material = cd.pointMaterial;
        tempRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        return tempRenderer;
    }

    public static void CreateWorldShell(GameObject Sphere, Texture2D map, Material mapMat)
    {
        GameObject duplicatedSphere = new GameObject("CO2 Heatmap Overlay");
        MeshFilter MF = duplicatedSphere.AddComponent<MeshFilter>();
        MF.mesh = Sphere.GetComponent<MeshFilter>().sharedMesh;//World.instance.gameObject.GetComponent<MeshFilter>().mesh;
        duplicatedSphere.transform.localScale *= 1.01f;
        duplicatedSphere.transform.eulerAngles = new Vector3(0, 180, 0);
        MeshRenderer MR = duplicatedSphere.AddComponent<MeshRenderer>();
        mapMat.mainTexture = map;
        duplicatedSphere.GetComponent<Renderer>().sharedMaterial = mapMat; //co2HeatMapMaterial;
        duplicatedSphere.GetComponent<Renderer>().sharedMaterial.color = new Color(1, 1, 1, 0.8f);
    }

    public static double InverseLerp(double a, double b, double value)
    {
        if (a != b)
            return Clamp01((value - a) / (b - a));
        else
            return 0.0f;
    }
    public static double Clamp01(double value)
    {
        if (value < 0D)
            return 0D;
        else if (value > 1D)
            return 1D;
        else
            return value;
    }
}
