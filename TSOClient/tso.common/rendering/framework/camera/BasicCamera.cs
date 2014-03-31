﻿/*The contents of this file are subject to the Mozilla Public License Version 1.1
(the "License"); you may not use this file except in compliance with the
License. You may obtain a copy of the License at http://www.mozilla.org/MPL/

Software distributed under the License is distributed on an "AS IS" basis,
WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License for
the specific language governing rights and limitations under the License.

The Original Code is the TSOClient.

The Initial Developer of the Original Code is
ddfczm. All Rights Reserved.

Contributor(s): ______________________________________.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.ComponentModel;

namespace TSO.Common.rendering.framework.camera
{
    [DisplayName("BasicCamera")]
    public class BasicCamera : ICamera
    {
        public float NearPlane { get; set; }
        public float FarPlane { get; set; }

        public float AspectRatioMultiplier { get; set; }

        protected Vector3 m_Position;
        protected Vector3 m_Target;
        protected Vector3 m_Up;
        protected GraphicsDevice m_Device;
        
        public BasicCamera(GraphicsDevice device, Vector3 Position, Vector3 Target, Vector3 Up)
        {
            m_Device = device;
            AspectRatioMultiplier = 1.0f;
            NearPlane = 1.0f;
            FarPlane = 800.0f;

            m_Position = Position;
            m_Target = Target;
            m_Up = Up;

            m_ViewDirty = true;


            /**
             * Assume the projection is full screen, center origin
             */
            ProjectionOrigin = new Vector2(
                m_Device.Viewport.Width / 2.0f,
                m_Device.Viewport.Height / 2.0f
            );
        }


        



        protected Vector2 m_ProjectionOrigin = Vector2.Zero;
        public Vector2 ProjectionOrigin
        {
            get
            {
                return m_ProjectionOrigin;
            }
            set
            {
                m_ProjectionOrigin = value;
                m_ProjectionDirty = true;
            }
        }

        protected Matrix m_Projection;
        protected bool m_ProjectionDirty;
        [Browsable(false)]
        public Matrix Projection
        {
            get
            {
                if (m_ProjectionDirty)
                {
                    CalculateProjection();
                    m_ProjectionDirty = false;
                }
                return m_Projection;
            }
        }

        protected virtual void CalculateProjection()
        {
            var device = m_Device;
            var aspect = device.Viewport.AspectRatio * AspectRatioMultiplier;

            var ratioX = m_ProjectionOrigin.X / device.Viewport.Width;
            var ratioY = m_ProjectionOrigin.Y / device.Viewport.Height;

            var projectionX = 0.0f - (1.0f * ratioX);
            var projectionY = (1.0f * ratioY);

            m_Projection = Matrix.CreatePerspectiveOffCenter(
                projectionX, projectionX + 1.0f,
                ((projectionY-1.0f) / aspect), (projectionY) / aspect,
                NearPlane, FarPlane
            );

            m_Projection = Matrix.CreateScale(Zoom, Zoom, 1.0f) * m_Projection;
        }




        protected virtual void CalculateView()
        {
            var translate = Matrix.CreateTranslation(m_Translation);
            var position = Vector3.Transform(m_Position, translate);
            var target = Vector3.Transform(m_Target, translate);

            m_View = Matrix.CreateLookAt(position, target, m_Up);
        }


        protected bool m_ViewDirty = false;
        protected Matrix m_View = Matrix.Identity;
        [Browsable(false)]
        public Matrix View
        {
            get
            {
                if (m_ViewDirty)
                {
                    m_ViewDirty = false;
                    CalculateView();
                }
                return m_View;
            }
        }

        
        protected float m_Zoom = 1.0f;
        public float Zoom
        {
            get { return m_Zoom; }
            set
            {
                //Matrix.CreateTranslation(Position) * Matrix.CreateScale(1 / Zoom)
                m_Zoom = value;
                m_ViewDirty = true;
                m_ProjectionDirty = true;
            }
        }

        protected Vector3 m_Translation;
        public Vector3 Translation
        {
            get
            {
                return m_Translation;
            }
            set
            {
                m_Translation = value;
                m_ViewDirty = true;
            }
        }

        public Vector3 Position
        {
            get
            {
                return m_Position;
            }
            set
            {
                m_Position = value;
                m_ViewDirty = true;
            }
        }

        public Vector3 Target
        {
            get
            {
                return m_Target;
            }
            set
            {
                m_Target = value;
                m_ViewDirty = true;
            }
        }

        public Vector3 Up
        {
            get
            {
                return m_Up;
            }
            set
            {
                m_Up = value;
                m_ViewDirty = true;
            }
        }




















        public bool DrawCamera = false;

        public void Draw(GraphicsDevice device)
        {
            device.RenderState.PointSize = 30.0f;
            device.VertexDeclaration = new VertexDeclaration(device, VertexPositionColor.VertexElements);
            //device.RenderState.CullMode = CullMode.None;

            var effect = new BasicEffect(device, null);


            //effect.Texture = TextureUtils.TextureFromColor(device, color);
            //effect.TextureEnabled = true;

            effect.World = Matrix.Identity;
            effect.View = View;
            effect.Projection = Projection;
            effect.VertexColorEnabled = true;
            //effect.EnableDefaultLighting();

            effect.CommitChanges();
            effect.Begin();
            foreach (var pass in effect.Techniques[0].Passes)
            {
                pass.Begin();

                var vertex = new VertexPositionColor(Position, Color.Green);
                var vertexList = new VertexPositionColor[1] { vertex };
                device.DrawUserPrimitives(PrimitiveType.PointList, vertexList, 0, 1);

                vertex.Color = Color.Red;
                vertex.Position = Target;
                device.DrawUserPrimitives(PrimitiveType.PointList, vertexList, 0, 1);


                pass.End();
            }
            effect.End();
        }
    }
}
