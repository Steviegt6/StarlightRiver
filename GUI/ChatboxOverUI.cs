﻿using Terraria;
using Terraria.UI;
using StarlightRiver.NPCs.TownUpgrade;
using static Terraria.ModLoader.ModContent;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace StarlightRiver.GUI
{
    public class ChatboxOverUI : UIState
    {
        public TownUpgrade activeUpgrade;

        private readonly TownButton button = new TownButton();

        public override void OnInitialize()
        {
            AddElement(button, Main.screenWidth / 2 - Main.chatBackTexture.Width / 2 - 104, 100, 86, 28, this);
        }

        internal void AddElement(UIElement element, int x, int y, int width, int height, UIElement appendTo)
        {
            element.Left.Set(x, 0);
            element.Top.Set(y, 0);
            element.Width.Set(width, 0);
            element.Height.Set(height, 0);
            appendTo.Append(element);
        }

        public void SetState(TownUpgrade state)
        {
            activeUpgrade = state;

            if(state != null)
                button.displayString = state._buttonName;

            OnInitialize();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (activeUpgrade != null) base.Draw(spriteBatch);
        }
    }

    public class TownButton : UIElement
    {
        public string displayString = "ERROR";

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsMouseHovering)
                Main.LocalPlayer.mouseInterface = true;

            bool locked = displayString == "Locked";

            Texture2D tex = GetTexture("StarlightRiver/GUI/Assets/NPCButton");
            spriteBatch.Draw(tex, GetDimensions().ToRectangle(), tex.Frame(), Color.White * (locked ? 0.4f : 0.8f));

            float x = Main.fontItemStack.MeasureString(displayString).X;
            float scale = x < 70 ? 1 : 70 / x;
            Utils.DrawBorderString(spriteBatch, displayString, GetDimensions().ToRectangle().Center() + new Vector2(0, 3), Color.White * (locked ? 0.4f : 1), scale, 0.5f, 0.5f);
        }

        public override void Click(UIMouseEvent evt)
        {
            if(Parent is ChatboxOverUI)
            {
                (Parent as ChatboxOverUI).activeUpgrade?.ClickButton();
            }
        }
    }
}
