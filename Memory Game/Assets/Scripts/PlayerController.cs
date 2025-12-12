using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Player Refernces")] 
    [SerializeField] private Rigidbody2D rb;

    [Header("Player Movement Vars")]
    private Vector2 playerInputValues; 
    [SerializeField] private float playerSpeed = 5f; 

    private TileInformation tileInformation; //we will need to store a reference of the TileInformation script of the tile we trigger.
    private bool isAbleToInteractWithTile = false; //we need a way to easily check if we can or can't interact with a tile.

    private void Awake()
    {
        InitializeGameState();
    }

    private void FixedUpdate()
    {
        PlayerMoveLogic();
    }

    //I created a separate method for the items I want to initialize during awake method. This was done for house keeping purposes.
    private void InitializeGameState()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError("Player RB was not found, make sure the gameobject the script is attached too has a RB 2D component");
        }
    }


    //This method was created because we have an action called "Move" on our input asset
    private void OnMove(InputValue value)
    {
        playerInputValues = value.Get<Vector2>();
        playerInputValues.Normalize();
    }

    //This method was created because we have an action called "Interact" on our input asset
    private void OnInteract()
    {
        //Check if the player can interact with the tile. (If player is in trigger, bool == true, if out of trigger, bool == false)
        //Just putting the variable name is shorthand for saying if isAbleToInteractWithTile == true
        //If we put a "!" infront of the variable name (!isAbleToInteractWithTile) would be shorthand for == false
        if (isAbleToInteractWithTile)
        {
            //Turn on the tile visual to indicate to the player the tile was interacted with
            tileInformation.tileVisual.SetActive(true);

            //play the sfx from the interacted tile, using the main game audiosource 
            //Our GameMaster singleton contains a ref to our main game audiosource
            //We access it through it's instance, and do a playoneshot of the tileSFX
            //The tile SFX comes from the tileInformation that we stored 
            GameMaster.Instance.gameAudioSource.PlayOneShot(tileInformation.tileSFX);

            //Since we interacted with the tile, which means we "picked it", we need to store it in our singleton
            //Since we have 2 slots we are working with, a simple if statement can be used to check if a slot is filled or not
            //First ill check if firstTileSelected is null, if so, store the tile
            if (GameMaster.Instance.firstTileSelected == null)
            {
                //Since the slot is null on gamemaster, I can fill the slot with a ref to the tile I interacted with
                //Since I only stored a ref to the tileInformation during the trigger event, I use tileinformation.gameobject 
                //to get the gameobject that the tileInformation script is attached to.
                GameMaster.Instance.firstTileSelected = tileInformation.gameObject;
            }
            else if (GameMaster.Instance.secondTileSelected == null) //Check if second slot is null
            {
                //We need to check to make sure we are NOT storing the same tile twice.
                //There are a few methods to achieve this, but i'll use a simple check method.
                //So I will check if the filled first slot does not match our current tileInformation.
                //If it doesn't, then we store the ref.
                // the symbol combo != means does not equal
                if (GameMaster.Instance.firstTileSelected != tileInformation.gameObject)
                {
                    GameMaster.Instance.secondTileSelected = tileInformation.gameObject;

                    //since we did our second "pick, we need to run our match check logic
                    //Since we made our check logic public on our singleton, we can call it from anywhere fairly easily.
                    GameMaster.Instance.CheckTileMatchLogic();
                }
                else //this will run if we try to store the same tile twice
                {
                    Debug.Log("Tried to store the same tile information twice");
                }
            }
        }
        else//This will run if the player is not in a trigger zone
        {
            Debug.Log("Player is not in a trigger zone, stand on a tile please!");
        }

    }


    //This method was created because we have an action called "Reload" on our input asset
    //We also have a public method on our singleton to reload the scene.
    private void OnReload()
    {
        GameMaster.Instance.ReloadScene();
    }

    //Player move logic that will be called in fixed update since physics is being used
    private void PlayerMoveLogic()
    {
        //Since this is running in FixedUpdate loop, the time interval it runs is always the same, so no need to multiply by Time.DeltaTime
        Vector2 moveSpeedAndDir = playerSpeed * playerInputValues;
        rb.linearVelocity = moveSpeedAndDir;
    }


    //OnTrigger Enter 2D requires one object to have a rigidbody 2d and both objects to have a collider
    //Ontrigger runs during the fixed update loop
    //OnTrigger Requires trigger object to have it's collider set to trigger
    //OnTrigger enter runs only once and that's when the Rigidbody object initially enters into the trigger zone
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Doing a null check, this is mainly for performance purposes.
        if (collision != null)
        {
            //Checking if the player is entering a trigger zone that has the tag "Tile"
            if (collision.CompareTag("Tile"))
            {
                Debug.Log("Player has entered a trigger");

                //if we trigger a tile, we want to get the tile information, which we do by
                //storing a reference of the TileInformation script from the collision object.
                tileInformation = collision.GetComponent<TileInformation>();

                //since the player is in the tile trigger, they are now allowed to interact with the tile.
                isAbleToInteractWithTile = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Doing a null check, this is mainly for performance purposes.
        if (collision != null)
        {
            //Checking if the player is exiting a trigger zone that has the tag "Tile"
            if (collision.CompareTag("Tile"))
            {
                Debug.Log("Player has exited a trigger");

                //Since we left the tile, we no longer want a reference to that tile, so we set it null
                tileInformation = null;

                //since the player is no longer in the tile trigger, they are not allowed to interact with the tile now.
                isAbleToInteractWithTile = false;
            }
        }
    }
}
