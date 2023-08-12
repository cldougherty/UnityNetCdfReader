using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public static World instance;
    float frequency;
    public Vector3 startingScale;
    float startingDistanceBetweenHandRayPoints;
    Vector3 startingLeftHandRayPoint;
    Vector3 startingRightHandRayPoint;
    float startingDistanceBetweenHands;
    //FacilitiesGet facilities;
    bool currentlyScaling;
    bool currentlyRotating;
    bool hoveringOverGlobe;
    MyPlayerController myController;
    public Transform tempWorldMap;
    private void Awake()
    {
        if(!instance)
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        startingScale = transform.localScale;
        myController = MyPlayerController.instance;
        //facilities = FacilitiesGet.instance;

        if (tempWorldMap)
        {
            transform.position = tempWorldMap.position;
            transform.rotation = tempWorldMap.rotation;
            transform.localScale = tempWorldMap.localScale;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (hoveringOverGlobe)
        {
            FindNearestPlants();
        }
        if (currentlyScaling)
        {
            float rightHandCursoryDistance = myController.rightHandRayPoint.y - startingRightHandRayPoint.y;
            float rightHandCursorxDistance = myController.rightHandRayPoint.x - startingRightHandRayPoint.x;
            float leftHandCursoryDistance = myController.leftHandRayPoint.y - startingLeftHandRayPoint.y;
            float leftHandCursorxDistance = myController.leftHandRayPoint.x - startingLeftHandRayPoint.x;

            float distanceBetweenHands = Vector3.Distance(myController.handLeft.transform.position, myController.handRight.transform.position);
            Vector3 pointBetweenHandRayPoints = myController.leftHandRayPoint + (myController.rightHandRayPoint - myController.leftHandRayPoint) / 2;
            Vector3 pointBetweenStartingHandRayPoints = startingLeftHandRayPoint + (startingRightHandRayPoint - startingLeftHandRayPoint) / 2;
            //float actualDistance;
            /*if (startingDistanceBetweenHandRayPoints > distanceBetweenHands)
                actualDistance = startingDistanceBetweenHandRayPoints - distanceBetweenHands;
            else
            {
                actualDistance = distanceBetweenHands - startingDistanceBetweenHandRayPoints;
            }*/

            //Debug.Log(distanceBetweenHandRayPoints);
            //ransform.localScale = transform.localScale + new Vector3(distanceBetweenHandRayPoints, distanceBetweenHandRayPoints, distanceBetweenHandRayPoints);
            if (Mathf.Abs(distanceBetweenHands) > .1)
            {
                //i should scale relativly based on distance between hands after starting scale 
                //float differenceBetweenCurrentAndStartingHandPositions = distanceBetweenHands - startingDistanceBetweenHands;  
                if (distanceBetweenHands > startingDistanceBetweenHands)
                    CoordinateSystem.ScaleAroundRelative(gameObject, pointBetweenStartingHandRayPoints, new Vector3(1.01f, 1.01f, 1.01f));
                else if (distanceBetweenHands < startingDistanceBetweenHands)
                    CoordinateSystem.ScaleAroundRelative(gameObject, pointBetweenStartingHandRayPoints, new Vector3(0.99f, 0.99f, 0.99f));
                //⤢
                /*if (leftHandCursorxDistance < 0 && rightHandCursorxDistance > 0  && leftHandCursoryDistance<0 && rightHandCursoryDistance >0)
                CoordinateSystem.ScaleAroundRelative(gameObject, pointBetweenStartingHandRayPoints, new Vector3(1.01f,1.01f,1.01f));
                //⤡
                else if(leftHandCursorxDistance < 0 && rightHandCursorxDistance > 0 && leftHandCursoryDistance > 0 && rightHandCursoryDistance < 0)
                    CoordinateSystem.ScaleAroundRelative(gameObject, pointBetweenStartingHandRayPoints, new Vector3(1.01f,1.01f,1.01f));
                else if (leftHandCursorxDistance > 0 && rightHandCursorxDistance < 0 && leftHandCursoryDistance < 0 && rightHandCursoryDistance > 0)
                    CoordinateSystem.ScaleAroundRelative(gameObject, pointBetweenStartingHandRayPoints, new Vector3(0.99f, 0.99f, 0.99f));
                else if (leftHandCursorxDistance > 0 && rightHandCursorxDistance < 0 && leftHandCursoryDistance > 0 && rightHandCursoryDistance < 0)
                    CoordinateSystem.ScaleAroundRelative(gameObject, pointBetweenStartingHandRayPoints, new Vector3(0.99f, 0.99f, 0.99f));*/
            }
        }
        else if (currentlyRotating)
        {
            if (myController.rightHandRay.SelectedInteractable && myController.rightHandRay.SelectedInteractable.gameObject.GetComponent<World>())
            {
                float rightHandCursoryDistance = startingRightHandRayPoint.y - myController.rightHandRayPoint.y;
                float rightHandCursorxDistance = startingRightHandRayPoint.x - myController.rightHandRayPoint.x;
                float rotX = rightHandCursorxDistance * 20 * Mathf.Deg2Rad;
                float rotY = rightHandCursoryDistance * 20 * Mathf.Deg2Rad;
                if (Mathf.Abs(rotX) > 0.1f)
                    transform.Rotate(Vector3.up, -rotX);
                if (Mathf.Abs(rotY) > 0.1f)
                    transform.Rotate(Vector3.right * rotY, Space.World);
                //transform.Rotate(Vector3.right, rotY);
                //myController.debugText.text = rotX.ToString() + " " + rotY.ToString(); 
            }
        }
    }
    public void GlobeSelected()
    {
        Debug.Log("Globe Selected");
        //myController.globeScaleModeEnabled = true;
        //Debug.Log(myController.leftHandRay.SelectedInteractable.gameObject.GetComponent<World>());
        if ((myController.rightHandRay.SelectedInteractable && myController.rightHandRay.SelectedInteractable.gameObject.GetComponent<World>()) || (myController.leftHandRay.SelectedInteractable && myController.leftHandRay.SelectedInteractable.gameObject.GetComponent<World>()))
        {//one of my hands have selected the globe
            //myController.debugText.text =  myController.leftHandRay.SelectedInteractable + " " + myController.leftHandRay.SelectedInteractable.gameObject.GetComponent<World>() + " " + (myController.rightHandRay.SelectedInteractable + " " + myController.rightHandRay.SelectedInteractable.gameObject.GetComponent<World>());
            // myController.debugText.text = (myController.leftHandRay.SelectedInteractable && myController.leftHandRay.SelectedInteractable.gameObject.GetComponent<World>()) + " " + (myController.rightHandRay.SelectedInteractable && myController.rightHandRay.SelectedInteractable.gameObject.GetComponent<World>());
            //if ((myController.rightHandRay.SelectedInteractable && myController.rightHandRay.SelectedInteractable.gameObject.GetComponent<World>()) && (myController.leftHandRay.SelectedInteractable && myController.leftHandRay.SelectedInteractable.gameObject.GetComponent<World>()))
            if ((myController.rightHandRay.SelectedInteractable) && (myController.leftHandRay.SelectedInteractable))
            {//both hand rays are on globe
                
                if (myController.globeScaleModeEnabled)
                {
                    Debug.Log("SCALING CUBE");
                    startingDistanceBetweenHands = Vector3.Distance(myController.handLeft.transform.position, myController.handRight.transform.position);
                    startingRightHandRayPoint = myController.rightHandRayPoint;
                    startingLeftHandRayPoint = myController.leftHandRayPoint;
                    currentlyScaling = true;
                    currentlyRotating = false;
                    startingDistanceBetweenHandRayPoints = Vector3.Distance(myController.leftHandRayPoint, myController.rightHandRayPoint);
                }
            }
            else
            {
                Debug.Log(myController.globeScaleModeEnabled + " " + myController.rightHandRay.SelectedInteractable);
                if (myController.globeScaleModeEnabled && myController.rightHandRay.SelectedInteractable && myController.rightHandRay.SelectedInteractable.gameObject.GetComponent<World>())
                {
                    Debug.Log("ROTATING GLOBE");
                    startingRightHandRayPoint = myController.rightHandRayPoint;
                    startingLeftHandRayPoint = myController.leftHandRayPoint;
                    currentlyRotating = true;
                    currentlyScaling = false;
                }
                else if (myController.leftHandRay.SelectedInteractable && myController.leftHandRay.SelectedInteractable.gameObject.GetComponent<World>())
                {
                }
            }
            Debug.Log("SELECT GLOBE");
        }
        startingDistanceBetweenHandRayPoints = Vector3.Distance(myController.leftHandRayPoint, myController.rightHandRayPoint);
    }
    public void GlobeHovering()
    {
        Debug.Log("GLOBE Hover");
        hoveringOverGlobe = true;
    }
    public void GlobeStopHovering()
    {
        hoveringOverGlobe = false;
        Debug.Log("StopHoveringOverGlobe");
    }
    public void FindNearestPlants()
    {
    }
    public void GlobeUnselected()
    {
        currentlyScaling = false;
        currentlyRotating = false;
        Debug.Log("GLOBE UNSELECTED");
    }
}
