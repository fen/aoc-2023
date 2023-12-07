namespace AdventOfCode.Solutions.Day07;

class PartOne : ISolution
{
    public int Day => 7;

    public int Part => 1;

    public async Task<string> RunAsync(FileInfo input) {
        var lines = await File.ReadAllLinesAsync(input.FullName);

        var hands = lines.Select(Hand.Parse).ToArray();

        Array.Sort(hands);

        for (var i = 0; i < hands.Length; i++) {
            hands[i].Rank = i + 1;
        }

        return hands.Select(h => h.Bid * h.Rank).Sum().ToString();
    }
}

file class Hand(HandType type, Card[] cards, int bid) : IComparable<Hand>
{
    public Card[] Cards => cards;
    public HandType Type => type;
    public int Bid => bid;

    public int Rank { get; set; } = 0;

    public static Hand Parse(string line) {
        var (rawHand, rawBid) = line.Split(' ');

        var cards = rawHand.Select(Card.Parse).ToArray();
        var cardCounts = GetGroupedCardCounts(cards);

        HandType type = cards switch {
            _ when IsFiveOfAKind(cards) => HandType.FiveOfAKind,
            _ when IsFourOfAKind(cardCounts) => HandType.FourOfAKind,
            _ when IsFullHouse(cardCounts) => HandType.FullHouse,
            _ when IsThreeOfAKind(cardCounts) => HandType.ThreeOfAKind,
            _ when IsTwoPair(cardCounts) => HandType.TwoPair,
            _ when IsOnePair(cardCounts) => HandType.OnePair,
            _ when IsHighCard(cardCounts) => HandType.HighCard,
            _ => throw new Exception("Invalid hand")
        };

        var bid = int.Parse(rawBid.Trim());
        return new Hand(type, cards, bid);

        static int[] GetGroupedCardCounts(Card[] cards)
            => cards.GroupBy(c => c.Label).Select(g => g.Count()).ToArray();

        static bool IsFiveOfAKind(Card[] cards) => cards.All(c => c.Equals(cards[0]));
        static bool IsFourOfAKind(int[] cardCounts)
            => cardCounts.Any(count => count == 4);
        static bool IsFullHouse(int[] cardCounts)
            => cardCounts.Count(count => count == 3) == 1 &&
               cardCounts.Count(count => count == 2) == 1;
        static bool IsThreeOfAKind(int[] cardCounts)
            => cardCounts.Any(count => count == 3);
        static bool IsTwoPair(int[] cardCounts) => cardCounts.Length == 3;
        static bool IsOnePair(int[] cardCounts) => cardCounts.Length == 4;
        static bool IsHighCard(int[] cardCounts)
            => cardCounts.Count() == 5;
    }

    public int CompareTo(Hand? other) {
        if (other == null) {
            return 1;
        }

        if (Type != other.Type) {
            return Type.CompareTo(other.Type);
        }

        return Cards
            .Zip(other.Cards)
            .Select(t => t.First.CompareTo(t.Second))
            .FirstOrDefault(c => c != 0);
    }

    public override string ToString() {
        return $"{string.Join("", Cards.Select(c => c.Label))} {Type} {Bid} {Rank}";
    }
}

file enum HandType
{
    HighCard,
    OnePair,
    TwoPair,
    ThreeOfAKind,
    FullHouse,
    FourOfAKind,
    FiveOfAKind
}

file readonly struct Card(char label, int strength) : IComparable<Card>, IEquatable<Card>
{
    public int Strength => strength;

    public static Card Parse(char label) {
        var strength = label switch {
            'A' => 14,
            'K' => 13,
            'Q' => 12,
            'J' => 11,
            'T' => 10,
            '9' => 9,
            '8' => 8,
            '7' => 7,
            '6' => 6,
            '5' => 5,
            '4' => 4,
            '3' => 3,
            '2' => 2,
            _ => throw new Exception("Invalid card label")
        };
        return new Card(label, strength);
    }

    public char Label => label;

    public int CompareTo(Card other) {
        return Strength.CompareTo(other.Strength);
    }

    public bool Equals(Card other) {
        return label == other.Label;
    }
}