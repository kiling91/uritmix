namespace Helpers.DataAccess;

/// <summary>
///     Representes updated entity container
/// </summary>
/// <typeparam name="TModel">The type of the entity.</typeparam>
public class UpdatedModel<TModel>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="UpdatedModel{TModel}" /> class.
    /// </summary>
    /// <param name="new">The new model data.</param>
    /// <param name="old">The old model data.</param>
    public UpdatedModel(TModel @new, TModel old)
    {
        New = @new;
        Old = old;
    }

    /// <summary>
    ///     Updated model
    /// </summary>
    public TModel New { get; }

    /// <summary>
    ///     Model before updating
    /// </summary>
    public TModel Old { get; }

    /// <summary>
    ///     Deconstructs this object
    /// </summary>
    /// <param name="new">The new model data.</param>
    /// <param name="old">The old model data.</param>
    public void Deconstruct(out TModel @new, out TModel old)
    {
        @new = New;
        old = Old;
    }
}