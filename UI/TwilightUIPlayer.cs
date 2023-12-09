using Terraria;
using Terraria.ModLoader;
namespace Twilight.UI
{
    public class TwilightUIPlayer : ModPlayer
    {
        public int CurrentPlay = 0;
        public int Timer = 0;
        public int ShakeTimer = 0;
        public float ShakeScale = 1;
        public void Initialise(int type)
        {
            Timer = 50;
            if (type == 3)
            {
                Timer = 70;
            }
            CurrentPlay = type;
        }
        public override void PostUpdateMiscEffects()
        {
            if (Timer > 0)
            {
                Timer--;
            }
            if (ShakeTimer > 0)
            {
                ShakeTimer--;
            }
        }

        public override void UpdateDead()
        {
            if (Timer > 0)
            {
                Timer--;
            }
            if (ShakeTimer > 0)
            {
                ShakeTimer--;
            }
        }
        public override void ModifyScreenPosition()
        {
            if (ShakeTimer > 0 && Twilight.config.UseScreenEffect)
            {
                Main.screenPosition.X += Main.rand.Next(-10, 11) * ShakeScale;
                Main.screenPosition.Y += Main.rand.Next(-10, 11) * ShakeScale;
            }
        }
        public void InitialiseShake(int t, float scale)
        {
            if (scale > ShakeScale)
            {
                ShakeScale = scale;
            }
            if (t > ShakeTimer)
            {
                ShakeTimer = t;
            }
        }
    }
}