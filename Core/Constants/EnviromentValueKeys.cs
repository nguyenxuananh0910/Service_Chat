namespace chat_app_service.Domain.Exceptions;



/// <summary>
/// Contains all enviroment value keys
/// </summary>
public class EnviromentValueKeys
{
    public static string ENVIRONMENT = "ENVIRONMENT";

    public static string USE_NEW_USER_API = "USE_NEW_USER_API";

    public static string PHONG_KINH_TE_PHONG_BAN_ID = "PHONG_KINH_TE_PHONG_BAN_ID";

    /// <summary>
    /// Logging config keys
    /// </summary>
    public static class LogConfigs
    {
        /// <summary>
        /// Config key for where log file will be saved
        /// </summary>
        public static string SAVE_PATH = "LOG_SAVE_PATH";

        /// <summary>
        /// Config key for minuum log level
        /// </summary>
        public static string MINIMUM_LEVEL = "LOG_MINIMUM_LEVEL";

        /// <summary>
        /// Config key for loging output template
        /// </summary>
        public static string OUTPUT_TEMPLATE = "LOG_OUTPUT_TEMPLATE";
    }

    /// <summary>
    /// Database config keys
    /// </summary>
    public class DatabaseConfigs
    {
        /// <summary>
        /// Application database connection string
        /// </summary>
        public static string CONNECTION_STRING = "DATABASE_CONNECTION_STRING";

        public static string QUARTZ_CONNECTION_STRING = "QUARTZ_CONNECTION_STRING";
    }



    public static class NotificationApiConfigs
    {
        public static string NOTIFICATION_API_URI = "NOTIFICATION_API_URI";
    }
}
