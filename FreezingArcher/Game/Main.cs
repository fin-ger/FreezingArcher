//
//  Main.cs
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
using FreezingArcher.Core;
using FreezingArcher.Math;
using FreezingArcher.Renderer.Scene;

namespace FreezingArcher.Game
{
    /// <summary>
    /// FurryLana static main class.
    /// </summary>
    public class FurryLana
    {
        /// <summary>
        /// The entry point of the program, where the program control starts and ends.
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        public static void Main (string[] args)
        {
            Application.Instance = new Application ("Freezing Archer", args);
            Application.Instance.Init ();
            Application.Instance.Load ();

            if (!Application.Instance.IsCommandLineInterface)
            {
                Content.Game game = Application.Instance.Game;
                var rc = Application.Instance.RendererContext;
                var msgmnr = Application.Instance.MessageManager;
                var objmnr = Application.Instance.ObjectManager;

                game.AddGameState("default", Content.Environment.Default, rc.Scene);
                rc.Scene = new CoreScene(msgmnr);
                rc.Scene.BackgroundColor = Color4.Crimson;
                //rc.Scene.CameraManager.AddCam (new BaseCamera (msgmnr),"test");
                var cammnr = rc.Scene.CameraManager;

                //new MazeTest(msgmnr, objmnr, rc.Scene, game);
                new ECSTest(msgmnr, rc.Scene);
                //new PhysicsTest(Application.Instance);

                //new InventoryTest();
            }

            Application.Instance.Run ();
            Application.Instance.Destroy ();
        }
    }
}
