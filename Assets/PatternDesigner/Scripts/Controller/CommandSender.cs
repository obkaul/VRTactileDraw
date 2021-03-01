namespace Assets.PatternDesigner.Scripts.misc
{
    public interface CommandSender
    {
        void updateActuator(int channel, float intensity, float activeTime);

        void updateActuators(float[] intensities, float[] activeTimes);

        void deactivateFeedback();
    }
}