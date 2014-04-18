using System;

namespace Merchello.Core.Gateways
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class GatewayMethodEditorAttribute : Attribute 
    {
           /// <summary>
        /// The name of the gateway provider editor title  
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// The description of the gateway provider editor 
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// The relative path to the editor view html
        /// </summary>
        public string EditorView { get; private set; }

        public GatewayMethodEditorAttribute(string title, string description, string editorView)
        {            
            Mandate.ParameterNotNullOrEmpty(title, "title");
            Mandate.ParameterNotNullOrEmpty(description, "description");
            Mandate.ParameterNotNullOrEmpty(editorView, "editorView");

            Title = title;
            Description = description;
            EditorView = editorView;
        }

        public GatewayMethodEditorAttribute(string title, string editorView)
        {
            Mandate.ParameterNotNullOrEmpty(title, "title");
            Mandate.ParameterNotNullOrEmpty(editorView, "editorView");

            Title = title;
            Description = string.Empty;
            EditorView = editorView;
        } 
    }
}