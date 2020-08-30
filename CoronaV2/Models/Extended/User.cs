using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using CoronaV2.Resources;

namespace CoronaV2.Models
{
    [MetadataType(typeof(UserMetadata))]
    public partial class User
    {
        public string ConfirmPassword { get; set; }
    }

    public class UserMetadata
    {
        [Display(Name = "FirstName", ResourceType = typeof(Language))]
        [Required(AllowEmptyStrings =false,ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "FieldRequired")]
        public string FirstName { get; set; }

        [Display(Name = "LastName", ResourceType = typeof(Language))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "FieldRequired")]
        public string LastName { get; set; }

        [Display(Name = "DocumentID", ResourceType = typeof(Language))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "FieldRequired")]
        public string DocumentID { get; set; }

        [Display(Name ="Email")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "FieldRequired")]
        [DataType(DataType.EmailAddress)]
        public string EmailID { get; set; }

        [Display(Name = "DateOfBirth", ResourceType = typeof(Language))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "FieldRequired")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode =true,DataFormatString ="{0:dd/MM/yyyy}")]
        public string DateOfBirth { get; set; }

        [Display(Name = "Country", ResourceType = typeof(Language))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "FieldRequired")]
        public string CountryID { get; set; }

        [Display(Name = "Password", ResourceType = typeof(Language))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "FieldRequired")]
        [DataType(DataType.Password)]
        [MinLength(6,ErrorMessage ="Minimun 6 characters required")]
        public string Password { get; set; }

        [Display(Name = "ConfirmPassword", ResourceType = typeof(Language))]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "FieldIncorrect")]
        public string ConfirmPassword { get; set; }

    }
}