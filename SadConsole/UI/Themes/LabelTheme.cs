﻿using System;
using System.Runtime.Serialization;
using SadConsole.UI.Controls;
using SadRogue.Primitives;

namespace SadConsole.UI.Themes
{
    /// <summary>
    /// A basic theme for a drawing surface that simply fills the surface based on the state.
    /// </summary>
    [DataContract]
    public class LabelTheme : ThemeBase
    {
        /// <summary>
        /// When true, only uses <see cref="ThemeStates.Normal"/> for drawing.
        /// </summary>
        [DataMember]
        public bool UseNormalStateOnly { get; set; } = true;

        /// <summary>
        /// The decorator to use when the <see cref="Controls.Label.ShowUnderline"/> is <see langword="true"/>.
        /// </summary>
        [DataMember]
        public CellDecorator DecoratorUnderline { get; set; }

        /// <summary>
        /// The decorator to use when the <see cref="Controls.Label.ShowStrikethrough"/> is <see langword="true"/>.
        /// </summary>
        [DataMember]
        public CellDecorator DecoratorStrikethrough { get; set; }

        /// <inheritdoc />
        public override void UpdateAndDraw(ControlBase control, TimeSpan time)
        {
            if (!control.IsDirty) return;
            if (!(control is Label label)) return;

            RefreshTheme(control.FindThemeColors(), control);
            ColoredGlyph appearance;

            if (!UseNormalStateOnly)
                appearance = ControlThemeState.GetStateAppearance(control.State);
            else
                appearance = ControlThemeState.Normal;

            label.Surface.Fill(label.TextColor ?? appearance.Foreground, appearance.Background, 0);
            label.Surface.Print(0, 0, label.DisplayText.Align(label.Alignment, label.Surface.BufferWidth));

            Font font = label.AlternateFont ?? label.Parent?.Host.ParentConsole?.Font;
            Color color = label.TextColor ?? appearance.Foreground;

            if (font != null)
            {
                if (label.ShowUnderline && label.ShowStrikethrough)
                    label.Surface.SetDecorator(0, label.Surface.BufferWidth, GetStrikethrough(font, color), GetUnderline(font, color));
                else if (label.ShowUnderline)
                    label.Surface.SetDecorator(0, label.Surface.BufferWidth, GetUnderline(font, color));
                else if (label.ShowStrikethrough)
                    label.Surface.SetDecorator(0, label.Surface.BufferWidth, GetStrikethrough(font, color));
            }

            label.IsDirty = false;
        }

        private CellDecorator GetStrikethrough(Font font, Color color)
        {
            if (DecoratorStrikethrough != CellDecorator.Empty)
                return DecoratorStrikethrough;

            if (font.HasGlyphDefinition("strikethrough"))
                return font.GetDecorator("strikethrough", color);

            return new CellDecorator(color, 196, Mirror.None);
        }

        private CellDecorator GetUnderline(Font font, Color color)
        {
            if (DecoratorUnderline != CellDecorator.Empty)
                return DecoratorUnderline;

            if (font.HasGlyphDefinition("underline"))
                return font.GetDecorator("underline", color);

            return new CellDecorator(color, 95, Mirror.None);
        }

        /// <inheritdoc />
        public override ThemeBase Clone() => new LabelTheme()
        {
            ControlThemeState = ControlThemeState.Clone(),
            UseNormalStateOnly = UseNormalStateOnly,
            DecoratorStrikethrough = DecoratorStrikethrough,
            DecoratorUnderline = DecoratorUnderline
        };
    }
}