using UnityEngine;

public class Draggable : MonoBehaviour
{
    Vector3 mousePositionOffset;
    private Vector3 GetMouseWorldPosition()
    {
        return Camera.main.ScreenToViewportPoint(Input.mousePosition);
    }
    private void OnMouseDown()
    {
        mousePositionOffset = gameObject.transform.position - GetMouseWorldPosition();
        Debug.Log("Mouse Position Offset: " + mousePositionOffset);
    }
    private void OnMouseDrag()
    {
        transform.position = GetMouseWorldPosition() + mousePositionOffset;
        Debug.Log("Mouse Position: " + transform.position);
    }
}
