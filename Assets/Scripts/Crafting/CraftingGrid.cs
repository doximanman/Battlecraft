using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

public class CraftingGrid : MonoBehaviour
{
    public Matrix<InventorySlot> inSlots;
    public InventorySlot outSlot;

    [SerializeField] private CraftingRecipes recipes;

    private CraftingRecipe currentRecipe = null;
    // Start is called before the first frame update
    void Start()
    {

        for (int i = 0; i < inSlots.Count(); i++)
        {
            for (int j = 0; j < inSlots[i].Count; j++)
            {
                inSlots[i][j].slotChangeListeners += (oldStack, newStack) =>
                {
                    currentRecipe = CheckRecipes();
                    ShowRecipe(currentRecipe);
                };
            }
        }
        outSlot.slotChangeListeners += (oldStack, newStack) =>
        {
            if (currentRecipe == null) return;
            if (oldStack == null && newStack == null) return;
            else if (oldStack != null && newStack == null)
            {
                // if crafting is possible, then necessarily
                // crafting was requested by the user
                if (CheckRecipe(currentRecipe))
                {
                    CraftSome(currentRecipe, oldStack.count / currentRecipe.outItem.count);
                }
                else
                {
                    Debug.Log("impossible case in CraftingGrid outSlot listener");
                }
            }
            else if (oldStack == null && newStack != null)
            {
                // just means a new crafting recipe was shown.
                // nothing should be crafted.
            }
            else
            {
                // this is just releasing shift/crafting partially
                if (newStack.count == currentRecipe.outItem.count)
                    return;
                // count didnt change or increased
                if (oldStack.count <= newStack.count) return;
                // if however many i can craft right now
                // minus the difference in the old and new counts (divided by how many items
                // each recipe provides)
                // is bigger than 0 then necessarily the user
                // crafted the difference
                int toCraft = Mathf.Max(currentRecipe.HowManyCraft(ToData()) - (oldStack.count - newStack.count) / currentRecipe.outItem.count, 0);
                if (toCraft > 0)
                {
                    CraftSome(currentRecipe, toCraft);
                }

            }
        };
    }

    public void ShowRecipe(CraftingRecipe recipe)
    {
        if (recipe == null)
            outSlot.RemoveItem();
        else
            outSlot.SetItem(recipe.outItem);
    }

    // logical only
    public bool CheckRecipe(CraftingRecipe recipe)
    {

        // check if the recipe can be crafted
        return recipe.CanCraft(ToData());
    }

    // logical only
    public CraftingRecipe CheckRecipes()
    {
        foreach (var recipe in recipes)
        {
            if (CheckRecipe(recipe))
                return recipe;
        }
        return null;
    }

    public void CraftSome(CraftingRecipe recipe, int count)
    {
        for (int i = 0; i < recipe.inItems.Count(); i++)
        {
            for (int j = 0; j < recipe.inItems[i].Count; j++)
            {
                if (inSlots[i][j] != null)
                    inSlots[i][j].RemoveSome(recipe.inItems[i][j].count * count);
            }
        }

        shiftPressed = false;
    }

    public void Craft(CraftingRecipe recipe)
    {
        for (int i = 0; i < recipe.inItems.Count(); i++)
        {
            for (int j = 0; j < recipe.inItems[i].Count; j++)
            {
                if (inSlots[i][j].GetStack() != null)
                {
                    inSlots[i][j].RemoveSome(recipe.inItems[i][j].count);
                }
            }
        }
    }

    private bool shiftPressed = false;
    private float lastClicked;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (!shiftPressed)
            {
                shiftPressed = true;
                // show maximum amount that can be crafted
                if (currentRecipe == null) return;
                int craftCount = currentRecipe.HowManyCraft(ToData());
                int itemCount = Mathf.Min(currentRecipe.outItem.type.maxStack, craftCount * currentRecipe.outItem.count);
                outSlot.AddSome(itemCount - outSlot.GetStack().ItemCount);
            }
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            shiftPressed = false;
            // show only one craft
            if (currentRecipe == null) return;
            outSlot.RemoveSome(outSlot.GetStack().ItemCount - currentRecipe.outItem.count);
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (Time.realtimeSinceStartup - lastClicked < MetaLogic.doubleClickDelay)
            {
                // double click
                // only do something if the inventory is open
                // and outSlot has an item
                if (!InventoryLogic.inventoryIsOpen || outSlot.GetStack() == null) return;

                InventorySlot slot;
                // raycast to check which slot was clicked
                //r = new Ray(Input.mousePosition + Vector3.back, Vector3.forward);
                var eventData = new PointerEventData(EventSystem.current)
                {
                    position = Input.mousePosition
                };
                List<RaycastResult> hit = new();
                EventSystem.current.RaycastAll(eventData, hit);
                foreach (var collider in hit)
                {
                    if (collider.gameObject.GetComponent<InventorySlot>() != null)
                    {
                        slot = collider.gameObject.GetComponent<InventorySlot>();
                        if (slot != outSlot)
                            return;
                    }
                }
                // the output of the grid was clicked
                // try to move to the inventory
                Inventory.MoveItem(outSlot, InventoryLogic.personalInventory);
            }
            else
            {
                // single click
                lastClicked = Time.realtimeSinceStartup;
            }
        }
    }

    public IEnumerable<IEnumerable<StackData>> ToData()
    {
        var result = new List<List<StackData>>();
        for(int i = 0; i < inSlots.Count(); i++)
        {
            result.Add(new(inSlots[i].Count));
            for(int j = 0; j < inSlots[i].Count; j++)
            {
                if (inSlots[i][j].GetStack() == null) result[i].Add(null);
                else result[i].Add(new StackData(inSlots[i][j].GetStack()));
            }
        }
        return result;
    }
}
