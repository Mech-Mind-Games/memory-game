using UnityEngine;
using UnityEngine.SceneManagement;

//This script is a singleton. Notice in the awake that it checks if there is an instance of this script in the scene.
//If there is, it will destroy this current script so the original instance stays intact and is the only script in the scene.
public class GameMaster : MonoBehaviour
{
    //Singleton instance of the GameMaster.
    public static GameMaster Instance { get; private set; }

    [Header("Audio Refs")]
    public AudioSource gameAudioSource; //you can set this in editor. A cleaner approach would to be store a ref during awake.
    [SerializeField] AudioClip successSFX; //sfx for when the player does something right
    [SerializeField] AudioClip badSFX; //sfx for when the player does something wrong

    public int currentTielIdToProgress = 1; //We need a way to keep track of our order/progression of allowed matches

    public GameObject firstTileSelected = null; //to store the first tile object (easy access to reflip the tiles)
    public GameObject secondTileSelected = null; //to store the second tile object

    private void Awake()
    {
        // If there is already an instance AND it's not this, destroy this object
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Set the instance
        Instance = this;
        //For a singleton, since we normally want to keep track of data across the entire game
        //we use a DontDestroy on the gameobject that contains this script, to prevent it from getting destroyed between scenes
        DontDestroyOnLoad(gameObject);
    }

    public void CheckTileMatchLogic()
    {
        //need to get a local ref to the tileInformation of each tile
        TileInformation firstTileInfo = firstTileSelected.GetComponent<TileInformation>();
        TileInformation secondTileInfo = secondTileSelected.GetComponent<TileInformation>();

        //So we can now store the tile ID's for our checks
        int firstTileID = firstTileInfo.tileID;
        int secondTileID = secondTileInfo.tileID;

        //First check if our tiles match
        if (firstTileID == secondTileID)
        {
            //Now check if our tile ID matches our progress order
            if (currentTielIdToProgress == firstTileID)
            {
                //Small audio cue to let player know they did it right
                gameAudioSource.PlayOneShot(successSFX);

                //Since our tile match also matched the correct order, we need to increment it
                //++ is shorthand for increment by 1
                currentTielIdToProgress++;

                //We also need to clean out or pick slots so we can store new tile choices
                firstTileSelected = null;
                secondTileSelected = null;
            }
            else//This else statement will run if the tiles match each other, but do not match the current order ID
            {
                //Small audio cue to let player know they did it wrong
                gameAudioSource.PlayOneShot(badSFX);

                //We now need to decide how to "reset" the game state.
                //We can either reset the entire scene via:
                //ReloadScene(); //look at the code at the bottom of the script to see how this ReloadScene() method works

                //OR we can reset the 2 tiles we selected to give the player another chance:
                firstTileInfo.tileVisual.SetActive(false);
                secondTileInfo.tileVisual.SetActive(false);

                //We also need to clear our stored picks so we can select new tiles:
                firstTileSelected = null;
                secondTileSelected = null;

            }

        }
        else // This else statement will only run if the selected tiles do not match each other
        {
            //Small audio cue to let player know they did it wrong
            gameAudioSource.PlayOneShot(badSFX);

            //We now need to decide how to "reset" the game state.
            //We can either reset the entire scene via:
            //ReloadScene(); //look at the code at the bottom of the script to see how this ReloadScene() method works

            //OR we can reset the 2 tiles we selected to give the player another chance:
            firstTileInfo.tileVisual.SetActive(false);
            secondTileInfo.tileVisual.SetActive(false);

            //We also need to clear our stored picks so we can select new tiles:
            firstTileSelected = null;
            secondTileSelected = null;
        }

    }

    //Generic Tile Reset Method
    public void GenericResetTileMethod()
    {
        //Since we have multiple lose conditions that could require the tiles to need to be reset
        //We could pull the logic out to handle the tile reset here
        //to make it so we don't have duplicate code segments
    }


    //Generic method to reload current scene. Since it's on our singleton object, it can be called fairly easy from anywhere else
    public void ReloadScene()
    {
        //first we clean up any stored data we don't want carried over during scene load
        firstTileSelected = null;
        secondTileSelected = null;
        currentTielIdToProgress = 1;

        int index = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(index);
    }
}
