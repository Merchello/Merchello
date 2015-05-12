namespace Merchello.Web.Validation
{
    public interface IValidatationHelper
    {
        IBankingValidationHelper Banking { get; }
    }
}