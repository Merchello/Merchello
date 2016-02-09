namespace Merchello.Web.PropertyEditors
{
    using Umbraco.Core.PropertyEditors;

    /// <summary>
    /// The Multi Product Picker.
    /// </summary>
    [PropertyEditor("Merchello.MultiProductPicker", "Merchello Multi-Product Picker", "../App_Plugins/Merchello/propertyeditors/multiproductpicker/merchello.multiproductpicker.html", ValueType = "JSON")]
    public class MultiProductPicker : PropertyEditor
    {
    }
}