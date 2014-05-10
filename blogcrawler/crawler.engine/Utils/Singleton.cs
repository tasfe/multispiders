using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crawler.Engine.Utils
{
    public static class Singleton<T> where T:new()
    {
        public static readonly T Instance = new T();
    }
}
