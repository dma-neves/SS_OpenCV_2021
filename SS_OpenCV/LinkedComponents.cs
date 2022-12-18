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
        public static void PrintLabels(int[,] labels, int height, int width)
        {
            /*
             * DEBUG
             */

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    string labelStr = labels[y, x].ToString();
                    if (labels[y, x] == -1)
                        labelStr = " ";
                    for (int i = 0; i < 3 - labelStr.Length; i++)
                        Console.Write(" ");

                    Console.Write(labelStr + " ");
                }
                Console.WriteLine();
            }

        }

        public static int[,] GetLabelsClassic(Image<Bgr, byte> img)
        {
            unsafe
            {
                var watch = new System.Diagnostics.Stopwatch();
                watch.Start();

                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.ImageData.ToPointer(); // Pointer to the image
                int widthStep = m.WidthStep;
                int width = img.Width;
                int height = img.Height;
                int nChan = m.NChannels; // number of channels - 3
                int padding = m.WidthStep - m.NChannels * m.Width; // alinhament bytes (padding)
                int x, y;

                int label = 1, neighbourLabel;
                int[,] labels = new int[height, width];

                List<List<int>> adjacencies = new List<List<int>>();

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

                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        int initialLabelValue = labels[y, x];
                        if (initialLabelValue != -1)
                        {
                            List<int> candidates = new List<int>();
                            for (int yoffset = (y == 0 ? 0 : -1); yoffset <= (y == height - 1 ? 0 : 1); yoffset++)
                            {
                                for (int xoffset = (x == 0 ? 0 : -1); xoffset <= (x == width - 1 ? 0 : 1); xoffset++)
                                {
                                    //if (yoffset == 0 || xoffset == 0) // TODO: remove this if (just for testing)
                                    {
                                        neighbourLabel = labels[y + yoffset, x + xoffset];

                                        if(neighbourLabel != -1)
                                        {
                                            if (neighbourLabel < initialLabelValue && !candidates.Contains(neighbourLabel))
                                                candidates.Add(neighbourLabel);

                                            if (neighbourLabel < labels[y, x])
                                                labels[y, x] = neighbourLabel;
                                        }
                                    }
                                }
                            }

                            if (candidates.Count > 1)
                                adjacencies.Add(candidates);
                        }
                    }
                }

                // Compute transitive closure

                for (int i = 0; i < adjacencies.Count; i++)
                {
                    var adj_a = adjacencies[i];
                    if (adj_a.Count == 0) continue;

                    for (int j = 0; j < adjacencies.Count; j++)
                    {
                        if (i != j)
                        {
                            var adj_b = adjacencies[j];
                            if (adj_b.Count == 0) continue;

                            if (adj_a.Intersect(adj_b).Any())
                            {
                                adj_a = adj_a.Union(adj_b).ToList();
                                adj_b = new List<int>();
                                adjacencies[j] = adj_b;
                            }
                        }
                    }

                    adjacencies[i] = adj_a;
                }

                List<List<int>> transitiveClosure = new List<List<int>>();
                for (int i = 0; i < adjacencies.Count; i++)
                    if (adjacencies[i].Count != 0)
                        transitiveClosure.Add(adjacencies[i]);

                // Save replacements found in the transitive closure in a dictionary

                Dictionary<int, int> replacements = new Dictionary<int, int>();
                for (int i = 0; i < transitiveClosure.Count; i++)
                {
                    int min = transitiveClosure[i].Min();
                    transitiveClosure[i].Remove(min);
                    for(int k = 0; k < transitiveClosure[i].Count; k++)
                    {
                        label = transitiveClosure[i][k];
                        if (replacements.ContainsKey(label))
                            ;// throw new Exception("Repeated key while applying transitive closure: " + label);
                        else
                            replacements.Add(label, min);
                    }
                }

                // Apply replacements using the dictionary

                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        label = labels[y, x];
                        if (label != -1 && replacements.ContainsKey(label))
                            labels[y, x] = replacements[label];
                    }
                }

                watch.Stop();
                //Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");

                return labels;
            }
        }

        public static int[,] GetLabelsIterative(Image<Bgr, byte> img)
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

                var watch = new System.Diagnostics.Stopwatch();
                watch.Start();

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

                watch.Stop();
                Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");

                return labels;
            }
        }

        public static Dictionary<int, BoundingBox> GetBoundingBoxes(int[,] labels, int height, int width)
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
