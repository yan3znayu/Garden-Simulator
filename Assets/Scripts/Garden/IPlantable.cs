using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Garden
{
    public interface IPlantable
    {
        void PlantSeed(SeedBag seedBag);
    }
}
