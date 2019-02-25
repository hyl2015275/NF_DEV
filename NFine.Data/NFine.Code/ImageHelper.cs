using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFine.Code
{
   public class ImageHelper
    {
       public static void CaptureImage(string sFromFilePath, string saveFilePath, int width, int height, int spaceX, int spaceY)
       {
           //载入底图   
           Image fromImage = Image.FromFile(sFromFilePath);
           int x = 0;   //截取X坐标   
           int y = 0;   //截取Y坐标   
           //原图宽与生成图片宽   之差       
           //当小于0(即原图宽小于要生成的图)时，新图宽度为较小者   即原图宽度   X坐标则为0     
           //当大于0(即原图宽大于要生成的图)时，新图宽度为设置值   即width         X坐标则为   sX与spaceX之间较小者   
           //Y方向同理   
           int sX = fromImage.Width - width;
           int sY = fromImage.Height - height;
           if (sX > 0)
           {
               x = sX > spaceX ? spaceX : sX;
           }
           else
           {
               width = fromImage.Width;
           }
           if (sY > 0)
           {
               y = sY > spaceY ? spaceY : sY;
           }
           else
           {
               height = fromImage.Height;
           }
           if (!Directory.Exists(Path.GetDirectoryName(saveFilePath)))
               Directory.CreateDirectory(Path.GetDirectoryName(saveFilePath));
           //创建新图位图   
           Bitmap bitmap = new Bitmap(width, height);
           //创建作图区域   
           Graphics graphic = Graphics.FromImage(bitmap);
           //截取原图相应区域写入作图区   
           graphic.DrawImage(fromImage, 0, 0, new Rectangle(x, y, width, height), GraphicsUnit.Point);
           fromImage.Dispose();
           //从作图区生成新图   
           Image saveImage = Image.FromHbitmap(bitmap.GetHbitmap());
           //保存图象   
           saveImage.Save(saveFilePath, ImageFormat.Jpeg);
           //释放资源  
           saveImage.Dispose();
           bitmap.Dispose();
           graphic.Dispose();
       }
    }
}
