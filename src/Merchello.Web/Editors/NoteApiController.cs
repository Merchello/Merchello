namespace Merchello.Web.Editors
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;

    using Merchello.Core;
    using Merchello.Core.Services;
    using Merchello.Web.Models.ContentEditing;
    using Merchello.Web.WebApi;

    using Umbraco.Core;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// The note api controller.
    /// </summary>
    [PluginController("Merchello")]
    public class NoteApiController : MerchelloApiController
    {
        /// <summary>
        /// The note service.
        /// </summary>
        private readonly INoteService _noteService;

        /// <summary>
        /// The <see cref="MerchelloHelper"/>.
        /// </summary>
        private readonly MerchelloHelper _merchelloHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="NoteApiController"/> class.
        /// </summary>
        public NoteApiController()
            : this(Core.MerchelloContext.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NoteApiController"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        public NoteApiController(IMerchelloContext merchelloContext)
            : base(merchelloContext)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");
            _noteService = merchelloContext.Services.NoteService;
            _merchelloHelper = new MerchelloHelper();
        }

        /// <summary>
        /// The get by entity key.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        [HttpGet]
        public IEnumerable<NoteDisplay> GetByEntityKey(Guid id)
        {
            return _noteService.GetNotesByEntityKey(id).Select(x => x.ToNoteDisplay());
        }

    }
}