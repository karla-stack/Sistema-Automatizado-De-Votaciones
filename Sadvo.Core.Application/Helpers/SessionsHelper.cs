using System.Text;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Sadvo.Core.Application.Helpers
{
    public static class SessionsHelper
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T? Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);

            return value != null ? JsonConvert.DeserializeObject<T>(value) : default;
        }
    }
}
