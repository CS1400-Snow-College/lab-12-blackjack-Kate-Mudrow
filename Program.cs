Console.Clear();

Console.WriteLine("=== BLACKJACK ===");
Console.WriteLine("What is your name?");
string name = Console.ReadLine().Trim();
string filename = name + ".txt";
decimal balance = 0;


if (File.Exists(filename))
{

    string[] parts = File.ReadAllText(filename).Split(',');
    balance = Convert.ToDecimal(parts[1]);
    Console.WriteLine($"Welcome back {name} your current balance amount is ${balance}");
}
else
{
    balance = 100;
    Console.WriteLine($"Welcome {name}! Since you are a new player, you are given a starting balance of ${balance}.");
    File.WriteAllText(filename, $"{name},{balance}");

}


//Ask for Bet
decimal bet = 0;
bool validBet = false;

while (!validBet)
{
    Console.WriteLine("Enter your bet:");
    string input = Console.ReadLine().Trim();

    if (decimal.TryParse(input, out bet))
    {
        if (bet <=0)
        {
            Console.WriteLine("Bet must be greater than 0.");
            continue;
        }

        else if (bet > (decimal)balance)
        {
            Console.WriteLine($"Insufficient funds. Please enter an amount less than or equal to ${balance}");
            continue;
        }
        else
        {
            validBet = true;
        }

    }
    else
    {
        Console.WriteLine("Invalid input. Please enter a number.");
    }
}
Console.Clear();
Console.WriteLine($"Player bet: ${bet}");

List<int> deck = CreateDeck();
Shuffle(deck);


List<int> playerHand = new List<int>();
List<int> dealerHand = new List<int>();

playerHand.Add(DrawCard(deck));
playerHand.Add(DrawCard(deck));

dealerHand.Add(DrawCard(deck));
dealerHand.Add(DrawCard(deck));

int playerTotal = HandTotal(playerHand);
int dealerTotal = HandTotal(dealerHand);

Console.WriteLine("{0,-20} {1,-15} Total: {2}", $"{name}'s hand:", HandToString(playerHand), playerTotal);
Console.WriteLine("{0,-20} {1,-15}", "Dealer Shows:", CardToString(dealerHand[0]) + "[?]");


Console.WriteLine();

bet = PlayerTurn(name, playerHand, deck, bet);


static decimal PlayerTurn(string name, List<int> playerHand, List<int> deck, decimal bet)
{
    string playerChoice = "";
    bool firstTurn = true;
    int playerTotal = HandTotal(playerHand);

    do
    {
        Console.Write("(H)it, (S)tand, (D)ouble? ");
        playerChoice = Console.ReadLine().Trim().ToUpper();

        switch (playerChoice)
        {
            case "H":
                playerTotal = Hit(playerHand, deck, name);
                break;
            case "S":
                playerTotal = Stand(playerHand, name);
                break;
            case "D":
                if (firstTurn)  // only double down on first turn
                {
                    playerTotal = (int)DoubleDown(playerHand, deck, ref bet, name);
                }
                else
                {
                    Console.WriteLine("You can only double down on your first turn.");
                }
                break;
            default:
                Console.WriteLine("Invalid choice. Please enter H, S, or D.");
                continue;
        }

        firstTurn = false;

    } while (playerChoice != "S" && playerTotal <= 21);

    return bet; 
}





//Player Turn Methods
static int Hit(List<int> hand, List<int> deck, string name)
{
    int card = DrawCard(deck);
    hand.Add(card);
    int total = HandTotal(hand);
    Console.WriteLine($"You drew {CardToString(card)}");
    Console.WriteLine($"{name}'s hand: {HandToString(hand)} Total: {total}");
    return total;
}

static int Stand(List<int> hand, string name)
{
    int total = HandTotal(hand);
    Console.WriteLine("You chose to stand.");
    Console.WriteLine($"{name}'s hand: {HandToString(hand)} Total: {total}");
    return total;
}

static decimal DoubleDown(List<int> hand, List<int> deck, ref decimal bet, string name)
{
    bet *= 2;
    int card = DrawCard(deck);
    hand.Add(card);
    int total = HandTotal(hand);
    Console.WriteLine($"You doubled down and drew {CardToString(card)}");
    Console.WriteLine($"{name}'s hand: {HandToString(hand)} Total: {total}");
    return total;
}


















//Deck Build
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

//Aesthetics
static string CardToString(int n)
{
    string[] suits = { "♥", "♦", "♣", "♠" };
    string[] ranks = { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };

    int rankIndex = n / 4;      // n / 4  0–12
    int suitIndex = n % 4;      // n % 4  0–3

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


static int HandTotal(List<int> hand)
{
    int total = 0;
    int aceCount = 0;
    foreach (int c in hand)
    {
        int rank = c / 4 + 1; 
        if (rank >= 10) total += 10;
        else if (rank == 1) { total += 11; aceCount++; } 
        else total += rank;
    }

    while (total > 21 && aceCount > 0)
    {
        total -= 10; //change Ace from 11 to 1
        aceCount--;
    }

    return total;
}