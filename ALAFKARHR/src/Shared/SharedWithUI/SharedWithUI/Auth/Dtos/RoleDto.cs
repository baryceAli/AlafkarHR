using System;
using System.Collections.Generic;
using System.Text;

namespace SharedWithUI.Auth.Dtos
{
    public class RoleDto
    {
        public string RoleName { get; set; }
        public Guid CompanyId { get; set; }
        public List<string> Permissions { get; set; }
    }
}
