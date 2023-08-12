using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class HdfDatabaseSet // represents a single file that could have multiple variables to plot.
{//todo: any variables here should probably only be needed for a set of files or a single file.
    public string path;
    public string fileName;
    public double[] latLegend;
    public double[] lonLegend;
    public DataSetTypes.BoundingCoordinates bounds;
    public string shortName;
    public string longName;
    public string title;//Short description of the file contents
    public Material co2HeatMapMaterial;
    //settings for 3d heatmap with thousands of cubes
    public GameObject co2PointModel;
    public int heatMapResolution;
    public Material pointMaterial;
    public List<VariableToPlot>variablesToPlot = new List<VariableToPlot>();

    //only some files have this
    public float SOUTH_WEST_CORNER_LAT;
    public float SOUTH_WEST_CORNER_LON;
    public float DX, DY;//also can be known as //LatitudeResolution

}