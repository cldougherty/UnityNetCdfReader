using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VariableToPlotUI : MonoBehaviour
{
    // Start is called before the first frame update
    public HdfSet set;
    public Slider plotSlider;
    public TextMeshProUGUI LongNameLabel;
    public Image colorRampTexture;
    public Material colorRampMaterial;
    public TextMeshProUGUI minValueText;
    public TextMeshProUGUI maxValueText;
    public TextMeshProUGUI fileNameText;
    public TextMeshProUGUI fileNumberText;
    public int databaseIndex = 0;
    public int variableIndex = 0;
    public float changeTimer = 1;
    public float currentTime;

    public Material testMAt;
    void Start()
    {
        plotSlider.maxValue = set.hdfFiles.Count-1;
        colorRampMaterial.mainTexture = null;
        DisplayPlot(0,0);
    }

    public void StartTimer()
    {
     
    }

    public void ChangePlot()
    {
        DisplayPlot(variableIndex, (int)plotSlider.value);
    }

    public void DisplayPlot(int variableToPlotIndex,int fileNum)
    {
        if (set)
        {
            plotSlider.maxValue = set.hdfFiles.Count - 1;
            LongNameLabel.text = set.hdfFiles[fileNum].variablesToPlot[variableToPlotIndex].longName + " (" + set.hdfFiles[fileNum].variablesToPlot[variableToPlotIndex].units + ")";
            testMAt.mainTexture = set.hdfFiles[fileNum].variablesToPlot[variableToPlotIndex].generatedHeatmap;
            if (set.hdfFiles[fileNum].variablesToPlot[0].colorramp)
                colorRampMaterial.mainTexture = set.hdfFiles[fileNum].variablesToPlot[variableToPlotIndex].colorramp;
            fileNumberText.text = fileNum.ToString() + "/" + set.hdfFiles.Count;
            fileNameText.text = set.hdfFiles[fileNum].fileName;
            minValueText.text = set.hdfFiles[fileNum].variablesToPlot[variableToPlotIndex].minValueInDataset.ToString("f6");
            maxValueText.text = set.hdfFiles[fileNum].variablesToPlot[variableToPlotIndex].maxValueInDataset.ToString("f6");
        }
        //colorRampTexture.sprite = set.hdfFiles[fileNum].variablesToPlot[0].colorramp;
    }

    // Update is called once per frame
    void Update()
    {
        //plotSlider.value += 1;//for testing
        ChangePlot();
        if (set)
        {
            currentTime += Time.deltaTime;
            if (currentTime >= changeTimer)
            {
                if (databaseIndex < set.hdfFiles.Count)
                {
                    databaseIndex++;
                    //plotSlider.value += 1;
                    //DisplayPlot(databaseIndex);
                }
                else
                {
                    databaseIndex = 0;
                    plotSlider.value = 0;
                }
                currentTime = 0;
            }
        }
    }
}
