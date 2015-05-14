namespace Merchello.Tests.UnitTests.Discounts
{
    using System;

    public class DiscountRuleEditorAttribute : Attribute
    {
        

        public DiscountRuleEditorAttribute(string editorView)
        {
            this.EditorView = editorView;
        }
        
        public string EditorView { get; set; }
    }
}