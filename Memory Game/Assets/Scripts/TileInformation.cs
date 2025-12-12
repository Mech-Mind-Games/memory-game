using UnityEngine;

public class TileInformation : MonoBehaviour
{

    //I made these variables public since I want to be able to access these values from other scripts
    public int tileID; //set this value in the editor
    public AudioClip tileSFX; //set this in the editor. This will be the sfx you want this tile to play
    public GameObject tileVisual; //want access to the tile visual (This is more of a polish item to see when player interacts with tile)

}
