namespace Merchello.Plugin.Taxation.Avalara.Models
{
    //// These are in general a direct copy of enums found in https://github.com/avadev/AvaTax-Calc-REST-csharp/blob/master/AvaTaxClasses/
    //// and is subject to https://github.com/avadev/AvaTax-Calc-REST-csharp/blob/master/LICENSE.md

    public enum DetailLevel
    {
        Tax,
        Document,
        Line,
        Diagnostic
    }

    public enum CancelCode
    {
        Unspecified,
        PostFailed,
        DocDeleted,
        DocVoided,
        AdjustmentCancelled
    }

    /// <summary>
    /// The type of statement taxes are to be applied to.
    /// </summary>
    /// <remarks>
    /// This enum is ranamed to avoid confusion with an Umbraco "doc type".
    /// </remarks>
    public enum StatementType
    {
        SalesOrder, // this is a quote
        SalesInvoice, // this is to record the sale
        ReturnOrder,
        ReturnInvoice,
        PurchaseOrder,
        PurchaseInvoice,
        ReverseChargeOrder,
        ReverseChargeInvoice
    }

    public enum SystemCustomerUsageType
    {
        L, // "Other",
        A, // "Federal government",
        B, // "State government",
        C, // "Tribe / Status Indian / Indian Band",
        D, // "Foreign diplomat",
        E, // "Charitable or benevolent organization",
        F, // "Regligious or educational organization",
        G, // "Resale",
        H, // "Commercial agricultural production",
        I, // "Industrial production / manufacturer",
        J, // "Direct pay permit",
        K, // "Direct Mail",
        N, // "Local Government",
        P, // "Commercial Aquaculture",
        Q, // "Commercial Fishery",
        R // "Non-resident"
    }
}