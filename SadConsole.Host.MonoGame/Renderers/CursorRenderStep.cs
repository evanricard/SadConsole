﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SadRogue.Primitives;
using Color = Microsoft.Xna.Framework.Color;
using XnaRectangle = Microsoft.Xna.Framework.Rectangle;
using SadConsole.Host.MonoGame;

namespace SadConsole.Renderers
{
    /// <summary>
    /// Renders a cursor.
    /// </summary>
    public class CursorRenderStep : IRenderStep
    {
        private Components.Cursor _cursor;

        ///  <inheritdoc/>
        public int SortOrder { get; set; } = 70;

        /// <summary>
        /// Sets the <see cref="Components.Cursor"/>.
        /// </summary>
        /// <param name="data">A <see cref="Components.Cursor"/> object.</param>
        public void SetData(object data)
        {
            if (data is Components.Cursor cursor)
                _cursor = cursor;
            else
                throw new Exception($"{nameof(CursorRenderStep)} must have a {nameof(Components.Cursor)} passed to the {nameof(SetData)} method");
        }

        ///  <inheritdoc/>
        public void Reset()
        {
            _cursor = null;
        }

        ///  <inheritdoc/>
        public bool Refresh(IRenderer renderer, IScreenSurface screenObject, bool backingTextureChanged, bool isForced) =>
            false;

        ///  <inheritdoc/>
        public void Composing(IRenderer renderer, IScreenSurface screenObject) { }

        ///  <inheritdoc/>
        public void Render(IRenderer renderer, IScreenSurface screenObject)
        {
            // If the tint isn't covering everything
            if (screenObject.Tint.A != 255)
            {
                // Draw any cursors
                foreach (Components.Cursor cursor in screenObject.GetSadComponents<Components.Cursor>())
                {
                    if (cursor.IsVisible && screenObject.Surface.IsValidCell(cursor.Position.X, cursor.Position.Y) && screenObject.Surface.View.Contains(cursor.Position))
                    {
                        GameHost.Instance.DrawCalls.Enqueue(
                            new DrawCalls.DrawCallGlyph(cursor.CursorRenderCell,
                                                        ((Host.GameTexture)screenObject.Font.Image).Texture,
                                                        new XnaRectangle(screenObject.Font.GetRenderRect(cursor.Position.X - screenObject.Surface.ViewPosition.X,
                                                                                                    cursor.Position.Y - screenObject.Surface.ViewPosition.Y,
                                                                                                    screenObject.FontSize).Translate(screenObject.AbsolutePosition).Position.ToMonoPoint(),
                                                                            screenObject.FontSize.ToMonoPoint()),
                                                        screenObject.Font.SolidGlyphRectangle.ToMonoRectangle(),
                                                        screenObject.Font.GetGlyphSourceRectangle(cursor.CursorRenderCell.Glyph).ToMonoRectangle()
                                                        )
                            );
                    }
                }
            }
        }


        ///  <inheritdoc/>
        public void Dispose() =>
            Reset();
    }
}
