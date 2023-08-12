using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MyPlayerController : MonoBehaviour
{
    public static MyPlayerController instance;

    public InteractorGroup leftHandInteractors;
    public InteractorGroup RightHandInteractors;
    public InteractorGroup leftControllerInteractors;
    public InteractorGroup RightControllerInteractors;
    public RayInteractor rightHandRay;
    public RayInteractor leftHandRay;
    public GameObject handLeft;
    public GameObject handRight;

    public bool globeScaleModeEnabled;


    public Vector3 rightHandRayPoint;
        public Vector3 leftHandRayPoint;

    public TextMeshPro debugText;

    // Start is called before the first frame update
    void Start()
    {
    }
    private void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if(rightHandRay.CollisionInfo.HasValue)
         rightHandRayPoint =   rightHandRay.CollisionInfo.Value.Point;
        if(leftHandRay.CollisionInfo.HasValue)
         leftHandRayPoint = leftHandRay.CollisionInfo.Value.Point;
    }

    public void ToggleGlobeScale()
    {
        globeScaleModeEnabled = !globeScaleModeEnabled;
    }

}
