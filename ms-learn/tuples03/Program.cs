namespace tuples03;

class Program
{
    public record Point(int X, int Y);
    static void Main(string[] args)
    {
        // 튜플은 고정 길이를 가진 정렬된 값 시퀀스입니다. 튜플의 각 요소에는 형식과 선택적 이름이 있습니다.
        var pt = (X: 1, Y: 2);

        var slope = (double)pt.Y / (double)pt.X;
        Console.WriteLine($"A line from the origin to the point {pt} has a slope of {slope}.");

        pt.X = pt.X + 5;
        Console.WriteLine($"The point is now at {pt}.");

        Console.WriteLine("==");
        var subscript = (A: 0, B: 0);
        subscript = pt;
        Console.WriteLine(subscript);

        Console.WriteLine("==");
        var namedData = (Name: "Morning observation", Temp: 17, Wind: 4);
        Console.WriteLine(namedData);
        // 튜플 형식에는 이름이 없으므로 값 집합에 의미를 전달할 수 없습니다. 튜플 형식은 동작을 추가할 수 없습니다.
        var person = (FirstName: "", LastName: "");
        var order = (Product: "guitar picks", style: "triangle", quantity: 500, UnitPrice: 0.10m);

        Console.WriteLine("==");
        Point pt3 = new Point(1, 1);
        var pt4 = pt3 with { Y = 10 };
        Console.WriteLine($"The two points are {pt3} and {pt4}");
        
    }
}
