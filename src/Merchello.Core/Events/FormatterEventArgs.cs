namespace Merchello.Core.Events
{
    using System;

    /// <summary>
    /// The formatter event args.
    /// </summary>
    /// <typeparam name="TFormatter">
    /// The type of the formatter
    /// </typeparam>
    /// <typeparam name="TModel">
    /// The type of the model
    /// </typeparam>
    public class FormatterEventArgs<TFormatter, TModel> : EventArgs 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormatterEventArgs{TFormatter,TModel}"/> class.
        /// </summary>
        /// <param name="formatter">
        /// The formatter.
        /// </param>
        /// <param name="model">
        /// The model.
        /// </param>
        public FormatterEventArgs(TFormatter formatter, TModel model)
        {
            Formatter = formatter;
            Model = model;
        }

        /// <summary>
        /// Gets or sets the formatter.
        /// </summary>
        public TFormatter Formatter { get; set; }

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        public TModel Model { get; set; }
    }
}