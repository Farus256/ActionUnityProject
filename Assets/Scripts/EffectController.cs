using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EffectController : MonoBehaviour
{
    [SerializeField] public GameObject slideDust;

    [SerializeField] public GameObject blockFlash;
    private Transform dustPos;
    private Transform blockPos;
    private Vector3 pos;
    private void Start()
    {
        dustPos = transform.Find("DustPos").GetComponent<Transform>();
        blockPos = transform.Find("BlockFlashPos").GetComponent<Transform>();

    }

    public void doBlockFlash(int facingDirection, Vector3 spawnPosition)
    {
        if (blockFlash != null)
        {
            pos = new Vector3(facingDirection, 0, 0);
            if(facingDirection == 1)
            {
                GameObject flash = Instantiate(blockFlash, blockPos.position, gameObject.transform.localRotation);
            }
            else
            {
                GameObject flash = Instantiate(blockFlash, blockPos.position + pos, gameObject.transform.localRotation);
            }
        }
    }
    public void doDust(int facingDirection, Vector3 spawnPosition)
    {
        if (slideDust != null)
        {
            GameObject dust = Instantiate(slideDust, dustPos.position, gameObject.transform.localRotation);
            dust.transform.localScale = new Vector3(facingDirection*2f, 2f, 2f);
        }
    }
}
