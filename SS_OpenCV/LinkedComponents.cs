using Emgu.CV.Structure;
using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SS_OpenCV
{
    struct BoundingBox
    {
        public int left;
        public int right;
        public int top;
        public int bottom;

        public int center_x;
        public int center_y;
    }

    internal class LinkedComponents
    {
        public static void printLabels(int[,] labels, int height, int width)
        {
            /*
             * DEBUG
             */

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    string labelStr = labels[y, x].ToString();
                    for (int i = 0; i < 3 - labelStr.Length; i++)
                        Console.Write(" ");

                    Console.Write(labelStr + " ");
                }
                Console.WriteLine();
            }

        }

        public static int[,] getLabels(Image<Bgr, byte> img)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.ImageData.ToPointer(); // Pointer to the image
                int widthStep = m.WidthStep;
                int width = img.Width;
                int height = img.Height;
                int nChan = m.NChannels; // number of channels - 3
                int padding = m.WidthStep - m.NChannels * m.Width; // alinhament bytes (padding)
                int x, y;

                int label = 0;
                int[,] labels = new int[height, width];

                // TODO: don't need to set initial labels -> improve performance

                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        if (dataPtr[0] == 0)
                            labels[y, x] = label++;
                        else
                            labels[y, x] = -1;

                        dataPtr += nChan;
                    }
                    dataPtr += padding;
                }

                // TODO: use classic algorithm instead of iterative -> improve performance

                bool change = true;

                while (change)
                {
                    change = false;

                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            int minLabel = labels[y, x];
                            for (int yoffset = (y == 0 ? 0 : -1); yoffset <= (y == height - 1 ? 0 : 1); yoffset++)
                            {
                                for (int xoffset = (x == 0 ? 0 : -1); xoffset <= (x == width - 1 ? 0 : 1); xoffset++)
                                {
                                    if ((label = labels[y + yoffset, x + xoffset]) < minLabel)
                                    {
                                        if (label != -1)
                                        {
                                            labels[y, x] = label;
                                            minLabel = label;
                                            change = true;
                                        }
                                    }
                                }

                            }
                        }
                    }
                }

                return labels;
            }
        }

        public static Dictionary<int, BoundingBox> getBoundingBoxes(int[,] labels, int height, int width)
        {
            Dictionary<int, BoundingBox> boundingBoxes = new Dictionary<int, BoundingBox>();

            for (int y = 0; y < height; y++)
            {
                for(int x = 0; x < width; x++)
                {
                    int label = labels[y, x];
                    if (label != -1)
                    {
                        BoundingBox bbox;

                        try
                        {
                            bbox = boundingBoxes[label];

                            if (x < bbox.left) bbox.left = x;
                            if (x > bbox.right) bbox.right = x;
                            if (y < bbox.top) bbox.top = y;
                            if (y > bbox.bottom) bbox.bottom = y;

                            boundingBoxes[label] = bbox;
                        }
                        catch (KeyNotFoundException)
                        {
                            bbox = new BoundingBox { left = x, right = x, top = y, bottom = y, center_x = -1, center_y = -1 };
                            boundingBoxes.Add(label, bbox);
                        }
                    }
                }
            }

            Dictionary<int, BoundingBox> boundingBoxesWithCenter = new Dictionary<int, BoundingBox>();

            foreach(KeyValuePair<int, BoundingBox> entry in boundingBoxes)
            {
                BoundingBox bbox = entry.Value;
                bbox.center_x = (int)Math.Round(bbox.left + (bbox.right - bbox.left) / 2.0);
                bbox.center_y = (int)Math.Round(bbox.top + (bbox.bottom - bbox.top) / 2.0);

                boundingBoxesWithCenter.Add(entry.Key, bbox);
            }

            return boundingBoxesWithCenter;
        }
    }
}
