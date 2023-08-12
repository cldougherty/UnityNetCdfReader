using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AS.HDFql;
public static class HdfqlTools
{
   public static bool UseFileFromPath(string path= "")
    {
        string defaultFilePath = "/Users/Name/File.nc4";

        if(path.Length <=0)
        {
            path = defaultFilePath;
        }

        string useFileCommand = "USE READONLY FILE " + path + ";";
        int result = HDFql.Execute(useFileCommand);
        if (result == HDFql.Success)
        {
            //SHOW FILE VALIDITY 
            HDFql.Execute("SHOW USE FILE;");
            while (HDFql.CursorNext() == HDFql.Success)
            {
                return true;
            }
        }
        else
        {
            Debug.LogError(HDFql.ErrorGetMessage());
        }
        return false;
    }
    public static List<string>  ReturnAllDatasets()
    {
        List<string> datasetList = new List<string>();
       var result =  HDFql.Execute("SHOW DATASET / LIKE ** WHERE DATA TYPE == FLOAT ");
        if (result == HDFql.Success)
        {
            //HDFql.Execute("SHOW USE FILE;");
            while (HDFql.CursorNext() == HDFql.Success)
            {
                datasetList.Add(HDFql.CursorGetChar());   
            }
        }
        else
        {
            Debug.LogError(HDFql.ErrorGetMessage());

        }
        if(datasetList.Count<=0)
        {
            Debug.LogError("could not retrieve any hdf datasets from file");
        }
        return datasetList;
    }
    public static bool CheckValidFile()
    {
        var result = HDFql.Execute("SHOW USE FILE ");
        if (HDFql.CursorNext() == HDFql.Success)
        {
            return true;
        }
        else
        {
            Debug.LogWarning(HDFql.ErrorGetMessage());
            return false;
        }
    }

    public static void ShowHdfObjectType(string objectName)
    {

        if (HDFql.Execute("SHOW TYPE " + objectName) == HDFql.Success)
        {
            HDFql.CursorNext();
            var type = HDFql.CursorGetInt();
            if (type == HDFql.Group)
            {
                Debug.Log("Object is a group\n");
            }
            else if (type == HDFql.Dataset)
            {
                Debug.Log("Object is a dataset\n");
            }
            else if (type == HDFql.Attribute)
            {
                Debug.Log("Object is an attribute\n");
            }
            else if (type == HDFql.SoftLink)
            {
                Debug.Log("Object is a soft link\n");
            }
            else if (type == HDFql.ExternalLink)
            {
                Debug.Log("Object is an external link\n");
            }
            /* else if (type == HDFql.SoftLink | HDFql.Group)
             {
                 printf("Object is a soft link pointing to a group\n");
             }
             else if (type == HDFql.SoftLink | HDFql.Dataset)
             {
                 printf("Object is a soft link pointing to a dataset\n");
             }
             else if (type == HDFql.ExternalLink | HDFql.Group)
             {
                 printf("Object is an external link pointing to a group\n");
             }
             else if (type == HDFql.ExternalLink | HDFql.Dataset)
             {
                 printf("Object is an external link pointing to a dataset\n");
             }
            */
            else
            {
               Debug.Log("Object is of unknown type!\n");
            }
        }
    }
    public static List<string> ShowAttributes(string varName)
    {
        List<string> tempAttributeList = new List<string>();
       int result =  HDFql.Execute("SHOW ATTRIBUTE " + varName);
        if (result == HDFql.Success)
        {
            //HDFql.CursorFirst();
            string ab = "";
            while (HDFql.CursorNext() == HDFql.Success)
            {
                ab = HDFql.CursorGetChar();
                tempAttributeList.Add(ab);
                //ab += (char)HDFql.CursorGetTinyint();
            }
            return tempAttributeList;
        }
        else
        {
            Debug.LogError(HDFql.ErrorGetMessage());
            return null;
        }
    }

    public static int CheckDataType(string objectName)
    {
        HDFql.Execute("SHOW DATA TYPE " + objectName);
        HDFql.CursorFirst();
        int dataType =-1;
        if (HDFql.CursorGetInt() != null)
       dataType = (int)HDFql.CursorGetInt();
        return dataType;
    }


    public static string GetHDFqlString(string attributeName)
    {//outdated? use method below?
        HDFql.Execute("SELECT FROM " + attributeName);
        string result = string.Empty;
        HDFql.CursorFirst();
        while (HDFql.SubcursorNext() == HDFql.Success)
        {//have to do it like this because https://forum.hdfgroup.org/t/hdfql-strange-characters-randomly-from-select-command/6561/3
            result += (char)HDFql.SubcursorGetUnsignedTinyint();
            //result += (char)HDFql.CursorGetTinyint(); // works with some files though 
            //result += (char)HDFql.CursorGetInt();
        }
        return result;
    }
    public static string GetHdfAttributeValueBasedOnType(string variableName, string attributeName)
    {
       int variableType = HdfqlTools.CheckDataType(variableName + "/" + attributeName);

        int hdfqlResponse = HDFql.Execute("SELECT FROM " + variableName + "/" + attributeName);
        HDFql.CursorFirst();
        if(hdfqlResponse != HDFql.Success)
        {
            Debug.LogError(HDFql.ErrorGetMessage());
            return "";
        }

        if (variableType == HDFql.Double || variableType == HDFql.Vardouble)
            return HDFql.CursorGetDouble().ToString();
        else if (variableType == HDFql.Float || variableType == HDFql.Varfloat)
            return HDFql.CursorGetFloat().ToString();
        else if (variableType == HDFql.Char) 
        {
            string result = "";
            while (HDFql.SubcursorNext() == HDFql.Success)
            {//have to do it like this because of bug with hdfql https://forum.hdfgroup.org/t/hdfql-strange-characters-randomly-from-select-command/6561/3
             result += (char)HDFql.SubcursorGetUnsignedTinyint();
                //result += (char)HDFql.CursorGetTinyint();
                //result += (char)HDFql.CursorGetInt();
            }
            result = result.Replace("\0", ""); //makes sure we do not have null operator which can cause visual bugs
            return result;
        }
        else if (variableType == HDFql.Varchar)
        {
            string result = "";
            result += HDFql.CursorGetChar();
            return result;
        }
        else if (variableType == HDFql.Smallint)
        {
            Debug.Log("small int");
            string result = "";
            result += HDFql.CursorGetSmallint();
            return result;
        }
            
        else
        {
            Debug.LogWarning(variableType + " type not implemented yet ");
            return null;
        }
    }

    public static void SaveVariableFromHdfToUnity(string varName, object unityVarCopy)
    {//only arrays of “SByte”, “Byte”, “Int16”, “UInt16”, “Int32”, “UInt32”, “Int64”, “UInt64”, “Single”, “Double” or “String”
       var result = HDFql.Execute("SELECT FROM " + varName + " INTO MEMORY " + HDFql.VariableRegister(unityVarCopy));
        if (HDFql.CursorNext() != HDFql.Success)
        {
            Debug.LogError(HDFql.ErrorGetMessage());
        }
        else
        {
            Debug.Log("save var from hdf " + varName);
        }
        HDFql.VariableUnregister(unityVarCopy);
    }
    public static bool CheckIfExists(string type, string name)
    {//types: ATTRIBUTE, DATASET, MEMBER, etc
        int exists = HDFql.Execute("SHOW " + type + " " + name);
        if (exists == HDFql.Success)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static void SaveTransientVariableFromHdfToUnity(string varName, object unityVarCopy)
    {
        var test = HDFql.VariableTransientRegister(unityVarCopy);
        int hdfqlResponse =  HDFql.Execute("SELECT FROM " + varName + " INTO MEMORY " +test );
        if(hdfqlResponse != HDFql.Success)
        {
            Debug.LogError(HDFql.ErrorGetMessage());
        }
    }

    public static double[] Save1DDoubleArrayBasedOnCalculatedCount(string hdfArrayName)
    {
        int hdfqlResponse = HDFql.Execute("SELECT FROM " + hdfArrayName);
        if (hdfqlResponse == HDFql.Success)
        {
            var arrayLength = HDFql.CursorGetCount();
            double[] tmpArray = new double[arrayLength];
            HdfqlTools.SaveVariableFromHdfToUnity(hdfArrayName, tmpArray);
            Debug.Log(tmpArray.Length);
            return tmpArray;
        }
        else
        {
        Debug.LogError(HDFql.ErrorGetMessage());
        return null;
        }
    }
    public static float[] Save1DFloatArrayBasedOnCalculatedCount(string hdfArrayName)
    {
        int hdfqlResponse = HDFql.Execute("SELECT FROM " + hdfArrayName);
        if (hdfqlResponse == HDFql.Success)
        {
            var arrayLength = HDFql.CursorGetCount();
            float[] tmpArray = new float[arrayLength];
            HdfqlTools.SaveVariableFromHdfToUnity(hdfArrayName, tmpArray);
            return tmpArray;
        }
        else
        {
            Debug.LogError(HDFql.ErrorGetMessage());
            return null;
        }
    }

    public static List<int> GetDimensionsFromVariable(string hdfVarName)
    {
        List<int> intList = new List<int>();
        HDFql.Execute("SHOW DIMENSION " + hdfVarName);
        while (HDFql.CursorNext() == HDFql.Success)
        {   
            // get the current result as a string
            int result = (int)HDFql.CursorGetInt();
            intList.Add(result);
        }
        return intList;
    }
}