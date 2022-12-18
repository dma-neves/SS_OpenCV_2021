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
        private static bool DEBUG_QR_BINARY = false;
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

        public static void RedChannel(Image<Bgr, byte> img)
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


                        dataPtr[0] = dataPtr[2];
                        dataPtr[1] = dataPtr[2];

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

        public static void Rotation_Bilinear(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float angle)
        {
            MIplImage m = img.MIplImage;
            double width_half = img.Width / 2.0;
            double height_half = img.Height / 2.0;
            RotationAroundPoint_Bilinear(img, imgCopy, angle, width_half, height_half);
        }

        public static void RotationAroundPoint_Bilinear(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float angle, double rotPoint_x, double rotPoint_y)
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
                int x, y, c;
                double origin_x, origin_y, offset_x, offset_y;
                int j, k;
                int B_j_k, B_jp1_k, B_j_kp1, B_jp1_kp1;
                int B_jpx_k, B_jpx_kp1, B_jpx_kpy;

                Image<Bgr, byte> imgPadded = getPaddedImg(img, 1);
                MIplImage mpadded = imgPadded.MIplImage;
                byte* dataBasePtrPadded = (byte*)mpadded.ImageData.ToPointer(); // Pointer to the image
                int widthStepPadded = mpadded.WidthStep;
                dataBasePtrPadded = dataBasePtrPadded + widthStepPadded * 1 + nChan * 1;

                double cos_theta = Math.Cos(angle);
                double sin_theta = Math.Sin(angle);

                if (nChan == 3) // image in RGB
                {
                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            origin_x = (int)Math.Round((x - rotPoint_x) * cos_theta - (rotPoint_y - y) * sin_theta + rotPoint_x);
                            origin_y = (int)Math.Round(rotPoint_y - (x - rotPoint_x) * sin_theta - (rotPoint_y - y) * cos_theta);

                            j = (int)origin_x;
                            k = (int)origin_y;
                            offset_x = origin_x - j;
                            offset_y = origin_y - k;

                            if (origin_x >= 0 && origin_x < width && origin_y >= 0 && origin_y < height)
                            {
                                // Bi-Linera interpolation
                                for(c = 0; c < 3; c++)
                                {
                                    B_j_k = (dataBasePtrPadded + j * 3 + k * widthStepPadded)[c];

                                    if (offset_x == 0 && offset_y == 0)
                                        dataPtr[c] = (byte)B_j_k;
                                    else
                                    {
                                        B_jp1_k = (dataBasePtrPadded + (j + 1) * 3 + k * widthStepPadded)[c];
                                        B_j_kp1 = B_j_k = (dataBasePtrPadded + j * 3 + (k + 1) * widthStepPadded)[c];
                                        B_jp1_kp1 = B_j_k = (dataBasePtrPadded + (j + 1) * 3 + (k + 1) * widthStepPadded)[c];

                                        B_jpx_k = (int)((1.0 - offset_x) * B_j_k + offset_x * B_jp1_k);
                                        B_jpx_kp1 = (int)((1.0 - offset_x) * B_j_kp1 + offset_x * B_jp1_kp1);
                                        B_jpx_kpy = (int)((1.0 - offset_y) * B_jpx_k + offset_y * B_jpx_kp1);
                                        dataPtr[c] = (byte)B_jpx_kpy;
                                    }                         
                                }
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
            Scale_point_xy(img, imgCopy, scaleFactor, 0, 0);
        }

        public static void Scale_Bilinear(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float scaleFactor)
        {
            Scale_point_xy_Bilinear(img, imgCopy, scaleFactor, 0, 0);
        }

        public static void Scale_point_xy_Bilinear(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float scaleFactor, int centerX, int centerY)
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
                int x, y, c;
                double origin_x, origin_y, offset_x, offset_y;
                int j, k;
                int B_j_k, B_jp1_k, B_j_kp1, B_jp1_kp1;
                int B_jpx_k, B_jpx_kp1, B_jpx_kpy;

                Image<Bgr, byte> imgPadded = getPaddedImg(img, 1);
                MIplImage mpadded = imgPadded.MIplImage;
                byte* dataBasePtrPadded = (byte*)mpadded.ImageData.ToPointer(); // Pointer to the image
                int widthStepPadded = mpadded.WidthStep;
                dataBasePtrPadded = dataBasePtrPadded + widthStepPadded * 1 + nChan * 1;


                if (nChan == 3) // image in RGB
                {
                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            // Get coordinates from source/origin image
                            origin_x = centerX + (x - centerX) / scaleFactor;
                            origin_y = centerY + (y - centerY) / scaleFactor;
                            j = (int)origin_x;
                            k = (int)origin_y;
                            offset_x = origin_x - j;
                            offset_y = origin_y - k;

                            //Console.WriteLine("offset_x: " + offset_x);

                            if (origin_x >= 0 && origin_x < width && origin_y >= 0 && origin_y < height)
                            {
                                // Bi-Linera interpolation
                                for (c = 0; c < 3; c++)
                                {
                                    B_j_k = (dataBasePtrPadded + j * 3 + k * widthStepPadded)[c];

                                    if (offset_x == 0 && offset_y == 0)
                                        dataPtr[c] = (byte)B_j_k;
                                    else
                                    {
                                        B_jp1_k = (dataBasePtrPadded + (j + 1) * 3 + k * widthStepPadded)[c];
                                        B_j_kp1 = B_j_k = (dataBasePtrPadded + j * 3 + (k + 1) * widthStepPadded)[c];
                                        B_jp1_kp1 = B_j_k = (dataBasePtrPadded + (j + 1) * 3 + (k + 1) * widthStepPadded)[c];

                                        B_jpx_k = (int)((1.0 - offset_x) * B_j_k + offset_x * B_jp1_k);
                                        B_jpx_kp1 = (int)((1.0 - offset_x) * B_j_kp1 + offset_x * B_jp1_kp1);
                                        B_jpx_kpy = (int)((1.0 - offset_y) * B_jpx_k + offset_y * B_jpx_kp1);
                                        dataPtr[c] = (byte)B_jpx_kpy;
                                    }
                                }
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

        public static void Scale_point_xy(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float scaleFactor, int centerX, int centerY)
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
                            origin_x = (int)Math.Round(centerX + (x - centerX) / scaleFactor);
                            origin_y = (int)Math.Round(centerY + (y - centerY) / scaleFactor);

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

        public static void Shear(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float x_shear, float y_shear)
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
                int widthStepCopy = mcopy.WidthStep;

                if (nChan == 3) // image in RGB
                {
                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            // Get coordinates from source/origin image
                            origin_x = (int)Math.Round(x - x_shear*y);
                            origin_y = (int)Math.Round(y - y_shear*x);

                            if (origin_x >= 0 && origin_x < width && origin_y >= 0 && origin_y < height)
                            {
                                byte* address = dataPtrCopy + origin_x * nChan + origin_y * widthStepCopy;
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

        public static void Roberts(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
        {

            int f, f_x_plus_one, f_y_plus_one, f_x_plus_one_y_plus_one;

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
                                f_x_plus_one_y_plus_one = (dataPtrPadded + widthStepPadded + nChan)[chan];

                                sx = f - f_x_plus_one_y_plus_one;
                                sy = f_x_plus_one - f_y_plus_one;
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

        public static int[,] Histogram_All(Emgu.CV.Image<Bgr, byte> img)
        {
            int[] hist_gray = Histogram_Gray(img);
            int[,] hist_rgb = Histogram_RGB(img);

            int[,] hist_all = new int[4, 256];

            for (int i = 0; i < 256; i++)
            {
                hist_all[0, i] = hist_gray[i];
                hist_all[1, i] = hist_rgb[0, i];
                hist_all[2, i] = hist_rgb[1, i];
                hist_all[3, i] = hist_rgb[2, i];

            }

            return hist_all;
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
                u1 = q1 == 0 ? 0 : u1 / q1;

                double u2 = 0;
                for (int i = t + 1; i < 256; i++)
                    u2 += i * hist[i];
                u2 /= total_sum;
                u2 = q2 == 0 ? 0 : u2 / q2;


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

        private static string GetBinaryCode(byte[,] pixels, int left, int right, int top, int bottom, Image<Bgr, byte> img)
        {
            /*
            Console.WriteLine("moduleSize: " + moduleSize);
            Console.WriteLine("left: " + left);
            Console.WriteLine("top: " + top);
            */

            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.ImageData.ToPointer(); // Pointer to the image

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
                                (int)Math.Round(left + x * (right - left) / 21.0),
                                (int)Math.Round(left + (x+1) * (right - left) / 21.0),
                                (int)Math.Round(top + y * (bottom-top)/21.0),
                                (int)Math.Round(top + (y+1) * (bottom - top) / 21.0)
                            ) == 0)
                                sb.Append("1");
                            else
                                sb.Append("0");

                            if (DEBUG_QR_BINARY)
                            {
                                string correct = "100011101011111101010111110101101010000011001110000010010111110010100001011100010110011011010011000111000011001110110010001010101010101100111011110011000001010111001110110111001101000110101111000010000111011011000011001010010111001100001001001100001";
                                bool failed = false;
                                if (sb[sb.Length - 1] != correct[sb.Length - 1])
                                {
                                    Console.WriteLine("Failed at x: " + x + " y: " + y);
                                    Console.WriteLine("value: " + sb[sb.Length - 1]);
                                    Console.WriteLine("");
                                    failed = true;
                                }

                                byte* address = dataPtr + (
                                    (int)Math.Round(top + y * (bottom - top) / 21.0) * m.WidthStep +
                                    (int)Math.Round(left + x * (right - left) / 21.0) * 3
                                );

                                if (failed)
                                {
                                    address[0] = 0;
                                    address[1] = 0;
                                    address[2] = 255;
                                }
                                else
                                {
                                    address[0] = 0;
                                    address[1] = 255;
                                    address[2] = 0;
                                }

                            }

                        }
                    }
                }

                return sb.ToString();
            }
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
        
        private static int[] CropImage(BoundingBox[] positioningBlocks, Image<Bgr, byte> img)
        {
            const int PADDING = 2;

            int left = Math.Min(Math.Min(positioningBlocks[0].left, positioningBlocks[1].left), positioningBlocks[2].left) - PADDING;
            int right = Math.Max(Math.Max(positioningBlocks[0].right, positioningBlocks[1].right), positioningBlocks[2].right) + PADDING;
            int top = Math.Min(Math.Min(positioningBlocks[0].top, positioningBlocks[1].top), positioningBlocks[2].top) - PADDING;
            int bottom = Math.Max(Math.Max(positioningBlocks[0].bottom, positioningBlocks[1].bottom), positioningBlocks[2].bottom) + PADDING;

            int longestDim = Math.Max((right - left), (bottom - top));

            var roi = new Rectangle(left, top, longestDim, longestDim);
            img.ROI = roi;

            return new int[] { left, top };
        }

        private struct QRPos
        {
            public Vector2D ul;
            public Vector2D ur;
            public Vector2D ll;
            public Vector2D lr;
            public Vector2D rightVec;
            public Vector2D downVec;
            public Vector2D diagonalVec;
            public Vector2D center;
            public bool deformed;
        }

        private static QRPos GetQRPositioning(BoundingBox[] positioningBlocks)
        {
            return GetQRPositioningRec(positioningBlocks, POSITIONING_BLOCKS_DIST_MARGIN, false, 1);
        }

        private static QRPos GetQRPositioningRec(BoundingBox[] positioningBlocks, double margin, bool deformed, int depth)
        {
            if (depth == 4)
                throw new Exception("Reached maximum recursion depth");

            for(int i = 0; i < 3; i++)
            {
                Vector2D v1 = new Vector2D()
                {
                    x = positioningBlocks[(i + 1) % 3].center_x - positioningBlocks[i].center_x,
                    y = positioningBlocks[(i + 1) % 3].center_y - positioningBlocks[i].center_y
                };

                Vector2D v2 = new Vector2D()
                {
                    x = positioningBlocks[(i + 2) % 3].center_x - positioningBlocks[i].center_x,
                    y = positioningBlocks[(i + 2) % 3].center_y - positioningBlocks[i].center_y
                };

                if (Math.Abs(SSUtils.Norm(v1) - SSUtils.Norm(v2)) < margin)
                {
                    Vector2D diagonalVec = SSUtils.AddVectors(v1, v2);
                    Vector2D ul = new Vector2D { x = positioningBlocks[i].center_x, y = positioningBlocks[i].center_y };
                    Vector2D center = SSUtils.AddVectors(ul, SSUtils.ScaleVector(diagonalVec, 0.5));
                    Vector2D rightVec;
                    Vector2D downVec;

                    if (SSUtils.CrossProd(v1, v2) > 0)
                    {
                        rightVec = v1;
                        downVec = v2;
                    }
                    else
                    {
                        rightVec = v2;
                        downVec = v1;
                    }

                    Vector2D ur = SSUtils.AddVectors(ul, rightVec);
                    Vector2D ll = SSUtils.AddVectors(ul, downVec);
                    Vector2D lr = SSUtils.AddVectors(ll, rightVec);

                    return new QRPos { ul = ul, ur = ur, ll = ll, lr = lr, rightVec = rightVec, downVec = downVec, diagonalVec = diagonalVec, center = center, deformed = deformed };
                }
            }

            return GetQRPositioningRec(positioningBlocks, margin*2, true, depth++);

            //throw new Exception("Error: Couldn't find diagonal");
        }

        private static void Dilation(Image<Bgr, byte> img, byte[,] pixels, int[,] mask)
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

                if (nChan == 3) // image in RGB
                {
                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            bool intersection = false;
                            for (int y_offset = (y == 0 ? 0 : -1); y_offset <= (y == height - 1 ? 0 : 1); y_offset++)
                            {
                                for (int x_offset = (x == 0 ? 0 : -1); x_offset <= (x == width - 1 ? 0 : 1); x_offset++)
                                {
                                    int maskVal = mask[1 + y_offset, 1 + x_offset];
                                    if (maskVal == -1) continue;
                                    if (pixels[y + y_offset, x + x_offset] == maskVal)
                                    {
                                        intersection = true;
                                        goto maskIterationFinished;
                                    }
                                }
                            }
                        maskIterationFinished:

                            if (intersection)
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

        private static void Erosion(Image<Bgr, byte> img, byte[,] pixels, int[,] mask)
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

                if (nChan == 3) // image in RGB
                {
                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            bool diff = false;
                            for(int y_offset = (y == 0 ? 0 : -1); y_offset <= (y == height-1 ? 0 : 1); y_offset++)
                            {
                                for (int x_offset = (x == 0 ? 0 : -1); x_offset <= (x == width-1 ? 0: 1); x_offset++)
                                {
                                    int maskVal = mask[1 + y_offset, 1 + x_offset];
                                    if (maskVal == -1) continue;
                                    if (pixels[y+y_offset,x+x_offset] != maskVal)
                                    {
                                        diff = true;
                                        goto maskIterationFinished;
                                    }
                                }
                            }
                        maskIterationFinished:

                            if (diff)
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

        public static void CompoundOperation(Image<Bgr, byte> img)
        {
            int[,] mask1 = {

                {0,0,0},
                {0,0,0},
                {0,0,0}
            };

            byte[,] pixels = ConvertToBinary(img);

            Erosion(img, pixels, mask1);
            Dilation(img, pixels, mask1);
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

            MIplImage m = img.MIplImage;
            int imgWidth = img.Width;
            int imgHeight = img.Height;
            int imgWidthStep = m.WidthStep;

            int left, top, right, bottom;

            if (level == 1)
            {
                byte[,] pixels = ConvertToBinary(img);
                int[] qrCodeLimits = GetQRCodeLimits(pixels, imgHeight, imgWidth);

                left = qrCodeLimits[0];
                right = qrCodeLimits[1];
                top = qrCodeLimits[2];
                bottom = top + (right - left);

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
                BinaryOut = GetBinaryCode(pixels, left, right, top, bottom, img);
            }
            else if (level == 2 || level == 3 || level == 4 || level == 5)
            {
                Image<Bgr, byte> auxImg = img.Copy();

                if (level >= 3)
                    ConvertToBW(auxImg, Math.Min(100, GetOtsuThreshold(auxImg)));

                int[,] labels = LinkedComponents.GetLabelsClassic(auxImg);
                Dictionary<int, BoundingBox> bboxes = LinkedComponents.GetBoundingBoxes(labels, imgHeight, imgWidth);
                BoundingBox[] positioningBlocks = GetPositioningBlocks(bboxes);

                if(level >= 3)
                {
                    auxImg = img.Copy();
                    CropImage(positioningBlocks, auxImg);
                    int threshold = GetOtsuThreshold(auxImg.Copy());
                    ConvertToBW(img, threshold);

                    //if (level == 5)
                    //    CompoundOperation(img);
                }

                /*
                int[] cropRes = CropImage(positioningBlocks, img);
                int cropLeft = cropRes[0];
                int cropTop = cropRes[1];
                for (int i = 0; i < 3; i++)
                {
                    positioningBlocks[i].left -= cropLeft;
                    positioningBlocks[i].right -= cropLeft;
                    positioningBlocks[i].center_x -= cropLeft;
                    positioningBlocks[i].bottom -= cropTop;
                    positioningBlocks[i].top -= cropTop;
                    positioningBlocks[i].center_y -= cropTop;
                }
                */

                QRPos qrpos = GetQRPositioning(positioningBlocks);
                QRPos qrposTransformed = qrpos;

                double angle = SSUtils.AngleFromV1ToV2(qrpos.rightVec, new Vector2D { x = 1, y = 0 });
                RotationAroundPoint(img, img.Copy(), (float)angle, qrpos.center.x, qrpos.center.y);

                qrposTransformed.ul = SSUtils.RotateVectorAroundPoint(qrpos.ul, angle, qrpos.center);
                qrposTransformed.ur = SSUtils.RotateVectorAroundPoint(qrpos.ur, angle, qrpos.center);
                qrposTransformed.ll = SSUtils.RotateVectorAroundPoint(qrpos.ll, angle, qrpos.center);
                qrposTransformed.lr = SSUtils.RotateVectorAroundPoint(qrpos.lr, angle, qrpos.center);
                qrposTransformed.rightVec = SSUtils.SubVectors(qrposTransformed.ur, qrposTransformed.ul);
                qrposTransformed.downVec = SSUtils.SubVectors(qrposTransformed.ll, qrposTransformed.ul);
                qrposTransformed.diagonalVec = SSUtils.SubVectors(qrposTransformed.lr, qrposTransformed.ul);
                //qrposTransformed.center = qrposTransformed.center;

                if (qrpos.deformed)
                {
                    double x_shear = -qrposTransformed.downVec.x / qrposTransformed.downVec.y;
                    Shear(img, img.Copy(), (float)x_shear, 0);


                    qrposTransformed.ul = SSUtils.ShearVector(qrposTransformed.ul, x_shear, 0);
                    qrposTransformed.ur = SSUtils.ShearVector(qrposTransformed.ur, x_shear, 0);
                    qrposTransformed.ll = SSUtils.ShearVector(qrposTransformed.ll, x_shear, 0);
                    qrposTransformed.lr = SSUtils.ShearVector(qrposTransformed.lr, x_shear, 0);
                    qrposTransformed.rightVec = SSUtils.SubVectors(qrposTransformed.ur, qrposTransformed.ul);
                    qrposTransformed.downVec = SSUtils.SubVectors(qrposTransformed.ll, qrposTransformed.ul);
                    qrposTransformed.diagonalVec = SSUtils.SubVectors(qrposTransformed.lr, qrposTransformed.ul);
                    //qrposTransformed.center = ...;

                    Console.WriteLine("rightVec.ul.x: " + qrposTransformed.rightVec.x);
                    Console.WriteLine("rightVec.ul.y: " + qrposTransformed.rightVec.y);
                    Console.WriteLine("downVec.ur.x: " + qrposTransformed.downVec.x);
                    Console.WriteLine("downVec.ur.y: " + qrposTransformed.downVec.y);
                }

                double positioningBlocksDistance = SSUtils.Norm(qrposTransformed.rightVec);
                double moduleSize = positioningBlocksDistance / 14.0;

                left = (int)Math.Round(qrposTransformed.ul.x - 3.5 * moduleSize);
                right = (int)Math.Round(qrposTransformed.ul.x + 17.5 * moduleSize);
                top = (int)Math.Round(qrposTransformed.ul.y - 3.5 * moduleSize);
                bottom = (int)Math.Round(qrposTransformed.ll.y + 3.5 * moduleSize);

                Console.WriteLine("moduleSize: " + moduleSize);
                Console.WriteLine("left: " + left);
                Console.WriteLine("top: " + top);

                Width = (int)(moduleSize * 21.0);
                Height = Width;
                Center_x = (int)qrpos.center.x;
                Center_y = (int)qrpos.center.y;
                Rotation = (float)(-angle*180.0/Math.PI);
                UL_x_out = (int)(qrpos.ul.x);
                UL_y_out = (int)(qrpos.ul.y);
                UR_x_out = (int)(qrpos.ur.x);
                UR_y_out = (int)(qrpos.ur.y);
                LL_x_out = (int)(qrpos.ll.x);
                LL_y_out = (int)(qrpos.ll.y);
                BinaryOut = GetBinaryCode(ConvertToBinary(img), left, right, top, bottom, img);
            }
        }
    }
}

