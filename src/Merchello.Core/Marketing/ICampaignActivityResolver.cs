namespace Merchello.Core.Marketing
{
    using System;
    using System.Collections.Generic;


    internal interface ICampaignActivityResolver
    {
        IEnumerable<CampaignActivityBase> GetAll(); 
            
        IEnumerable<CampaignActivityBase> GetByCampaignKey(Guid key);

        CampaignActivityBase GetByCampaignActivityKey(Guid key);
    }
}