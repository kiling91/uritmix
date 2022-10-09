using System.Collections.Generic;
using System.Linq;

namespace Helpers.DataAccess;

/// <summary>
///     Syncronization result model
/// </summary>
/// <typeparam name="TItem">The type of the items.</typeparam>
public class SyncronizationResultModel<TItem>
{
    /// <summary>
    ///     New items
    /// </summary>
    public List<TItem> NewItems { get; set; }

    /// <summary>
    ///     Updated items
    /// </summary>
    public List<UpdatedModel<TItem>> UpdatedItems { get; set; }

    /// <summary>
    ///     Deleted items
    /// </summary>
    public List<TItem> DeletedItems { get; set; }

    /// <summary>
    ///     Gets all new or updated items
    /// </summary>
    public List<TItem> ExistingItems => NewItems.Concat(UpdatedItems.Select(updated => updated.New)).ToList();
}