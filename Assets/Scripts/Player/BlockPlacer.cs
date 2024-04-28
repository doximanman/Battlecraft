using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class BlockPlacer : MonoBehaviour
{
    [SerializeField] private Transform interactables;
    [SerializeField] private Hotbar hotbar;
    [SerializeField] private BoxCollider2D player;
    [SerializeField] private GameObject chestPrefab;
    [SerializeField] private GameObject benchPrefab;
    [SerializeField] private float yOffsetChest;
    [SerializeField] private float yOffsetBench;

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
            string selectedItem = hotbar.SelectedSlot.GetStack().Type.name;
            if (!selectedItem.Contains("Chest") && !selectedItem.Contains("Bench")) return;
            selectedItem = selectedItem.Contains("Chest") ? "Chest" : "Bench";

            InventoryData items=null;
            if (selectedItem.Equals("Chest"))
            {
                // get chest items
                items = hotbar.SelectedSlot.GetStack().Type.invData.Copy();
            }

            // close enough to player
            Vector2 mousePosition=Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // get ground levels of x=mousePosition.x
            var groundMaybeNull = Logic.GetGroundHeightBelow(mousePosition);

            // no ground??
            if (groundMaybeNull == null) return;
            float ground = groundMaybeNull.Value;

            // assert chest/bench can fit
            float yOffset = selectedItem.Equals("Chest") ? yOffsetChest : yOffsetBench;
            var sprite=selectedItem.Equals("Chest") ? chestPrefab.GetComponent<SpriteRenderer>() : benchPrefab.GetComponent<SpriteRenderer>();
            Vector2 placePosition = new(mousePosition.x,ground+sprite.bounds.extents.y+yOffset);
            Vector2 boxSize = (Vector2)sprite.bounds.size - leniency * Vector2.up;

            // assert place position close enough to player
            if (Vector2.Distance(player.bounds.center, placePosition) > maxPlaceDistance) return;

            var result = Physics2D.BoxCast(placePosition, boxSize,0,Vector2.right,0.01f,LayerMask.GetMask("Game"));
            if (result) return;

            // place chest or bench
            hotbar.SelectedSlot.RemoveOne();
            if (selectedItem.Equals("Chest"))
            {
                var chest = Instantiate(chestPrefab, placePosition, new Quaternion(0, 0, 0, 0), interactables);
                chest.name = "Chest";
                chest.GetComponent<Chest>().SetItems(items);
            }
            else
            {
                var bench = Instantiate(benchPrefab, placePosition, new Quaternion(0, 0, 0, 0), interactables);
                bench.name = "CraftingBench";
            }
        }

    }
}
