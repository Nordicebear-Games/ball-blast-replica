﻿using UnityEngine;

public class SpawnControl : MonoBehaviour
{
    [Header("Spawn")]
    [Range(0f, 25f)]
    public float spawnTime = 10f;
    public GameObject meteor;
    public GameObject splitMeteor;

    [Header("Spawn Points")]
    public Transform leftSpawnPoint;
    public Transform leftTargetPoint;
    public Transform rightSpawnPoint;
    public Transform rightTargetPoint;

    [Header("Meteor Container")]
    public GameObject meteorContainer;

    [Header("Level Control")]
    public GameObject lvlControl;

    [HideInInspector]
    public bool insMetControl = true; //instance meteor or not 

    GameObject meteorClone;
    Transform selectedTargetSide;
    Transform selectedSpawnSide;

    private float nextSpawn = 0.0f;
    private bool moveToTargetControl = true; //meteor moves to target point until reach it

    private void Start()
    {
        GetComponents();
    }

    private void Update()
    {
        InstanceMeteor();
    }

    private void GetComponents()
    {
        meteorClone = meteor.gameObject;
        //default side
        selectedTargetSide = rightTargetPoint;
        selectedSpawnSide = rightSpawnPoint;
    }

    private void InstanceMeteor() 
    {
        if (Time.time > nextSpawn)
        {
            //calculate spawn time
            nextSpawn = Time.time + spawnTime;
            //spawn side
            chooseSpawnSide();
            //instance meteor
            if (insMetControl)
            {
                meteorClone = Instantiate(meteor, selectedSpawnSide.position, selectedSpawnSide.rotation);
                (meteorClone as GameObject).transform.parent = meteorContainer.transform; //add meteors to meteor container
                lvlControl.GetComponent<LevelControl>().FillLvlBar(); //after every meteor instance fill level bar
                moveToTargetControl = true;
            }
        }

        if (moveToTargetControl)
        {
            MoveToTarget();
        }
    }

    public void InstanceSplitMeteor(Vector3 scale, Vector3 pos, Quaternion rot, Color color, int parentDur) //instance split meteors
    {
        for (int i = 0; i < 2; i++)
        {
            //instance split meteors
            GameObject insSplitMet = Instantiate(splitMeteor, pos, rot);
            (insSplitMet as GameObject).transform.parent = meteorContainer.transform; //add meteors to meteor container

            //seperate split meteors
            if (i == 0)
            {
                insSplitMet.transform.position += new Vector3(-0.3f, 0f, 0f);
                insSplitMet.GetComponent<SplitMeteor>().BounceSplitMeteor(insSplitMet, "left");
            }
            else if(i == 1)
            {
                insSplitMet.transform.position += new Vector3(0.3f, 0f, 0f);
                insSplitMet.GetComponent<SplitMeteor>().BounceSplitMeteor(insSplitMet, "right");
            }

            // assign some values to split meteors
            insSplitMet.GetComponent<SpriteRenderer>().color = color;
            insSplitMet.transform.localScale = scale * 0.5f;
            //!!! after second split these components disabled itself
            insSplitMet.GetComponent<SplitMeteor>().enabled = true;
            insSplitMet.GetComponent<CircleCollider2D>().enabled = true;
            //durability
            insSplitMet.GetComponent<SplitMeteor>().maxDurability = parentDur; //splitmeteor's durability can't more than it's parent's durability
        }
    }

    private void chooseSpawnSide()
    {
        float possibility = Random.Range(0f, 100f);
        if (possibility < 50f)
        {
            selectedSpawnSide = leftSpawnPoint;
            selectedTargetSide = leftTargetPoint;
        }
        else
        {
            selectedSpawnSide = rightSpawnPoint;
            selectedTargetSide = rightTargetPoint;
        }
    }

    private void MoveToTarget()
    {
        float step = 2 * Time.deltaTime; // calculate distance to move
        meteorClone.transform.position = Vector3.MoveTowards(meteorClone.transform.position, selectedTargetSide.position, step);
        if (selectedTargetSide.position == meteorClone.transform.position) //when meteor reach the target point
        {
            moveToTargetControl = false;
            meteorClone.GetComponent<SpawnedMeteor>().startControl = true;
            meteorClone.GetComponent<SpawnedMeteor>().MeteorActivate();
        }
    }
}
