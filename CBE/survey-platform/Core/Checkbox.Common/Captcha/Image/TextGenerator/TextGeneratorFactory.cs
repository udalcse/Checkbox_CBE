 using System;

namespace Checkbox.Common.Captcha.Image.TextGenerators
{
	/// <summary>
	/// Summary description for TextGeneratorFactory.
	/// </summary>
    public abstract class TextGeneratorFactory
	{
        /// <summary>
        /// Create a text image generator based on the text style.
        /// </summary>
        /// <param name="textStyle">Style of text.</param>
        /// <returns>Text generator object or NULL of no generator exists for the specified style.</returns>
		public static ITextGenerator CreateGenerator(TextStyleEnum textStyle)
		{
			ITextGenerator generator = null;

			switch(textStyle) 
			{
				case TextStyleEnum.BlackOverlap:
				{
					generator = new BlackOverlapTextGenerator();
					break;
				}
				case TextStyleEnum.Chess:
				{
					generator = new ChessTextGenerator();
					break;
				}
				case TextStyleEnum.Chess3D:
				{
					generator = new Chess3DTextGenerator();
					break;
				}
				case TextStyleEnum.Chipped:
				{
					generator = new ChippedTextGenerator();
					break;
				}
				case TextStyleEnum.Circles:
				{
					generator = new CirclesTextGenerator();
					break;
				}
				case TextStyleEnum.Corrosion:
				{
					generator = new CorrosionTextGenerator();
					break;
				}
				case TextStyleEnum.Distortion:
				{
					generator = new DistortionTextGenerator();
					break;
				}
				case TextStyleEnum.Flash:
				{
					generator = new FlashTextGenerator();
					break;
				}
				case TextStyleEnum.Jail:
				{
					generator = new GridTextGenerator();
					break;
				}
				case TextStyleEnum.Mass:
				{
					generator = new MassTextGenerator();
					break;
				}
				case TextStyleEnum.Negative:
				{
					generator = new NegativeTextGenerator();
					break;
				}
				case TextStyleEnum.Overlap:
				{
					generator = new OverlapTextGenerator();
					break;
				}
				case TextStyleEnum.Overlap2:
				{
					generator = new Overlap2TextGenerator();
					break;
				}
				case TextStyleEnum.Rough:
				{
					generator = new RoughTextGenerator();
					break;
				}
				case TextStyleEnum.Snow:
				{
					generator = new SnowTextGenerator();
					break;
				}
				case TextStyleEnum.Split:
				{
					generator = new SplitTextGenerator();
					break;
				}
				case TextStyleEnum.Stitch:
				{
					generator = new StitchTextGenerator();
					break;
				}
				case TextStyleEnum.WantedCircular:
				{
					generator = new WantedCircularTextGenerator();
					break;
				}
				case TextStyleEnum.Wave:
				{
					generator = new WaveTextGenerator();
					break;
				}
				case TextStyleEnum.Darts:
				{
					generator = new	DartsTextGenerator();
					break;
				}
				case TextStyleEnum.FingerPrints:
				{
					generator = new	FingerPrintsTextGenerator();
					break;
				}
				case TextStyleEnum.Lego:
				{
				generator = new	LegoTextGenerator();
					break;
				}
				case TextStyleEnum.Strippy:
				{
					generator = new	StrippyTextGenerator();
					break;
				}
				case TextStyleEnum.CrossShadow:
				{
					generator = new	CrossShadowTextGenerator();
					break;
				}
				case TextStyleEnum.CrossShadow2:
				{
					generator = new	CrossShadow2TextGenerator();
					break;
				}
				case TextStyleEnum.ThickThinLines:
				{
					generator = new	ThickThinLinesTextGenerator();
					break;
				}
				case TextStyleEnum.ThickThinLines2:
				{
					generator = new	ThickThinLines2TextGenerator();
					break;
				}	
				case TextStyleEnum.SunRays:
				{
					generator = new	SunRaysTextGenerator();
					break;
				}
				case TextStyleEnum.SunRays2:
				{
					generator = new	SunRays2TextGenerator();
					break;
				}				
				case TextStyleEnum.ThinWavyLetters:
				{
					generator = new	ThinWavyLettersTextGenerator();
					break;
				}
				case TextStyleEnum.Chalkboard:
				{
					generator = new	ChalkboardTextGenerator();
					break;
				}
				case TextStyleEnum.WavyColorLetters:
				{
					generator = new	WavyColorLettersTextGenerator();
					break;
				}
				case TextStyleEnum.AncientMosaic:
				{
					generator = new	AncientMosaicTextGenerator();
					break;
				}	
				case TextStyleEnum.	Vertigo:
				{
					generator = new	VertigoTextGenerator();
					break;
				}
				case TextStyleEnum.WavyChess:
				{
					generator = new	WavyChessTextGenerator();
					break;
				}
				case TextStyleEnum.MeltingHeat:
				{
					generator = new	MeltingHeatTextGenerator();
					break;
				}
				case TextStyleEnum.SunAndWarmAir:
				{
					generator = new	SunAndWarmAirTextGenerator();
					break;
				}
				case TextStyleEnum.Graffiti:
				{
					generator = new	GraffitiTextGenerator();
					break;
				}
				case TextStyleEnum.Graffiti2:
				{
					generator = new	Graffiti2TextGenerator();
					break;				
				}
				case TextStyleEnum.Halo:
				{
					generator = new	HaloTextGenerator();
					break;
				}
				case TextStyleEnum.Bullets:
				{
					generator = new	BulletsTextGenerator();
					break;
				}
				case TextStyleEnum.Bullets2:
				{
					generator = new	Bullets2TextGenerator();
					break;
				}
				case TextStyleEnum.CaughtInTheNet:
				{
					generator = new	CaughtInTheNetTextGenerator();
					break;
				}
				case TextStyleEnum.CaughtInTheNet2:
				{
					generator = new	CaughtInTheNet2TextGenerator();
					break;
				}
				case TextStyleEnum.Cut:
				{
					generator = new	CutTextGenerator();
					break;
				}
				case TextStyleEnum.Ghostly:
				{
					generator = new	GhostlyTextGenerator();
					break;
				}
				case TextStyleEnum.InBandages:
				{
					generator = new	InBandagesTextGenerator();
					break;
				}
				case TextStyleEnum.PaintMess:
				{
					generator = new	PaintMessTextGenerator();
					break;
				}
				case TextStyleEnum.Collage:
				{
					generator = new	CollageTextGenerator();
					break;
				}
				case TextStyleEnum.Spiderweb:
				{
					generator = new	SpiderWebTextGenerator();
					break;
				}
			}

			return generator;
		}
	}
}
