using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class VariableToPlot //: IMappable
{
    public string name;//example: Co2 Emissions
    public string longName = "";
    public string units;
    public string cellMethods;
    public double[,] dataset2dArray;
    public double maxValueInDataset;
    public double minValueInDataset;
    public double customMin;
    public double customMax;
    public DataSetTypes.BoundingCoordinates bounds;
    public Gradient gradient;
    public Texture2D colorramp;
    public Texture2D generatedHeatmap;
    public double missingValue;// any missing/empty data from array will be represented by this number
    public List<GameObject> HeatMapGameObjects = new List<GameObject>();//does not need to be per dataset?
    /* public readonly int index;
     public VariableToPlot(int index)
     {
         this.index = index;
     }
     public override string ToString()
     {
         return "index: " + index;
     }*/
}