using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceMatters.Domain.Entities;

namespace VoiceMatters.Domain.Repositories
{
    public interface ITagRepository
    {
        Task<Tag?> GetTagByNameAsync(string tagName);
    }
}
