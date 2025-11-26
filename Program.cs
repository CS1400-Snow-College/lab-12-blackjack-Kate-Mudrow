Console.Clear();

Console.WriteLine("=== BLACKJACK ===");
Console.WriteLine("What is your name?");
string name = Console.ReadLine().Trim();
string filename = name + ".txt";
double balance = 0;


if (File.Exists(filename))
{

    string[] parts = File.ReadAllText(filename).Split(',');
    balance = Convert.ToDouble(parts[1]);
    Console.WriteLine($"Welcome back {name} your current balance amount is ${balance}");
}
else
{
    balance = 100;
    Console.WriteLine($"Welcome {name}! Since you are a new player, you are given a starting balance of ${balance}.");
    string bankBalance = balance.ToString();
    File.WriteAllText(filename, name + "," + bankBalance);
}



List<int> deck = CreateDeck();
Shuffle(deck);


//Deck
static List<int> CreateDeck()
{
    List<int> deck = new List<int>();

    for (int i = 0; i < 52; i++)
    {
        deck.Add(i);
    }

    return deck;
}

//Shuffle
static void Shuffle(List<int> deck)
{
    Random rand = new Random();

    for (int i = deck.Count - 1; i > 0; i--)
    {
        int j = rand.Next(i + 1);

        int temp = deck[i];
        deck[i] = deck[j];
        deck[j] = temp;
    }
}


//DrawCard
static int DrawCard(List<int> deck)
{
    int card = deck[0];     
    deck.RemoveAt(0);        
    return card;
}


static string CardToString(int n)
{
    string[] suits = { "♥", "♦", "♣", "♠" };
    string[] ranks = { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };

    int rankIndex = n / 4;      // n / 4 → 0–12
    int suitIndex = n % 4;      // n % 4 → 0–3

    return "[" + suits[suitIndex] + ranks[rankIndex] + "]";
}

static string HandToString(List<int> hand)
{
    List<string> cards = new List<string>();
    foreach (int c in hand)
    {
        cards.Add(CardToString(c));
    }
        
    return string.Join("", cards);
}
