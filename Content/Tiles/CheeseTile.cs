using Microsoft.Xna.Framework;
using StarlightRiver.Content.Items;
using Terraria.ID;
using Terraria.ModLoader;
using StarlightRiver.Core;
using static Terraria.ModLoader.ModContent;

namespace StarlightRiver.Content.Tiles
{
    internal class CheeseTile : ModTile
    { public override void SetDefaults() => QuickBlock.QuickSet(this, 0, DustID.AmberBolt, SoundID.Drown, new Color(255, 255, 200), ItemType<CheeseTileItem>(), true); }

    internal class CheeseTileItem : QuickTileItem
    { public CheeseTileItem() : base("Cheese", "A chunk of the moon", TileType<CheeseTile>(), ItemRarityID.Expert) { } }
}