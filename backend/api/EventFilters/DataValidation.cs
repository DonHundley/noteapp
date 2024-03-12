using System.ComponentModel.DataAnnotations;
using Fleck;
using lib;

namespace api.EventFilters;

public class DataValidation : BaseEventFilter
{
    public override Task Handle<T>(IWebSocketConnection socket, T dto)
    {
        var validate = new ValidationContext(dto ?? throw new ArgumentNullException(nameof(dto)));
        Validator.ValidateObject(dto, validate, true);
        return Task.CompletedTask;
    }
}