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
    
    public partial class SIPDialplanOption
    {
        public string AllowedCountryCodes { get; set; }
        public Nullable<int> AreaCode { get; set; }
        public Nullable<int> CountryCode { get; set; }
        public string ENUMServers { get; set; }
        public string ExcludedPrefixes { get; set; }
        public string ID { get; set; }
        public string Owner { get; set; }
        public string Timezone { get; set; }
        public string WhitepagesKey { get; set; }
        public bool EnableSafeguards { get; set; }
        public string DialPlanID { get; set; }
    
        public virtual SIPDialPlan sipdialplan { get; set; }
    }
}
