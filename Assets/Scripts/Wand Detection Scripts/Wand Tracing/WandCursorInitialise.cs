using UnityEngine;
using UnityEngine.InputSystem;

// Controls and has accesable value wand tip pos for new pos
public class WandCursorInitialise : MonoBehaviour
{

    #region Runtime Variables
    private WandInputActions inputActions;
    private Camera mainCam;
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
        Cursor.visible = GameConstants.CursorVisible;

        inputActions = new WandInputActions();
        mainCam = Camera.main;

        //if (positionInput.Equals(null) ||
        //    !(
        //    positionInput.Equals(inputActions.Wand.PositionLeft) ||
        //    positionInput.Equals(inputActions.Wand.PositionRight)
        //    ))
        //{
        //    Debug.LogError($"{nameof(WandCursorInitialise)}: No valid position input assigned on {gameObject.name}.");
        //    Debug.Log("Setting game object to inactive.");
        //    gameObject.SetActive(false);
        //}
    }

    private void OnEnable() => inputActions.Enable();
    private void OnDisable() => inputActions.Disable();

    private void Update()
    {
        Vector2 mousePos = inputActions.Wand.PositionLeft.ReadValue<Vector2>();

        Vector3 worldPos = mainCam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, GameConstants.DistanceToCamera));

        transform.position = worldPos;
        transform.rotation = Quaternion.Euler(GameConstants.QuatRotation);
    }
    #endregion
}
