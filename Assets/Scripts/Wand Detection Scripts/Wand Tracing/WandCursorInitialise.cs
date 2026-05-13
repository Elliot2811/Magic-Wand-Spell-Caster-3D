using UnityEngine;

// Controls and has accesable value wand tip pos for new pos
public class WandCursorInitialise : MonoBehaviour
{
    #region Global Variables

    #region Inspector Variables
    public float distanceToCamera = 5f;
    public Vector3 quatRotation = new Vector3(0, 0, 15);

    public bool cursorVisable = true;
    #endregion

    #region Runtime Variables
    private WandInputActions inputActions;
    private Camera mainCam;
    #endregion

    #endregion

    #region Getter Setters
    public Vector2 pos2
    {
        get { return transform.position;  }
    }
    #endregion

    #region MonoBehaviour Functions
    private void Awake()
    {
        Cursor.visible = cursorVisable;

        inputActions = new WandInputActions();
        mainCam = Camera.main;
    }

    private void OnEnable() => inputActions.Enable();
    private void OnDisable() => inputActions.Disable();

    private void Update()
    {
        Vector2 mousePos = inputActions.Wand.Position.ReadValue<Vector2>();

        Vector3 worldPos = mainCam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, distanceToCamera));

        transform.position = worldPos;
        transform.rotation = Quaternion.Euler(0, 0, 15);
    }
    #endregion
}
