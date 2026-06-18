using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrowStore.Domain.Interfaces
{
    public interface IAuthRepository
    {
        Task<string> LoginAsync(string email, string password);
    }
}