using Data;

public class CardData
{
    public CardSuit CardSuit { get; private set; }
    public int CardNumber { get; private set; }

    public CardData(CardSuit cardSuit, int cardNumber)
    {
        CardSuit = cardSuit;
        CardNumber = cardNumber;
    }
}
