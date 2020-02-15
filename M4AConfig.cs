using MedicareForAll.UI;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace MedicareForAll
{
    public class M4AConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [Tooltip("When set to true, disables the wait time between appointments.")]
        [DefaultValue(false)]
        public bool IdealisticMode
        {
            get;
            set;
        }

        [Tooltip("The wait time between appointments when Idealistic Mode is disabled.")]
        [Range(30, 1800)]
        [Increment(30)]
        [DefaultValue(300)]
        [SliderColor(183, 88, 25)]
        [Units(Unit.Time)]
        [CustomModConfigItem(typeof(SpecialIntRangeElement))]
        public int WaitTime
        {
            get;
            set;
        }

        public override void OnLoaded()
        {
            M4AWorld.serverConfig = this;
        }
    }
}
