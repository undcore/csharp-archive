/*
# 튜플은 복잡한 LINQ 표현식에서 유용해. 예전에는 익명 타입을 사용했지 (그리고 IQueryable에서는 LINQ 제공자가 표현식 트리를 검사할 때 그걸 기대하기 때문에 보통 그렇게 해야 해) 근데 ValueTuple을 사용하면 "루프 안에서" 힙 할당을 피할 수 있어.
# 여러 값을 반환하기 위해서만 존재하는 일회용 클래스의 확산을 줄여줍니다. 이는 더 큰 프로젝트에서 각 클래스에 대한 수많은 개별 파일을 생성하고 IntelliSense 및 유사한 시나리오에서 혼잡을 야기하여 상당한 양의 노이즈를 유발할 수 있습니다. 또한 패턴 매칭 에서도 유용하며, 많은 경우 익명 클래스의 대안으로 사용됩니다.
*/
namespace tuples03;

class Program
{
    public record Point(int X, int Y){
        public double Slope() => (double)Y / (double)X;
    }
    public record struct Point2(int X, int Y);
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
        
        Console.WriteLine("==");
        double slopeResult = pt4.Slope();
        Console.WriteLine($"The slope of {pt4} is {slopeResult}");
    }
}
/*
class - reference type
struct - value type
*/