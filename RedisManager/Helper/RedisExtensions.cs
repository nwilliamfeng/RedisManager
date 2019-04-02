using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace RedisManager
{
    public static class RedisExtensions
    {
        public static IServer SelfServer(this ConnectionMultiplexer connection)
        {
            return connection.GetServer(connection.Configuration);
        }
    }
}
