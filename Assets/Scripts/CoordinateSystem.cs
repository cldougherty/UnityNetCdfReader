using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Mathf;

public static class CoordinateSystem
{


	public static Coordinate PointToCoordinate(Vector3 pointOnUnitSphere)
	{
		float latitude = Mathf.Asin(pointOnUnitSphere.y);
		float a = pointOnUnitSphere.x;
		float b = -pointOnUnitSphere.z;

		float longitude = Mathf.Atan2(a, b);

		return new Coordinate(longitude, latitude);
	}

	// Calculate point on sphere given longitude and latitude (in radians), and the radius of the sphere
	public static Vector3 CoordinateToPoint(Coordinate coordinate, float radius)
	{
		float y = Mathf.Sin(coordinate.latitude);
		float r = Mathf.Cos(coordinate.latitude); // radius of 2d circle cut through sphere at 'y'
		float x = Mathf.Sin(coordinate.longitude) * r;
		float z = -Mathf.Cos(coordinate.longitude) * r;

		return new Vector3(x, y, z) * radius;
	}

	public static float DistanceBetweenPointsOnSphere(Vector3 a, Vector3 b, float radius)
	{
		return radius * DistanceBetweenPointsOnUnitSphere(a / radius, b / radius);
	}

	public static float DistanceBetweenPointsOnUnitSphere(Vector3 a, Vector3 b)
	{
		return Mathf.Acos(Vector3.Dot(a, b));
	}

	/// <summary>
	/// Scales the target around an arbitrary point by scaleFactor.
	/// This is relative scaling, meaning using  scale Factor of Vector3.one
	/// will not change anything and new Vector3(0.5f,0.5f,0.5f) will reduce
	/// the object size by half.
	/// The pivot is assumed to be the position in the space of the target.
	/// Scaling is applied to localScale of target.
	/// </summary>
	/// <param name="target">The object to scale.</param>
	/// <param name="pivot">The point to scale around in space of target.</param>
	/// <param name="scaleFactor">The factor with which the current localScale of the target will be multiplied with.</param>
	public static void ScaleAroundRelative(GameObject target, Vector3 pivot, Vector3 scaleFactor)
	{
		// pivot
		var pivotDelta = target.transform.localPosition - pivot;
		pivotDelta.Scale(scaleFactor);
		target.transform.localPosition = pivot + pivotDelta;

		// scale
		var finalScale = target.transform.localScale;
		finalScale.Scale(scaleFactor);
		target.transform.localScale = finalScale;
	}

	/// <summary>
	/// Scales the target around an arbitrary pivot.
	/// This is absolute scaling, meaning using for example a scale factor of
	/// Vector3.one will set the localScale of target to x=1, y=1 and z=1.
	/// The pivot is assumed to be the position in the space of the target.
	/// Scaling is applied to localScale of target.
	/// </summary>
	/// <param name="target">The object to scale.</param>
	/// <param name="pivot">The point to scale around in the space of target.</param>
	/// <param name="scaleFactor">The new localScale the target object will have after scaling.</param>
	public static void ScaleAround(GameObject target, Vector3 pivot, Vector3 newScale)
	{
		// pivot
		Vector3 pivotDelta = target.transform.localPosition - pivot; // diff from object pivot to desired pivot/origin
		Vector3 scaleFactor = new Vector3(
			newScale.x / target.transform.localScale.x,
			newScale.y / target.transform.localScale.y,
			newScale.z / target.transform.localScale.z);
		pivotDelta.Scale(scaleFactor);
		target.transform.localPosition = pivot + pivotDelta;

		//scale
		target.transform.localScale = newScale;
	}

}

[System.Serializable]
public struct Coordinate
{
	// Longitude/latitude in radians
	[Range(-Mathf.PI, Mathf.PI)]
	public float longitude;
	[Range(-Mathf.PI / 2, Mathf.PI / 2)]
	public float latitude;

	public Coordinate(float longitude, float latitude)
	{
		this.longitude = longitude;
		this.latitude = latitude;
	}

	public Vector2 ToVector2()
	{
		return new Vector2(longitude, latitude);
	}

	public Vector2 ToUV()
	{
		return new Vector2((longitude + PI) / (2 * PI), (latitude + PI / 2) / PI);
	}
}
