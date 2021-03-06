﻿using Terraria;
using Terraria.ModLoader;

namespace StarlightRiver.Buffs
{
    public abstract class SmartBuff : ModBuff
    {
        private readonly string ThisName;
        private readonly string ThisTooltip;
        private readonly bool Debuff;
        private readonly bool Summon;

        public bool InflictedPlayer(Player player) => player.HasBuff(Type);
        public bool InflictedNPC(NPC npc) => npc.HasBuff(Type);

        public virtual void SafeSetDetafults() { }
        protected SmartBuff(string name, string tooltip, bool debuff,bool summon=false)
        {
            ThisName = name;
            ThisTooltip = tooltip;
            Debuff = debuff;
            Summon = summon;
        }

        public override void SetDefaults()
        {
            DisplayName.SetDefault(ThisName);
            Description.SetDefault(ThisTooltip);
            Main.debuff[Type] = Debuff;
            if (Summon)
            {
                Main.buffNoSave[Type] = true;
                Main.buffNoTimeDisplay[Type] = true;
            }
        }
    }
}
