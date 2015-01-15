//
//  DummyGraphicsResource.cs
//
//  Author:
//       Fin Christensen <christensen.fin@gmail.com>
//
//  Copyright (c) 2014 Fin Christensen
//
//  This program is free software; you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation; either version 2 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
//
using System;
using System.Collections.Generic;
using FurryLana.Engine.Graphics.Interfaces;
using FurryLana.Engine.Input;
using Pencil.Gaming.MathUtils;
using FurryLana.Engine.Interaction;

namespace FurryLana.Engine.Graphics
{
    /// <summary>
    /// Dummy graphics resource.
    /// </summary>
    public class DummyGraphicsResource : IGraphicsResource, IPosition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FurryLana.Engine.Graphics.DummyGraphicsResource"/> class.
        /// </summary>
        public DummyGraphicsResource ()
        {
            GraphicsObject = new GraphicsObject ("Graphics/Shader/RenderTarget/stdmodel.fsh",
                "Graphics/Shader/RenderTarget/stdmodel.vsh",
                "Model/Data/Stone.obj", new string[] {"Model/Data/Stone1.tif"});

            GraphicsObject2 = new GraphicsObject ("Graphics/Shader/RenderTarget/stdmodel.fsh",
                "Graphics/Shader/RenderTarget/stdmodel.vsh",
                "Model/Data/Stone.obj", new string[] {"Model/Data/Stone1.tif"});
        }

        readonly GraphicsObject GraphicsObject;
        readonly GraphicsObject GraphicsObject2;

        #region IResource implementation

        /// <summary>
        /// Init this resource. This method may not be called from the main thread as the initialization process is
        /// multi threaded.
        /// </summary>
        public void Init ()
        {
            Loaded = false;
            NeedsLoad ((Action) this.Load, null);
        }

        /// <summary>
        /// Gets the init jobs.
        /// </summary>
        /// <returns>The init jobs.</returns>
        /// <param name="list">List.</param>
        public List<Action> GetInitJobs (List<Action> list)
        {
            list.Add (Init);
            list = GraphicsObject.GetInitJobs (list);
            list = GraphicsObject2.GetInitJobs (list); 
            return list;
        }

        /// <summary>
        /// Load this resource. This method *should* be called from an extra loading thread with a shared gl context.
        /// </summary>
        public void Load ()
        {
            Loaded = true;
        }

        /// <summary>
        /// Gets the load jobs.
        /// </summary>
        /// <returns>The load jobs.</returns>
        /// <param name="list">List.</param>
        /// <param name="reloader">The NeedsLoad event handler.</param>
        public List<Action> GetLoadJobs (List<Action> list, EventHandler reloader)
        {
            list.Add (Load);
            NeedsLoad = reloader;
            list = GraphicsObject.GetLoadJobs (list, reloader);
            list = GraphicsObject2.GetLoadJobs (list, reloader);
            return list;
        }

        /// <summary>
        /// Destroy this resource.
        /// 
        /// Why not IDisposable:
        /// IDisposable is called from within the grabage collector context so we do not have a valid gl context there.
        /// Therefore I added the Destroy function as this would be called by the parent instance within a valid gl
        /// context.
        /// </summary>
        public void Destroy ()
        {
            Loaded = false;
            GraphicsObject.Destroy ();
            GraphicsObject2.Destroy ();
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="FurryLana.Engine.Graphics.DummyGraphicsResource"/>
        /// is loaded.
        /// </summary>
        /// <value><c>true</c> if loaded; otherwise, <c>false</c>.</value>
        public bool Loaded { get; protected set; }

        /// <summary>
        /// Fire this event when you need the Load function to be called.
        /// For example after init or when new resources needs to be loaded.
        /// </summary>
        /// <value>NeedsLoad handlers.</value>
        public EventHandler NeedsLoad { get; set; }

        #endregion

        #region IFrameSyncedUpdate implementation

        /// <summary>
        /// This update is called before every frame draw inside a gl context.
        /// </summary>
        /// <param name="deltaTime">Time delta.</param>
        public void FrameSyncedUpdate (float deltaTime)
        {
            GraphicsObject.FrameSyncedUpdate (deltaTime);
            GraphicsObject2.FrameSyncedUpdate (deltaTime);
        }

        #endregion

        #region IUpdate implementation

        /// <summary>
        /// This update is called in an extra thread which does not have a valid gl context.
        /// The updaterate might differ from the framerate.
        /// </summary>
        /// <param name="desc">Update description.</param>
        public void Update (UpdateDescription desc)
        {
            GraphicsObject.Update (desc);
            GraphicsObject2.Update (desc);
            GraphicsObject.Position = Position;
            GraphicsObject2.Position = Position;
            foreach (var k in desc.Keys)
            {
                if (k.Action == Pencil.Gaming.KeyAction.Repeat || k.Action == Pencil.Gaming.KeyAction.Press)
                {
                    float x = 0, y = 0;
                    if (k.Key == Pencil.Gaming.Key.W)
                        x += 0.1f;
                    if (k.Key == Pencil.Gaming.Key.A)
                        y += 0.1f;
                    if (k.Key == Pencil.Gaming.Key.S)
                        x -= 0.1f;
                    if (k.Key == Pencil.Gaming.Key.D)
                        y -= 0.1f;

                    if (k.Modifier == Pencil.Gaming.KeyModifiers.Shift)
                    {
                        x *= 2;
                        y *= 2;
                    }
                    GraphicsObject2.Position = new Vector3 (GraphicsObject2.Position.X + x,
                                                            GraphicsObject2.Position.Y,
                                                            GraphicsObject2.Position.Z + y);
                }
            }
        }

        #endregion

        #region IDrawable implementation

        /// <summary>
        /// Draw this instance.
        /// </summary>
        public void Draw ()
        {
            GraphicsObject.Draw ();
            GraphicsObject2.Draw ();
        }

        #endregion

        #region IPosition implementation

        /// <summary>
        /// Position in space
        /// </summary>
        /// <value>The position.</value>
        public Vector3 Position { get; set; }

        #endregion
    }
}
