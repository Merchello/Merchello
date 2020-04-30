using System;

namespace Merchello.Core.Gateways
{
    using Umbraco.Core;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class GatewayProviderEditorAttribute : Attribute
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

        public GatewayProviderEditorAttribute(string title, string description, string editorView)
        {            
            Ensure.ParameterNotNullOrEmpty(title, "title");
            Ensure.ParameterNotNullOrEmpty(description, "description");
            Ensure.ParameterNotNullOrEmpty(editorView, "editorView");

            Title = title;
            Description = description;
            EditorView = editorView;
        }

        public GatewayProviderEditorAttribute(string title, string editorView)
        {
            Ensure.ParameterNotNullOrEmpty(title, "title");
            Ensure.ParameterNotNullOrEmpty(editorView, "editorView");

            Title = title;
            Description = string.Empty;
            EditorView = editorView;
        }
    }
}