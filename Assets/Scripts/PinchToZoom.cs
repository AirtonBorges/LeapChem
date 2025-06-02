using System;
using Leap;
using UnityEngine;

// From: https://github.com/ultraleap/Unity-Hand-Interaction-Experiments/blob/TabletopExperiments/Tabletop%20Experiments/Assets/Experiments/Two%20Handed%20View%20Manipulation/Scripts/Camera/CameraZoom.cs
public class PinchToZoom : MonoBehaviour
{
    public LeapProvider inputProvider;

    [Header("Camera"), SerializeField]
    private bool modifyCamera = true;
    [SerializeField]
    private bool cameraZoom = true;

    [SerializeField]
    private bool pullToZoom = false;

    [SerializeField]
    private bool cameraRotate = false;

    [SerializeField]
    private bool cameraRotateAround = false;

    [SerializeField]
    private bool cameraPan = false;

    [SerializeField]
    public new Camera camera;
    [SerializeField, Range(0, 180)]
    private float cameraFOVMin;
    [SerializeField, Range(0, 180)]
    private float cameraFOVMax;
    [SerializeField, Range(-90, 90)]
    private float cameraXRotMin;

    [SerializeField, Range(-90, 90)]
    private float cameraXRotMax;

    [SerializeField, Range(-90, 90)]
    private float cameraXAroundRotMin;

    [SerializeField, Range(-90, 90)]
    private float cameraXAroundRotMax;

    [SerializeField, Range(-1, 2)]
    private float cameraPanMin;

    [SerializeField, Range(-1, 2)]
    private float cameraPanMax;

    private float _currentFOV, _desiredFOV, _currentXRot, _desiredXRot, _currentXAroundRot, _desiredXAroundRot, _currentPan, _desiredPan;

    [Header("Object"), SerializeField]
    private bool modifyObject = false;
    [SerializeField]
    private bool objectSize = false;

    [SerializeField]
    private bool objectRotation = false;

    [SerializeField]
    private bool objectPan = false;

    [SerializeField]
    private Transform panTransform;

    [SerializeField]
    private Transform scaleTransform;

    [SerializeField, Range(0, 4)]
    private float objectSizeMin;
    [SerializeField, Range(0, 4)]
    private float objectSizeMax;
    [SerializeField, Range(-1, 2)]
    private float objectPanMin;

    [SerializeField, Range(-1, 2)]
    private float objectPanMax;

    private float _currentObjectSize, _desiredObjectSize, _currentObjectRot, _desiredObjectRot, _currentObjectPan, _desiredObjectPan;

    [Header("Interaction Settings"), SerializeField]
    private float lerpSpeed = 0.5f;

    [SerializeField]
    private bool usePinch = false;

    [SerializeField]
    private bool useGrab = true;

    [SerializeField]
    private float grabAmount = 0.5f;

    [SerializeField]
    private float pinchDistance = 0.02f;

    [SerializeField]
    private float handDistanceScale = 0.5f;

    [SerializeField]
    private float pullScale = 0.5f;

    [SerializeField]
    private float rotScale = 0.5f;

    [SerializeField]
    private float rotateAroundScale = 0.5f;

    [SerializeField]
    private float cameraPanScale = 0.5f;

    [SerializeField]
    private float objectSizeScale = 0.5f;

    [SerializeField]
    private float objectRotationScale = 0.5f;

    [SerializeField]
    private float objectPanScale = 0.5f;

    private Hand _leftHand, _rightHand;
    private bool _wasGrabbing = false;
    private float _oldGrabDistance = 0f, _oldGrabHeight = 0f, _oldPullDistance = 0f, _oldHandDot = 0f;
    private Vector3 _originalPan, _originalObjectPan;

    private void Start()
    {
        _currentFOV = camera.fieldOfView;
        _desiredFOV = camera.fieldOfView;
        _currentXRot = camera.transform.rotation.eulerAngles.x;
        _desiredXRot = _currentXRot;
        _currentXAroundRot = camera.transform.parent.rotation.eulerAngles.x;
        _desiredXAroundRot = _currentXAroundRot;
        _currentPan = camera.transform.parent.position.y;
        _originalPan = camera.transform.parent.position;
        _desiredPan = _currentPan;
        if (scaleTransform != null)
        {
            _currentObjectSize = scaleTransform.localScale.x;
            _desiredObjectSize = _currentObjectSize;
            _currentObjectRot = scaleTransform.rotation.eulerAngles.y;
            _desiredObjectRot = _currentObjectRot;
        }
        if (panTransform != null)
        {
            _originalObjectPan = panTransform.position;
            _currentObjectPan = panTransform.position.y;
            _desiredObjectPan = _currentObjectPan;
        }
    }

    private void Update()
    {
        _leftHand = inputProvider.GetHand(Chirality.Left);
        _rightHand = inputProvider.GetHand(Chirality.Right);

        if (_leftHand != null && _rightHand != null &&
            ((usePinch && (_leftHand.PinchDistance / 1000f) < pinchDistance && (_rightHand.PinchDistance / 1000f) < pinchDistance)
            || (useGrab && _leftHand.GetFistStrength() > grabAmount && _rightHand.GetFistStrength() > grabAmount)))
        {
            ProcessHandData();
        }
        else
        {
            _wasGrabbing = false;
        }
        if (modifyCamera)
        {
            if (cameraZoom)
            {
                if (Mathf.Abs(_currentFOV - _desiredFOV) > 1e-5f)
                {
                    _currentFOV = Mathf.Lerp(_currentFOV, _desiredFOV, Time.deltaTime * (1.0f / lerpSpeed));

                    camera.fieldOfView = _currentFOV;
                }
            }
            if (cameraRotate)
            {
                if (cameraRotateAround)
                {
                    if (Mathf.Abs(_currentXAroundRot - _desiredXAroundRot) > 1e-7f)
                    {
                        _currentXAroundRot = Mathf.Lerp(_currentXAroundRot, _desiredXAroundRot, Time.deltaTime * (1.0f / lerpSpeed));
                        camera.transform.parent.rotation = Quaternion.Euler(_currentXAroundRot, 0, 0);
                    }
                }
                else
                {
                    if (Mathf.Abs(_currentXRot - _desiredXRot) > 1e-7f)
                    {
                        _currentXRot = Mathf.Lerp(_currentXRot, _desiredXRot, Time.deltaTime * (1.0f / lerpSpeed));
                        camera.transform.rotation = Quaternion.Euler(_currentXRot, 0, 0);
                    }
                }
            }
            if (cameraPan)
            {
                if (Mathf.Abs(_currentPan - _desiredPan) > 1e-7f)
                {
                    _currentPan = Mathf.Lerp(_currentPan, _desiredPan, Time.deltaTime * (1.0f / lerpSpeed));
                    camera.transform.parent.position = new Vector3(_originalPan.x, _currentPan, _originalPan.z);
                }
            }
        }

        if (modifyObject)
        {
            if (Mathf.Abs(_currentObjectSize - _desiredObjectSize) > 1e-5f)
            {
                _currentObjectSize = Mathf.Lerp(_currentObjectSize, _desiredObjectSize, Time.deltaTime * (1.0f / lerpSpeed));

                scaleTransform.localScale = Vector3.one * _currentObjectSize;
            }

            if (Mathf.Abs(_currentObjectPan - _desiredObjectPan) > 1e-7f)
            {
                _currentObjectPan = Mathf.Lerp(_currentObjectPan, _desiredObjectPan, Time.deltaTime * (1.0f / lerpSpeed));
                panTransform.transform.position = new Vector3(_originalObjectPan.x, _currentObjectPan, _originalObjectPan.z);
            }

            if (Mathf.Abs(_currentObjectPan - _desiredObjectPan) > 1e-7f)
            {
                _currentObjectRot = Mathf.Lerp(_currentObjectRot, _desiredObjectRot, Time.deltaTime * (1.0f / lerpSpeed));
                scaleTransform.rotation = Quaternion.Euler(0, _currentObjectRot, 0);
            }
        }
    }

    private void ProcessHandData()
    {
        float currentXDistance = _leftHand.PalmPosition.x - _rightHand.PalmPosition.x;
        float pullDistance = (_leftHand.PalmPosition.z + _rightHand.PalmPosition.z) / 2f;
        float currentYHeight = (_leftHand.PalmPosition.y + _rightHand.PalmPosition.y) / 2f;
        float currentDot = Vector3.Dot((_leftHand.PalmPosition - _rightHand.PalmPosition).normalized, Vector3.forward);
        if (!_wasGrabbing)
        {
            _oldHandDot = currentDot;
            _oldGrabDistance = currentXDistance;
            _oldPullDistance = pullDistance;
            _oldGrabHeight = currentYHeight;
            _wasGrabbing = true;
        }

        if (cameraZoom)
        {
            if (pullToZoom)
            {
                _desiredFOV += (pullDistance - _oldPullDistance) * pullScale;
            }
            else
            {
                _desiredFOV += (currentXDistance - _oldGrabDistance) * handDistanceScale;
            }
            _desiredFOV = Mathf.Clamp(_desiredFOV, cameraFOVMin, cameraFOVMax);
        }
        if (cameraRotate)
        {
            if (cameraRotateAround)
            {
                _desiredXAroundRot += (currentYHeight - _oldGrabHeight) * rotateAroundScale;
            }
            else
            {
                _desiredXRot += (currentYHeight - _oldGrabHeight) * rotScale;
            }
            _desiredXRot = Mathf.Clamp(_desiredXRot, cameraXRotMin, cameraXRotMax);
            _desiredXAroundRot = Mathf.Clamp(_desiredXAroundRot, cameraXAroundRotMin, cameraXAroundRotMax);
        }
        if (cameraPan)
        {
            _desiredPan += (currentYHeight - _oldGrabHeight) * cameraPanScale;
            _desiredPan = Mathf.Clamp(_desiredPan, cameraPanMin, cameraPanMax);
        }

        if (objectSize)
        {
            if (pullToZoom)
            {
                _desiredObjectSize += (pullDistance - _oldPullDistance) * objectSizeScale;
            }
            else
            {
                _desiredObjectSize += (currentXDistance - _oldGrabDistance) * objectSizeScale;
            }
            _desiredObjectSize = Mathf.Clamp(_desiredObjectSize, objectSizeMin, objectSizeMax);
        }

        if (objectPan)
        {
            _desiredObjectPan += (currentYHeight - _oldGrabHeight) * objectPanScale;
            _desiredObjectPan = Mathf.Clamp(_desiredObjectPan, objectPanMin, objectPanMax);
        }

        if (objectRotation)
        {
            _desiredObjectRot += (currentDot - _oldHandDot) * objectRotationScale;
        }

        _oldHandDot = currentDot;
        _oldGrabDistance = currentXDistance;
        _oldPullDistance = pullDistance;
        _oldGrabHeight = currentYHeight;
    }
}