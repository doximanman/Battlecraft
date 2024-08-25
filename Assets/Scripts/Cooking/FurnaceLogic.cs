using UnityEngine;

public class FurnaceLogic : MonoBehaviour
{
    private static Furnace furnace;

    [InspectorName("Furnace")]
    [SerializeField] private Furnace _furnace;

    // how many times per second will the fuel level update (times/second)
    [SerializeField] private float _fuelUpdateRate;
    public static float fuelUpdateRate;
    // how fast the level will update (amount lost/second)
    [SerializeField] private float _fuelUsageSpeed;
    public static float fuelUsageSpeed;

    #region Default Values

    [SerializeField] private float _defaultMinFuel;
    [SerializeField] private float _defaultMaxFuel;

    public static FurnaceProperties defaultProperties;

    [SerializeField] private float _defaultFuel;
    [SerializeField] private float _defaultCookProgress;
    [SerializeField] private SlotData _defaultFuelSlot;
    [SerializeField] private SlotData _defaultItemSlot;
    [SerializeField] private SlotData _defaultOutSlot;

    public static FurnaceState defaultState;

    #endregion

    public static bool enableFurnace = false;

    public delegate void FurnaceListener(bool open);
    /// <summary>
    /// notify when furnace is opened/closed
    /// </summary>
    public static FurnaceListener furnaceListeners;

    private void Start()
    {
        furnace = _furnace;
        fuelUpdateRate = _fuelUpdateRate;
        fuelUsageSpeed = _fuelUsageSpeed;
        defaultProperties = new(_defaultMinFuel, _defaultMaxFuel);
        defaultState = new(defaultProperties,_defaultFuel, _defaultCookProgress, _defaultFuelSlot, _defaultItemSlot, _defaultOutSlot);

        furnace.gameObject.SetActive(false);

        InventoryLogic.invListeners += (on) =>
        {
            // it is my responsibility to handle the second inventory
            if (InventoryLogic.responsible != InventoryLogic.Responsible.FURNACE) return;

            // if the furnace is enabled - notify when the inventory opens and closes
            if (enableFurnace && on)
            {
                furnaceListeners?.Invoke(true);
                furnace.gameObject.SetActive(true);
                InventoryLogic.externalInventory.SetSlots(furnace.GetComponent<SlotList>().slots);
            }
            else
            {
                furnaceListeners?.Invoke(false);
                furnace.gameObject.SetActive(false);
            }
        };
    }

    public static void SetProperties(FurnaceProperties props)
    {
        furnace.Props = props;
    }

    public static FurnaceProperties GetProperties()
    {
        return furnace.Props;
    }

    public static void LoadState(FurnaceState state)
    {
        furnace.LoadState(state);
    }

    public static void SetProps(FurnaceProperties props)
    {
        furnace.Props = props;
    }

    public static void UnloadState()
    {
        furnace.UnloadState();
    }

    public static void EnableFurnace()
    {
        enableFurnace = true;
        InventoryLogic.responsible = InventoryLogic.Responsible.FURNACE;
    }

    public static void DisableFurnace()
    {
        enableFurnace = false;
        InventoryLogic.responsible = InventoryLogic.defaultResponsible;
    }
}
