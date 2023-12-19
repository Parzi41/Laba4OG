using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Numerics;
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

        for (int i = 0; i < point.Length; i++)
        {
            if (i + 1 < point.Length)
            {
                points.Add(new Vector2(Convert.ToInt32(point[i]), Convert.ToInt32(point[i + 1])));
            }
        }

        Bitmap bitmap = DrawWeightCenters(points, 500, 500);
        bitmap.Save("WeightCenters.png", ImageFormat.Png);

        // Будуємо Діаграму Вороного
        bitmap = DrawVoronoiDiagram(points, 500, 500);
        bitmap.Save("VoronoiDiagram.png", ImageFormat.Png);
    }

    // Метод для знаходження центрів ваги та відображення на координатній площині
    static Bitmap DrawWeightCenters(List<Vector2> points, int width, int height)
    {
        Bitmap bitmap = new Bitmap(width, height);
        using Graphics g = Graphics.FromImage(bitmap);
        g.Clear(Color.White);

        foreach (Vector2 point in points)
        {
            g.FillEllipse(Brushes.Red, point.X - 5, point.Y - 5, 10, 10);
        }

        // Знаходження центрів ваги та відображення
        Vector2 weightCenter = CalculateWeightCenter(points);
        g.FillEllipse(Brushes.Blue, weightCenter.X - 5, weightCenter.Y - 5, 10, 10);

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
    static Bitmap DrawVoronoiDiagram(List<Vector2> points, int width, int height)
    {
        // Побудова Діаграми Вороного
        VoronoiGraph voronoiGraph = Fortune.ComputeVoronoiGraph(points);

        Bitmap bitmap = new Bitmap(width, height);
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

        return bitmap;
    }

}
