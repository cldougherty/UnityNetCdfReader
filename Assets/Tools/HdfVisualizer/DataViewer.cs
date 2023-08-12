using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DataViewer : MonoBehaviour
{
    public List<HdfSet> sets = new List<HdfSet>();
    public VariableToPlotUI variableToPlotUi;
    public ToggleGroup DatasetSelectUI;
    public ToggleGroup VariableSelectUI;
    public int currentDatasetIndex;
    // Start is called before the first frame update
    void Start()
    {
        foreach (Toggle toggle in DatasetSelectUI.ActiveToggles())
        {
            Debug.Log(toggle, toggle);
        }
        Toggle[] toggles = DatasetSelectUI.GetComponentsInChildren<Toggle>();
        for (int i = 0; i < sets.Count; i++)
        {
            if(sets[i].hdfFiles[0].shortName.Length>0)
            toggles[i].GetComponentInChildren<TextMeshProUGUI>().text = sets[i].hdfFiles[0].shortName;
            else if (sets[i].hdfFiles[0].longName.Length > 0)
                toggles[i].GetComponentInChildren<TextMeshProUGUI>().text = sets[i].hdfFiles[0].longName;
            else
                toggles[i].GetComponentInChildren<TextMeshProUGUI>().text = sets[i].hdfFiles[0].title;
        }
       
      /*  for (int i = 0; i < toggles.Length; i++)
        {
            toggles[i].GetComponent<TextMeshPro>().text = 
        }
        foreach (var t in toggles)
        {
            t.GetComponentInChildren<TextMeshPro>().text = 
        }*/
    }

    public void DatasetSelect()
    {
        Toggle[] datasetToggles = DatasetSelectUI.GetComponentsInChildren<Toggle>();
        for (int i = 0; i < datasetToggles.Length; i++)
        {
            VariableSelectUI.SetAllTogglesOff();
            variableToPlotUi.set = null;
            variableToPlotUi.plotSlider.value = 0;
            if (datasetToggles[i].isOn)
            {
                Debug.Log(i);
                ListVariables(i);
                currentDatasetIndex = i;
            }
        }
    }
    public void VariableSelect()
    {
        Toggle[] VariableToggles = VariableSelectUI.GetComponentsInChildren<Toggle>();
        for (int i = 0; i < VariableToggles.Length; i++)
        {
            if (VariableToggles[i].isOn)
            {
                Debug.Log(i + " is on");
                variableToPlotUi.set = sets[currentDatasetIndex];
                variableToPlotUi.variableIndex = i;
                variableToPlotUi.DisplayPlot(i,0);
            }
        }
    }
    public void ListVariables(int datasetIndex)
    {
        Toggle[] VariableToggles = VariableSelectUI.GetComponentsInChildren<Toggle>(true);
        for (int i = sets[datasetIndex].hdfFiles[0].variablesToPlot.Count; i < VariableToggles.Length; i++)//disable unused
        {
            VariableToggles[i].GetComponentInChildren<TextMeshProUGUI>().text = "default";
            VariableToggles[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < sets[datasetIndex].hdfFiles[0].variablesToPlot.Count; i++)
        {
            VariableToggles[i].gameObject.SetActive(true);
            VariableToggles[i].GetComponentInChildren<TextMeshProUGUI>().text = sets[datasetIndex].hdfFiles[0].variablesToPlot[i].name;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }




}
