namespace Models.Constants
{
    public static class Constants
    {
        public const long MaxModFileSize = 10 * 1024 * 1024;
        public const long MaxImageSize = 5 * 1024 * 1024;
        public static readonly string[] Types = { "VEHICLE", "ENVIRONMENT", "UI", "PACK", "MISC" };
        public const string NotOwnerMessage = "You do not have permission to modify this resource";
    }
}