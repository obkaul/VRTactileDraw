namespace Assets.PatternDesigner.Scripts.PaintSystem.Player
{
    /// <summary>
    /// Interface for the tactile player
    /// </summary>
    public interface ITactilePlayer
    {
        bool IsLooping();

        void SetLooping(bool looping);

        float GetSpeed();

        void SetSpeed(float speed);

        void Play();

        void Stop();

        float GetTime();

        bool IsPlaying();

        float GetDuration();
    }
}