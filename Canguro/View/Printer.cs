using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using System.Drawing;
using System.Drawing.Imaging;

namespace Canguro.View
{
    class Printer
    {
        // 3 Megapixels
        static int theoreticalGoalWidth = 2048;
        static int theoreticalGoalHeight = 1536;

        private bool landscape;

        public bool IsOrientedLandscape
        {
            get { return inquiryLandscape(); }
        }

        private Bitmap hiresBitmap = null;

        public Bitmap HiResBitmap
        {
            get { return hiresBitmap; }
        }

        private Printer()
        {
        }

        public static readonly Printer Instance = new Printer();

        private bool inquiryLandscape()
        {
            return (float)GraphicViewManager.Instance.ActiveView.Viewport.Width / (float)GraphicViewManager.Instance.ActiveView.Viewport.Height >= 1.0f;
        }

        public void PrintHRImage(Device device)
        {
            float viewportAspectRatio;
            float currViewportMaxValue;
            landscape = true;

            GraphicViewManager gvm = GraphicViewManager.Instance;

            GraphicView activeView = gvm.ActiveView;

            // Store local variables containing goal size
            int hrImWidth = theoreticalGoalWidth;
            int hrImHeight = theoreticalGoalHeight;

            // Current viewport size
            int currentWidth = activeView.Viewport.Width;
            int currentHeight = activeView.Viewport.Height;

            // Current max size in viewport 
            currViewportMaxValue = currentWidth;

            // Get Viewport Aspect Ratio
            viewportAspectRatio = (float)currentWidth / (float)currentHeight;

            float vpScale = (float)hrImWidth / (float)currentWidth;
            vpScale = (vpScale < (float)hrImHeight / (int)currentHeight) ? (float)hrImHeight / (float)currentHeight : vpScale;

            // Swap dimensios for having the right aspect ratio
            landscape = inquiryLandscape();
            if (!landscape)
            {
                //// Swap sizes in a fast way
                //currentWidth ^= currentHeight;
                //currentHeight ^= currentWidth;
                //currentWidth ^= currentHeight;

                currViewportMaxValue = activeView.Viewport.Height;

                landscape = false;
            }

            float scale = hrImWidth / currViewportMaxValue;
            //scale = vpScale;

            #region Build a bitmap for storing the high resolution image
            // Get the current scene in device
            Surface renderTarget = gvm.PresentRender;
            // Set default pixel format
            PixelFormat pixFormat = PixelFormat.Format32bppArgb;

            // Get surface's format and pixel format
            int bitsPerPixel = getRenderTargetFormat(renderTarget, ref pixFormat);
            // Get number of byte used to represent a pixel
            int bytesPerPixel = bitsPerPixel >> 3;

            // Create the hires Image
            int goalWidth = (int)(scale * currentWidth);
            int goalHeight = (int)(scale * currentHeight);

            if (hiresBitmap != null)
            {
                hiresBitmap.Dispose();
                hiresBitmap = null;
            }

            hiresBitmap = new Bitmap(goalWidth, goalHeight);

            renderTarget.Dispose();
            renderTarget = null;
            #endregion

            // Store the current status of GraphicViewManager.DrawingPickingSurface
            bool pickingMode = gvm.DrawingPickingSurface;
            bool drawingworldAxes = gvm.DrawWorldAxes;
            gvm.PrintingHiResImage = true;

            // Set picking mode on false, so we can render the interest image
            gvm.DrawingPickingSurface = false;

            makePrintingImage(device, gvm, activeView, scale, currentWidth, currentHeight, bytesPerPixel, hiresBitmap, pixFormat);

            //hiresBitmap.Save("./test.bmp");

            // Restore the status of graphicViewManager
            gvm.DrawingPickingSurface = pickingMode;
            gvm.DrawWorldAxes = drawingworldAxes;
            gvm.PrintingHiResImage = false;
            activeView.ViewMatrix = activeView.ArcBallCtrl.ViewMatrix;
            activeView.Update(device);
        }

        void makePrintingImage(Device device, GraphicViewManager gvm, GraphicView activeView, float scale, int currentWidth, int currentHeight, int bytesPerPixel, Bitmap hiresBitmap, PixelFormat pixFormat)
        {
            ArcBall arcBallCopy = (ArcBall)activeView.ArcBallCtrl.Clone();
            Surface renderTarget;

            // Set zoom factor for building an appropriate printing screenshot
            arcBallCopy.ZoomAbsolute(scale * arcBallCopy.ScalingFac);

            // Code for updating each of the parts in the hires Image
            float smallestGEIntegerScale = (float)Math.Ceiling(scale);
            float xMov = 0, yMov = 0;
            float xStep = currentWidth;// goalWidth / (int)(scale);
            float yStep = currentHeight;// goalHeight / (int)(scale);
            Rectangle lockedRectangle = new Rectangle(0, 0, currentWidth, currentHeight);

            xMov = (int)(xStep);
            yMov = (int)(yStep);

            // Generate a mouse event for emulating pan movements
            System.Windows.Forms.MouseEventArgs e;
            int startingX, startingY;

            startingX = -(int)(((scale - 1) / 2 - 1) * currentWidth);
            startingY = -(int)(((scale - 1) / 2 - 1) * currentHeight);

            e = new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 0, startingX, startingY, 0);
            arcBallCopy.OnBeginPan(e);

            gvm.DrawWorldAxes = true;

            for (int h = 0; h < (int)smallestGEIntegerScale; ++h)
            {
                int presentHeight = (h == (int)smallestGEIntegerScale - 1) ? (int)((scale - h) * currentHeight) : currentHeight;
                lockedRectangle.Height = presentHeight;

                for (int w = 0; w < (int)smallestGEIntegerScale; ++w)
                {
                    e = new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 0, (int)(xMov + 0.5f), (int)(yMov + 0.5f), 0);
                    arcBallCopy.OnMovePan(e);

                    // Here we set the active Matrix 
                    activeView.ViewMatrix = arcBallCopy.ViewMatrix;

                    // Update view -> Render according to the present options
                    activeView.Update(device);

                    // After update, ask for the last rendered scene, the one who actually is the present scene
                    renderTarget = gvm.PresentRender;

                    // Append that image to the hires Image
                    int presentWidth = (w == (int)smallestGEIntegerScale - 1) ? (int)((scale - w) * currentWidth) : currentWidth;
                    lockedRectangle.Width = presentWidth;
                    copySurfaceToBitmap(renderTarget, presentWidth, presentHeight, bytesPerPixel, hiresBitmap, lockedRectangle, pixFormat);

                    arcBallCopy.OnBeginPan(e);

                    // Update xMov and yMov 
                    xMov -= (int)xStep;

                    // Update Rectangle for the next copied image                    
                    lockedRectangle.X = (w + 1) * (int)xStep;

                    gvm.DrawWorldAxes = false;

                    renderTarget.Dispose();
                    renderTarget = null;
                }
                xMov = (int)xStep;
                yMov -= (int)yStep;
                lockedRectangle.X = 0;
                lockedRectangle.Width = currentWidth;
                lockedRectangle.Y = (h + 1) * (int)yStep;
            }
        }

        void copySurfaceToBitmap(Surface lastRenderedScene, int currWidth, int currHeight, int bytesPerPixel, Bitmap hiresBitmap, Rectangle rect2Lock, PixelFormat pixFormat)
        {
            GraphicsStream gs = SurfaceLoader.SaveToStream(ImageFileFormat.Bmp, lastRenderedScene);

            Bitmap renderedBitmap = new System.Drawing.Bitmap(gs, false);

            BitmapData hiResData = hiresBitmap.LockBits(rect2Lock, ImageLockMode.WriteOnly, pixFormat);
            BitmapData renderedData = renderedBitmap.LockBits(new Rectangle(0, 0, currWidth, currHeight), ImageLockMode.ReadOnly, pixFormat);

            unsafe
            {
                byte* renderedDataPtr = (byte*)renderedData.Scan0;
                byte* hiResImPtr = (byte*)hiResData.Scan0;

                for (int i = 0; i < currHeight; ++i)
                {
                    for (int j = 0; j < currWidth; ++j)
                    {
                        *(hiResImPtr + 0) = *(renderedDataPtr + 0);    // B
                        *(hiResImPtr + 1) = *(renderedDataPtr + 1);    // G
                        *(hiResImPtr + 2) = *(renderedDataPtr + 2);    // R
                        *(hiResImPtr + 3) = *(renderedDataPtr + 3);    // A

                        renderedDataPtr += bytesPerPixel;
                        hiResImPtr += bytesPerPixel;
                    }
                    hiResImPtr += hiResData.Stride - hiResData.Width * bytesPerPixel;
                    renderedDataPtr += renderedData.Stride - renderedData.Width * bytesPerPixel;
                }

                renderedDataPtr = null;
                hiResImPtr = null;
            }

            hiresBitmap.UnlockBits(hiResData);
            renderedBitmap.UnlockBits(renderedData);

            renderedBitmap.Dispose();
            gs.Dispose();
        }

        private int getRenderTargetFormat(Surface renderTarget, ref System.Drawing.Imaging.PixelFormat pixFormat)
        {
            int bitsPerPixel = 0;

            switch (renderTarget.Description.Format)
            {
                // 128
                case Format.A32B32G32R32F:
                    pixFormat = System.Drawing.Imaging.PixelFormat.DontCare;
                    bitsPerPixel = 128;
                    break;
                // 64 bits
                case Format.A16B16G16R16: case Format.A16B16G16R16F:
                    pixFormat = System.Drawing.Imaging.PixelFormat.Format64bppArgb;
                    bitsPerPixel = 64;
                    break;
                // 32 bits
                // case Format.A2B10G10R10: case Format.A2R10G10B10: case Format.A2W10V10U10:  case Format.R32F:
                case Format.R8G8B8G8:  case Format.X8L8V8U8: case Format.X8R8G8B8: case Format.A8B8G8R8: case Format.A8R8G8B8: case Format.X8B8G8R8:
                    pixFormat = System.Drawing.Imaging.PixelFormat.Format32bppArgb;
                    bitsPerPixel = 32;
                    break;
                // 24 bits
                case Format.R8G8B8:
                    pixFormat = System.Drawing.Imaging.PixelFormat.Format24bppRgb;
                    bitsPerPixel = 24;
                    break;
                // 16 bits
                //case Format.A4R4G4B4: case Format.A8L8: case Format.A8P8: case Format.A8R3G3B2:  case Format.R16F: case Format.X4R4G4B4:
                case Format.A1R5G5B5: case Format.X1R5G5B5: 
                    pixFormat = System.Drawing.Imaging.PixelFormat.Format16bppArgb1555;
                    bitsPerPixel = 16;
                    break;
                case Format.R5G6B5: 
                    pixFormat = System.Drawing.Imaging.PixelFormat.Format16bppRgb565;
                    bitsPerPixel = 16;
                    break;
                // 8 bits
                case Format.A8: case Format.A4L4: case Format.R3G3B2:
                    pixFormat = System.Drawing.Imaging.PixelFormat.Format8bppIndexed;
                    bitsPerPixel = 8;
                    break;
            }

            return bitsPerPixel;
        }
    }
}
