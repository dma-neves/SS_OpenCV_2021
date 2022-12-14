using System;
using System.Collections.Generic;
using System.Text;
using Emgu.CV.Structure;
using Emgu.CV;
using System.Drawing;
using System.Diagnostics.Contracts;
using System.Linq;

namespace SS_OpenCV
{
    class ImageClass
    {

        private static bool VERIFY = false;
        private static double COMPONENT_CENTER_DIST_MARGIN = 2.0;
        private static double POSITIONING_BLOCKS_DIST_MARGIN = 2.0;

        /// <summary>
        /// Image Negative using EmguCV library
        /// Slower method
        /// </summary>
        /// <param name="img">Image</param>
        public static void Negative(Image<Bgr, byte> img)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.ImageData.ToPointer(); // Pointer to the image
                // byte blue, green, red, gray;
                int width = img.Width;
                int height = img.Height;
                int nChan = m.NChannels; // number of channels - 3
                int padding = m.WidthStep - m.NChannels * m.Width; // alinhament bytes (padding)
                int x, y;

                // Bgr aux;
                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        // acesso pela biblioteca : mais lento 
                        //aux = img[y, x];
                        //img[y, x] = new Bgr(255 - aux.Blue, 255 - aux.Green, 255 - aux.Red);


                        dataPtr[0] = (byte)(255 - dataPtr[0]);
                        dataPtr[1] = (byte)(255 - dataPtr[1]);
                        dataPtr[2] = (byte)(255 - dataPtr[2]);

                        dataPtr += nChan;
                    }

                    dataPtr += padding;
                }
            }
        }

        /// <summary>
        /// Convert to gray
        /// Direct access to memory - faster method
        /// </summary>
        /// <param name="img">image</param>
        public static void ConvertToGray(Image<Bgr, byte> img)
        {
            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right

                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.ImageData.ToPointer(); // Pointer to the image
                byte blue, green, red, gray;
                int width = img.Width;
                int height = img.Height;
                int nChan = m.NChannels; // number of channels - 3
                int padding = m.WidthStep - m.NChannels * m.Width; // alinhament bytes (padding)
                int x, y;

                if (nChan == 3) // image in RGB
                {
                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            //retrieve 3 colour components
                            blue = dataPtr[0];
                            green = dataPtr[1];
                            red = dataPtr[2];

                            // convert to gray
                            gray = (byte)Math.Round(((int)blue + green + red) / 3.0);

                            // store in the image
                            dataPtr[0] = gray;
                            dataPtr[1] = gray;
                            dataPtr[2] = gray;

                            // advance the pointer to the next pixel
                            dataPtr += nChan;
                        }

                        //at the end of the line advance the pointer by the aligment bytes (padding)
                        dataPtr += padding;
                    }
                }
            }
        }

        public static void Green(Image<Bgr, byte> img)
        {
            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right

                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.ImageData.ToPointer(); // Pointer to the image
                int width = img.Width;
                int height = img.Height;
                int nChan = m.NChannels; // number of channels - 3
                int padding = m.WidthStep - m.NChannels * m.Width; // alinhament bytes (padding)
                int x, y;

                if (nChan == 3) // image in RGB
                {
                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            dataPtr[0] = dataPtr[1];
                            dataPtr[2] = dataPtr[1];

                            // advance the pointer to the next pixel
                            dataPtr += nChan;
                        }

                        //at the end of the line advance the pointer by the aligment bytes (padding)
                        dataPtr += padding;
                    }
                }
            }
        }
        public static void BrightContrast(Image<Bgr, byte> img, int bright, double contrast)
        {
            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right

                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.ImageData.ToPointer(); // Pointer to the image
                int width = img.Width;
                int height = img.Height;
                int nChan = m.NChannels; // number of channels - 3
                int padding = m.WidthStep - m.NChannels * m.Width; // alinhament bytes (padding)
                int x, y;

                int red, green, blue;

                if (nChan == 3) // image in RGB
                {
                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            red = (int)Math.Round(contrast * dataPtr[0]) + bright;
                            green = (int)Math.Round(contrast * dataPtr[1]) + bright;
                            blue = (int)Math.Round(contrast * dataPtr[2]) + bright;

                            dataPtr[0] = (byte)(red > 255 ? 255 : (red >= 0 ? red : 0));
                            dataPtr[1] = (byte)(green > 255 ? 255 : (green >= 0 ? green : 0));
                            dataPtr[2] = (byte)(blue > 255 ? 255 : (blue >= 0 ? blue : 0));

                            // advance the pointer to the next pixel
                            dataPtr += nChan;
                        }

                        //at the end of the line advance the pointer by the aligment bytes (padding)
                        dataPtr += padding;
                    }
                }
            }
        }

        public static void Rotation(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float angle)
        {
            MIplImage m = img.MIplImage;
            double width_half = img.Width / 2.0;
            double height_half = img.Height / 2.0;
            RotationAroundPoint(img, imgCopy, angle, width_half, height_half);
        }

        public static void RotationAroundPoint(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float angle, double rotPoint_x, double rotPoint_y)
        {
            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right

                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.ImageData.ToPointer(); // Pointer to the image
                int widthStep = m.WidthStep;
                int width = img.Width;
                int height = img.Height;
                int nChan = m.NChannels; // number of channels - 3
                int padding = m.WidthStep - m.NChannels * m.Width; // alinhament bytes (padding)
                int x, y, origin_x, origin_y;

                MIplImage mcopy = imgCopy.MIplImage;
                byte* dataPtrCopy = (byte*)mcopy.ImageData.ToPointer();

                double cos_theta = Math.Cos(angle);
                double sin_theta = Math.Sin(angle);

                if (nChan == 3) // image in RGB
                {
                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            // Get coordinates from source/origin image
                            // Note: change coordinate system's origin to perform rotation around the center of the image
                            origin_x = (int)Math.Round((x - rotPoint_x) * cos_theta - (rotPoint_y - y) * sin_theta + rotPoint_x);
                            origin_y = (int)Math.Round(rotPoint_y - (x - rotPoint_x) * sin_theta - (rotPoint_y - y) * cos_theta);

                            if (origin_x >= 0 && origin_x < width && origin_y >= 0 && origin_y < height)
                            {
                                byte* address = dataPtrCopy + origin_x * nChan + origin_y * widthStep;
                                dataPtr[0] = (address)[0];
                                dataPtr[1] = (address)[1];
                                dataPtr[2] = (address)[2];

                            }
                            else
                            {
                                dataPtr[0] = 0;
                                dataPtr[1] = 0;
                                dataPtr[2] = 0;
                            }

                            // advance the pointer to the next pixel
                            dataPtr += nChan;
                        }

                        //at the end of the line advance the pointer by the aligment bytes (padding)
                        dataPtr += padding;
                    }
                }
            }
        }

        public static void Translation(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, int dx, int dy)
        {
            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right

                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.ImageData.ToPointer(); // Pointer to the image
                int widthStep = m.WidthStep;
                int width = img.Width;
                int height = img.Height;
                int nChan = m.NChannels; // number of channels - 3
                int padding = m.WidthStep - m.NChannels * m.Width; // alinhament bytes (padding)
                int x, y, origin_x, origin_y;

                MIplImage mcopy = imgCopy.MIplImage;
                byte* dataPtrCopy = (byte*)mcopy.ImageData.ToPointer();


                if (nChan == 3) // image in RGB
                {
                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            // Get coordinates from source/origin image
                            // Note: change coordinate system's origin to perform rotation around the center of the image
                            origin_x = x - dx;
                            origin_y = y - dy;


                            // if can be removed if fors are rewritten to only iterate within the required bounds
                            if (origin_x >= 0 && origin_x < width && origin_y >= 0 && origin_y < height)
                            {
                                byte* address = dataPtrCopy + origin_x * nChan + origin_y * widthStep;
                                dataPtr[0] = (address)[0];
                                dataPtr[1] = (address)[1];
                                dataPtr[2] = (address)[2];

                            }
                            else
                            {
                                dataPtr[0] = 0;
                                dataPtr[1] = 0;
                                dataPtr[2] = 0;
                            }

                            // advance the pointer to the next pixel
                            dataPtr += nChan;
                        }

                        //at the end of the line advance the pointer by the aligment bytes (padding)
                        dataPtr += padding;
                    }
                }
            }
        }

        public static void Scale(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float scaleFactor)
        {

            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right

                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.ImageData.ToPointer(); // Pointer to the image
                int widthStep = m.WidthStep;
                int width = img.Width;
                int height = img.Height;
                int nChan = m.NChannels; // number of channels - 3
                int padding = m.WidthStep - m.NChannels * m.Width; // alinhament bytes (padding)
                int x, y, origin_x, origin_y;

                MIplImage mcopy = imgCopy.MIplImage;
                byte* dataPtrCopy = (byte*)mcopy.ImageData.ToPointer();


                if (nChan == 3) // image in RGB
                {
                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            // Get coordinates from source/origin image
                            origin_x = (int)Math.Round(x / scaleFactor);
                            origin_y = (int)Math.Round(y / scaleFactor);

                            if (origin_x >= 0 && origin_x < width && origin_y >= 0 && origin_y < height)
                            {
                                byte* address = dataPtrCopy + origin_x * nChan + origin_y * widthStep;
                                dataPtr[0] = (address)[0];
                                dataPtr[1] = (address)[1];
                                dataPtr[2] = (address)[2];

                            }
                            else
                            {
                                dataPtr[0] = 0;
                                dataPtr[1] = 0;
                                dataPtr[2] = 0;
                            }

                            // advance the pointer to the next pixel
                            dataPtr += nChan;
                        }

                        //at the end of the line advance the pointer by the aligment bytes (padding)
                        dataPtr += padding;
                    }
                }
            }
        }

        private static Image<Bgr, byte> getPaddedImg(Image<Bgr, byte> img, int bsize)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataBasePtr = (byte*)m.ImageData.ToPointer(); // Pointer to the image
                int widthStep = m.WidthStep;
                int width = img.Width;
                int height = img.Height;
                int nChan = m.NChannels; // number of channels - 3
                int padding = m.WidthStep - m.NChannels * m.Width; // alinhament bytes (padding)
                int x, y;

                Image<Bgr, byte> imgPadded = new Image<Bgr, byte>(width + bsize*2, height + bsize*2);
                MIplImage mpadded = imgPadded.MIplImage;
                byte* dataBasePtrPadded = (byte*)mpadded.ImageData.ToPointer(); // Pointer to the image
                int widthStepPadded = mpadded.WidthStep;
                int widthPadded = imgPadded.Width;
                int heightPadded = imgPadded.Height;
                int paddingPadded = mpadded.WidthStep - mpadded.NChannels * mpadded.Width; // alinhament bytes (padding)

                byte* dataPtr, dataPtrPadded;

                /*
                Smaller less efficient version

                for (y = 0; y < heightPadded; y++)
                {
                    for (x = 0; x < widthPadded; x++)
                    {
                        int y_origin = y - bsize;
                        int x_origin = x - bsize;

                        if (y_origin < 0) y_origin = 0;
                        if (y_origin >= height) y_origin = height - 1;
                        if (x_origin < 0) x_origin = 0;
                        if (x_origin >= width) x_origin = width - 1;

                        for (int c = 0; c < 3; c++)
                        {
                            (dataBasePtrPadded + x * nChan + y * widthStepPadded)[c] = (dataBasePtr + x_origin * nChan + y_origin * widthStep)[c];
                        }
                    }
                }
                */
                
                // ######################### Core #########################


                dataPtr = dataBasePtr;
                dataPtrPadded = dataBasePtrPadded + widthStepPadded*bsize + nChan*bsize;

                for (y = bsize; y < heightPadded - bsize; y++)
                {
                    for (x = bsize; x < widthPadded - bsize; x++)
                    {
                        dataPtrPadded[0] = dataPtr[0];
                        dataPtrPadded[1] = dataPtr[1];
                        dataPtrPadded[2] = dataPtr[2];

                        dataPtr += nChan;
                        dataPtrPadded += nChan;
                    }

                    dataPtr += padding;
                    dataPtrPadded += nChan*bsize + paddingPadded + nChan*bsize;
                }

                // ######################### Top Borders #########################

                for (y = 0; y < bsize; y++)
                {
                    dataPtr = dataBasePtr;
                    dataPtrPadded = dataBasePtrPadded + nChan*bsize + widthStepPadded*y;

                    for (x = bsize; x < widthPadded - bsize; x++)
                    {
                        dataPtrPadded[0] = dataPtr[0];
                        dataPtrPadded[1] = dataPtr[1];
                        dataPtrPadded[2] = dataPtr[2];

                        dataPtr += nChan;
                        dataPtrPadded += nChan;
                    }
                }

                // ######################### Bottom Border #########################

                for (y = 0; y < bsize; y++)
                {
                    dataPtr = dataBasePtr + (height-1) * widthStep;
                    dataPtrPadded = dataBasePtrPadded + nChan*bsize + widthStepPadded * (heightPadded - 1) - widthStepPadded * y;

                    for (x = bsize; x < widthPadded - bsize; x++)
                    {
                        dataPtrPadded[0] = dataPtr[0];
                        dataPtrPadded[1] = dataPtr[1];
                        dataPtrPadded[2] = dataPtr[2];

                        dataPtr += nChan;
                        dataPtrPadded += nChan;
                    }
                }    
               
                
                // ######################### Left Border #########################

                dataPtr = dataBasePtr;
                dataPtrPadded = dataBasePtrPadded + widthStepPadded*bsize;

                for (y = 1; y < heightPadded - 1; y++)
                {
                    for(x = 0; x < bsize; x++)
                    {
                        dataPtrPadded[0] = dataPtr[0];
                        dataPtrPadded[1] = dataPtr[1];
                        dataPtrPadded[2] = dataPtr[2];

                        dataPtrPadded += nChan;
                    }

                    dataPtr += widthStep;
                    dataPtrPadded -= bsize * nChan;
                    dataPtrPadded += widthStepPadded;
                }

                // ######################### Right Border #########################

                dataPtr = dataBasePtr + nChan * (width - 1);
                dataPtrPadded = dataBasePtrPadded + nChan*(widthPadded - 1) + widthStepPadded * bsize;

                for (y = 1; y < heightPadded - 1; y++)
                {
                    for (x = 0; x < bsize; x++)
                    {
                        dataPtrPadded[0] = dataPtr[0];
                        dataPtrPadded[1] = dataPtr[1];
                        dataPtrPadded[2] = dataPtr[2];

                        dataPtrPadded -= nChan;
                    }

                    dataPtr += widthStep;
                    dataPtrPadded += bsize * nChan;
                    dataPtrPadded += widthStepPadded;
                }

                // ######################### Corners #########################

                for(y = 0; y < bsize; y++)
                {
                    for(x = 0; x < bsize; x++)
                    {
                        for(int c = 0; c < 3; c++)
                        {
                            (dataBasePtrPadded + y * widthStepPadded + x * nChan)[c] = (dataBasePtr)[c]; // top left
                            (dataBasePtrPadded + y * widthStepPadded + (widthPadded - 1 - x) * nChan)[c] = (dataBasePtr + nChan * (width - 1))[c]; // top right
                            (dataBasePtrPadded + (heightPadded - 1 - y) * widthStepPadded + x * nChan)[c] = (dataBasePtr + widthStep * (height - 1))[c]; // bottom left
                            (dataBasePtrPadded + (heightPadded - 1 - y) * widthStepPadded + (widthPadded - 1 - x) * nChan)[c] = (dataBasePtr + widthStep * (height - 1) + nChan * (width - 1))[c]; // bottom right
                        }
                    }
                }

                // ######################### Verification #########################


                if (VERIFY)
                {
                    for (y = 0; y < heightPadded; y++)
                    {
                        for (x = 0; x < widthPadded; x++)
                        {
                            int y_origin = y - bsize;
                            int x_origin = x - bsize;

                            if (y_origin < 0) y_origin = 0;
                            if (y_origin >= height) y_origin = height - 1;
                            if (x_origin < 0) x_origin = 0;
                            if (x_origin >= width) x_origin = width - 1;

                            for (int c = 0; c < 3; c++)
                            {
                                //(dataBasePtrPadded + x * nChan + y * widthStepPadded)[c] = (dataBasePtr + x_origin * nChan + y_origin * widthStep)[c];

                                if ((dataBasePtrPadded + x * nChan + y * widthStepPadded)[c] != (dataBasePtr + x_origin * nChan + y_origin * widthStep)[c])
                                    throw new Exception("Padded Image wrong y:" + y as String + " x:" + x as String + "\n height:" + height as String + " width:" + width as String);
                            }
                        }
                    }
                }

                return imgPadded;
            }
        }

        private static int[,,] getPaddedRGBMatrix(int[,,] mat, int width, int height, int nChan, int bsize)
        {
            // TODO: Make more efficient like getPaddedImg method

            unsafe
            {
                int heightPadded = height + bsize * 2;
                int widthPadded = width + bsize * 2;

                int[,,] matPadded = new int[heightPadded, widthPadded, nChan];
                int x, y;

                for (y = 0; y < heightPadded; y++)
                {
                    for (x = 0; x < widthPadded; x++)
                    {
                        int y_origin = y - bsize;
                        int x_origin = x - bsize;

                        if (y_origin < 0) y_origin = 0;
                        if (y_origin >= height) y_origin = height - 1;
                        if (x_origin < 0) x_origin = 0;
                        if (x_origin >= width) x_origin = width - 1;

                        for (int c = 0; c < 3; c++)
                        {
                            matPadded[y, x, c] = mat[y_origin, x_origin, c];
                        }
                    }
                }

                return matPadded;
            }
        }


        public static void Mean(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
        {
            //Mean_A(img, imgCopy);
            Mean_solutionB(img, imgCopy);
        }

        public static void Mean_A(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
        {

            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right

                MIplImage m = img.MIplImage;
                byte* dataBasePtr = (byte*)m.ImageData.ToPointer(); // Pointer to the image
                int widthStep = m.WidthStep;
                int width = img.Width;
                int height = img.Height;
                int nChan = m.NChannels; // number of channels - 3
                int padding = m.WidthStep - m.NChannels * m.Width; // alinhament bytes (padding)
                int x, y;


                Image<Bgr, byte> imgPadded = getPaddedImg(img,1);
                MIplImage mpadded = imgPadded.MIplImage;
                byte* dataBasePtrPadded = (byte*)mpadded.ImageData.ToPointer(); // Pointer to the image
                int widthStepPadded = mpadded.WidthStep;
                int widthPadded = imgPadded.Width;
                int heightPadded = imgPadded.Height;
                int paddingPadded = mpadded.WidthStep - mpadded.NChannels * mpadded.Width; // alinhament bytes (padding)

                if (nChan == 3) // image in RGB
                {
                    byte* dataPtr = dataBasePtr;
                    byte* dataPtrPadded = dataBasePtrPadded + widthStepPadded + nChan;

                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            dataPtr[0] = (byte)Math.Round((
                                (dataPtrPadded - widthStepPadded - nChan)[0] + (dataPtrPadded - widthStepPadded)[0] + (dataPtrPadded - widthStepPadded + nChan)[0] +
                                (dataPtrPadded - nChan)[0] + (dataPtrPadded)[0] + (dataPtrPadded + nChan)[0] +
                                (dataPtrPadded + widthStepPadded - nChan)[0] + (dataPtrPadded + widthStepPadded)[0] + (dataPtrPadded + widthStepPadded + nChan)[0]
                            ) / 9.0);

                            dataPtr[1] = (byte)Math.Round((
                                (dataPtrPadded - widthStepPadded - nChan)[1] + (dataPtrPadded - widthStepPadded)[1] + (dataPtrPadded - widthStepPadded + nChan)[1] +
                                (dataPtrPadded - nChan)[1] + (dataPtrPadded)[1] + (dataPtrPadded + nChan)[1] +
                                (dataPtrPadded + widthStepPadded - nChan)[1] + (dataPtrPadded + widthStepPadded)[1] + (dataPtrPadded + widthStepPadded + nChan)[1]
                            ) / 9.0);

                            dataPtr[2] = (byte)Math.Round((
                                (dataPtrPadded - widthStepPadded - nChan)[2] + (dataPtrPadded - widthStepPadded)[2] + (dataPtrPadded - widthStepPadded + nChan)[2] +
                                (dataPtrPadded - nChan)[2] + (dataPtrPadded)[2] + (dataPtrPadded + nChan)[2] +
                                (dataPtrPadded + widthStepPadded - nChan)[2] + (dataPtrPadded + widthStepPadded)[2] + (dataPtrPadded + widthStepPadded + nChan)[2]
                            ) / 9.0);

                            // advance the pointer to the next pixel
                            dataPtr += nChan;
                            dataPtrPadded += nChan;
                        }

                        dataPtr += padding;
                        dataPtrPadded += nChan + paddingPadded + nChan;
                    }
                }
            }
        }

        public static void Mean_solutionB(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
        {

            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right

                MIplImage m = img.MIplImage;
                byte* dataBasePtr = (byte*)m.ImageData.ToPointer(); // Pointer to the image
                int widthStep = m.WidthStep;
                int width = img.Width;
                int height = img.Height;
                int nChan = m.NChannels; // number of channels - 3
                int padding = m.WidthStep - m.NChannels * m.Width; // alinhament bytes (padding)
                int x, y;


                Image<Bgr, byte> imgPadded = getPaddedImg(img, 1);
                MIplImage mpadded = imgPadded.MIplImage;
                byte* dataBasePtrPadded = (byte*)mpadded.ImageData.ToPointer(); // Pointer to the image
                int widthStepPadded = mpadded.WidthStep;
                int widthPadded = imgPadded.Width;
                int heightPadded = imgPadded.Height;
                int paddingPadded = mpadded.WidthStep - mpadded.NChannels * mpadded.Width; // alinhament bytes (padding)

                int[,,] sums = new int[height, width,3];

                if (nChan == 3) // image in RGB
                {
                    byte* dataPtrPadded = dataBasePtrPadded + widthStepPadded + nChan;

                    int[] prevSum = new int[3];

                    for (y = 0; y < height; y++)
                    {
                        // For x = 0 calculate mean normally

                        sums[y,0,0] = (int)(
                            (dataPtrPadded - widthStepPadded - nChan)[0] + (dataPtrPadded - widthStepPadded)[0] + (dataPtrPadded - widthStepPadded + nChan)[0] +
                            (dataPtrPadded - nChan)[0] + (dataPtrPadded)[0] + (dataPtrPadded + nChan)[0] +
                            (dataPtrPadded + widthStepPadded - nChan)[0] + (dataPtrPadded + widthStepPadded)[0] + (dataPtrPadded + widthStepPadded + nChan)[0]
                        );

                        sums[y,0,1] = (int)(
                            (dataPtrPadded - widthStepPadded - nChan)[1] + (dataPtrPadded - widthStepPadded)[1] + (dataPtrPadded - widthStepPadded + nChan)[1] +
                            (dataPtrPadded - nChan)[1] + (dataPtrPadded)[1] + (dataPtrPadded + nChan)[1] +
                            (dataPtrPadded + widthStepPadded - nChan)[1] + (dataPtrPadded + widthStepPadded)[1] + (dataPtrPadded + widthStepPadded + nChan)[1]
                        );

                        sums[y,0,2] = (int)(
                            (dataPtrPadded - widthStepPadded - nChan)[2] + (dataPtrPadded - widthStepPadded)[2] + (dataPtrPadded - widthStepPadded + nChan)[2] +
                            (dataPtrPadded - nChan)[2] + (dataPtrPadded)[2] + (dataPtrPadded + nChan)[2] +
                            (dataPtrPadded + widthStepPadded - nChan)[2] + (dataPtrPadded + widthStepPadded)[2] + (dataPtrPadded + widthStepPadded + nChan)[2]
                        );

                        prevSum[0] = sums[y,0,0];
                        prevSum[1] = sums[y,0,1];
                        prevSum[2] = sums[y,0,2];

                        dataPtrPadded += nChan;

                        // For x > 0 calculate mean using previous sum

                        for (x = 1; x < width; x++)
                        {
                            sums[y,x,0] = (int)(
                                prevSum[0]
                                - ((dataPtrPadded - 2 * nChan - widthStepPadded)[0] + (dataPtrPadded - 2 * nChan)[0] + (dataPtrPadded - 2 * nChan + widthStepPadded)[0])
                                + ((dataPtrPadded + nChan - widthStepPadded)[0] + (dataPtrPadded + nChan)[0] + (dataPtrPadded + nChan + widthStepPadded)[0])
                            );

                            sums[y, x, 1] = (int)(
                                prevSum[1]
                                - ((dataPtrPadded - 2 * nChan - widthStepPadded)[1] + (dataPtrPadded - 2 * nChan)[1] + (dataPtrPadded - 2 * nChan + widthStepPadded)[1])
                                + ((dataPtrPadded + nChan - widthStepPadded)[1] + (dataPtrPadded + nChan)[1] + (dataPtrPadded + nChan + widthStepPadded)[1])
                            );

                            sums[y, x, 2] = (int)(
                                prevSum[2]
                                - ((dataPtrPadded - 2 * nChan - widthStepPadded)[2] + (dataPtrPadded - 2 * nChan)[2] + (dataPtrPadded - 2 * nChan + widthStepPadded)[2])
                                + ((dataPtrPadded + nChan - widthStepPadded)[2] + (dataPtrPadded + nChan)[2] + (dataPtrPadded + nChan + widthStepPadded)[2])
                            );


                            prevSum[0] = sums[y, x, 0];
                            prevSum[1] = sums[y, x, 1];
                            prevSum[2] = sums[y, x, 2];

                            dataPtrPadded += nChan;
                        }

                        dataPtrPadded += nChan + paddingPadded + nChan;
                    }

                    byte* dataPtr = dataBasePtr;

                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            dataPtr[0] = (byte)(Math.Round(sums[y, x, 0] / 9.0));
                            dataPtr[1] = (byte)(Math.Round(sums[y, x, 1] / 9.0));
                            dataPtr[2] = (byte)(Math.Round(sums[y, x, 2] / 9.0));

                            // advance the pointer to the next pixel
                            dataPtr += nChan;
                        }

                        dataPtr += padding;
                    }
                }
            }
        }

        /*public static void Mean_solutionC(Image<Bgr,byte> img,Image<Bgr,byte> imgCopy,int size)
        {

            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right


                MIplImage m = img.MIplImage;
                byte* dataBasePtr = (byte*)m.ImageData.ToPointer(); // Pointer to the image
                int widthStep = m.WidthStep;
                int width = img.Width;
                int height = img.Height;
                int nChan = m.NChannels; // number of channels - 3
                int padding = m.WidthStep - m.NChannels * m.Width; // alinhament bytes (padding)
                int x, y;

                Image<Bgr, byte> imgPadded = getPaddedImg(img, size/2);
                MIplImage mpadded = imgPadded.MIplImage;
                byte* dataBasePtrPadded = (byte*)mpadded.ImageData.ToPointer(); // Pointer to the image
                int widthStepPadded = mpadded.WidthStep;
                int widthPadded = imgPadded.Width;
                int heightPadded = imgPadded.Height;
                int paddingPadded = mpadded.WidthStep - mpadded.NChannels * mpadded.Width; // alinhament bytes (padding)

                Image<Bgr, int> imgSums = new Image<Bgr, int>(width, height);
                MIplImage msums = imgSums.MIplImage;
                int* dataBasePtrSums = (int*)msums.ImageData.ToPointer(); // Pointer to the image
                int widthStepSums = msums.WidthStep;
                int widthSums = imgSums.Width;
                int heightSums = imgSums.Height;
                int paddingSums = widthStepSums - msums.NChannels * msums.Width; // alinhament bytes (padding)
                paddingSums = 0; // TODO: why do we need to set padding to 0 for it to work?


                if (nChan == 3) // image in RGB
                {
                    int* dataPtrSums = dataBasePtrSums;
                    byte* dataPtrPadded = dataBasePtrPadded + widthStepPadded + nChan;

                    int[] prevSum = new int[3];
                    
                    Console.WriteLine("YO");

                    for (y = 0; y < height; y++)
                    {
                        // For x = 0 calculate mean normally

                        dataPtrSums[0] = (int)(
                            (dataPtrPadded - widthStepPadded - nChan)[0] + (dataPtrPadded - widthStepPadded)[0] + (dataPtrPadded - widthStepPadded + nChan)[0] +
                            (dataPtrPadded - nChan)[0] + (dataPtrPadded)[0] + (dataPtrPadded + nChan)[0] +
                            (dataPtrPadded + widthStepPadded - nChan)[0] + (dataPtrPadded + widthStepPadded)[0] + (dataPtrPadded + widthStepPadded + nChan)[0]
                        );

                        dataPtrSums[1] = (int)(
                            (dataPtrPadded - widthStepPadded - nChan)[1] + (dataPtrPadded - widthStepPadded)[1] + (dataPtrPadded - widthStepPadded + nChan)[1] +
                            (dataPtrPadded - nChan)[1] + (dataPtrPadded)[1] + (dataPtrPadded + nChan)[1] +
                            (dataPtrPadded + widthStepPadded - nChan)[1] + (dataPtrPadded + widthStepPadded)[1] + (dataPtrPadded + widthStepPadded + nChan)[1]
                        );

                        dataPtrSums[2] = (int)(
                            (dataPtrPadded - widthStepPadded - nChan)[2] + (dataPtrPadded - widthStepPadded)[2] + (dataPtrPadded - widthStepPadded + nChan)[2] +
                            (dataPtrPadded - nChan)[2] + (dataPtrPadded)[2] + (dataPtrPadded + nChan)[2] +
                            (dataPtrPadded + widthStepPadded - nChan)[2] + (dataPtrPadded + widthStepPadded)[2] + (dataPtrPadded + widthStepPadded + nChan)[2]
                        );

                        prevSum[0] = dataPtrSums[0];
                        prevSum[1] = dataPtrSums[1];
                        prevSum[2] = dataPtrSums[2];

                        dataPtrSums += nChan;
                        dataPtrPadded += nChan;

                        // For x > 0 calculate mean using previous sum

                        Console.WriteLine("YO");

                        for (x = 1; x < width; x++)
                        {
                            dataPtrSums[0] = (int)(
                                prevSum[0]
                                - ((dataPtrPadded - 2 * nChan - widthStepPadded)[0] + (dataPtrPadded - 2 * nChan)[0] + (dataPtrPadded - 2 * nChan + widthStepPadded)[0])
                                + ((dataPtrPadded + nChan - widthStepPadded)[0] + (dataPtrPadded + nChan)[0] + (dataPtrPadded + nChan + widthStepPadded)[0])
                            );

                            dataPtrSums[1] = (int)(
                                prevSum[1]
                                - ((dataPtrPadded - 2 * nChan - widthStepPadded)[1] + (dataPtrPadded - 2 * nChan)[1] + (dataPtrPadded - 2 * nChan + widthStepPadded)[1])
                                + ((dataPtrPadded + nChan - widthStepPadded)[1] + (dataPtrPadded + nChan)[1] + (dataPtrPadded + nChan + widthStepPadded)[1])
                            );

                            dataPtrSums[2] = (int)(
                                prevSum[2]
                                - ((dataPtrPadded - 2 * nChan - widthStepPadded)[2] + (dataPtrPadded - 2 * nChan)[2] + (dataPtrPadded - 2 * nChan + widthStepPadded)[2])
                                + ((dataPtrPadded + nChan - widthStepPadded)[2] + (dataPtrPadded + nChan)[2] + (dataPtrPadded + nChan + widthStepPadded)[2])
                            );


                            prevSum[0] = dataPtrSums[0];
                            prevSum[1] = dataPtrSums[1];
                            prevSum[2] = dataPtrSums[2];

                            dataPtrSums += nChan;
                            dataPtrPadded += nChan;
                        }

                        dataPtrSums += paddingSums;
                        dataPtrPadded += nChan + paddingPadded + nChan;
                    }

                    byte* dataPtr = dataBasePtr;
                    dataPtrSums = dataBasePtrSums;

                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            dataPtr[0] = (byte)(Math.Round(dataPtrSums[0] / 9.0));
                            dataPtr[1] = (byte)(Math.Round(dataPtrSums[1] / 9.0));
                            dataPtr[2] = (byte)(Math.Round(dataPtrSums[2] / 9.0));

                            // advance the pointer to the next pixel
                            dataPtr += nChan;
                            dataPtrSums += nChan;
                        }

                        dataPtr += padding;
                        dataPtrSums += paddingSums;
                    }
                }
            }

        }*/

        public static void NonUniform(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float[,] matrix, float matrixWeight, float offset)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataBasePtr = (byte*)m.ImageData.ToPointer(); // Pointer to the image
                int widthStep = m.WidthStep;
                int width = img.Width;
                int height = img.Height;
                int nChan = m.NChannels; // number of channels - 3
                int padding = m.WidthStep - m.NChannels * m.Width; // alinhament bytes (padding)
                int x, y;

                Image<Bgr, byte> imgPadded = getPaddedImg(img, 1);
                MIplImage mpadded = imgPadded.MIplImage;
                byte* dataBasePtrPadded = (byte*)mpadded.ImageData.ToPointer(); // Pointer to the image
                int widthStepPadded = mpadded.WidthStep;
                int widthPadded = imgPadded.Width;
                int heightPadded = imgPadded.Height;
                int paddingPadded = mpadded.WidthStep - mpadded.NChannels * mpadded.Width; // alinhament bytes (padding)

                int[,,] sums = new int[height, width, nChan];

                if (nChan == 3) // image in RGB
                {
                    // Assuming the filter is seperable

                    /*

                    // First passage: sum values vertically

                    float[] vert = new float[3];
                    float[] horz = new float[3];

                    // gaussian filter example:
                    vert[0] = 1; vert[1] = 2; vert[2] = 1;
                    horz[0] = 1; horz[1] = 2; horz[2] = 1;

                    // TODO: convert matrix into vert and horz components

                    byte* dataPtrPadded = dataBasePtrPadded + widthStepPadded + nChan;

                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            sums[y,x,0] = (int)(vert[0] * (dataPtrPadded - widthStepPadded)[0] + vert[1] * (dataPtrPadded)[0] + vert[2] * (dataPtrPadded + widthStepPadded)[0]);
                            sums[y,x,1] = (int)(vert[0] * (dataPtrPadded - widthStepPadded)[1] + vert[1] * (dataPtrPadded)[1] + vert[1] * (dataPtrPadded + widthStepPadded)[1]);
                            sums[y,x,2] = (int)(vert[0] * (dataPtrPadded - widthStepPadded)[2] + vert[1] * (dataPtrPadded)[2] + vert[1] * (dataPtrPadded + widthStepPadded)[2]);

                            dataPtrPadded += nChan;
                        }

                        dataPtrPadded += nChan + paddingPadded + nChan;
                    }

                    // Second passage: sum values from first passage horizontally


                    int[,,] sumsPadded = getPaddedRGBMatrix(sums, width, height, nChan, 1);

                    int xp;
                    int yp;

                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            xp = x + 1;
                            yp = y + 1;
                            sums[y, x, 0] = (int)(horz[0] * sumsPadded[yp, xp - 1, 0] + horz[1] * sumsPadded[yp, xp, 0] + horz[2] * sumsPadded[yp, xp + 1, 0]);
                            sums[y, x, 1] = (int)(horz[0] * sumsPadded[yp, xp - 1, 1] + horz[1] * sumsPadded[yp, xp, 1] + horz[2] * sumsPadded[yp, xp + 1, 1]);
                            sums[y, x, 2] = (int)(horz[0] * sumsPadded[yp, xp - 1, 2] + horz[1] * sumsPadded[yp, xp, 2] + horz[2] * sumsPadded[yp, xp + 1, 2]);
                        }
                    }

                    */

                     
                    // less efficient version
                    // filter doesn't have to be seperable

                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            for(int col = 0; col < 3; col++)
                            {
                                int px = x + 1;
                                int py = y + 1;

                                int a = (dataBasePtrPadded + (py - 1) * widthStepPadded + (px - 1) * nChan)[col];
                                int b = (dataBasePtrPadded + (py - 1) * widthStepPadded + (px + 0) * nChan)[col];
                                int c = (dataBasePtrPadded + (py - 1) * widthStepPadded + (px + 1) * nChan)[col];

                                int d = (dataBasePtrPadded + (py + 0) * widthStepPadded + (px - 1) * nChan)[col];
                                int e = (dataBasePtrPadded + (py + 0) * widthStepPadded + (px + 0) * nChan)[col];
                                int f = (dataBasePtrPadded + (py + 0) * widthStepPadded + (px + 1) * nChan)[col];

                                int g = (dataBasePtrPadded + (py + 1) * widthStepPadded + (px - 1) * nChan)[col];
                                int h = (dataBasePtrPadded + (py + 1) * widthStepPadded + (px + 0) * nChan)[col];
                                int i = (dataBasePtrPadded + (py + 1) * widthStepPadded + (px + 1) * nChan)[col];

                                int s = (int)(
                                            matrix[0, 0] * a +
                                            matrix[0, 1] * b +
                                            matrix[0, 2] * c +

                                            matrix[1, 0] * d +
                                            matrix[1, 1] * e +
                                            matrix[1, 2] * f +

                                            matrix[2, 0] * g +
                                            matrix[2, 1] * h +
                                            matrix[2, 2] * i);

                                sums[y, x, col] = s;
                            }
                        }
                    }

                    byte* dataPtr = dataBasePtr;


                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            int b = (int)(Math.Round(sums[y, x, 0] / matrixWeight + offset));
                            int g = (int)(Math.Round(sums[y, x, 1] / matrixWeight + offset));
                            int r = (int)(Math.Round(sums[y, x, 2] / matrixWeight + offset));

                            if (r < 0) r = 0;
                            else if (r > 255) r = 255;
                            if (g < 0) g = 0;
                            else if (g > 255) g = 255;
                            if (b < 0) b = 0;
                            else if (b > 255) b = 255;

                            dataPtr[0] = (byte)b;
                            dataPtr[1] = (byte)g;
                            dataPtr[2] = (byte)r;

                            dataPtr += nChan;
                        }

                        dataPtr += padding;
                    }
                }
            }
         
        }

        public static void Sobel(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
        {
            byte a, b, c, d, e, f, g, h, i;

            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right

                MIplImage m = img.MIplImage;
                byte* dataBasePtr = (byte*)m.ImageData.ToPointer(); // Pointer to the image
                int widthStep = m.WidthStep;
                int width = img.Width;
                int height = img.Height;
                int nChan = m.NChannels; // number of channels - 3
                int padding = m.WidthStep - m.NChannels * m.Width; // alinhament bytes (padding)
                int x, y, chan;


                Image<Bgr, byte> imgPadded = getPaddedImg(img, 1);
                MIplImage mpadded = imgPadded.MIplImage;
                byte* dataBasePtrPadded = (byte*)mpadded.ImageData.ToPointer(); // Pointer to the image
                int widthStepPadded = mpadded.WidthStep;
                int widthPadded = imgPadded.Width;
                int heightPadded = imgPadded.Height;
                int paddingPadded = mpadded.WidthStep - mpadded.NChannels * mpadded.Width; // alinhament bytes (padding)

                int sx,sy,s;

                if (nChan == 3) // image in RGB
                {

                    byte* dataPtr = dataBasePtr;
                    byte* dataPtrPadded = dataBasePtrPadded + widthStepPadded + nChan;

                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            for (chan = 0; chan < nChan; chan++)
                            {
                                a = (dataPtrPadded - widthStepPadded - nChan)[chan];
                                b = (dataPtrPadded - widthStepPadded)[chan];
                                c = (dataPtrPadded - widthStepPadded + nChan)[chan];
                                d = (dataPtrPadded - nChan)[chan];
                                e = (dataPtrPadded)[chan];
                                f = (dataPtrPadded + nChan)[chan];
                                g = (dataPtrPadded + widthStepPadded - nChan)[chan];
                                h = (dataPtrPadded + widthStepPadded)[chan];
                                i = (dataPtrPadded + widthStepPadded + nChan)[chan];

                                sx = (a + 2 * d + g) - (c + 2 * f + i);
                                sy = (g + 2 * h + i) - (a + 2 * b + c);
                                s = Math.Abs(sx) + Math.Abs(sy);
                                s = s > 255 ? 255 : s;
                                dataPtr[chan] = (byte)s;
                            }


                            // advance the pointer to the next pixel
                            dataPtr += nChan;
                            dataPtrPadded += nChan;
                        }

                        dataPtr += padding;
                        dataPtrPadded += nChan + paddingPadded + nChan;
                    }
                }
            }
        }

        public static void Diferentiation(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
        {
            int f,f_x_plus_one, f_y_plus_one;

            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right

                MIplImage m = img.MIplImage;
                byte* dataBasePtr = (byte*)m.ImageData.ToPointer(); // Pointer to the image
                int widthStep = m.WidthStep;
                int width = img.Width;
                int height = img.Height;
                int nChan = m.NChannels; // number of channels - 3
                int padding = m.WidthStep - m.NChannels * m.Width; // alinhament bytes (padding)
                int x, y, chan;


                Image<Bgr, byte> imgPadded = getPaddedImg(img, 1);
                MIplImage mpadded = imgPadded.MIplImage;
                byte* dataBasePtrPadded = (byte*)mpadded.ImageData.ToPointer(); // Pointer to the image
                int widthStepPadded = mpadded.WidthStep;
                int widthPadded = imgPadded.Width;
                int heightPadded = imgPadded.Height;
                int paddingPadded = mpadded.WidthStep - mpadded.NChannels * mpadded.Width; // alinhament bytes (padding)

                int sx, sy, s;

                if (nChan == 3) // image in RGB
                {
                    byte* dataPtr = dataBasePtr;
                    byte* dataPtrPadded = dataBasePtrPadded + widthStepPadded + nChan;

                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            for (chan = 0; chan < nChan; chan++)
                            {
                                f = (dataPtrPadded)[chan];
                                f_x_plus_one = (dataPtrPadded + nChan)[chan];
                                f_y_plus_one = (dataPtrPadded + widthStepPadded)[chan];

                                sx = f - f_x_plus_one;
                                sy = f - f_y_plus_one;
                                s = Math.Abs(sx) + Math.Abs(sy);
                                s = s < 0 ? 0 : (s > 255 ? 255 : s);
                                dataPtr[chan] = (byte)s;
                            }


                            // advance the pointer to the next pixel
                            dataPtr += nChan;
                            dataPtrPadded += nChan;
                        }

                        dataPtr += padding;
                        dataPtrPadded += nChan + paddingPadded + nChan;
                    }
                }
            }
        }

        public static void Median(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
        {
            CvInvoke.MedianBlur(imgCopy, img, 3);
        }


        private unsafe static double get_dist_sum(byte* dataPtrPadded, int widthStepPadded, int nChan, byte b, byte g, byte r)
        {
            double sum = 0;

            for (int yoffset = -1; yoffset <= 1; yoffset++)
            {
                for (int xoffset = -1; xoffset <= 1; xoffset++)
                {
                    byte* adr = dataPtrPadded + yoffset * widthStepPadded + xoffset * nChan;
                    sum += Math.Abs(b - adr[0]) + Math.Abs(g - adr[1]) + Math.Abs(r - adr[2]);

                }
            }

            return sum;
        }

        public static void Median3D(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
        {
            // Ineffiecient version

            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right

                MIplImage m = img.MIplImage;
                byte* dataBasePtr = (byte*)m.ImageData.ToPointer(); // Pointer to the image
                int widthStep = m.WidthStep;
                int width = img.Width;
                int height = img.Height;
                int nChan = m.NChannels; // number of channels - 3
                int padding = m.WidthStep - m.NChannels * m.Width; // alinhament bytes (padding)
                int x, y;


                Image<Bgr, byte> imgPadded = getPaddedImg(img, 1);
                MIplImage mpadded = imgPadded.MIplImage;
                byte* dataBasePtrPadded = (byte*)mpadded.ImageData.ToPointer(); // Pointer to the image
                int widthStepPadded = mpadded.WidthStep;
                int widthPadded = imgPadded.Width;
                int heightPadded = imgPadded.Height;
                int paddingPadded = mpadded.WidthStep - mpadded.NChannels * mpadded.Width; // alinhament bytes (padding)

                if (nChan == 3) // image in RGB
                {
                    byte* dataPtr = dataBasePtr;
                    byte* dataPtrPadded = dataBasePtrPadded + widthStepPadded + nChan;

                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            double mindist = -1;
                            byte* adr_mindist = dataPtrPadded;

                            for(int yoffset = -1; yoffset <= 1; yoffset++)
                            {
                                for(int xoffset = -1; xoffset <= 1; xoffset++)
                                {
                                    byte* adr = dataPtrPadded + yoffset * widthStepPadded + xoffset * nChan;
                                    double dist = get_dist_sum(dataPtrPadded, widthStepPadded, nChan, adr[0], adr[1], adr[2]);
                                    if(mindist == -1 || dist < mindist)
                                    {
                                        mindist = dist;
                                        adr_mindist = adr;
                                    }
                                }
                            }

                            dataPtr[0] = adr_mindist[0];
                            dataPtr[1] = adr_mindist[1];
                            dataPtr[2] = adr_mindist[2];

                            // advance the pointer to the next pixel
                            dataPtr += nChan;
                            dataPtrPadded += nChan;
                        }

                        dataPtr += padding;
                        dataPtrPadded += nChan + paddingPadded + nChan;
                    }
                }
            }

            // Efficient version

            // TODO
        }

        public static int[] Histogram_Gray(Emgu.CV.Image<Bgr, byte> img)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataBasePtr = (byte*)m.ImageData.ToPointer();
                int widthStep = m.WidthStep;
                int width = img.Width;
                int height = img.Height;
                int nChan = m.NChannels;
                int padding = m.WidthStep - m.NChannels * m.Width;
                int x, y;

                int[] hist = new int[256];
                Array.Clear(hist, 0, 256);

                if(nChan == 3)
                {

                    byte* dataPtr = dataBasePtr;

                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            byte val = (byte)Math.Round( (dataPtr[0] + dataPtr[1] + dataPtr[2]) / 3.0 );
                            hist[val]++;

                            dataPtr += nChan;
                        }

                        dataPtr += padding;
                    }
                }

                return hist;
            }
        }

        public static int[,] Histogram_RGB(Emgu.CV.Image<Bgr, byte> img)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataBasePtr = (byte*)m.ImageData.ToPointer();
                int widthStep = m.WidthStep;
                int width = img.Width;
                int height = img.Height;
                int nChan = m.NChannels;
                int padding = m.WidthStep - m.NChannels * m.Width;
                int x, y;

                int[,] hist = new int[3,256];
                Array.Clear(hist, 0, 256*3);

                if (nChan == 3)
                {

                    byte* dataPtr = dataBasePtr;

                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            byte b = dataPtr[0];
                            byte g = dataPtr[1];
                            byte r = dataPtr[2];

                            hist[0, b]++;
                            hist[1, g]++;
                            hist[2, r]++;

                            dataPtr += nChan;
                        }

                        dataPtr += padding;
                    }
                }

                return hist;
            }
        }

        public static void ConvertToBW(Emgu.CV.Image<Bgr, byte> img, int threshold)
        {
            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right

                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.ImageData.ToPointer(); // Pointer to the image
                int widthStep = m.WidthStep;
                int width = img.Width;
                int height = img.Height;
                int nChan = m.NChannels; // number of channels - 3
                int padding = m.WidthStep - m.NChannels * m.Width; // alinhament bytes (padding)
                int x, y;

                if (nChan == 3) // image in RGB
                {
                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            byte gray = (byte)Math.Round((dataPtr[0] + dataPtr[1] + dataPtr[2]) / 3.0);
                            if(gray <= threshold)
                            {
                                dataPtr[0] = 0;
                                dataPtr[1] = 0;
                                dataPtr[2] = 0;
                            }
                            else
                            {
                                dataPtr[0] = 255;
                                dataPtr[1] = 255;
                                dataPtr[2] = 255;
                            }

                            // advance the pointer to the next pixel
                            dataPtr += nChan;
                        }

                        //at the end of the line advance the pointer by the aligment bytes (padding)
                        dataPtr += padding;
                    }
                }
            }
        }

        private static int GetOtsuThreshold(Emgu.CV.Image<Bgr, byte> img)
        {
            int[] hist = Histogram_Gray(img);
            int total_sum = hist.Sum();

            int best_t = 0;
            double best_var = 0;
            for (int t = 0; t < 256; t++)
            {
                double q1 = 0;
                for (int i = 0; i <= t; i++)
                    q1 += hist[i];
                q1 /= total_sum;

                double q2 = 0;
                for (int i = t + 1; i < 256; i++)
                    q2 += hist[i];
                q2 /= total_sum;

                double u1 = 0;
                for (int i = 0; i <= t; i++)
                    u1 += i * hist[i];
                u1 /= total_sum;
                u1 /= q1;

                double u2 = 0;
                for (int i = t + 1; i < 256; i++)
                    u2 += i * hist[i];
                u2 /= total_sum;
                u2 /= q2;

                double var = q1 * q2 * (u1 - u2) * (u1 - u2);
                if (t == 0 || var > best_var)
                {
                    best_t = t;
                    best_var = var;
                }
            }

            /* more efficient version but not working for some reason

            int best_t = 0;
            double best_var = 0;

            // t = 0
            double q1 = hist[0]/total_sum;
            double q2 = (total_sum - hist[0])/total_sum;
            double u1s = 0;
            double u2s = 0;
            for (int i = 1; i < 256; i++)
                u2s += i * hist[i];
            u2s /= total_sum;

            for (int t = 1; t < 256; t++)
            {
                q1 += hist[t] / total_sum;
                q2 -= hist[t] / total_sum;

                u1s += t * (hist[t] / total_sum);
                u2s -= t * (hist[t] / total_sum);

                double var = q1 * q2 * (u1s/q1 - u2s/q2) * (u1s/q1 - u2s/q2);
                if (t == 0 || var > best_var)
                {
                    best_t = t;
                    best_var = var;
                }
            }
            */

            return best_t;
        }

        public static void ConvertToBW_Otsu(Emgu.CV.Image<Bgr, byte> img)
        {
            ConvertToBW(img, GetOtsuThreshold(img));
        }

        //                                                         |                           |
        // ####################################################### V QR Code related functions V #######################################################

        public static byte[,] ConvertToBinary(Emgu.CV.Image<Bgr, byte> img)
        {
            byte[,] pixels = new byte[img.Height,img.Width];
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

                if (nChan == 3) // image in RGB
                {
                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            if (dataPtr[0] + dataPtr[1] + dataPtr[2] == 255*3)
                                pixels[y,x] = 1;
                            else
                                pixels[y,x] = 0;

                            // advance the pointer to the next pixel
                            dataPtr += nChan;
                        }

                        //at the end of the line advance the pointer by the aligment bytes (padding)
                        dataPtr += padding;
                    }
                }
            }

            return pixels;
        }

        private static byte ModeInSection(byte[,] pixels, int left, int right, int top, int bottom)
        {
            int zeroCount = 0;
            int oneCount = 0;

            for (int y = top; y <= bottom; y++)
            {
                for(int x = left; x <= right; x++)
                {
                    if (pixels[y, x] == 1)
                        oneCount++;
                    else
                        zeroCount++;
                }
            }

            return (byte)(oneCount > zeroCount ? 1 : 0);
        }

        private static string GetBinaryCode(byte[,] pixels, double moduleSize, int left, int top)
        {
            StringBuilder sb = new StringBuilder();
            for (int y = 0; y < 21; y++)
            {
                for (int x = 0; x < 21; x++)
                {
                    if (
                        !(x <= 7 && y <= 7) &&
                        !(x <= 7 && y >= 13) &&
                        !(x >= 13 && y <= 7)
                    )
                    {
                        if (ModeInSection(
                            pixels, 
                            (int)(left + x * moduleSize),
                            (int)(left + (x + 1) * moduleSize - 1),
                            (int)(top + y * moduleSize),
                            (int)(top + (y + 1) * moduleSize - 1)
                        ) == 0)
                            sb.Append("1");
                        else
                            sb.Append("0");

                    }
                }
            }

            return sb.ToString();
        }

        public static double GetCenterDist(BoundingBox bbox_a, BoundingBox bbox_b)
        {
            double delta_x = bbox_b.center_x - bbox_a.center_x;
            double delta_y = bbox_b.center_y - bbox_a.center_y;

            return Math.Sqrt(delta_x*delta_x + delta_y*delta_y);
        }

        public static bool IsContained(BoundingBox bbox_a, BoundingBox bbox_b)
        {
            return bbox_a.left >= bbox_b.left && bbox_a.right <= bbox_b.right && bbox_a.top >= bbox_b.top && bbox_a.bottom <= bbox_b.bottom;
        }

        public static BoundingBox[] GetPositioningBlocks(Dictionary<int, BoundingBox> bboxes)
        {
            int idx = 0;
            BoundingBox[] positioningBlocks = new BoundingBox[3];
            List<BoundingBox> bboxesList = new List<BoundingBox>(bboxes.Values);

            for (int i = 0; i < bboxesList.Count; i++)
            {
                for (int j = i + 1; j < bboxesList.Count; j++)
                {
                    BoundingBox bbox_a = bboxesList[i];
                    BoundingBox bbox_b = bboxesList[j];

                    if (GetCenterDist(bbox_a, bbox_b) < COMPONENT_CENTER_DIST_MARGIN)
                    {
                        if(idx == 3)
                            throw new Exception("Found more than 3 positioning blocks");

                        if (IsContained(bbox_b, bbox_a))
                            positioningBlocks[idx++] = bbox_a;
                        else if (IsContained(bbox_a, bbox_b))
                            positioningBlocks[idx++] = bbox_b;
                    }
                }
            }

            if (idx != 3)
                throw new Exception("Found less than 3 positioning blocks");

            return positioningBlocks;
        }

        private static int[] GetQRCodeLimits(byte[,] pixels, int imgHeight, int imgWidth)
        {
            int x, y;
            int left = 0, top = 0, right = 0;
            bool foundFirstLine = false;
            for (y = 0; y < imgHeight && !foundFirstLine; y++)
            {
                for (x = 0; x < imgWidth; x++)
                {
                    if (pixels[y, x] == 0)
                    {
                        if (!foundFirstLine)
                        {
                            foundFirstLine = true;
                            left = x;
                            top = y;
                        }
                        else
                            right = x;
                    }
                }
            }

            return new int[] { left, right, top };
        }

        private static Vector2D[] GetTopLeftAndDiagonalVector(BoundingBox[] positioningBlocks)
        {
            double DX1, DX2, DY1, DY2, D1, D2;
            Vector2D diagonal = new Vector2D { x = 0, y = 0 };
            Vector2D topLeft = new Vector2D { x = 0, y = 0 };

            for(int i = 0; i < 3; i++)
            {
                DX1 = positioningBlocks[(i + 1) % 3].center_x - positioningBlocks[i].center_x;
                DY1 = positioningBlocks[(i + 1) % 3].center_y - positioningBlocks[i].center_y;
                DX2 = positioningBlocks[(i + 2) % 3].center_x - positioningBlocks[i].center_x;
                DY2 = positioningBlocks[(i + 2) % 3].center_y - positioningBlocks[i].center_y;
                D1 = Math.Sqrt(DX1 * DX1 + DY1 * DY1);
                D2 = Math.Sqrt(DX2 * DX2 + DY2 * DY2);

                if (Math.Abs(D1 - D2) < POSITIONING_BLOCKS_DIST_MARGIN)
                {
                    topLeft.x = positioningBlocks[i].center_x;
                    topLeft.y = positioningBlocks[i].center_y;

                    diagonal.x = DX1 + DX2;
                    diagonal.y = DY1 + DY2;

                    return new Vector2D[] { topLeft, diagonal };
                }
            }

            throw new Exception("Error: Couldn't find diagonal");
        }

        /// <summary>
        /// QR code reader
        /// </summary>
        /// <param name="img"> imagem de trabalho </param>
        /// <param name="imgCopy">imagem original </param>
        /// <param name="level">nivel de dificuldade</param>
        /// <param name="Center_x">centro x do Qr code</param>
        /// <param name="Center_y">centro x do Qr code</param>
        /// <param name="Width">largura do Qr code</param>
        /// <param name="Height">altura do Qr code</param>
        /// <param name="Rotation">rotação do Qr code</param>
        /// <param name="BinaryOut">String contendo o Qr code lido em binário</param>
        /// <param name="UL_x_out">centro x do posicionador UL</param>
        /// <param name="UL_y_out">centro y do posicionador UL</param>
        /// <param name="UR_x_out">centro x do posicionador UR</param>
        /// <param name="UR_y_out">centro y do posicionador UR</param>
        /// <param name="LL_x_out">centro x do posicionador LL</param>
        /// <param name="LL_y_out">centro y do posicionador LL</param>
        public static void QRCodeReader(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, int level,
            out int Center_x, out int Center_y, out int Width, out int Height, out float Rotation, out string BinaryOut,
            out int UL_x_out, out int UL_y_out, out int UR_x_out, out int UR_y_out, out int LL_x_out, out int LL_y_out)
        {
            try
            {
                MIplImage m = img.MIplImage;
                int imgWidth = img.Width;
                int imgHeight = img.Height;

                int left = 0, right = 0, top = 0;
                byte[,] pixels = new byte[0, 0];


                if (level == 1)
                {
                    //ConvertToBW(img, 0);
                    pixels = ConvertToBinary(img);
                    int[] qrCodeLimits = GetQRCodeLimits(pixels, imgHeight, imgWidth);

                    left = qrCodeLimits[0];
                    right = qrCodeLimits[1];
                    top = qrCodeLimits[2];
                }
                else if (level == 2 || level == 3)
                {
                    if (level == 3)
                        ConvertToBW_Otsu(img);

                    int[,] labels = LinkedComponents.GetLabels(img);
                    // LinkedComponents.printLabels(labels, height, width);
                    Dictionary<int, BoundingBox> bboxes = LinkedComponents.GetBoundingBoxes(labels, imgHeight, imgWidth);
                    BoundingBox[] positioningBlocks = GetPositioningBlocks(bboxes);

                    Vector2D[] res = GetTopLeftAndDiagonalVector(positioningBlocks);
                    Vector2D topLeft = res[0];
                    Vector2D diagonal = res[1];

                    Vector2D qrCenter = SSUtils.AddVectors(topLeft, SSUtils.ScaleVector(diagonal, 0.5));
                    double angle = SSUtils.AngleFromV1ToV2(diagonal, new Vector2D { x = 1, y = 1 });
                    RotationAroundPoint(img, img.Copy(), (float)angle, qrCenter.x, qrCenter.y);
                    Console.WriteLine("angle: " + angle);

                    double positioningBlocksDistance = SSUtils.Norm(diagonal) / Math.Sqrt(2);
                    Vector2D topLeftAfterRotation = SSUtils.AddVectors(
                        qrCenter,
                        new Vector2D() { x = -positioningBlocksDistance / 2.0, y = -positioningBlocksDistance / 2.0 }
                    );

                    pixels = ConvertToBinary(img);
                    left = (int)(topLeftAfterRotation.x - 3.5 * (positioningBlocksDistance / 14.0));
                    right = (int)(topLeftAfterRotation.x + 17.5 * (positioningBlocksDistance / 14.0));
                    top = (int)(topLeftAfterRotation.y - 3.5 * (positioningBlocksDistance / 14.0));
                }

                Width = right - left + 1;
                Height = Width;
                double moduleSize = Width / 21.0;
                Center_x = left + Width / 2;
                Center_y = top + Height / 2;
                Rotation = 0;
                UL_x_out = (int)(left + 3.5 * moduleSize);
                UL_y_out = (int)(top + 3.5 * moduleSize);
                UR_x_out = (int)(right - 3.5 * moduleSize);
                UR_y_out = (int)(top + 3.5 * moduleSize);
                LL_x_out = (int)(left + 3.5 * moduleSize);
                LL_y_out = (int)(top + (Height - 1 * moduleSize) - 3.5 * moduleSize);
                //BinaryOut = "";
                if (left == 0 && right == 00)
                    BinaryOut = "";
                else
                    BinaryOut = GetBinaryCode(pixels, moduleSize, left, top);

            }
            catch(Exception e)
            {
                Console.WriteLine(e.StackTrace);
                Width = 0;
                Height = 0;
                Center_x = 0;
                Center_y = 0;
                Rotation = 0;
                UL_x_out = 0;
                UL_y_out = 0;
                UR_x_out = 0;
                UR_y_out = 0;
                LL_x_out = 0;
                LL_y_out = 0;
                BinaryOut = "";
            }
        }
    }
}

