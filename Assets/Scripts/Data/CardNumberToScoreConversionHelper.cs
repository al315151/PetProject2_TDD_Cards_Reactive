using System.Collections.Generic;
public static class CardNumberToScoreConversionHelper
{
    public static Dictionary<int, int> CardNumberToScoreConversion = new()
    {
        {1, 11},
        {3, 10},
        {12, 4},
        {11, 3},
        {10, 2},
    };
}
