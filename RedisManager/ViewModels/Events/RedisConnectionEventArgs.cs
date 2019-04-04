using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace RedisManager.ViewModels
{
    public class RedisConnectionEventArgs : EventArgs
    {
        public RedisConnectionEventArgs(ConnectionMultiplexer conn)
        {
            this.Connection = conn;
        }

        public ConnectionMultiplexer Connection { get; private set; }
    }
}
