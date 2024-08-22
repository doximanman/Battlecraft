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

    private void Start()
    {
        hotbar.OnItemUse += (ItemStack stack)=>
        {

            if (stack == null || !stack.Type.placable) return;
            ItemType stackType = stack.Type;
            // stack is not null and is placable - try to place it.

            // close enough to player and not inside ground
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // get ground levels of x=mousePosition.x
            var groundMaybeNull = Logic.GetGroundHeightBelow(mousePosition);

            // can't get ground
            if (groundMaybeNull == null) return;
            float ground = groundMaybeNull.Value;

            // assert chest/bench can fit
            float yOffset = stackType.placableProps.placeYOffset;
            float maxPlaceDistance = stackType.placableProps.maxPlaceDistance;
            float maxHeightDifference = stackType.placableProps.maxHeightDifference;
            var collider = stackType.placableProps.blockPrefab.GetComponent<BoxCollider2D>();

            Vector2 centerOfCollider = new(mousePosition.x, ground + collider.size.y / 2 + 0.01f);
            Vector2 placePosition = new(mousePosition.x, ground + collider.size.y / 2 + yOffset);
            //Vector2 boxSize = (Vector2)collider.bounds.size - leniency * Vector2.up;
            Vector2 boxSize = collider.size;

            // for gizmos
            _boxCenter = centerOfCollider;
            _boxSize = boxSize;

            BoxCollider2D playerCollider = Player.current.playerCollider;

            // assert place position close enough to player
            if (Vector2.Distance(playerCollider.bounds.center, placePosition) > maxPlaceDistance) return;
            // assert close enough to mouse position
            if (Mathf.Abs(centerOfCollider.y - mousePosition.y) > maxHeightDifference) return;

            var result = Physics2D.BoxCast(centerOfCollider, boxSize, 0, Vector2.zero, 0f, LayerMask.GetMask("Game"));
            if (result) return;

            // place block according to its prefab
            stack.ItemCount--;
            var block = Instantiate(stackType.placableProps.blockPrefab,
                placePosition,
                new Quaternion(0, 0, 0, 0),
                interactables);
            block.name = stackType.name;
        };
    }

    private void Update()
    {
        /*// cant place down a chest when the game is paused
        if (MetaLogic.paused) return;

        if(Input.GetMouseButtonDown(0))
        {
            // assert a chest is selected
            if (hotbar.SelectedSlot.GetStack() == null) return;
            string selectedItem = hotbar.SelectedSlot.GetStack().Type.name;
            if (!selectedItem.Contains("Chest") && !selectedItem.Contains("Bench")) return;
            selectedItem = selectedItem.Contains("Chest") ? "Chest" : "Bench";

            InventoryData items=null;

            // close enough to player and not inside ground
            Vector2 mousePosition=Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // get ground levels of x=mousePosition.x
             var groundMaybeNull = Logic.GetGroundHeightBelow(mousePosition);

            // can't get ground
            if (groundMaybeNull == null) return;
            float ground = groundMaybeNull.Value;

            // assert chest/bench can fit
            float yOffset = selectedItem.Equals("Chest") ? yOffsetChest : yOffsetBench;
            var collider=selectedItem.Equals("Chest") ? chestPrefab.GetComponent<BoxCollider2D>() : benchPrefab.GetComponent<BoxCollider2D>();

            Vector2 centerOfCollider = new(mousePosition.x, ground + collider.size.y / 2 + 0.01f);
            Vector2 placePosition = new(mousePosition.x,ground+collider.size.y/2+yOffset);
            //Vector2 boxSize = (Vector2)collider.bounds.size - leniency * Vector2.up;
            Vector2 boxSize = collider.size;

            // for gizmos
            _boxCenter = centerOfCollider;
            _boxSize = boxSize;

            // assert place position close enough to player
            if (Vector2.Distance(player.bounds.center, placePosition) > maxPlaceDistance) return;
            // assert close enough to mouse position
            if (Mathf.Abs(centerOfCollider.y-mousePosition.y) > maxHeightDifference) return;

            var result = Physics2D.BoxCast(centerOfCollider, boxSize,0,Vector2.zero,0f,LayerMask.GetMask("Game"));
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
*/
    }

    private Vector2 _boxCenter=new();
    private Vector2 _boxSize=new();

    private void OnDrawGizmos()
    {

        Gizmos.color = Color.white;
        Gizmos.DrawCube(_boxCenter, _boxSize);
    }

}
