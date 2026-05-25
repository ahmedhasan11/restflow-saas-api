namespace RestflowAPI.Constants
{
	public class Permissions
	{
		public static class Roles
		{
			public const string SuperAdmin = "SuperAdmin";
			public const string Owner = "Owner";
			public const string Employee = "Employee";
			public const string Cashier = "Cashier";
			public const string Manager = "Manager";
			public const string KitchenStaff = "Kitchen Staff";
			public const string InventoryClerk = "Inventory Clerk";
		}

		public static class Policies
		{
			public const string SuperAdminOnly = "SuperAdminOnly";
			public const string OwnerOnly = "OwnerOnly";
			public const string EmployeeOnly = "EmployeeOnly";
			public const string TenantAccess = "TenantAccess"; // Owner or Employee
		}
	}
}
