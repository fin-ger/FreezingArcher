﻿using System;
using System.Drawing;
using System.Drawing.Text;

namespace Gwen.Renderer
{
    /// <summary>
    /// Uses System.Drawing for 2d text rendering.
    /// </summary>
    public sealed class TextRenderer : IDisposable
    {
        readonly Bitmap bmp;
        readonly Graphics gfx;
        readonly Gwen.Texture texture;
        bool disposed;

        public Gwen.Texture Texture { get { return texture; } }

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="width">The width of the backing store in pixels.</param>
        /// <param name="height">The height of the backing store in pixels.</param>
        /// <param name="renderer">GWEN renderer.</param>
        public TextRenderer(int width, int height, FreezingArcherGwenRenderer renderer)
        {
            if (width <= 0)
                throw new ArgumentOutOfRangeException("width");
            if (height <= 0)
                throw new ArgumentOutOfRangeException("height");

            bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            gfx = Graphics.FromImage(bmp);
            gfx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            // NOTE:    TextRenderingHint.AntiAliasGridFit looks sharper and in most cases better
            //          but it comes with a some problems.
            //
            //          1.  Graphic.MeasureString and format.MeasureCharacterRanges 
            //              seem to return wrong values because of this.
            //
            //          2.  While typing the kerning changes in random places in the sentence.
            // 
            //          Until 1st problem is fixed we should use TextRenderingHint.AntiAlias...  :-(

            gfx.TextRenderingHint = TextRenderingHint.AntiAlias;
            gfx.Clear(Color.Transparent);
            texture = new Texture(renderer) {Width = width, Height = height};
        }

        /// <summary>
        /// Draws the specified string to the backing store.
        /// </summary>
        /// <param name="text">The <see cref="System.String"/> to draw.</param>
        /// <param name="font">The <see cref="System.Drawing.Font"/> that will be used.</param>
        /// <param name="brush">The <see cref="System.Drawing.Brush"/> that will be used.</param>
        /// <param name="point">The location of the text on the backing store, in 2d pixel coordinates.
        /// The origin (0, 0) lies at the top-left corner of the backing store.</param>
        public void DrawString(string text, System.Drawing.Font font, Brush brush, Point point, StringFormat format)
        {
            gfx.DrawString(text, font, brush, point, format); // render text on the bitmap
           
            FreezingArcherGwenRenderer rend = texture.m_Renderer as FreezingArcherGwenRenderer;

            FreezingArcher.Renderer.Texture2D tex = rend.PrivateRendererContext.CreateTexture2D("" + DateTime.Now.Ticks, true, bmp);

            tex.Sampler.MagnificationFilter = FreezingArcher.Renderer.MagnificationFilter.InterpolateLinear;
            tex.Sampler.MinificationFilter = FreezingArcher.Renderer.MinificationFilter.InterpolateLinear;

            texture.RendererData = tex;
        }

        void Dispose(bool manual)
        {
            if (!disposed)
            {
                if (manual)
                {
                    bmp.Dispose();
                    gfx.Dispose();
                    texture.Dispose();
                }

                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~TextRenderer()
        {
            Console.WriteLine("[Warning] Resource leaked: {0}", typeof(TextRenderer));
        }
    }
}