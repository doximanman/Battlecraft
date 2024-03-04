using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class MovementTests
{

    [UnityTest]
    public IEnumerator MoveRight()
    {
        var player = new GameObject("Player");

        var playerController=player.AddComponent<PlayerControl>();

        playerController.MoveRight();

        yield return null;

        Assert.AreEqual(new Vector2(playerController.transform.position.x, playerController.transform.position.y), new Vector2(playerController.velocity,0));

    }
}
