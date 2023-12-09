using Terraria;
using Terraria.ModLoader;
namespace Twilight
{
    public class MusicSystem : ModSystem
    {
        public override void Load()
        {
            Terraria.On_Main.UpdateAudio_DecideOnTOWMusic += Main_UpdateAudio_DecideOnTOWMusic;
            Terraria.On_Main.UpdateAudio_DecideOnNewMusic += Main_UpdateAudio_DecideOnNewMusic;
        }

        private void Main_UpdateAudio_DecideOnNewMusic(Terraria.On_Main.orig_UpdateAudio_DecideOnNewMusic orig, Main self)
        {
            orig.Invoke(self);
            if (Main.gameMenu || Main.myPlayer == -1 || !Main.LocalPlayer.active) return;

            int music = Main.newMusic;
            UpdateMusic(ref music);
            Main.newMusic = music;
        }

        private void Main_UpdateAudio_DecideOnTOWMusic(Terraria.On_Main.orig_UpdateAudio_DecideOnTOWMusic orig, Main self)
        {
            orig.Invoke(self);
            if (Main.gameMenu || Main.myPlayer == -1 || !Main.LocalPlayer.active) return;

            int music = Main.newMusic;
            UpdateMusic(ref music);
            Main.newMusic = music;
        }

        private void UpdateMusic(ref int music)
        {
            if (Main.myPlayer == -1 || Main.gameMenu || !Main.LocalPlayer.active)
            {
                return;
            }
            if (BirdUtils.FindBody() != -1 && Twilight.config.UseBGM)
            {
                music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Binah");
            }
        }

        public override void Unload()
        {
            Terraria.On_Main.UpdateAudio_DecideOnTOWMusic -= Main_UpdateAudio_DecideOnTOWMusic;
            Terraria.On_Main.UpdateAudio_DecideOnNewMusic -= Main_UpdateAudio_DecideOnNewMusic;
        }

    }
}