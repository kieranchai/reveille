using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ring : MonoBehaviour
{
    private GameObject ring;
    private GameObject slot;
    private GameObject target;

    private bool rotating = true;
    private bool activate = false;

    public int speed;

    private void Start()
    {
        ring = this.gameObject;
        slot = this.gameObject.transform.GetChild(0).gameObject;
        target = this.gameObject.transform.GetChild(1).gameObject;
    }

    private void Update()
    {
        if (rotating)
        {
            ring.transform.Rotate(0, 0, speed * Time.deltaTime);
        }

        if (Input.GetMouseButtonDown(0))
        {
            activate = true;
        }

        if (activate)
        {
            slot.transform.position = Vector2.MoveTowards(slot.transform.position, target.transform.position, speed * Time.deltaTime);
            rotating = false;
        }
    }
}
