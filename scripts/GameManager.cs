using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject PlayerPrefab;
    private PlayerMovement activePlayer;
    public static int Score;

    //public static void StaticFnc()
    public static GameManager instance;
    //{
    //    if (activePlayer = null)
    //    {

    //    }
    //}



    // Start is called before the first frame update
    private void Awake()
    {
        //DontDestroyOnLoad(gameObject);
        //SceneManager.activeSceneChanged += SpawnPlayerOnLoad;
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            SceneManager.activeSceneChanged += SpawnPlayerOnLoad;

            instance = this;
        }
    }
    private void SpawnPlayerOnLoad(Scene prev, Scene next)
    {
        Debug.Log("Entering Scene is:" + next.buildIndex);
        //if(next.buildIndex == 3) // main menu is 3
        //{
        //    if (activePlayer != null)
        //    {
        //        Destroy(activePlayer);
        //        activePlayer = null;
        //    }
        //}
        //else
        //{

        //}
        playerSpawnSpot playerSpot = FindObjectOfType<playerSpawnSpot>();
        if (activePlayer == null)
        {
            GameObject player = Instantiate(PlayerPrefab, playerSpot.transform.position, playerSpot.transform.rotation);
            activePlayer = player.GetComponent<PlayerMovement>();
        }
        else
        {
            activePlayer.transform.position = playerSpot.transform.position;
            activePlayer.transform.rotation = playerSpot.transform.rotation;
        }
    }
    void Update()
    {
        
    }
}
