using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class OCRResult
{
    public string Language { get; set; }
    public float TextAngle { get; set; }
    public Region[] Regions { get; set; }
}

public class Region
{
    public string BoundingBox { get; set; }
    public Line[] Lines { get; set; }

}

public class Line
{
    public string BoundingBox { get; set; }
    public Word[] Words { get; set; }

}

public class Word
{
    public string BoundingBox { get; set; }
    public string Text { get; set; }
}



