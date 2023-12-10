namespace AdventOfCode.Solutions.Day07;

class PartTwo : ISolution
{
    public int Day => 7;

    public int Part => 2;

    public async Task<string> RunAsync(FileInfo file) {
        var lines = await File.ReadAllLinesAsync(file.FullName);

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
        var (maxCardCount, numberOfCards, jokers) = Count(cards);

        if (jokers > 0) {
            maxCardCount += jokers;
            numberOfCards -= 1;
        } else {
            maxCardCount = Math.Max(maxCardCount, jokers);
        }

        HandType type = (maxCardCount, numberOfCards) switch {
            (5, _) => HandType.FiveOfAKind,
            (4, _) => HandType.FourOfAKind,
            (_, 2) => HandType.FullHouse,
            (3, _) => HandType.ThreeOfAKind,
            (_, 3) => HandType.TwoPair,
            (2, _) => HandType.OnePair,
            _ => HandType.HighCard
        };

        var bid = int.Parse(rawBid.Trim());
        return new Hand(type, cards, bid);

        static (int maxCardCount, int numberOfCards, int jokers) Count(Card[] cards) {
            int maxCardCount = 0;
            int numberOfCards = 0;

            Span<byte> counts = stackalloc byte[Card.NumberOfCards];
            foreach (var card in cards) {
                var cardCount = ++counts[card.Index];

                if (cardCount == 1) {
                    numberOfCards += 1;
                }

                if (card.Label != 'J') {
                    maxCardCount = Math.Max(maxCardCount, cardCount);
                }
            }

            return (maxCardCount, numberOfCards, counts[Card.JokerIndex]);
        }
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

file readonly struct Card(int index, char label, int strength) : IComparable<Card>, IEquatable<Card>
{
    public const int JokerIndex = 0;
    public const int NumberOfCards = 14;

    public int Index => index;
    public int Strength => strength;
    public char Label => label;

    public static Card Parse(char label) {
        var strength = label switch {
            'A' => 14,
            'K' => 13,
            'Q' => 12,
            'T' => 10,
            '9' => 9,
            '8' => 8,
            '7' => 7,
            '6' => 6,
            '5' => 5,
            '4' => 4,
            '3' => 3,
            '2' => 2,
            'J' => 1,
            _ => throw new Exception("Invalid card label")
        };
        return new Card(strength - 1, label, strength);
    }

    public int CompareTo(Card other) {
        return Strength.CompareTo(other.Strength);
    }

    public bool Equals(Card other) {
        return label == other.Label;
    }
}