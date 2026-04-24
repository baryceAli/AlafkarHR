using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SharedWithUI.Auth.Dtos
{
    public class RoleDto
    {
        [Required(ErrorMessage ="Role name is required")]
        public string RoleName { get; set; }


        [Required(ErrorMessage ="Company is required")]
        public Guid? CompanyId { get; set; }
        public List<string> Permissions { get; set; }
    }
}
