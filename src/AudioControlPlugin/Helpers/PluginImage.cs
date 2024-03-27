namespace Loupedeck.AudioControlPlugin
{
    using System;
    using System.Drawing;
    using System.IO;

    using WindowsInterop.Win32;

    internal static class PluginImage
    {
        public static Bitmap ToBitmap(BitmapImage image)
        {
            using (MemoryStream stream = new MemoryStream(image.ToArray()))
            {
                return new Bitmap(stream);
            }
        }

        public static Bitmap ReadBitmap(string bitmapName)
        {
            using (BitmapImage image = PluginResources.ReadImage(bitmapName))
            {
                return ToBitmap(image);
            }
        }

        public static BitmapImage ToBitmapImage(Bitmap image)
        {
            //if (PluginSettings.SaveImageOnDisk)
            //{
            //    if (!string.IsNullOrEmpty(PluginData.Directory))
            //    {
            //        string path = Path.Combine(PluginData.Directory, DateTime.Now.GetTotalMilliseconds() + ".png");
            //        try
            //        {
            //            image.Save(path, ImageFormat.Png);
            //        }
            //        catch
            //        {
            //        }
            //    }
            //}
            ImageConverter converter = new ImageConverter();
            return BitmapImage.FromArray((byte[])converter.ConvertTo(image, typeof(byte[])));
        }

        public static void GetImageSize(PluginImageSize imageSize, out int imageWidth, out int imageHeight)
        {
            if (imageSize == PluginImageSize.Width60)
            {
                imageWidth = 50;
                imageHeight = 50;
            }
            else if (imageSize == PluginImageSize.Width90)
            {
                imageWidth = 80;
                imageHeight = 80;
            }
            else
            {
                imageWidth = 80;
                imageHeight = 80;
            }
        }

        public static PluginImageSize GetImageSize(int imageWidth, int imageHeight)
        {
            if (imageWidth == 50 && imageHeight == 50)
            {
                return PluginImageSize.Width60;
            }
            else if (imageWidth == 80 && imageHeight == 80)
            {
                return PluginImageSize.Width90;
            }
            else
            {
                return PluginImageSize.None;
            }
        }

        public static BitmapImage DrawBlackImage(PluginImageSize imageSize)
        {
            PluginImage.GetImageSize(imageSize, out int imageWidth, out int imageHeight);
            using (Bitmap image = new Bitmap(imageWidth, imageHeight))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                graphics.Clear(Color.Black);
                return PluginImage.ToBitmapImage(image);
            }
        }

        public static BitmapImage DrawFolderTextImage(bool pluginName, string text, PluginImageSize imageSize)
        {
            PluginImage.GetImageSize(imageSize, out int imageWidth, out int imageHeight);
            using (Bitmap image = new Bitmap(imageWidth, imageHeight))
            using (Graphics graphics = Graphics.FromImage(image))
            using (Font calibri10Font = new Font("Calibri", 10, FontStyle.Regular))
            using (Font calibri12Font = new Font("Calibri", 12, FontStyle.Regular))
            using (Brush orangeBrush = new SolidBrush(Color.Orange))
            using (Brush whiteABrush = new SolidBrush(Color.FromArgb(192, Color.White.BlueLightFilter())))
            using (Brush whiteBrush = new SolidBrush(Color.White.BlueLightFilter()))
            using (StringFormat format = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                graphics.Clear(Color.Black);

                int pluginNameY = 12;
                int textY = 0;

                if (PluginSettings.FolderDecorationEnabled)
                {
                    graphics.FillRectangle(orangeBrush, 8, 6, 64, 2);
                    graphics.FillRectangle(orangeBrush, 8, 72, 64, 2);
                }

                if (pluginName)
                {
                    textY += 15;
                    if (PluginSettings.FolderDecorationEnabled)
                    {
                        pluginNameY += 2;
                        textY -= 2;
                    }
                    graphics.DrawString("Audio", calibri10Font, whiteABrush, new RectangleF(2, pluginNameY, 76, 12), format);
                    graphics.DrawString("Control", calibri10Font, whiteABrush, new RectangleF(2, pluginNameY + 12, 76, 12), format);
                }

                graphics.DrawString(text, calibri12Font, whiteBrush, new RectangleF(0, textY, image.Width, image.Height), format);

                return PluginImage.ToBitmapImage(image);
            }
        }

        public static BitmapImage DrawFolderIconImage(bool pluginName, string iconPath, PluginImageSize imageSize)
        {
            PluginImage.GetImageSize(imageSize, out int imageWidth, out int imageHeight);
            using (Bitmap icon = PluginImage.ReadBitmap(iconPath))
            using (Bitmap image = new Bitmap(imageWidth, imageHeight))
            using (Graphics graphics = Graphics.FromImage(image))
            using (Font calibri10Font = new Font("Calibri", 10, FontStyle.Regular))
            using (Brush orangeBrush = new SolidBrush(Color.Orange))
            using (Brush whiteABrush = new SolidBrush(Color.FromArgb(192, Color.White.BlueLightFilter())))
            using (StringFormat format = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                graphics.Clear(Color.Black);

                int pluginNameY = 12;
                int iconY = 0;

                if (PluginSettings.FolderDecorationEnabled)
                {
                    graphics.FillRectangle(orangeBrush, 8, 6, 64, 2);
                    graphics.FillRectangle(orangeBrush, 8, 72, 64, 2);
                }

                if (pluginName)
                {
                    iconY += 15;
                    if (PluginSettings.FolderDecorationEnabled)
                    {
                        pluginNameY += 2;
                        iconY -= 2;
                    }
                    graphics.DrawString("Audio", calibri10Font, whiteABrush, new RectangleF(2, pluginNameY, 76, 12), format);
                    graphics.DrawString("Control", calibri10Font, whiteABrush, new RectangleF(2, pluginNameY + 12, 76, 12), format);
                }

                icon.Recolor(Color.White.BlueLightFilter());
                graphics.DrawImage(icon, (image.Width - icon.Width) / 2, (image.Height - icon.Height) / 2 + iconY, icon.Width, icon.Height);

                return PluginImage.ToBitmapImage(image);
            }
        }

        public static BitmapImage DrawTextImage(string buttonName, bool bold, PluginImageSize imageSize)
        {
            PluginImage.GetImageSize(imageSize, out int imageWidth, out int imageHeight);
            using (Bitmap image = new Bitmap(imageWidth, imageHeight))
            using (Graphics graphics = Graphics.FromImage(image))
            using (Font calibri10Font = new Font("Calibri", 12, bold ? FontStyle.Bold : FontStyle.Regular))
            using (Brush whiteBrush = new SolidBrush(Color.White.BlueLightFilter()))
            using (StringFormat format = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                graphics.Clear(Color.Black);
                graphics.DrawString(buttonName, calibri10Font, whiteBrush, new RectangleF(0, 0, image.Width, image.Height), format);
                return PluginImage.ToBitmapImage(image);
            }
        }

        public static BitmapImage DrawIconImage(string iconPath, PluginImageSize imageSize)
        {
            PluginImage.GetImageSize(imageSize, out int imageWidth, out int imageHeight);
            using (Bitmap image = new Bitmap(imageWidth, imageHeight))
            using (Graphics graphics = Graphics.FromImage(image))
            using (Bitmap icon = PluginImage.ReadBitmap(iconPath))
            {
                graphics.Clear(Color.Black);
                icon.Recolor(Color.White.BlueLightFilter());
                graphics.DrawImage(icon, (image.Width - icon.Width) / 2, (image.Height - icon.Height) / 2, icon.Width, icon.Height);
                return PluginImage.ToBitmapImage(image);
            }
        }

        public static Icon GetIcon(string iconPath)
        {
            Icon icon = null;
            if (!string.IsNullOrEmpty(iconPath))
            {
                string[] values = iconPath.Split(',');
                string path = values[0];
                if (path.EndsWithNoCase(".dll"))
                {
                    try
                    {
                        int index = values.Length > 1 && int.TryParse(values[1], out index) ? index : 0;
                        Shell32.ExtractIconEx(path, index, out IntPtr large, out IntPtr small, 1);
                        User32.DestroyIcon(small);
                        icon = Icon.FromHandle(large);
                    }
                    catch
                    {
                        icon = null;
                    }
                }
                else if (path.EndsWithNoCase(".exe"))
                {
                    try
                    {
                        icon = Icon.ExtractAssociatedIcon(iconPath);
                    }
                    catch
                    {
                        icon = null;
                    }
                }
            }
            return icon;
        }
    }
}
