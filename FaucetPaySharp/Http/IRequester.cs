using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FaucetPaySharp.Models;

namespace FaucetPaySharp.Http
{
    public interface IRequester
    {
        ApiConfig Configuration { get; }

        Task<T> Post<T>(string resource, Dictionary<string, string> parameters = null, bool noThrow = false) where T : FaucetPayResponse;
    }
}