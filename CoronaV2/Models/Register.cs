//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CoronaV2.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Register
    {
        public int RegisterID { get; set; }
        public int UserID { get; set; }
        public string Latlng { get; set; }
        public string Message { get; set; }
        public int MarkerID { get; set; }
        public string Address { get; set; }
        public int CityID { get; set; }
        public System.DateTime CrtdDate { get; set; }
        public bool Active { get; set; }
        public string ImagePath { get; set; }
        public string Contact { get; set; }
    
        public virtual Marker Marker { get; set; }
        public virtual User User { get; set; }
    }
}