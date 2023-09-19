using OpenCvSharp;
using System;
using System.Diagnostics;
public class program
{
    static Mat rotate_frame(Mat image, double angle)
    {
        var height = image.Height;
        var width = image.Width;
        Point centre = new Point(width / 2, height / 2);
        Mat rot_mat = Cv2.GetRotationMatrix2D(centre, angle, 1.0);
        Mat result = new Mat();
        Cv2.WarpAffine(image, result, rot_mat, image.Size(), InterpolationFlags.Linear, BorderTypes.Constant, Scalar.All(0));
        return result;
    }
    static Mat normalise(Mat image)
    {
        double maxVal, minVal;
        float val;
        Point minLoc, maxLoc;
        Cv2.MinMaxLoc(image, out minVal, out maxVal, out minLoc, out maxLoc);
        Mat result = new Mat(image.Size(), MatType.CV_32FC1);
        for (int row = 0; row < image.Rows; row++)
        {
            for(int cols=0; cols < image.Cols; cols++)
            {
                val = (float) (image.At<Byte>(row, cols) / maxVal);
                result.At<float>(row,cols) = val;
            }
        }
        return result;
    }
    public static List<Point2f> processframe(Mat bef, Mat aft, double angle)//assume bef and aft have same dimension
    {
        //image preparation
        int rows, cols;
        rows = bef.Rows;
        cols = bef.Cols;
        Mat diff = new Mat(rows,cols,MatType.CV_8UC1);
        Mat diffthres = new Mat(rows, cols, MatType.CV_8UC1);
        //find the difference between bef and aft
        Cv2.Subtract(bef, aft, diff);
        //threshold diff
        Cv2.Threshold(diff, diffthres, 120, 0, ThresholdTypes.Tozero);
        //rotate image
        diff = rotate_frame(diffthres, angle);
        //normalise
        diff = normalise(diff);
        //IGGM
        double sum1, sum2;
        float pixval;
        Mat testimg = new Mat(rows,cols,MatType.CV_8UC1);
        List<Point2f> result = new List<Point2f>();
        for (int j=0; j<cols; j++)
        {
            sum1 = 0;
            sum2 = 0;
            for(int x =0; x<rows; x++)
            {
             
                pixval = (float)diff.At<float>(x, j);
                sum1 += pixval*x;
                sum2 += pixval;
            }
            if (sum2 == 0)
            {
                continue;
            }
            result.Add(new Point2f((float)(sum1 / sum2),j));
            testimg.At<Byte>((int)(sum1 / sum2),j)= (Byte)255;
        }
        //output line
        testimg = rotate_frame(testimg, -angle);
        Cv2.Add(diffthres, testimg, testimg);
        Cv2.ImShow("Out img", testimg);
        Cv2.WaitKey(0);
        //get height data
        //return
        return result;
    }
    static void Main(string[] args)
    {
        using var img1 = new Mat("C:\\Users\\james\\source\\repos\\Iggmv2\\Iggmv2\\pic1.jpg", ImreadModes.Grayscale);
        using var img2 = new Mat("C:\\Users\\james\\source\\repos\\Iggmv2\\Iggmv2\\pic2.jpg", ImreadModes.Grayscale);
        List<Point2f> bruh = new List<Point2f>();
        Mat gamer = new Mat(img1.Size(), MatType.CV_8UC1);
        double test;
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        bruh = processframe(img1, img2, 45.0);
        stopwatch.Stop();
        long elap = stopwatch.ElapsedMilliseconds;
        Console.WriteLine(elap);
        foreach(Point2f point in bruh)
        {
            gamer.At<Byte>((int)point.X,(int)(point.Y-100))=(Byte)255;
        }
        Cv2.ImShow("Balls", gamer);
        Cv2.WaitKey(0);
    }
}