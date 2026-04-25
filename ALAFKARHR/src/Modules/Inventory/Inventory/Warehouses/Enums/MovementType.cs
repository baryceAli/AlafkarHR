namespace Inventory.Warehouses.Enums;

/// <summary>
/// Enum for stock movement types in the Inventory module
/// </summary>
public enum MovementType
{
    /// <summary>Purchase or supplier return</summary>
    IN = 0,

    /// <summary>Sales or customer order</summary>
    OUT = 1,

    /// <summary>Manual stock correction/adjustment</summary>
    ADJUSTMENT = 2,

    /// <summary>Transfer out from this warehouse</summary>
    TRANSFER_OUT = 3,

    /// <summary>Transfer in to this warehouse</summary>
    TRANSFER_IN = 4,

    /// <summary>Order pending - stock reserved</summary>
    RESERVATION = 5,

    /// <summary>Cancel reservation - release reserved stock</summary>
    RELEASE = 6,
    /// <summary>Add Initial Quantity when creating inventory - Initial stock</summary>
    OPENING = 7,
    REMOVAL = 8
}
