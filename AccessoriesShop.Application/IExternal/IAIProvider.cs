using AccessoriesShop.Application.DTOs.ChatboxDto;
using AccessoriesShop.Domain.Enums;
using AutoMapper.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Application.Interfaces.External
{
    public interface IAIProvider
    {
        string ProviderName { get; }
        string ModelName { get; }
        int Priority { get; }  

 
        bool SkillSupports(AICapability capability);

        Task<bool> IsAvailableAsync();
    
        Task<string> ExecuteAsync(AIRequest request, CancellationToken ct = default);
    }
}
