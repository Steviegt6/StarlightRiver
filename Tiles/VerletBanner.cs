using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using static Terraria.ModLoader.ModContent;
using StarlightRiver.Projectiles.Dummies;
using Terraria.ID;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Enums;
using StarlightRiver.Physics;

namespace StarlightRiver.Tiles
{
    class VerletBanner : DummyTile
	{
        public override int DummyType => ProjectileType<VerletBannerDummy>();

		public override void SetDefaults() 
		{
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, 2, 0);
            QuickBlock.QuickSetFurniture(this, 2, 4, DustType<Dusts.Air>(), SoundID.Tink, false, new Color(120, 100, 100));
        }
    }

    internal class VerletBannerDummy : Dummy
    {
        public VerletBannerDummy() : base(TileType<VerletBanner>(), 32, 32) { }

        private VerletChainInstance Chain;

        public override void SafeSetDefaults()
        {
            Chain = new VerletChainInstance(false)
            {
                segmentCount = 16,
                segmentDistance = 16,//if your using a texture to connect all the points, keep this near the texture size
                constraintRepetitions = 2,//defaults to 2, raising this lowers stretching at the cost of performance
                drag = 2f,//This number defaults to 1, Is very sensitive
                forceGravity = new Vector2(0f, 0.25f),//gravity x/y
                gravityStrengthMult = 1f
            };
        }

        public override void Update()
        {
            Chain.UpdateChain(projectile.Center);

            if (Chain.init) Chain.IterateRope(WindForce);
            projectile.ai[0] += 0.005f;
        }

        private void WindForce(int index)//wind
        {
            int offset = (int)(projectile.position.X / 16 + projectile.position.Y / 16);

            float sin = (float)System.Math.Sin(StarlightWorld.rottime + offset - index / 3f);

            float cos = (float)System.Math.Cos(projectile.ai[0]);
            float sin2 = (float)System.Math.Sin(StarlightWorld.rottime + offset + cos);

            Vector2 pos = new Vector2(Chain.ropeSegments[index].posNow.X + 1 + sin2 * 1.2f, Chain.ropeSegments[index].posNow.Y + sin * 1.4f);

            Color color = (new Color(150, 10, 35).MultiplyRGB(Color.White * (1 - sin * 0.2f)).MultiplyRGB(Lighting.GetColor((int)pos.X / 16, (int)pos.Y / 16)));

            Chain.ropeSegments[index].posNow = pos;
            Chain.ropeSegments[index].color = color;
        }

        public override void Kill(int timeLeft)
        {
            VerletChainInstance.toDraw.Remove(Chain);
        }
    }
}