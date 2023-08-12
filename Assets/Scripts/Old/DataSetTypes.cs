using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DataSetTypes
{
    public struct BoundingCoordinates
    {
        //public float SouthBoundingCoordinate;
        //public float NorthBoundingCoordinate;
        //public float WestBoundingCoordinate;
        //public float EastBoundingCoordinate;
        public double SouthBoundingCoordinate;
        public double NorthBoundingCoordinate;
        public double WestBoundingCoordinate;
        public double EastBoundingCoordinate;
    }

    [System.Serializable]
    public struct DoubleArr
    {
        public double[] array;
    }

    [System.Serializable]
    public struct DoubleDoubleArr//used in early stages for visibilty in inspector
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

}

