
namespace chat_app_service.Domain.Exceptions;

/// <summary>
/// An extension class for get .env variable
/// </summary>
public static partial class EnvironmentExt
{
    /// <summary>
    /// Get variable from .env file. If not found, return default value.
    /// 
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    public static T getVariable<T>(string key)
    {
        var type = typeof(T);
        return type.Name switch
        {
            nameof(String) => (T)Convert.ChangeType(DotNetEnv.Env.GetString(key, string.Empty), type),
            nameof(Boolean) => (T)Convert.ChangeType(DotNetEnv.Env.GetBool(key, false), type),
            nameof(Double) => (T)Convert.ChangeType(DotNetEnv.Env.GetDouble(key, 0.0), type),
            nameof(Int32) => (T)Convert.ChangeType(DotNetEnv.Env.GetInt(key, 0), type),
            _ => (T)Convert.ChangeType(DotNetEnv.Env.GetString(key, string.Empty), type),
        };
    }

    public static bool IsDevelopment()
    {
        return getVariable<string>(EnviromentValueKeys.ENVIRONMENT) == "DEV";
    }

    public static bool IsProduction()
    {
        return getVariable<string>(EnviromentValueKeys.ENVIRONMENT) == "PROD";
    }

    public static bool IsStaging()
    {
        return getVariable<string>(EnviromentValueKeys.ENVIRONMENT) == "STG";
    }
}