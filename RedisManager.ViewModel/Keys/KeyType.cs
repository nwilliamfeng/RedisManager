using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisManager.ViewModel
{
    public enum KeyType
    {
        Unknown = 0,

        String = 1,

        List = 2,

        Set = 3,

        Hash = 4,

        ZSet = 5,

    }
}
