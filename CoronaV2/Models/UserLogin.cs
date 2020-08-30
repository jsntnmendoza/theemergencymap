using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using CoronaV2.Resources;

namespace CoronaV2.Models
{
    public class UserLogin
    {
        //[Display(Name = "User", ResourceType = typeof(Language))]
        [Display(Name = "Email")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "FieldRequired")]
        public string EmailID { get; set; }

        [Display(Name ="Password",ResourceType = typeof(Language))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "FieldRequired")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "RememberMe", ResourceType = typeof(Language))]
        public bool RememberMe { get; set; }
    }
}