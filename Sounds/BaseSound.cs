using Microsoft.Xna.Framework.Audio;
using Terraria.ModLoader;

namespace Twilight.Sounds
{
	public abstract class BaseSound : ModSound
	{
		public override SoundEffectInstance PlaySound(ref SoundEffectInstance soundInstance, float volume, float pan, SoundType type)
		{
			// By creating a new instance, this ModSound allows for overlapping sounds. Non-ModSound behavior is to restart the sound, only permitting 1 instance.
			soundInstance = sound.CreateInstance();
			soundInstance.Volume = volume * .5f;
			//soundInstance.Pan = pan;
			//soundInstance.Pitch = -1.0f;
			return soundInstance;

		}
	}
	public class BossBirdBirth : BaseSound
    {

    }
	public class ClawAttack : BaseSound
    {

    }
	public class EyeBulletBig : BaseSound
	{

	}
	public class EyeBulletFire : BaseSound
	{

	}
	public class EyeBulletSmall : BaseSound
	{

	}
	public class EyeBulletStart : BaseSound
	{

	}
	public class JusticeOn : BaseSound
	{

	}
	public class LanternAttract : BaseSound
	{

	}
	public class PeckAttack : BaseSound
	{

	}
	public class TwilightAtk1 : BaseSound
	{

	}
	public class TwilightAtk2 : BaseSound
	{

	}
	public class TwilightAtk3 : BaseSound
	{

	}
}
