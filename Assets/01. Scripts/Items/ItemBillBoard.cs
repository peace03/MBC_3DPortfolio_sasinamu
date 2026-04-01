using UnityEngine;

public class ItemBillBoard : MonoBehaviour
{
    private Transform camera;

    private void OnEnable()
    {
        camera = Camera.main.transform;
    }

    private void Update()
    {
        Vector3 camForward = camera.forward;
        camForward.y = 0f;

        if (Vector3.Distance(camForward.normalized, transform.forward) < 0.01f)
            return;

        transform.forward = camForward.normalized;
    }
}