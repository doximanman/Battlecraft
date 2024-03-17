using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChestPlacer : MonoBehaviour
{
    [SerializeField] private Transform interactables;
    [SerializeField] private Hotbar hotbar;
    [SerializeField] private BoxCollider2D player;
    [SerializeField] private GameObject chestPrefab;
    [SerializeField] private float yOffset;

    public float leniency;
    public float maxPlaceDistance;

    private void Update()
    {
        // cant place down a chest when the game is paused
        if (MetaLogic.paused) return;

        if(Input.GetMouseButtonDown(0))
        {
            // assert a chest is selected
            if (hotbar.SelectedSlot.GetStack() == null) return;
            if (!hotbar.SelectedSlot.GetStack().Type.name.Contains("Chest")) return;

            // get chest items
            var items = hotbar.SelectedSlot.GetStack().Type.invData.Copy();

            Vector2 mousePosition=Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // get ground levels of x=mousePosition.x
            var grounds = Logic.GetGroundHeight(mousePosition.x);

            // no ground??
            if (grounds.Count() == 0) return;

            // get closes ground to player
            float minValue = grounds.Min(gr => Mathf.Abs(player.bounds.min.y - gr));
            var ground = grounds.First(gr => Mathf.Abs(player.bounds.min.y - gr) == minValue);

            // assert chest can fit
            var sprite=chestPrefab.GetComponent<SpriteRenderer>();
            Vector2 placePosition = new(mousePosition.x,ground+sprite.bounds.extents.y+yOffset);
            Vector2 boxSize = (Vector2)sprite.bounds.size - leniency * Vector2.up;

            // assert place position close enough to player
            if (Vector2.Distance(player.bounds.center, placePosition) > maxPlaceDistance) return;

            var result = Physics2D.BoxCast(placePosition, boxSize,0,Vector2.right,0.01f,LayerMask.GetMask("Game"));
            if (result) return;

            // place chest
            hotbar.SelectedSlot.RemoveOne();
            var chest=Object.Instantiate(chestPrefab,placePosition,new Quaternion(0,0,0,0),interactables);
            chest.name = "Chest";
            chest.GetComponent<Chest>().SetItems(items);
        }

    }
}
