using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Pencil.Gaming.Graphics;

namespace FreezingArcher.Renderer
{
    public class Texture1D : Texture
    {
        public int Width { get; private set; }

        internal Texture1D(int width, bool mipmapsgen, string name, int id)
            : base(name, id, GraphicsResourceType.Texture1D)
        {
            Width = width;
            MipMapsGenerated = mipmapsgen;
        }

        private int last_bound_index = 0;

        public override void Bind(int index)
        {
            last_bound_index = index;

            GL.ActiveTexture(TextureUnit.Texture0 + index);
            GL.BindTexture(TextureTarget.Texture2D, ID);

            if (SamplerAllowed)
            {
                if (m_Sampler != null)
                    m_Sampler.Bind(index);
            }
        }

        public override void Unbind()
        {
            GL.ActiveTexture(TextureUnit.Texture0 + last_bound_index);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            if (SamplerAllowed)
            {
                if (m_Sampler != null)
                    m_Sampler.Unbind();
            }
        }
    }

    public class Texture2D : Texture
    {
        public int Width { get; private set; }
        public int Height { get; private set; }


        internal Texture2D(int width, int height, bool mipmapsgen, string name, int id) : base(name, id, GraphicsResourceType.Texture2D)
        {
            Width = width;
            Height = height;
            MipMapsGenerated = mipmapsgen;
        }

        private int last_bound_index = 0;

        public override void Bind(int index)
        {
            GL.BindSampler(index, 0);
            last_bound_index = index;

            GL.ActiveTexture(TextureUnit.Texture0 + index);
            GL.BindTexture(TextureTarget.Texture2D, ID);

            if (SamplerAllowed)
            {
                if (m_Sampler != null)
                    m_Sampler.Bind(index);
            }
        }

        public override void Unbind()
        {
            GL.ActiveTexture(TextureUnit.Texture0 + last_bound_index);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            if (SamplerAllowed)
            {
                if (m_Sampler != null)
                    m_Sampler.Unbind();
            }
        }

        public override void Resize(int width, int height)
        {
            GL.BindTexture(TextureTarget.Texture2D, ID);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero);

            GL.BindTexture(TextureTarget.Texture2D, 0);

            Width = width;
            Height = height;
        }

        public FreezingArcher.Math.Color4 GetPixelColor(int x, int y)
        {
            FreezingArcher.Math.Color4 col;

            Bind(0);

            byte[] data = new byte[Width * Height * 4];

            GL.GetTexImage<byte>(TextureTarget.Texture2D, 0, PixelFormat.Rgba, PixelType.UnsignedByte, data);

            Unbind();

            int index = (y * Width + x) * 4;

            col = new FreezingArcher.Math.Color4(data[index], data[index + 1], data[index + 2],
                data[index + 3]);

            return col;
        }
    }

    public class Texture3D : Texture
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Depth { get; private set; }

        internal Texture3D(int width, int height, int depth, bool mipmapsgen, string name, int id)
            : base(name, id, GraphicsResourceType.Texture3D)
        {
            Width = width;
            Height = height;
            Depth = depth;
            MipMapsGenerated = mipmapsgen;
        }

        private int last_bound_index = 0;

        public override void Bind(int index)
        {
            last_bound_index = index;

            GL.ActiveTexture(TextureUnit.Texture0 + index);
            GL.BindTexture(TextureTarget.Texture2D, ID);

            if (SamplerAllowed)
            {
                if (m_Sampler != null)
                    m_Sampler.Bind(index);
            }
        }

        public override void Unbind()
        {
            GL.ActiveTexture(TextureUnit.Texture0 + last_bound_index);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            if (SamplerAllowed)
            {
                if (m_Sampler != null)
                    m_Sampler.Unbind();
            }
        }
    }

    public class TextureDepthStencil : Texture2D
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        internal TextureDepthStencil(int width, int height, string name, int id) : base(width, height, false, name, id)
        {
            Width = width;
            Height = height;
        }

        public override void Resize(int width, int height)
        {
            GL.BindTexture(TextureTarget.Texture2D, ID);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Depth24Stencil8, width, height, 0, PixelFormat.DepthStencil, PixelType.UnsignedInt248, IntPtr.Zero);

            GL.BindTexture(TextureTarget.Texture2D, 0);

            Width = width;
            Height = height;
        }
    }

    public class Texture : GraphicsResource
    {
        public bool MipMapsGenerated { get; protected set; }

        protected Sampler m_Sampler;
        public Sampler Sampler
        {
            get
            {
                if (!Created)
                    throw new Exception("Resource is not created!");

                return m_Sampler;
            }

            internal set
            {
                if (!Created)
                    throw new Exception("Resource is not created!");

                if (m_Sampler != null)
                    m_Sampler.SetUseCount(m_Sampler.InternalUseCount - 1);

                m_Sampler = value;

                if (m_Sampler != null)
                    m_Sampler.SetUseCount(m_Sampler.InternalUseCount + 1);
            }
        }
        internal bool SamplerAllowed { get; set; }

        internal Texture(string name, int id, GraphicsResourceType grt) : base (name, id, grt)
        {
            
        }

        public virtual void Resize(int width, int height) {}

        public virtual void Bind(int index) { }
        public virtual void Unbind() { }
    }
}
