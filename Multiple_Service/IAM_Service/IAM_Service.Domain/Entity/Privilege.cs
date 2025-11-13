namespace IAM_Service.Domain.Entity
{
    /// <summary>
    /// Represents an atomic permission that can be granted to a role.
    /// </summary>
    public class Privilege
    {
        /// <summary>Primary key.</summary>
        public int PrivilegeId { get; set; }
        /// <summary>Display name of the privilege.</summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>Optional description for documentation.</summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>Many-to-many link to roles through the join entity.</summary>
        public ICollection<RolePrivilege> RolePrivileges { get; set; } = new List<RolePrivilege>();
    }
}


