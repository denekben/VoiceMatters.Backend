using VoiceMatters.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceMatters.Domain.Repositories
{
    public interface INewsRepository
    {
        Task<News?> GetAsync(Guid id);
        Task AddAsync(News news);
        Task DeleteAsync(News news);
        Task UpdateAsync(News news);
    }
}
