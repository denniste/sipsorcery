//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SIPSorcery.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class PayPalIPN
    {
        public string ID { get; set; }
        public string RawRequest { get; set; }
        public string ValidationResponse { get; set; }
        public bool IsSandbox { get; set; }
        public string TransactionID { get; set; }
        public string TransactionType { get; set; }
        public string PaymentStatus { get; set; }
        public string PayerFirstName { get; set; }
        public string PayerLastName { get; set; }
        public string PayerEmailAddress { get; set; }
        public string Currency { get; set; }
        public Nullable<decimal> Total { get; set; }
        public Nullable<decimal> PayPalFee { get; set; }
        public System.DateTime Inserted { get; set; }
        public string ItemId { get; set; }
        public string CustomerID { get; set; }
        public string ActionTaken { get; set; }
    }
}
