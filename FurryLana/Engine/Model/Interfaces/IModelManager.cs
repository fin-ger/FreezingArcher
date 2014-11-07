//
//  IModelManager.cs
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

namespace FurryLana.Engine.Model.Interfaces
{
    /// <summary>
    /// Model manager interface.
    /// </summary>
    public interface IModelManager : IManager<IModel>
    {
        /// <summary>
        /// Loads model from location.
        /// </summary>
        /// <returns>The model.</returns>
        /// <param name="location">The path to the model.</param>
        IModel LoadFromLocation (string location);

        /// <summary>
        /// Loads models from xml.
        /// </summary>
        /// <returns>The model.</returns>
        /// <param name="filepath">Path to xml file.</param>
        IModel LoadFromXML (string filepath);

        /// <summary>
        /// Clear all models from model manager.
        /// </summary>
        void Clear ();
    }
}