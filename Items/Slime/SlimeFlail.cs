using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using StarlightRiver.Core;

namespace StarlightRiver.Items.Slime
{
    public class SlimeFlail : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Snail Flail");
            Tooltip.SetDefault("Yabba Dabba Doo");
        }

        public override void SetDefaults()
        {
            item.damage = 20;
            item.magic = true;
            item.mana = 10;
            item.width = 18;
            item.height = 34;
            item.useTime = 30;
            item.useAnimation = 30;
            item.value = Item.sellPrice(0, 0, 10, 0);//todo
            item.rare = ItemRarityID.Green;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.UseSound = SoundID.Item43;
            item.knockBack = 0f;
            item.shoot = ModContent.ProjectileType<Projectiles.WeaponProjectiles.StarwoodStaffProjectile>();
            item.shootSpeed = 15f;
            item.noMelee = true;
            item.autoReuse = true;
        }
    }
}