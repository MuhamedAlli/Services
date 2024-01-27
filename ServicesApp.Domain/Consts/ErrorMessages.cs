namespace ServicesApp.Domain.Consts
{
    public static class ErrorMessages
    {
        public const string failed = "Failed";

        //Account Controller Error Messages
        public const string somthingWentWrong = "Somthing went wrong!";
        public const string maxImageLength = "Max length must be less than 2MB!";
        public const string validImageExtensions = ".Jpg , .Png , .Jpeg just is allowed!";
        public const string imageFeildRequired = "Image field is required!";
        public const string appUserIdRequired = "Application User Id field is required!";
        public const string userNotFound = "This user is not found!";
        public const string tokenIsRequird = "Token is required!";
        public const string tokenInvalid = "Token is Invalid!";
        public const string usersNotFound = "Users not found!";

        public const string invalidPageOrPageSize = "Invalid page or pageSize values";
        public const string invalidInput = "Invalid search value!";

        public const string serviceIdRequired = "Service Id field is required!";

        public const string serviceNotFound = "This service is not found!";
        public const string providerNotFound = "This provider is not found!";
        public const string clientNotFound = "This client is not found!";



    }
}
