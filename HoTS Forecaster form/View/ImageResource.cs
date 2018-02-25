using HoTS_Forecaster_form.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HoTS_Forecaster_form.View
{

    //компонент для работы с картинками
    public class ImageResource
    {
        ImageList imageList;

        HashSet<string> renderType = new HashSet<string>();

        string resource;

        public HashSet<string> RenderType { get => renderType;}
        public string Resource {
            get => resource;
            set => resource = value;
        }

        public ImageResource(ImageList imageList)
        {
            this.imageList = imageList;
        }

        public void ImageSizeChange(int value)
        {
            imageList.ImageSize = new Size(value, value);
            Load(resource);
        }

        public void Load(string path)
        {
            string[] files = Directory.GetFiles(path)
                .Where(s => s.EndsWith(".png")).ToArray();
            foreach(var res in files)
            {
                var key = res.Split('/').Last();
                var renderMode = key.Split('.')[0].Split('_');
                if (renderMode.Length>1 && renderMode[1] != "h" && 
                    renderType.Contains(renderMode[1]) == false && renderMode[1] != "Damage")
                    renderType.Add(renderMode[1]);
                imageList.Images.Add(key, new Bitmap(res));
            }
        }

        public Image Bitmap(string name)
        {
            return imageList.Images[name];
        }

        public int Index(string name)
        {
            return imageList.Images.Keys.IndexOf(name);
        }
    }
}
