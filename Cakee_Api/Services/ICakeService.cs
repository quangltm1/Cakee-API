using Cakee_Api.Datas;
using Cakee_Api.Models;
using System.Collections.Generic;

namespace Cakee_Api.Services
{
    public interface ICakeService
    {
        Task<List<Cake>> GetAllCakes();
        Task<Cake> CreateAsync(Cake cake);
        Task<IEnumerable<Cake>> GetCakesWithDetailsAsync();
    }
}
