using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Tools/CreateVariableToPlot")]
public class VariableToPlotListAsset: ScriptableObject
    {
    public List<VariableToPlot> plots;
    public void Reset()
    {
        plots = new()
        {
            new() { name = "test1"},
            new() { name = "test1"},
        };
    }
   // public bool IsSwitchEnabled(string switchName) => switches.Find(s => s.name == switchName).enabled;
}