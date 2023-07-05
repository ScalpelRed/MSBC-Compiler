using System.Drawing;

Console.Write("File path (may be relative), name and extension: ");
string fullName = Console.ReadLine()!.Replace('/', '\\');
Bitmap bmp;

try
{
    bmp = new(fullName);
}
catch (IOException)
{
    Console.WriteLine("File or folder does not exist");
    Console.ReadLine();
    return;
}

List<ByteVector4> data = new();

for (int x = 0; x < bmp.Width; x++)
{
    Console.WriteLine($"Processing line {x}");
    for (int y = 0; y < bmp.Height; y++)
    {
        Color c = bmp.GetPixel(x, y);
        data.Add(new(c.R, c.G, c.B, c.A));
    }
}

Console.WriteLine($"IMPORTANT: some graphic cards does not support large amounts of data in shaders " +
    $"(for example, AMD Radeon Vega 8 does not support arrays larger than 65536 elements). " +
    $"\nYour image contains {data.Count} pixels. Each pixel is an array element.");
Console.WriteLine("Saving...");

string defaultFileName = "image.glsl";
if (!File.Exists(defaultFileName)) File.Create(defaultFileName).Close();
StreamWriter sw = new(defaultFileName);
sw.WriteLine($"const vec4[{data.Count}] data = vec4[{data.Count}] (");

ByteVector4 last = data[^1];
data.RemoveAt(data.Count - 1);

foreach (ByteVector4 vec in data)
{
    sw.WriteLine($"\t{vec},");
}
sw.WriteLine($"\t{last}\n);");

sw.WriteLine($"#define name {fullName.Split("\\")[^1]}");
sw.WriteLine($"#define width {bmp.Width}");
sw.WriteLine($"#define height {bmp.Height}");

sw.WriteLine("\nvec4 getPixel(int x, int y) {\n\treturn data[height * x + y];\n}");

sw.Close();

Console.WriteLine("Done!\a");
Console.ReadLine();

struct ByteVector4
{
    public byte x;
    public byte y;
    public byte z;
    public byte w;

    public ByteVector4(byte x, byte y, byte z, byte w)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }

    public override string ToString()
    {
        return $"vec4({x}.0, {y}.0, {z}.0, {w}.0)";
    }
}