namespace Assets.PatternDesigner.Scripts.Values
{
    public static class Constants
    {
        public const float
            PLAYER_DEFAULT_SPEED = 1.0f,
            PLAYER_MAX_SPEED = 3.0f,
            PLAYER_MIN_SPEED = 0.1f,
            MIN_DISTANCE_BETWEEN_KEYS = 0.002f,
            MAIN_MENU_MIN_Y = -0.25f,
            MAIN_MENU_MAX_Y = 0.5f,
            FINE_SECONDS_MAX = 2f,
            STROKE_MIN_DURATION = 0.1f;

        public const int SAMPLE_RATE = 120;

        // Milliseconds
        public const int
            SLIDER_NO_MANUAL_CANCEL_TIME = 500, // 0.5 seconds
            TIME_UNTIL_HINT = 2000; // 10 seconds
    }
}