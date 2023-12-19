using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Numerics;
using System.Linq;
using FortuneVoronoi;

class Program
{
    static void Main()
    {
        // Читаємо точки з файлу DS2.txt
        string buff = File.ReadAllText("DS2.txt");
        string[] point = buff.Split(' ', '\n', '\r');

        point = point.Where(o => o != "").ToArray();

        List<Vector2> points = new List<Vector2> { };

        for (int i = 0; i < point.Length; i += 2)
        {
            if (i + 1 < point.Length)
            {
                if (float.TryParse(point[i], out float x) && float.TryParse(point[i + 1], out float y))
                {
                    points.Add(new Vector2(x, y));
                }
                else
                {
                    // Обробка помилок при зчитуванні координат точок
                    Console.WriteLine($"Помилка при зчитуванні координат точки {i / 2 + 1}");
                }
            }
        }

        Bitmap bitmap = DrawWeightCenters(points);
        bitmap.Save("WeightCenters.png", ImageFormat.Png);

        // Будуємо Діаграму Вороного
        bitmap = DrawVoronoiDiagram(points);
        bitmap.Save("VoronoiDiagram.png", ImageFormat.Png);
    }

    // Метод для знаходження центрів ваги та відображення на координатній площині
    static Bitmap DrawWeightCenters(List<Vector2> points)
    {
        Bitmap bitmap = new Bitmap(960, 540);
        using Graphics g = Graphics.FromImage(bitmap);
        g.Clear(Color.White);

        foreach (Vector2 point in points)
        {
            g.FillEllipse(Brushes.Red, point.X - 5, point.Y - 5, 10, 10);
        }

        // Знаходження центрів ваги та відображення
        Vector2 weightCenter = CalculateWeightCenter(points);
        g.FillEllipse(Brushes.Blue, weightCenter.X - 5, weightCenter.Y - 5, 10, 10);

        // Відображення точок вихідного датасету з насиченістю 10%
        foreach (Vector2 point in points)
        {
            g.FillEllipse(new SolidBrush(Color.FromArgb(25, 0, 0, 0)), point.X - 5, point.Y - 5, 10, 10);
        }
        Console.WriteLine("Saved to WeightCenters.png");

        return bitmap;
    }

    // Метод для знаходження центра ваги
    static Vector2 CalculateWeightCenter(List<Vector2> points)
    {
        float totalX = 0;
        float totalY = 0;

        foreach (Vector2 point in points)
        {
            totalX += point.X;
            totalY += point.Y;
        }

        float centerX = totalX / points.Count;
        float centerY = totalY / points.Count;

        return new Vector2(centerX, centerY);
    }

    // Метод для побудови Діаграми Вороного
    static Bitmap DrawVoronoiDiagram(List<Vector2> points)
    {
        Console.WriteLine("Drawing voronoi diagram...");

        // Побудова Діаграми Вороного
        VoronoiGraph voronoiGraph = Fortune.ComputeVoronoiGraph(points);

        Bitmap bitmap = new Bitmap(960, 540);
        using Graphics g = Graphics.FromImage(bitmap);
        g.Clear(Color.White);

        // Відображення ребер Діаграми Вороного
        foreach (var edge in voronoiGraph.Edges)
        {
            if (edge.LeftData != null && edge.RightData != null)
            {
                PointF pointA = new PointF(edge.LeftData.X, edge.LeftData.Y);
                PointF pointB = new PointF(edge.RightData.X, edge.RightData.Y);

                g.DrawLine(Pens.Black, pointA, pointB);
            }
        }

        // Відображення точок
        foreach (Vector2 point in points)
        {
            g.FillEllipse(Brushes.Red, point.X - 5, point.Y - 5, 10, 10);
        }
        
        Console.WriteLine("Saved to VoronoiDiagram.png");

        return bitmap;
    }
}
