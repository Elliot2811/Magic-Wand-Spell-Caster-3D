using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandCursor : MonoBehaviour
{
    public float distanceFromCamera = 5f;
    public Transform WandTip;

    private Camera mainCam;
    private WandInputActions inputActions;
    private void Awake()
    {
        mainCam = Camera.main;
        inputActions = new WandInputActions();
        Cursor.visible = false; //Hide cursor
    }
    private void OnEnable() => inputActions.Enable();
    private void OnDisable() => inputActions.Disable();
    private void Update()
    {
        //Read mouse position from the input system
        Vector2 mousePos = inputActions.Wand.Position.ReadValue<Vector2>();
        //Convert screen to world point
        Vector3 worldPos = mainCam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, distanceFromCamera));
        // Update wand position
        transform.position = worldPos;

        transform.rotation = Quaternion.Euler(0, 0, 15); //tilting the wand slightly
    }
}
