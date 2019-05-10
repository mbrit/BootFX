// BootFX - Application framework for .NET applications
// 
// File: PaintHelper.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Provides comon paint functionality.
	/// </summary>
	public sealed class PaintHelper
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		private PaintHelper()
		{
		}

		/// <summary>
		/// Gets the colors used for graduated background painting.
		/// </summary>
		/// <returns></returns>
		public static Color[] GetGraduatedBackgroundColors(GraduatedBackground background)
		{
			switch(background)
			{
				case GraduatedBackground.None:
					return new Color[] { SystemColors.Control, SystemColors.Control };
				case GraduatedBackground.ToDialog:
					return new Color[] { SystemColors.ControlLightLight, SystemColors.Control };

				default:
                    throw new NotSupportedException(string.Format("Cannot handle '{0}'.", background));

			}
		}

		/// <summary>
		/// Paints graduated to dialog background.
		/// </summary>
		/// <param name="graphics"></param>
		/// <param name="rectangle"></param>
		/// <param name="clipRectangle"></param>
		public static void PaintGraduatedBackground(Graphics graphics, GraduatedBackground background, Rectangle rectangle, Rectangle clipRectangle)
		{
			if(graphics == null)
				throw new ArgumentNullException("graphics");

			Color[] colors = GetGraduatedBackgroundColors(background);
			PaintGraduated(graphics, rectangle, clipRectangle, colors[0], colors[1]);
		}

		/// <summary>
		/// Paints graduated to dialog background.
		/// </summary>
		/// <param name="graphics"></param>
		/// <param name="rectangle"></param>
		/// <param name="clipRectangle"></param>
		public static void PaintGraduated(Graphics graphics, Rectangle rectangle, Rectangle clipRectangle, Color fromColor, Color toColor)
		{
			if(graphics == null)
				throw new ArgumentNullException("graphics");
			
			// paint...
			using(Brush brush = new LinearGradientBrush(rectangle, fromColor, toColor, 90))
			{
				graphics.FillRectangle(brush, rectangle);
			}
		}
	}
}
