namespace Merchello.Core.Marketing.Offer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// A base class for offer redemption results.
    /// </summary>
    /// <typeparam name="TAward">
    /// The type of the offer award
    /// </typeparam>
    public abstract class OfferRedemptionResultBase<TAward> : IOfferRedemptionResult<TAward>
        where TAward : class
    {
        /// <summary>
        /// The list of messages.
        /// </summary>
        private readonly List<string> _message = new List<string>(); 
         
        /// <summary>
        /// Initializes a new instance of the <see cref="OfferRedemptionResultBase{TAward}"/> class for success. 
        /// </summary>
        /// <param name="award">
        /// The award.
        /// </param>
        /// <param name="messages">
        /// The messages.
        /// </param>
        protected OfferRedemptionResultBase(TAward award, IEnumerable<string> messages = null)
        {
            Ensure.ParameterNotNull(award, "award");
            Award = award;
            Success = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferRedemptionResultBase{TAward}"/> class for fail. 
        /// </summary>
        /// <param name="exception">
        /// The exception.
        /// </param>
        /// <param name="messages">
        /// The messages.
        /// </param>
        protected OfferRedemptionResultBase(Exception exception, IEnumerable<string> messages = null)
        {
            Ensure.ParameterNotNull(exception, "exception");            
            Exception = exception;
            Success = false;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the offer application was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the award.
        /// </summary>
        /// <remarks>
        /// Can be null on exception
        /// </remarks>
        public TAward Award { get; set; }

        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Gets the messages.
        /// </summary>
        public IEnumerable<string> Messages 
        {
            get
            {
                return _message;
            } 
        }

        /// <summary>
        /// Adds a message.
        /// </summary>
        /// <param name="msg">
        /// The message.
        /// </param>
        public void AddMessage(string msg)
        {
            _message.Add(msg);
        }

        /// <summary>
        /// Adds a collection of messages.
        /// </summary>
        /// <param name="messages">
        /// The messages.
        /// </param>
        public void AddMessage(IEnumerable<string> messages)
        {
            _message.AddRange(Messages);
        }
    }
}