using VoiceMatters.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceMatters.Domain.Repositories
{
    public interface IRoleRepository
    {
        Task<Role?> GetAsync(Guid id);
        Task<Role?> GetByNameAsync(string name);
    }
}
