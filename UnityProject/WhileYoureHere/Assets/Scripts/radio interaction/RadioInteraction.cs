using System;
using Interactable;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

public class RadioInteraction : MonoBehaviour
{
    /* TODO:: add a raycast that detects when objects are hitting when E is pressed
        add a layer mask var in both unity and code
        add script to player if already exists
    */
    
    [SerializeField] private LayerMask dialLayer;
    [SerializeField] private LayerMask buttonLayer;
    [SerializeField] private LayerMask radioLayerMask;
    [SerializeField] private Camera cam;
    [SerializeField] private float lookDistance = 100f;
    [SerializeField] private Transform startButtonLocation;
    [SerializeField] private Transform sliderLocation;
    // [SerializeField] private GameObject onSwitch;
    // [SerializeField] private GameObject offSwitch;
    [SerializeField] private GameObject canvas;
    private bool _isOn;
    private bool isOnClicked;
    private RadioController radioController;
    private bool isDragging;
    private float tuneValue;
    private Vector3 startMousePos;


}
