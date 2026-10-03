using AboutGame;
using AboutGame.Enums;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WPF.Models;

public sealed class AgeRatingDisplay(AgeRating rating)
{
    public string? ImagePath => rating.Age switch
    {
        AgeRatingsValue.PEGI3 => "pack://application:,,,/Assets/AgeRatings/PEGI/pegi_3_CMM_small.png",
        AgeRatingsValue.PEGI7 => "pack://application:,,,/Assets/AgeRatings/PEGI/pegi_7_CMM_small.png",
        AgeRatingsValue.PEGI12 => "pack://application:,,,/Assets/AgeRatings/PEGI/pegi_12_CMM_small.png",
        AgeRatingsValue.PEGI16 => "pack://application:,,,/Assets/AgeRatings/PEGI/pegi_16_CMM_small.png",
        AgeRatingsValue.PEGI18 => "pack://application:,,,/Assets/AgeRatings/PEGI/pegi_18_CMM_small.png",
        AgeRatingsValue.USK0 => "pack://application:,,,/Assets/AgeRatings/USK/0.png",
        AgeRatingsValue.USK6 => "pack://application:,,,/Assets/AgeRatings/USK/6.png",
        AgeRatingsValue.USK12 => "pack://application:,,,/Assets/AgeRatings/USK/12.png",
        AgeRatingsValue.USK16 => "pack://application:,,,/Assets/AgeRatings/USK/16.png",
        AgeRatingsValue.USK18 => "pack://application:,,,/Assets/AgeRatings/USK/18.png",
        AgeRatingsValue.ACB_G => "pack://application:,,,/Assets/AgeRatings/ACB/classification-g-square.png",
        AgeRatingsValue.ACB_PG => "pack://application:,,,/Assets/AgeRatings/ACB/classification-pg-square.png",
        AgeRatingsValue.ACB_M => "pack://application:,,,/Assets/AgeRatings/ACB/classification-m-square.png",
        AgeRatingsValue.ACB_MA15 => "pack://application:,,,/Assets/AgeRatings/ACB/classification-ma15-square.png",
        AgeRatingsValue.ACB_R18 => "pack://application:,,,/Assets/AgeRatings/ACB/classification-r18-square.png",
        _ => null
    };

    public ImageSource? BadgeImage => ImagePath is null
        ? null
        : new BitmapImage(new Uri(ImagePath, UriKind.Absolute));
}
