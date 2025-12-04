/*
Name: Kate Mudrow
Date: 12/3/2025
Lab: Lab 12 Blackjack
*/


Console.Clear();

bool playAgain = true;

while (playAgain)
{
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

Console.WriteLine("{0,-25} Total: {1}", $"{name}'s hand: {HandToString(playerHand)}", playerTotal);
Console.WriteLine("{0,-25}", $"Dealer Shows: {CardToString(dealerHand[0])} [?]");


Console.WriteLine();

playerTotal = PlayerTurn(name, playerHand, deck, ref bet);

playerTotal = HandTotal(playerHand);
dealerTotal = DealerTurn(dealerHand, deck);



//Payout
bool playerBlackjack = playerHand.Count == 2 && playerTotal == 21;
bool dealerBlackjack = dealerHand.Count == 2 && dealerTotal == 21;

Console.WriteLine(); 

if (playerBlackjack && !dealerBlackjack) 
{
    decimal payout = bet * 1.5m;
    Console.WriteLine($"Blackjack! You win {payout}!");
    balance += payout;
}
else if (dealerBlackjack && !playerBlackjack)
{
    Console.WriteLine("Dealer has Blackjack! You lose.");
    balance -= bet;
}
else if (playerTotal > 21)
{
    Console.WriteLine("You busted! Dealer wins.");
    balance -= bet;
}
else if (dealerTotal > 21)
{
    Console.WriteLine("Dealer busted! You win!");
    balance += bet;
}
else if (playerTotal > dealerTotal)
{
    Console.WriteLine("You win!");
    balance += bet;
}
else if (playerTotal < dealerTotal)
{
    Console.WriteLine("Dealer wins!");
    balance -= bet;
}
else // tie
{
    Console.WriteLine("Push! You get your bet back.");
}

if(balance <= 0)
{
    Console.WriteLine("The casino takes pity on you and wants you to play again to feed your gambling addiction. You are given $50");
    balance = 50;
}

Console.WriteLine($"Your balance is now: ${balance}");


File.WriteAllText(filename, $"{name},{balance}");

Console.WriteLine();

Console.Write("\nWould you like to play again? (Y/N)" );
string again = Console.ReadLine().Trim().ToUpper();

if (again != "Y")
{
    playAgain = false;
    Console.WriteLine("Thanks for playing!");
}

}








static int DealerTurn(List<int> dealerHand, List<int> deck)
{
    int dealerTotal = HandTotal(dealerHand);
    Console.WriteLine("Dealer's hand: " + HandToString(dealerHand) + $" Total: {dealerTotal}");

    while (dealerTotal < 17) // hit on 16 or less
    {
        int card = DrawCard(deck);
        dealerHand.Add(card);
        dealerTotal = HandTotal(dealerHand);
        Console.WriteLine($"Dealer draws {CardToString(card)}");
        Console.WriteLine($"Dealer's hand: {HandToString(dealerHand)} Total: {dealerTotal}");

    }

    if (dealerTotal >= 17 && dealerTotal <= 21)
        Console.WriteLine("Dealer stands.");
    else if (dealerTotal > 21)
        Console.WriteLine("Dealer busts!");

    return dealerTotal;
}








static int PlayerTurn(string name, List<int> playerHand, List<int> deck, ref decimal bet)
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
                if (playerTotal == 21)
                {
                Console.WriteLine("You have 21! Automatically standing.");
                playerChoice = "S";
                }
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

    return playerTotal; 
}




//Player Turn Methods
static int Hit(List<int> hand, List<int> deck, string name)
{
    int card = DrawCard(deck);
    hand.Add(card);
    int total = HandTotal(hand);
    Console.WriteLine($"You drew {CardToString(card)}");
    Console.WriteLine("{0,-25} Total: {1}", $"{name}'s hand: {HandToString(hand)}", total);
    return total;
}

static int Stand(List<int> hand, string name)
{
    int total = HandTotal(hand);
    Console.WriteLine("You chose to stand.");
    Console.WriteLine("{0,-25} Total: {1}", $"{name}'s hand: {HandToString(hand)}", total);
    return total;
}

static decimal DoubleDown(List<int> hand, List<int> deck, ref decimal bet, string name)
{
    bet *= 2;
    int card = DrawCard(deck);
    hand.Add(card);
    int total = HandTotal(hand);
    Console.WriteLine($"You doubled down and drew {CardToString(card)}");
    Console.WriteLine("{0,-25} Total: {1}", $"{name}'s hand: {HandToString(hand)}", total);
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

    string card = "[" + suits[suitIndex] + ranks[rankIndex] + "]";
    
    return card.PadRight(5); 
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