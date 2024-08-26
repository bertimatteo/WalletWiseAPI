namespace WalletWise.Repository
{
    public class Constants
    {
        // USER 
        public const string GET_USER_BY_USERNAME_SP                    = "[User].[GetUserByUsername]";
        public const string GET_USER_CREDENTIAL_RECOVERY_BY_USER_ID_SP = "[User].[GetUserCredentialRecoveryByUserId]";

        public const string INSERT_USER_SP                     = "[User].[InsertUser]";
        public const string INSERT_USER_CREDENTIAL_RECOVERY_SP = "[User].[InsertUserCredentialRecovery]";

        public const string UPDATE_USER_SP                     = "[User].[UpdateUser]";
        public const string UPDATE_USER_CREDENTIAL_RECOVERY_SP = "[User].[UpdateUserCredentialRecovery]";

        // BALANCE
        public const string GET_CATEGORIES_BY_USER_SP = "[Balance].[GetCategoriesByUser]";
        public const string GET_ITEMS_SP              = "[Balance].[GetItems]";
        public const string GET_ITEM_BY_ID_SP         = "[Balance].[GetItemById]";

        public const string INSERT_CATEGORY_SP = "[Balance].[InsertCategory]";
        public const string INSERT_ITEM_SP     = "[Balance].[InsertItem]";

        public const string UPDATE_CATEGORY_SP = "[Balance].[UpdateCategory]";
        public const string UPDATE_ITEM_SP     = "[Balance].[UpdateItem]";

        public const string DELETE_ITEM_SP = "[Balance].[DeleteItem]";
    }
}
