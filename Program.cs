Console.Clear();

Console.WriteLine("=== BLACKJACK ===");
Console.WriteLine("What is your name?");
string name = Console.ReadLine().Trim();
string filename = name + ".txt";
double balance = 0;

if (File.Exists(filename))
{
    if (File.Exists(filename))
        {
            string[] parts = File.ReadAllText(filename).Split(',');
            balance = Convert.ToDouble(parts[1]);
            Console.WriteLine($"Welcome back {name} your current balance amount is ${balance}");
        }
}
else
{
    balance = 100;
    Console.WriteLine($"Welcome {name}! Since you are a new player, you are given a starting balance of ${balance}.");
    string bankBalance = balance.ToString();
    File.WriteAllText(filename, name + "," + bankBalance);
}