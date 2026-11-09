using WanderLore.Models;
using Microsoft.Xna.Framework;

namespace WanderLore.Services
{
    public interface IAtlasService
    {
        void LoadAtlasData() { }
        void ManageAtlasData() { }
        Rectangle this[RegionModel region] { get; }
    }
}