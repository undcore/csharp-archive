Console.WriteLine("Hello, World!");

string aFriend = "Bill";
Console.WriteLine(aFriend);
aFriend = "Maira";
Console.WriteLine(aFriend);
Console.WriteLine("Hello " + aFriend);
Console.WriteLine($"Hello {aFriend}");

string firstFriend = "Maria";
string secondFriend = "Sage";
Console.WriteLine($"My friends are {firstFriend} and {secondFriend}");

Console.WriteLine($"The name {firstFriend} has {firstFriend.Length} letters.");
Console.WriteLine($"The name {secondFriend} has {secondFriend.Length} letters.");
Console.WriteLine("");
string greeting = "      Hello World!       ";
Console.WriteLine($"[{greeting}]");

Console.WriteLine("");
Console.WriteLine("[" + greeting.TrimStart() + "]");
Console.WriteLine($"[{greeting.TrimEnd()}]");
Console.WriteLine($"[{greeting.Trim()}]");

Console.WriteLine("");
string trimmedGreeting = greeting.Trim();
Console.WriteLine($"[{trimmedGreeting}]");
Console.WriteLine($"[{greeting}]");

Console.WriteLine("");
string sayHello = "Hello World!";
Console.WriteLine(sayHello);
Console.WriteLine(sayHello.Replace("Hello", "Greetings"));

Console.WriteLine("");
string name = "hongGilDong";
Console.WriteLine(name.ToUpper());
Console.WriteLine(name.ToLower());

Console.WriteLine("");
string songLyrics = "You say goodbye, and I say hello";
Console.WriteLine(songLyrics.Contains("goodbye"));
Console.WriteLine(songLyrics.Contains("greetings"));

Console.WriteLine("");
bool bFlag = true;
Console.WriteLine(bFlag);
Boolean bFlag2 = true;
Console.WriteLine(bFlag2); // 출력은 True False 대문자로

//StartsWith, EndsWith Contains
//You 로 시작하고 hello로 끝나는 경우 true를 받아야 하며
//goodbye로 시작하거나 끝나는 경우 false를 받아야 한다

Console.WriteLine(songLyrics.StartsWith("You") && songLyrics.EndsWith("hello"));
Console.WriteLine(songLyrics.StartsWith("goodbye") || songLyrics.EndsWith("goodbye"));