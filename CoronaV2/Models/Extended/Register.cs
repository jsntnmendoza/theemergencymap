using CoronaV2.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CoronaV2.Models
{

    [MetadataType(typeof(RegisterMetadata))]

    public partial class Register
    {
        public HttpPostedFileBase ImageFile { get; set; }
    }
    public class RegisterMetadata
    {
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "FieldRequired")]
        [Display(Name = "Location", ResourceType = typeof(Language))]
        public string Latlng { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "FieldRequired")]
        [Display(Name = "City ID")]
        public string CityID { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "FieldRequired")]
        [Display(Name = "Address", ResourceType = typeof(Language))]
        public string Address { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "FieldRequired")]
        [Display(Name = "Message", ResourceType = typeof(Language))]
        [MaxLength(500)]
        public string Message { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "FieldRequired")]
        [Display(Name = "Contact", ResourceType = typeof(Language))]
        [MaxLength(50)]
        public string Contact { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "FieldRequired")]
        [Display(Name = "Marker", ResourceType = typeof(Language))]
        public int MarkerID { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Date", ResourceType = typeof(Language))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "FieldRequired")]
        public System.DateTime CrtdDate { get; set; }
        [Display(Name = "Image", ResourceType = typeof(Language))]
        public string ImagePath { get; set; }
        public int UserID { get; set; }
    }
}