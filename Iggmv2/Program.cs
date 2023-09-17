using OpenCvSharp;
using System;
public class program
{
    static Mat normalise(Mat image)
    {
        double maxVal, minVal;
        Point minLoc, maxLoc;
        Cv2.MinMaxLoc(image, out minVal, out maxVal, out minLoc, out maxLoc);
        Mat result = new Mat();
        Cv2.Divide(image, maxVal, result);
        return result;
    }
    public static List<Point2f> processframe(Mat bef, Mat aft, double angle)//assume bef and aft have same dimension
    {
        //image preparation
        int rows, cols;
        rows = bef.Rows;
        cols = bef.Cols;
        Mat diff = new Mat(rows,cols,MatType.CV_8UC1);
        //find the difference between bef and aft
        Cv2.Subtract(bef, aft, diff);
        //threshold diff
        Cv2.Threshold(diff, diff, 120, 0, ThresholdTypes.Tozero);
        //normalise diff
        diff = normalise(diff);
        //Create double sized image
        Mat rot = new Mat(rows*2, cols*2, MatType.CV_8UC1);
        //copy diff to rot with (0,0) in diff mapping to the centre of rot
        for(int row = 0; row < rows; row++)
        {
            for(int col = 0; col < cols; col++)
            {
                rot.At<Byte>(row + 1079, col + 1919) = diff.At<Byte>(row, col);
            }
        }
        //rotate rot around (1079,1919)
        Point centre = new Point(1079,1919);
        Mat rotation = Cv2.GetRotationMatrix2D(centre, angle, 1.0);
        Mat post = new Mat();
        Cv2.WarpAffine(rot, post, rotation,rot.Size());
        //Grey gravity search
        float sum1, sum2;
        List<Point2f> points = new List<Point2f>();
        for (int j = 0; j < rows*2; j++)
        {
            sum1 = 0;
            sum2 = 0;
            for (int x = 0; x < cols*2; x++)
            {
                sum1 += post.At<Byte>(j, x) * x;

            }
            for (int y = 0; y < cols - 1; y++)
            {
                sum2 += post.At<Byte>(j, y);
            }
            if (sum2 == 0)
            {
                continue;
            }
            points.Add(new Point2f(j, sum1 / sum2));
        }
        //Rotate points back to inital frame
        List<Point2f> rotatedPoints = new List<Point2f>();
        double ang = angle * (Math.PI / 180);
        foreach(Point2f point in points)
        {
            double x = (point.X - 1079)*(Math.Cos(ang))- Math.Sin(ang)*(point.Y-1919);
            double y = (point.X - 1079) * (Math.Sin(ang)) + Math.Cos(ang) * (point.Y - 1919);
            rotatedPoints.Add(new Point2f());
        }
        //output points
        foreach(Point2f point in points)
        {
            Console.WriteLine(point.ToString());
            post.At<Byte>((int)point.X, (int)point.Y) = (Byte)255;
        }
        Cv2.ImShow("out", post);
        Cv2.WaitKey(0);
        //null condidtion
        List<Point2f> bruh = new List<Point2f>();
        return bruh;
    }
    static void Main(string[] args)
    {
        using var img1 = new Mat("C:\\Users\\james\\source\\repos\\Iggmv2\\Iggmv2\\pic1.jpg", ImreadModes.Grayscale);
        using var img2 = new Mat("C:\\Users\\james\\source\\repos\\Iggmv2\\Iggmv2\\pic2.jpg", ImreadModes.Grayscale);
        List<Point2f> bruh = new List<Point2f>();
        bruh = processframe(img1, img2, 45.0);
    }
}