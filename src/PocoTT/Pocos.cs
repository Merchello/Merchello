using System;
using Merchello.Core.Persistence;

namespace Merchello.Core.Model.Rdbms
{
    
	[TableName("merchInventory")]
	[PrimaryKey("warehouseId", autoIncrement=false)]
	[ExplicitColumns]
    public class InventoryDto
    {
		[Column("warehouseId")] 
        public int WarehouseId { get; set; }

		[Column("sku")] 
        public string Sku { get; set; }

		[Column("count")] 
        public int Count { get; set; }

		[Column("lowCount")] 
        public int LowCount { get; set; }

		[Column("updateDate")] 
        public DateTime UpdateDate { get; set; }

		[Column("createDate")] 
        public DateTime CreateDate { get; set; }

	}
    
	[TableName("merchInvoice")]
	[PrimaryKey("id")]
	[ExplicitColumns]
    public class InvoiceDto
    {
		[Column("id")] 
        public int Id { get; set; }

		[Column("customerId")] 
        public int CustomerId { get; set; }

		[Column("invoiceNumber")] 
        public string InvoiceNumber { get; set; }

		[Column("invoiceDate")] 
        public DateTime InvoiceDate { get; set; }

		[Column("invoiceStatusId")] 
        public int InvoiceStatusId { get; set; }

		[Column("billToName")] 
        public string BillToName { get; set; }

		[Column("billToAddress1")] 
        public string BillToAddress1 { get; set; }

		[Column("billToAddress2")] 
        public string BillToAddress2 { get; set; }

		[Column("billToLocality")] 
        public string BillToLocality { get; set; }

		[Column("billToRegion")] 
        public string BillToRegion { get; set; }

		[Column("billToPostalCode")] 
        public string BillToPostalCode { get; set; }

		[Column("billToCountryCode")] 
        public string BillToCountryCode { get; set; }

		[Column("billToEmail")] 
        public string BillToEmail { get; set; }

		[Column("billToPhone")] 
        public string BillToPhone { get; set; }

		[Column("billToCompanyName")] 
        public string BillToCompanyName { get; set; }

		[Column("exported")] 
        public bool Exported { get; set; }

		[Column("paid")] 
        public bool Paid { get; set; }

		[Column("shipped")] 
        public bool Shipped { get; set; }

		[Column("total")] 
        public decimal Total { get; set; }

		[Column("updateDate")] 
        public DateTime UpdateDate { get; set; }

		[Column("createdDate")] 
        public DateTime CreatedDate { get; set; }

	}
    
	[TableName("merchInvoiceItem")]
	[PrimaryKey("id")]
	[ExplicitColumns]
    public class InvoiceItemDto
    {
		[Column("id")] 
        public int Id { get; set; }

		[Column("parentId")] 
        public int? ParentId { get; set; }

		[Column("invoiceId")] 
        public int InvoiceId { get; set; }

		[Column("updateDate")] 
        public DateTime UpdateDate { get; set; }

		[Column("createDate")] 
        public DateTime CreateDate { get; set; }

	}
    
	[TableName("merchInvoiceStatus")]
	[PrimaryKey("id", autoIncrement=false)]
	[ExplicitColumns]
    public class InvoiceStatusDto
    {
		[Column("id")] 
        public int Id { get; set; }

		[Column("name")] 
        public string Name { get; set; }

		[Column("sortOrder")] 
        public int SortOrder { get; set; }

	}
    
	[TableName("merchPayment")]
	[PrimaryKey("id")]
	[ExplicitColumns]
    public class PaymentDto
    {
		[Column("id")] 
        public int Id { get; set; }

		[Column("invoiceId")] 
        public int InvoiceId { get; set; }

		[Column("memberId")] 
        public int MemberId { get; set; }

		[Column("userId")] 
        public int? UserId { get; set; }

		[Column("gatewayAlias")] 
        public string GatewayAlias { get; set; }

		[Column("amount")] 
        public decimal Amount { get; set; }

		[Column("exported")] 
        public bool Exported { get; set; }

		[Column("updateDate")] 
        public DateTime UpdateDate { get; set; }

		[Column("createDate")] 
        public DateTime CreateDate { get; set; }

	}
    
	[TableName("merchPaymentTransaction")]
	[PrimaryKey("id", autoIncrement=false)]
	[ExplicitColumns]
    public class PaymentTransactionDto
    {
		[Column("id")] 
        public int Id { get; set; }

		[Column("paymentId")] 
        public int PaymentId { get; set; }

		[Column("type")] 
        public int Type { get; set; }

		[Column("description")] 
        public string Description { get; set; }

		[Column("amount")] 
        public decimal Amount { get; set; }

		[Column("exported")] 
        public bool Exported { get; set; }

		[Column("updateDate")] 
        public DateTime UpdateDate { get; set; }

		[Column("createDate")] 
        public DateTime CreateDate { get; set; }

	}
    
	[TableName("merchShipment")]
	[PrimaryKey("id", autoIncrement=false)]
	[ExplicitColumns]
    public class ShipmentDto
    {
		[Column("id")] 
        public int Id { get; set; }

		[Column("invoiceId")] 
        public int InvoiceId { get; set; }

		[Column("address1")] 
        public string Address1 { get; set; }

		[Column("address2")] 
        public string Address2 { get; set; }

		[Column("locality")] 
        public string Locality { get; set; }

		[Column("region")] 
        public string Region { get; set; }

		[Column("postalCode")] 
        public string PostalCode { get; set; }

		[Column("countryCode")] 
        public string CountryCode { get; set; }

		[Column("shipMethodId")] 
        public int ShipMethodId { get; set; }

		[Column("phone")] 
        public string Phone { get; set; }

		[Column("updateDate")] 
        public DateTime UpdateDate { get; set; }

		[Column("createDate")] 
        public DateTime CreateDate { get; set; }

	}
    
	[TableName("merchShipMethod")]
	[PrimaryKey("id", autoIncrement=false)]
	[ExplicitColumns]
    public class ShipMethodDto
    {
		[Column("id")] 
        public int Id { get; set; }

		[Column("name")] 
        public string Name { get; set; }

		[Column("gatewayAlias")] 
        public int GatewayAlias { get; set; }

		[Column("type")] 
        public string Type { get; set; }

		[Column("surcharge")] 
        public decimal Surcharge { get; set; }

		[Column("serviceCode")] 
        public string ServiceCode { get; set; }

		[Column("updateDate")] 
        public DateTime UpdateDate { get; set; }

		[Column("createDate")] 
        public DateTime CreateDate { get; set; }

	}
    
	[TableName("merchShipMethod2Warehouse")]
	[PrimaryKey("shipMethodId", autoIncrement=false)]
	[ExplicitColumns]
    public class ShipMethod2WarehouseDto
    {
		[Column("shipMethodId")] 
        public int ShipMethodId { get; set; }

		[Column("warehouseId")] 
        public int WarehouseId { get; set; }

	}
    
	[TableName("merchWarehouse")]
	[PrimaryKey("id", autoIncrement=false)]
	[ExplicitColumns]
    public class WarehouseDto
    {
		[Column("id")] 
        public int Id { get; set; }

		[Column("name")] 
        public string Name { get; set; }

		[Column("address1")] 
        public string Address1 { get; set; }

		[Column("address2")] 
        public string Address2 { get; set; }

		[Column("locality")] 
        public string Locality { get; set; }

		[Column("region")] 
        public string Region { get; set; }

		[Column("postalCode")] 
        public string PostalCode { get; set; }

		[Column("updateDate")] 
        public DateTime? UpdateDate { get; set; }

		[Column("createDate")] 
        public DateTime? CreateDate { get; set; }

	}
    
	[TableName("merchCustomer")]
	[PrimaryKey("id", autoIncrement=false)]
	[ExplicitColumns]
    public class CustomerDto
    {
		[Column("id")] 
        public int Id { get; set; }

		[Column("memberId")] 
        public int? MemberId { get; set; }

		[Column("firstName")] 
        public string FirstName { get; set; }

		[Column("lastName")] 
        public string LastName { get; set; }

		[Column("totalInvoiced")] 
        public decimal TotalInvoiced { get; set; }

		[Column("totalPayments")] 
        public decimal TotalPayments { get; set; }

		[Column("lastPaymentDate")] 
        public DateTime LastPaymentDate { get; set; }

	}
    
	[TableName("merchCustomerAddress")]
	[PrimaryKey("id", autoIncrement=false)]
	[ExplicitColumns]
    public class CustomerAddressDto
    {
		[Column("id")] 
        public int Id { get; set; }

		[Column("customerId")] 
        public int? CustomerId { get; set; }

		[Column("label")] 
        public string Label { get; set; }

		[Column("fullName")] 
        public string FullName { get; set; }

		[Column("company")] 
        public string Company { get; set; }

		[Column("type")] 
        public int? Type { get; set; }

		[Column("address1")] 
        public string Address1 { get; set; }

		[Column("address2")] 
        public string Address2 { get; set; }

		[Column("locality")] 
        public string Locality { get; set; }

		[Column("region")] 
        public string Region { get; set; }

		[Column("postalCode")] 
        public string PostalCode { get; set; }

		[Column("phone")] 
        public string Phone { get; set; }

		[Column("updateDate")] 
        public DateTime UpdateDate { get; set; }

		[Column("createDate")] 
        public DateTime CreateDate { get; set; }

	}
}
