using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class ColorRampConversionTools
{

  public static Color32[] LoadColorsFromActFile(string path)
    {
        //string path = EditorUtility.OpenFilePanel("open Adobe Color Table", "", "act");
        Color32[] colorsFromActFile;
        if (path.Length != 0)
        {
            using (FileStream stream = File.OpenRead(path))
            {
                Debug.Log(stream.Length);
                if (stream.Length / 3 < 256)
                {
                    Debug.LogError("Adobe Color Table has less than 256 colors");
                    return null;
                }
                int numOfColors = 256;
                if (stream.Length == 772 || stream.Length == 770)// adobe cs2? 
                {//4 last bytes- Two bytes for the number of colors to use. Two bytes for the color index with the transparency color to use
                    byte[] bytesToRead = new byte[2];
                    stream.Seek(768, SeekOrigin.Begin);
                    //Debug.Log((byte)stream.ReadByte() + (byte)stream.ReadByte());
                    stream.Read(bytesToRead, 0, bytesToRead.Length);
                    Debug.Log(bytesToRead[0] + bytesToRead[1]);
                   numOfColors = bytesToRead[0] + bytesToRead[1];// should be num of colors?
                    Debug.Log(numOfColors);
                    if (numOfColors <= 0) //full files have this byte set to 00
                        numOfColors = 256;
                    else if (numOfColors < 256)
                        Debug.LogWarning("This ACT has less then 256 colors, there could be artifacts when making a 256px colorramp");
                    stream.Seek(0, SeekOrigin.Begin); 
                }
                colorsFromActFile = new Color32[numOfColors];
                int counter = 0;
                for (int y = 0; y < numOfColors; ++y)
                {
                        colorsFromActFile[counter] = new Color32((byte)stream.ReadByte(), (byte)stream.ReadByte(), (byte)stream.ReadByte(),255);
                        counter++;
                }
                /* for (int i = 0; i < (768/3); i++)
                     {
                         byte[] bytesToRead = new byte[3];
                         stream.Read(bytesToRead, 0, 3);// should be num of colors?

                     }*/

                //for (int y = 0; y < 16; ++y)
                //    for (int x = 0; x < 16; ++x)
                //    {
                //        Debug.Log( (byte)stream.ReadByte());
                //    }
            }
            return colorsFromActFile;
        }
        else
        {
            Debug.LogError("cant load act file, Invalid path");
            return null;
        }
    }
    public static Texture2D SaveColorArrayAsColorRamp(Color32[] colors)
    {//has issues with trying to evenly space n colors to 256. very rare. ex: GISS_isccp_rainbow_20.act, possibly EO_carbon_density
        Texture2D generatedColorramp = new Texture2D(256, 16);
        // Calculate the distance between each color
        //we use this for files that have less than 256 colors to essently stretch them
        float distance = (256 + (colors.Length/2)) / colors.Length;
        Debug.Log("distance: " + distance);
        // Iterate through all of the pixels in the x axis and set each pixel to the correct color
        for (int x = 0; x < 256; x++)
        {
            // Calculate the index of the color that the current pixel should be set to
            float colorIndex = x / distance;
            colorIndex = Math.Clamp(colorIndex, 0, colors.Length - 1);

            // Set the pixel to the correct color
            for (int y = 0; y < 16; y++)
            {
                generatedColorramp.SetPixel(x, y, colors[(int)colorIndex]);
            }
        }
        generatedColorramp.Apply();
        return generatedColorramp;
    }

    public static void ConvertFolderOfActFilesToTextures()
    {
         string sourcePath = EditorUtility.OpenFolderPanel("Load folder with act files", "", "");
        string destPath = EditorUtility.OpenFolderPanel("folder to save generated colorramps", "", "");
        if (sourcePath.Length > 0 && destPath.Length > 0)
        {
            string[] files = Directory.GetFiles(sourcePath);
            List<string> ActFiles = new List<string>();
            foreach (string file in files)
            {
                if (file.EndsWith(".act"))
                {
                    Color32[] colors = LoadColorsFromActFile(file);
                    Texture2D tex = SaveColorArrayAsColorRamp(colors);
                    Co2Tools.SaveTextureToFolder(tex, destPath + "/" + Path.GetFileNameWithoutExtension(file) + ".png");
                    //ActFiles.Add(file);
                }
            }
        }
    }
}