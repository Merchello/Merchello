namespace Merchello.Core.Discounts.Rules
{
    using System;

    using Umbraco.Core;

    /// <summary>
    /// Used to decorate Discount Rule classes that require a back office Angular dialog for additional configuration.
    /// </summary>
    public class DiscountRuleEditorAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DiscountRuleEditorAttribute"/> class.
        /// </summary>
        /// <param name="editorView">
        /// The editor view.
        /// </param>
        public DiscountRuleEditorAttribute(string editorView)
        {
            Mandate.ParameterNotNullOrEmpty(editorView, "editorView");

            this.EditorView = editorView;
        }

        /// <summary>
        /// Gets or sets the editor view.
        /// </summary>
        public string EditorView { get; set; }
    }
}