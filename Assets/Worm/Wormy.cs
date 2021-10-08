
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Wormy : MonoBehaviour
{
    public Rig rig = null;
    public Transform target = null;
    public float Movespeed = 3.5f;

    private void Update()
    {
        float vert = Input.GetAxis("Vertical");
        float horz = Input.GetAxis("Horizontal");

        target.Translate(Vector3.up * Movespeed * vert * Time.deltaTime);
        target.Translate(Vector3.right * Movespeed * horz * Time.deltaTime);
    }
}
