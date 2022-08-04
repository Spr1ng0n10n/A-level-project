using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawn : MonoBehaviour
{
    public GameObject room;
    public bool spawned;
    private Transform loc;
    private RoomOrganiser RoomOrganiser;
    public int ranNum;
    public int doors;
    private GameObject newSpawn;
    private Timer Timer;

    // Start is called before the first frame update
    void Start()
    {
        loc = gameObject.GetComponent<Transform>();
        Timer = gameObject.GetComponent<Timer>();
        RoomOrganiser = GameObject.Find("Room Manager").GetComponent<RoomOrganiser>();

        Invoke("Spawner", 0.4f); //calls the function after 0.4 seconds to give time to destroy the spawnpoint before it runs if necessary.
        
    }
    void Spawner()
    {
        if ((spawned == false) && RoomOrganiser.roomsNum < 10) //Checks that a room hasn't been spawned yet, and the room limit hasn't been reached.
        {
            //controls the probability of generating a room with 1, 2 or 3 rooms.
            doors = Random.Range(1, 15);
            if (doors < 4)
            {
                ranNum = Random.Range(0, RoomOrganiser.oneDoor);
            }
            else if ((doors > 3) && (doors < 13))
            {
                ranNum = Random.Range(RoomOrganiser.oneDoor + 1, RoomOrganiser.twoDoor);
            }
            else
            {
                ranNum = Random.Range(RoomOrganiser.twoDoor + 1, RoomOrganiser.threeDoor);
            }

            // checks the position of the spawnpoint, and so selects a room with a door in the opposite direction to spawn.
            if (loc.localPosition.y == 1)
            {
                room = RoomOrganiser.downRooms[ranNum];
            }
            else if (loc.localPosition.x == 1)
            {
                room = RoomOrganiser.leftRooms[ranNum];
            }
            else if (loc.localPosition.y == -1)
            {
                room = RoomOrganiser.upRooms[ranNum];
            }
            else if (loc.localPosition.x == -1)
            {
                room = RoomOrganiser.rightRooms[ranNum];
            }
            Instantiate(room, transform.position, Quaternion.identity);
            RoomOrganiser.roomsNum += 1;
            spawned = true;
        }
    }

    // if two spawnpoints collide,
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("spawn") && gameObject.CompareTag("spawn")) //we only want the function to run if the two gameobjects are both spawnpoints
        {
            newSpawn = other.gameObject; //other cannot be referenced outside of this module
            Invoke("blockCheck", 0.1f); // called after 0.1 seconds to allow other scripts to run before the rest of the code is excecuted
            
        }
    }

    void blockCheck()
    {
        if (newSpawn != null) // if this runs false, the other spawnpoint has already been destroyed, so we avoid running the rest of the code to prevent null reference errors
        {
            if ((newSpawn.GetComponent<spawn>().spawned == false) && (spawned == false)) //this will only run true if the two spawnpoints were created on top of each other at the same time
            {
                Instantiate(RoomOrganiser.wall, transform.position, Quaternion.identity);
                Debug.Log("WOAH");
                spawned = true;
            }
            else
            {
                // destroys the newer spawnpoint to prevent rooms overalapping
                if (Timer.timeElapsed < newSpawn.GetComponent<Timer>().timeElapsed)
                {
                    Debug.Log("overlap gone");
                    Destroy(gameObject);
                }
            }
        }
    }

}
