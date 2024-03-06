using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class MovementTests
{
    public float waitTime=1;

    private float aboutEqual = 0.2f;
    private PlayerControl playerController;
    private GameObject player;

    // Summary:
    //  Sets up the playing scene
    //  Disables input keys
    //  And initializes references to the player and to control the player
    [UnitySetUp]
    public IEnumerator SetUp()
    {
        // load the scene and wait until loading is done
        var sceneLoaded=SceneManager.LoadSceneAsync("Playing",LoadSceneMode.Single);

        yield return new WaitUntil(() => sceneLoaded.isDone);

        // disable key input.
        // without disabling, the player stops moving once no key is pressed.
        // this is just a test for player movement, not for key bindings.
        var keysHandlerObject = GameObject.FindGameObjectWithTag("Keys");
        if (keysHandlerObject != null)
        {
            var keysHandler = keysHandlerObject.GetComponent<KeyInput>();
            keysHandler.disableKeys();
        }

        // wait for player to hit the ground
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerControl>();

        yield return new WaitForSeconds(0.5f);

    }

    // Summary:
    //  Moves right for a predefined time,
    //  And checks that the movement was completed and the player moved right.
    [UnityTest]
    public IEnumerator MoveRight()
    {
        float startingY=player.transform.position.y;
        float startingX=player.transform.position.x;

        playerController.MoveRight();

        yield return new WaitForSeconds(waitTime);

        playerController.StopMoving();

        Vector2 playerPosition = new Vector2(player.transform.position.x, player.transform.position.y);
        Vector2 supposedPosition = new Vector2(startingX+playerController.velocity*waitTime, startingY);

        Debug.Log("player position: " + playerPosition);
        Debug.Log("expected position: " + supposedPosition);

        // assert correct position
        Assert.IsTrue(Vector2.Distance(supposedPosition, playerPosition)<aboutEqual);

        // assert correct sprite
        Assert.IsTrue(player.GetComponent<SpriteRenderer>().flipX == false);
        
    }

    // Summary:
    //  Moves left for a predefined time,
    //  And checks that the movement was completed and the player moved left.
    [UnityTest]
    public IEnumerator MoveLeft()
    {

        float startingY = player.transform.position.y;
        float startingX = player.transform.position.x;

        playerController.MoveLeft();

        yield return new WaitForSeconds(waitTime);

        playerController.StopMoving();

        Vector2 playerPosition = new Vector2(player.transform.position.x, player.transform.position.y);
        Vector2 supposedPosition = new Vector2(startingX-playerController.velocity * waitTime, startingY);

        Debug.Log("player position: " + playerPosition);
        Debug.Log("expected position: " + supposedPosition);

        Assert.IsTrue(Vector2.Distance(supposedPosition, playerPosition) < aboutEqual);

        // assert correct sprite
        Assert.IsTrue(player.GetComponent<SpriteRenderer>().flipX == true);
    }

    // Summary:
    //  Jumps, waits, and asserts that the starting and ending position are the same.
    [UnityTest]
    public IEnumerator Jump()
    {


        float startingY = player.transform.position.y;
        float startingX = player.transform.position.x;

        playerController.Jump();

        yield return new WaitForSeconds(waitTime);

        Vector2 playerPosition=new Vector2(player.transform.position.x, player.gameObject.transform.position.y);
        Vector2 supposedPosition=new Vector2(startingX,startingY);

        Debug.Log("player position: " + playerPosition);
        Debug.Log("expected position: " + supposedPosition);

        Assert.IsTrue(Vector2.Distance(playerPosition, supposedPosition) < aboutEqual);

    }
}
