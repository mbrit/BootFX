// BootFX - Application framework for .NET applications
// 
// File: SimpleMarkupRendererBase.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Diagnostics;
using System.ComponentModel;

namespace BootFX.Common.UI
{
	/// <summary>
	/// Summary description for SimpleMarkupRenderer.
	/// </summary>
	[Obsolete("Deprecated - use BootCMS instead.")]
	public abstract class SimpleMarkupRendererBase
	{
		private const string BrTag = "<br />";

		/// <summary>
		/// Raised when a link needs to be formatted.
		/// </summary>
		public event FormatLinkEventHandler FormatLink;
		
		/// <summary>
		/// Raised when an image URL needs to be expanded.
		/// </summary>
		public event UrlEventHandler ExpandImageUrl;
		
		/// <summary>
		/// Private field to support <c>ImageCssClass</c> property.
		/// </summary>
		private string _imageCssClass;
		
		/// <summary>
		/// Private field to support <c>LinkCssClass</c> property.
		/// </summary>
		private string _linkCssClass;
		
		private string _contentLinkPattern = null;

		private Regex _boldRegex;
 
		private Regex _boxRegex;
 
		private Regex _breakoutRegex;
 
		private Regex _bulletRegex;
 
		private Regex _emailRegex;
 
		private string _heading1CssStyle;
 
		private string _heading2CssStyle;
 
		private string _heading3CssStyle;
 
		private string _heading4CssStyle;
 
		private string _heading5CssStyle;

		private string _tdCssStyle;

		private Regex _imageRegex;
 
		private Regex _italicsRegex;
 
		private Regex _codeInTextRegex;
 
		private Regex _linkRegex;
 
		private Regex _specialCharsRegex;
 
		/// <summary>
		/// Private field to support <c>EmbeddedRegex</c> property.
		/// </summary>
		private Regex _embeddedRegex;
		
		public SimpleMarkupRendererBase()
		{
			this._heading1CssStyle = "heading1";
			this._heading2CssStyle = "heading2";
			this._heading3CssStyle = "heading3";
			this._heading4CssStyle = "heading4";
			this._heading5CssStyle = "heading5";
			this._tdCssStyle = "bodytext";
			this._codeInTextRegex = new Regex(@"\$(?<text>[^\$]*)\$", (RegexOptions.Singleline | RegexOptions.IgnoreCase));
			this._italicsRegex = new Regex("_(?<text>[^_]*)_", (RegexOptions.Singleline | RegexOptions.IgnoreCase));
			this._boldRegex = new Regex(@"\*(?<text>[^\*]*)\*", (RegexOptions.Singleline | RegexOptions.IgnoreCase));
			this._boxRegex = new Regex(@"^(?<name>[\w]*):\s*(?<content>.*)", (RegexOptions.Singleline | RegexOptions.IgnoreCase));
			this._bulletRegex = new Regex(@"^\s*(?<type>[\*1])\s*(?<text>.*)", (RegexOptions.Singleline | RegexOptions.IgnoreCase));
			this._specialCharsRegex = new Regex("[\\\"<>&\u2026]", (RegexOptions.Singleline | RegexOptions.IgnoreCase));
			this._linkRegex = new Regex(@"\|(?<url>[^\|]*)\|(?<text>[^\|]*)\|", (RegexOptions.Singleline | RegexOptions.IgnoreCase));
			this._breakoutRegex = new Regex(@"^\s*%(?<type>[\w\s]*)%\s*(?<content>.*)$", (RegexOptions.Singleline | RegexOptions.IgnoreCase));
			this._emailRegex = new Regex(@"\b(?<address>(?<name>[\w*]*)@(?<domain>[\w\.*]*))\b", (RegexOptions.Singleline | RegexOptions.IgnoreCase));
			this._imageRegex = new Regex(@"\[\s*(?<url>[^\]]*)\s*\]", (RegexOptions.Singleline | RegexOptions.IgnoreCase));
			this._embeddedRegex = new Regex(@"<%[^%]%>", (RegexOptions.Singleline | RegexOptions.IgnoreCase));
		}
 
		public void AppendLineBreaks(StringBuilder builder, int numBreaks)
		{
			if (builder == null)
			{
				throw new ArgumentNullException("builder");
			}
			string text1 = builder.ToString().TrimEnd(new char[0]);
			int num1 = 0;
			while (text1.EndsWith(BrTag))
			{
				++num1;
				text1 = text1.Substring(0, (text1.Length - 6)).TrimEnd(new char[0]);
			}
			int num2 = num1;
			while ((num2 < numBreaks))
			{
				builder.Append(BrTag);
				++num2;
			}
		}
 
//		private string EmailEvaluator(Match match)
//		{
//			if (match == null)
//			{
//				throw new ArgumentNullException("match");
//			}
//			EmailObfuscator obfuscator1 = new EmailObfuscator();
//			string text1 = EmailObfuscator.CreateControlId();
//			StringBuilder builder1 = new StringBuilder();
//			builder1.Append("<a id=\"");
//			builder1.Append(text1);
//			builder1.Append("\" class=\"jlink\">");
//			builder1.Append("(If you can't read this - please review the 'Can't see e-mail addresses' notes on the 'Contact' page)");
//			builder1.Append("</a>");
//			DateTime time1 = DateTime.Now;
//			string text2 = string.Format("mailto:{0}.{1}@{2}", match.Groups["name"].Value, time1.ToString("MMMyy").ToLower(), match.Groups["domain"].Value);
//			string text3 = obfuscator1.GetScriptHtml(match.Groups["address"].Value, text2, text1);
//			builder1.Append(text3);
//			return builder1.ToString();
//		}
 
		private string GetEndTag(string tag)
		{
			return ("</" + tag + ">");
		}
 
		public string GetHtml(string text)
		{
			string text7;
			if ((text == null) || (text.Length == 0))
			{
				text7 = string.Empty;
			}
			else
			{
				Stack stack1 = new Stack();
				StringBuilder builder1 = new StringBuilder();
				builder1.Append("<font class=\"bodytext\">");
				string[] textArray2 = this.GetParagraphs(text);
				int num2 = 0;
				while ((num2 < textArray2.Length))
				{
					string text1 = textArray2[num2];
					string trimmed = text1.Trim();
					if(!(trimmed.StartsWith("<[[TABLE")) && !(trimmed.StartsWith("<[[HTML")))
						text1 = ParseParagraph(text1);

					bool flag1 = false;
					if (text1.TrimStart(new char[0]).Length > 0)
					{
						Match match1 = this.BoxRegex.Match(text1);
						if (match1.Length > 0)
						{
							this.AppendLineBreaks(builder1, 2);
							builder1.Append("<table cellspacing=0 cellpadding=0>");
							builder1.Append("<tr><td height=10></td></tr>");
							builder1.Append("<tr><td class=\"box\">");
							string caption = match1.Groups["name"].Value.Trim();
							if(caption.Length > 0)
							{
								builder1.Append("<b>");
								builder1.Append(caption);
								builder1.Append("</b>: ");
							}
							builder1.Append(match1.Groups["content"].Value);
							builder1.Append("</td><tr>");
							builder1.Append("<tr><td height=10></td></tr>");
							builder1.Append("</table>");
							this.AppendLineBreaks(builder1, 1);
							flag1 = true;
						}
						else
						{
							match1 = this.BulletRegex.Match(text1);
							if (match1.Success)
							{
								string text2 = match1.Groups["type"].Value;
								if (text2 != "1")
								{
									text2 = "*";
								}
								string text3 = null;
								if (stack1.Count > 0)
								{
									text3 = ((string) stack1.Peek());
								}
								string text4 = "ul";
								if (text2 == "1")
								{
									text4 = "ol";
								}
								if (text4 != text3)
								{
									builder1.Append(this.GetStartTag(text4));
									stack1.Push(text4);
								}
								builder1.Append(this.GetStartTag("li"));
								builder1.Append(match1.Groups["text"].Value);
								builder1.Append(this.GetEndTag("li"));
								flag1 = true;
							}
							else
							{
								if (stack1.Count > 0)
								{
									string text5 = ((string) stack1.Pop());
									builder1.Append(this.GetEndTag(text5));
								}
								match1 = this.BreakoutRegex.Match(text1);
								if (match1.Success)
								{
									builder1.Append("<div class=\"");
									builder1.Append(match1.Groups["type"].Value);
									builder1.Append("\">");
									builder1.Append(match1.Groups["content"].Value);
									builder1.Append("</div>");
									flag1 = true;
								}
							}
						}
						if (!flag1)
						{
							if (text1.StartsWith("!!!!!"))
							{
								builder1.Append("<font class=\"");
								builder1.Append(this.Heading5CssStyle);
								builder1.Append("\">");
								builder1.Append(text1.Substring(5));
								builder1.Append("</font>");
								this.AppendLineBreaks(builder1, 1);
							}
							else if (text1.StartsWith("!!!!"))
							{
								builder1.Append("<font class=\"");
								builder1.Append(this.Heading4CssStyle);
								builder1.Append("\">");
								builder1.Append(text1.Substring(4));
								builder1.Append("</font>");
								this.AppendLineBreaks(builder1, 1);
							}
							else if (text1.StartsWith("!!!"))
							{
								builder1.Append("<font class=\"");
								builder1.Append(this.Heading3CssStyle);
								builder1.Append("\">");
								builder1.Append(text1.Substring(3));
								builder1.Append("</font>");
								this.AppendLineBreaks(builder1, 1);
							}
							else if (text1.StartsWith("!!"))
							{
								builder1.Append("<font class=\"");
								builder1.Append(this.Heading2CssStyle);
								builder1.Append("\">");
								builder1.Append(text1.Substring(2));
								builder1.Append("</font>");
								this.AppendLineBreaks(builder1, 1);
							}
							else if (text1.StartsWith("!"))
							{
								builder1.Append("<font class=\"");
								builder1.Append(this.Heading1CssStyle);
								builder1.Append("\">");
								builder1.Append(text1.Substring(1));
								builder1.Append("</font>");
								this.AppendLineBreaks(builder1, 1);
							}
							else if (text1 == "---")
							{
								builder1.Append("<hr color=#808080>");
								this.AppendLineBreaks(builder1, 1);
							}
							else if (text1 == "===")
							{
								builder1.Append("<hr color=#000000>");
								this.AppendLineBreaks(builder1, 1);
							}
							else if(text1.StartsWith("<[[HTML"))
							{
								// find the end...
								int index = text1.IndexOf("]]>");
								if(index == -1)
									throw new InvalidOperationException("Failed to find end of HTML block.");

								// render that...								
								trimmed = trimmed.Substring(7);
								trimmed = trimmed.Substring(0, index - 7);
								builder1.Append(trimmed);

								// add two line breaks...
								this.AppendLineBreaks(builder1, 1);
							}
							else if(text1.StartsWith("<[[TABLE"))
							{
								builder1.Append("<table>\r\n");

								StringBuilder tableContents = new StringBuilder(text1.Substring(8));
								// Extract the table contents out so we can parse separately
								while (!text1.EndsWith("]]>"))
								{
									num2+=1;
									// We have reached the end of the array
									if(num2 > textArray2.Length-1)
										break;

									text1 = textArray2[num2];
									// If we start with the table end element break
									if(text1.StartsWith("]]>"))
										break;

									// If we end with the end table we strip it otherwise append
									if(text1.EndsWith("]]>"))
										tableContents.Append(text1.Substring(0,text1.Length-3));
									else
										tableContents.Append(text1);
									tableContents.Append("\r\n");
								}

								// Now we can append the table rows
								AppendTableRows(builder1,tableContents.ToString());

								builder1.Append("</table>");
							}
							else
							{
								int num1 = 2;
								string text6 = text1;
								if (text1.EndsWith("$"))
								{
									text6 = text1.Substring(0, (text1.Length - 1));
									num1 = 1;
								}
								builder1.Append(text6);
								this.AppendLineBreaks(builder1, num1);
							}
						}
					}
					++num2;
				}
				builder1.Append("</font>");
				text7 = builder1.ToString();
			}

			// mbr - 28-07-2005 - trim...
			Regex regex = new Regex(@"\<br\s*/\>\</font\>$", RegexOptions.Singleline | RegexOptions.IgnoreCase);
			while(true)
			{
				Match match = regex.Match(text7);
				if(match.Success)
					text7 = text7.Substring(0, match.Index) + "</font>";
				else
					break;
			}

			return text7;
		}

		private void AppendTableRows(StringBuilder builder, string tableContents)
		{
			TextReader reader = new StringReader(tableContents);
			StringBuilder rowDefinition = new StringBuilder();
			string data = reader.ReadLine();
			bool rowsStarted = false;
			
			while(data != null)
			{
				if(data.StartsWith("$") && !data.StartsWith("]]>"))
				{
					if(rowsStarted == false)
						rowsStarted = true;
					else
						AppendTableColumns(builder,rowDefinition.ToString());

					rowDefinition = new StringBuilder();
					rowDefinition.Append(data.Substring(1));
				}
				else
					rowDefinition.Append(data);
				
				rowDefinition.Append("\r\n");
				data = reader.ReadLine();
			}

			if(rowsStarted && rowDefinition.Length > 0)
				AppendTableColumns(builder,rowDefinition.ToString());
		}

		private void AppendTableColumns(StringBuilder builder, string rowContents)
		{
			string[] columns = rowContents.Split('$');
			builder.Append("<tr>\r\n");
			foreach(string value in columns)
			{
				if(this.TdCssStyle != string.Empty)
				{
					builder.Append("\t<td class=\"");
					builder.Append(this.TdCssStyle);
					builder.Append("\">\r\n\t\t");
				}
				else
					builder.Append("\t<td>\r\n\t\t");

				builder.Append(GetHtml(value));
				builder.Append("\r\n\t</td>\r\n");
			}
			builder.Append("</tr>\r\n");
		}
 
		private string[] GetParagraphs(string text)
		{
			string[] textArray3;
			if ((text == null) || (text.Length == 0))
			{
				textArray3 = new string[0];
			}
			else
			{
				char[] chArray1 = new char[1];
				chArray1[0] = '\n';
				string[] textArray1 = text.Split(chArray1);
				string[] textArray2 = new string[textArray1.Length];
				int num1 = 0;
				while ((num1 < textArray1.Length))
				{
					textArray2[num1] = textArray1[num1].TrimEnd(new char[0]);
					++num1;
				}
				textArray3 = textArray2;
			}
			return textArray3;
		}
 
		public string ParseParagraph(string paragraph)
		{
			paragraph = this.SpecialCharsRegex.Replace(paragraph, new MatchEvaluator(this.SpecialCharsEvaluator));
			paragraph = this.LinkRegex.Replace(paragraph, new MatchEvaluator(this.LinksEvaluator));
			paragraph = this.ItalicsRegex.Replace(paragraph, "<i>${text}</i>");
			paragraph = this.BoldRegex.Replace(paragraph, "<b>${text}</b>");
			paragraph = this.CodeInTextRegex.Replace(paragraph, "<font class=codeintext>${text}</font>");
			//					textArray2[num1] = this.EmailRegex.Replace(textArray2[num1], new MatchEvaluator(this.EmailEvaluator));
			paragraph = this.ImageRegex.Replace(paragraph, new MatchEvaluator(this.ImageEvaluator));
			return paragraph;
		}


		private string GetStartTag(string tag)
		{
			return ("<" + tag + ">");
		}
 
		private string ImageEvaluator(Match match)
		{
			if(match == null)
				throw new ArgumentNullException("match");

			// get...
			string url = match.Groups["url"].Value;
			UrlEventArgs e = new UrlEventArgs(url);
			OnExpandImageUrl(e);
			return string.Format("<img src=\"{0}\" class=\"{1}\">", e.Url, ImageCssClass);
		}

			/// <summary>
			/// Raises the <c>ExpandImageUrl</c> event.
			/// </summary>
			protected virtual void OnExpandImageUrl(UrlEventArgs e)
			{
				// raise...
				if(ExpandImageUrl != null)
					ExpandImageUrl(this, e);
			}
 
		private string LinksEvaluator(Match match)
		{
			if(match == null)
				throw new ArgumentNullException("match");

			// get it...
			string url = ResolveUrl(match.Groups["url"].Value, this.ContentLinkPattern);
			string text = match.Groups["text"].Value;

			// mbr - 28-07-2005 - added resolve...
			FormatLinkEventArgs e = new FormatLinkEventArgs(url, text);
			this.OnFormatLink(e);

			// render...
			return string.Format("<a class=\"{0}\" href=\"{1}\">{2}</a>", this.LinkCssClass, e.Url, e.Text);
		}

		/// <summary>
		/// Raises the <c>FormatLink</c> event.
		/// </summary>
		protected virtual void OnFormatLink(FormatLinkEventArgs e)
		{
			// raise...
			if(FormatLink != null)
				FormatLink(this, e);
		}

		/// <summary>
		/// Resolves a URL.
		/// </summary>
		/// <param name="url"></param>
		/// <param name="pattern"></param>
		/// <returns></returns>
		protected abstract string ResolveUrl(string url, string pattern);

		/// <summary>
		/// Gets or sets the contentlinkpattern
		/// </summary>
		public string ContentLinkPattern
		{
			get
			{
				return _contentLinkPattern;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _contentLinkPattern)
				{
					// set the value...
					_contentLinkPattern = value;
				}
			}
		}

		public string LoadHtml(string filePath)
		{
			if (filePath == null)
			{
				throw new ArgumentNullException("filePath");
			}
			if (filePath.Length == 0)
			{
				throw new ArgumentOutOfRangeException("'filePath' is zero-length.");
			}
			string text1 = this.LoadText(filePath);
			return this.GetHtml(text1);
		}
 
		public string LoadText(string filePath)
		{
			string text1;
			if (filePath == null)
			{
				throw new ArgumentNullException("filePath");
			}
			if (filePath.Length == 0)
			{
				throw new ArgumentOutOfRangeException("'filePath' is zero-length.");
			}
			Encoding encoding1 = Encoding.ASCII;
			using (FileStream stream1 = new FileStream(filePath, FileMode.Open, FileAccess.Read))
			{
				int num1;
				StringBuilder builder1 = new StringBuilder();
				Debug.WriteLine(encoding1.GetBytes("-")[0]);
				byte[] buffer1 = new byte[10240];
				goto Label_00D4;
			Label_005A:
				num1 = stream1.Read(buffer1, 0, buffer1.Length);
				if (num1 == 0)
				{
					goto Label_00D6;
				}
				int num2 = 0;
				while ((num2 < num1))
				{
					switch (buffer1[num2])
					{
						case 0x91:
						case 0x92:
						{
							buffer1[num2] = 0x27;
							goto Label_00B7;
						}
						case 0x93:
						case 0x94:
						{
							buffer1[num2] = 0x22;
							goto Label_00B7;
						}
						case 150:
						{
							goto Label_00AF;
						}
					}
					goto Label_00B7;
				Label_00AF:
					buffer1[num2] = 0x2d;
				Label_00B7:
					++num2;
				}
				builder1.Append(encoding1.GetString(buffer1, 0, num1));
			Label_00D4:
				goto Label_005A;
			Label_00D6:
				text1 = builder1.ToString();
			}
			return text1;
		}
 
		private string SpecialCharsEvaluator(Match match)
		{
			string text2;
			if (match == null)
			{
				throw new ArgumentNullException("match");
			}
			string text1 = match.Value;
			if (text1 == null)
			{
				throw new InvalidOperationException("special is null.");
			}
			if (text1.Length != 1)
			{
				throw new InvalidOperationException(string.Format("Special should be one character: {0}.", text1));
			}
			char ch1 = text1[0];
			if (ch1 <= '&')
			{
				if (ch1 == '"')
				{
					goto Label_0099;
				}
				if (ch1 == '&')
				{
					goto Label_00B9;
				}
				goto Label_00C9;
			}
			switch (ch1)
			{
				case '<':
				{
					text2 = "&lt;";
					goto Label_00F6;
				}
				case '=':
				{
					goto Label_00C9;
				}
				case '>':
				{
					text2 = "&gt;";
					goto Label_00F6;
				}
			}
			switch (ch1)
			{
				case '\u2018':
				case '\u2019':
				{
					text2 = "'";
					goto Label_00F6;
				}
				case '\u201a':
				case '\u201b':
				{
					goto Label_00C9;
				}
				case '\u201c':
				case '\u201d':
				{
					goto Label_0099;
				}
			}
			if (ch1 == '\u2026')
			{
				goto Label_00C1;
			}
			goto Label_00C9;
			Label_0099:
				text2 = "&quot;";
			goto Label_00F6;
			Label_00B9:
				text2 = "&amp;";
			goto Label_00F6;
			Label_00C1:
				text2 = "...";
			goto Label_00F6;
			Label_00C9:
				throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", text1[0], text1[0].GetType()));
			Label_00F6:
				return text2;
		}
 
		private Regex CodeInTextRegex
		{
			get
			{
				return this._codeInTextRegex;
			}
		}
 
		private Regex BoldRegex
		{
			get
			{
				return this._boldRegex;
			}
		}
 
		private Regex BoxRegex
		{
			get
			{
				return this._boxRegex;
			}
		}
 
		private Regex BreakoutRegex
		{
			get
			{
				return this._breakoutRegex;
			}
		}
 
		private Regex BulletRegex
		{
			get
			{
				return this._bulletRegex;
			}
		}
 
		private Regex EmailRegex
		{
			get
			{
				return this._emailRegex;
			}
		}
 
		public string Heading1CssStyle
		{
			get
			{
				return this._heading1CssStyle;
			}
			set
			{
				if (value != this._heading1CssStyle)
				{
					this._heading1CssStyle = value;
				}
			}
		}
 
		public string Heading2CssStyle
		{
			get
			{
				return this._heading2CssStyle;
			}
			set
			{
				if (value != this._heading2CssStyle)
				{
					this._heading2CssStyle = value;
				}
			}
		}
 
		public string Heading3CssStyle
		{
			get
			{
				return this._heading3CssStyle;
			}
			set
			{
				if (value != this._heading3CssStyle)
				{
					this._heading3CssStyle = value;
				}
			}
		}
 
		public string Heading4CssStyle
		{
			get
			{
				return this._heading4CssStyle;
			}
			set
			{
				if (value != this._heading4CssStyle)
				{
					this._heading4CssStyle = value;
				}
			}
		}
 
		public string Heading5CssStyle
		{
			get
			{
				return this._heading5CssStyle;
			}
			set
			{
				if (value != this._heading5CssStyle)
				{
					this._heading5CssStyle = value;
				}
			}
		}

		public string TdCssStyle
		{
			get
			{
				return this._tdCssStyle;
			}
			set
			{
				if (value != this._tdCssStyle)
				{
					this._tdCssStyle = value;
				}
			}
		}
 
 
		private Regex ImageRegex
		{
			get
			{
				return this._imageRegex;
			}
		}
 
		private Regex ItalicsRegex
		{
			get
			{
				return this._italicsRegex;
			}
		}
 
		private Regex LinkRegex
		{
			get
			{
				return this._linkRegex;
			}
		}
 
		private Regex SpecialCharsRegex
		{
			get
			{
				return this._specialCharsRegex;
			}
		}
 
		/// <summary>
		/// Gets the embeddedregex.
		/// </summary>
		private Regex EmbeddedRegex
		{
			get
			{
				// returns the value...
				return _embeddedRegex;
			}
		}

		/// <summary>
		/// Gets or sets the linkcssclass
		/// </summary>
		public string LinkCssClass
		{
			get
			{
				return _linkCssClass;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _linkCssClass)
				{
					// set the value...
					_linkCssClass = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the imagecssclass
		/// </summary>
		public string ImageCssClass
		{
			get
			{
				return _imageCssClass;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _imageCssClass)
				{
					// set the value...
					_imageCssClass = value;
				}
			}
		}
	}
}
