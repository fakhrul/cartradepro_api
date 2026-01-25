namespace SPOT_API.Models
{
    /// <summary>
    /// Permission types for module and submodule access control
    /// </summary>
    public enum PermissionType
    {
        /// <summary>
        /// Can view/read data
        /// </summary>
        View = 1,

        /// <summary>
        /// Can create new records
        /// </summary>
        Add = 2,

        /// <summary>
        /// Can update existing records
        /// </summary>
        Update = 3,

        /// <summary>
        /// Can delete records
        /// </summary>
        Delete = 4
    }
}
