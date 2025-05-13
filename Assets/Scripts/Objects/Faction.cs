using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Objects
{
    internal class Faction
    {
        private FactionDataModel factionData;
        public string Name {get; set; }
        public Faction(FactionDataModel factionData)
        {
            this.factionData = factionData;
            Name = factionData.Name;
        }
    }
}
