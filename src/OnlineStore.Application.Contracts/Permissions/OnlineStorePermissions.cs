namespace OnlineStore.Permissions
{
   
    public static class OnlineStorePermissions
    {
       
        public const string GroupName = "OnlineStore";

        /// <summary>
        /// Categories permissions
        /// </summary>
        public static class Categories
        {
            /// <summary>
            /// Base permission for categories (View access)
            /// </summary>
            public const string Default = GroupName + ".Categories";

            /// <summary>
            /// Permission to create new categories
            /// </summary>
            public const string Create = Default + ".Create";

            /// <summary>
            /// Permission to edit existing categories
            /// </summary>
            public const string Edit = Default + ".Edit";

            /// <summary>
            /// Permission to delete categories
            /// </summary>
            public const string Delete = Default + ".Delete";
        }

        /// <summary>
        /// Products permissions
        /// </summary>
        public static class Products
        {
           
            public const string Default = GroupName + ".Products";

            public const string Create = Default + ".Create";

         
            public const string Edit = Default + ".Edit";

            public const string Delete = Default + ".Delete";

            public const string Publish = Default + ".Publish";

            public const string ManageStock = Default + ".ManageStock";
        }
    }
}