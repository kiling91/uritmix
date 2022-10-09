namespace Model;

public static class ModelSettings
{
    public const int PasswordMinLength = 6;
    public const int PasswordMaxLength = 64;

    public const int PersonNameAndEmailMinLength = 2;
    public const int PersonNameAndEmailMaxLength = 64;

    public const int RoomNameMinLength = 2;
    public const int RoomNameMaxLength = 64;

    public const int LessonDurationMinuteMin = 15;
    public const int LessonDurationMinuteMax = 180;
    public const float LessonBasePriceMin = 1.0f;
    public const float LessonDBasePriceMax = 100000.0f;

    public const byte AbonnementNumberOfVisitsMin = 1;
    public const byte AbonnementNumberOfVisitsMax = 100;
    //public const byte AbonnementDaysOfFreezingMin = 0;
    //public const byte AbonnementDaysOfFreezingMax = 10;
    public const float AbonnementBasePriceMin = 1.0f;
    public const float AbonnementBasePriceMax = 100000.0f;

    public const int DescriptionMaxLength = 4096;
}