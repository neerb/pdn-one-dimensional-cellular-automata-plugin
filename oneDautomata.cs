// Name:
// Submenu:
// Author:
// Title:
// Version:
// Desc:
// Keywords:
// URL:
// Help:

// For help writing a Bitmap plugin: https://boltbait.com/pdn/CodeLab/help/tutorial/bitmap/

#region UICode
IntSliderControl rulesetVal = 218; // [0,255] Ruleset Value
ColorWheelControl color1 = ColorBgra.FromBgra(34, 34, 178, 255); // [Firebrick?] Primary Color
ColorWheelControl color2 = ColorBgra.FromBgra(220, 248, 255, 255); // [Cornsilk?] Secondary Color
IntSliderControl startPos = 50; // [0,99] #1 Origin Position (Top)
CheckboxControl num2Enable = false; // Enable #2 Origin Position
IntSliderControl startPos2 = 25; // [0,99] {num2Enable} #2 Origin Position (Top)
CheckboxControl num3Enable = false; // Enable #3 Origin Position
IntSliderControl startPos3 = 75; // [0,99] {num3Enable} #3 Origin Position (Top)
#endregion

int[] NewArray(int[] cells, int size, int[] ruleSet)
{
    int[] newCells = new int[size];

    for(int i = 0; i < newCells.Length; i++)
    {
        int sum = 0;
        int degree = 2;
        for(int x = i - 1; x <= i + 1; x++)
        {
            if (x >= 0 && x < size)
            {
                sum += (int)Math.Pow(2, degree) * cells[x];
            }

            degree--;
        }

        newCells[i] = ruleSet[sum];
    }

    return newCells;
}

string Reverse(string s)
{
    string temp = string.Empty;
    
    for(int i = s.Length - 1; i >= 0; i--)
    {
        temp += s[i];
    }
    
    return temp;
}

string ToBinary(int n)
{
    string str = string.Empty;
    while(n > 0)
    {
        str = (n % 2) + str;
        n = n / 2;
    }
    while(str.Length < 8)
    {
        str = "0" + str;
    }
    return str;
}

int[,] generateAutomataImageArray(int width, int height) {
    // Generate array One-Dimensional Cellular Automata
    int[] rulesetArray = new int[8]; // 8 bit ruleset array value

    string binaryStr = ToBinary(rulesetVal);

    
    //Debug.WriteLine("");
       
    while(binaryStr.Length < 8) {
        binaryStr = "0" + binaryStr;
    }

    Debug.WriteLine("binarystr: " + binaryStr);


    int j = 0;
    for(int i = rulesetArray.Length - 1; i >= 0; i--) {
        rulesetArray[i] = (int)Char.GetNumericValue(binaryStr[j++]);
    }


    //Debug.WriteLine("binarystr:" + binaryStr);

    /*
    string bruh = "";
    for(int i = 0; i < rulesetArray.Length; i++)
        bruh += rulesetArray[i].ToString();


    Debug.WriteLine("bruh: " + bruh);
    */


    /*
    int width =  (outputBounds.Right - outputBounds.Left);
    int height = (outputBounds.Bottom - outputBounds.Top);
    */

    //Debug.WriteLine("width: " + width);
    //Debug.WriteLine("height: " + height);


    int[,] automata = new int[width, height];

    //Debug.WriteLine("automatawidth: " + automata.GetLength(0));
    //Debug.WriteLine("automataheight: " + automata.GetLength(1));

    int[] cells = new int[width];

    double pos = startPos / 100.0;
    cells[Convert.ToInt32(width * pos)] = 1;

    if(num2Enable) {
        double pos2 = startPos2 / 100.0;
        cells[Convert.ToInt32(width * pos2)] = 1;
    }

    if(num3Enable) {
        double pos3 = startPos3 / 100.0;
        cells[Convert.ToInt32(width * pos3)] = 1;
    }

    /*
     for(int i = 0; i < width; i++)
        Debug.Write(cells[i] + "  ");

    Debug.WriteLine("");
    */

    int[] pastArray;

    for(int y = 0; y < height; y++) {
        pastArray = cells;

        for(int x = 0; x < width; x++) {
            automata[x, y] = cells[x];
            //Debug.Write(automata[x,y] + " ");
        }

        cells = NewArray(pastArray, width, rulesetArray);
        //Debug.WriteLine("");
    }

    /*
    for (int y = 0; y < height; ++y)
    {
        //if (IsCancelRequested) return;

        for (int x = 0; x < width; ++x)
        {
            // Get your source pixel
            //ColorBgra32 sourcePixel = sourceRegion[x,y];
            
            //if(automata[x,y] == 1)
                Debug.Write(automata[x,y] + " ");
               //sourcePixel = SrgbColors.Black;

            // Save your pixel to the output canvas
           // outputRegion[x,y] = sourcePixel;
        }

        Debug.WriteLine("");
    }
    */

    return automata;
}

protected override void OnRender(IBitmapEffectOutput output)
{
    using IEffectInputBitmap<ColorBgra32> sourceBitmap = Environment.GetSourceBitmapBgra32();
    using IBitmapLock<ColorBgra32> sourceLock = sourceBitmap.Lock(new RectInt32(0, 0, sourceBitmap.Size));
    RegionPtr<ColorBgra32> sourceRegion = sourceLock.AsRegionPtr();

    RectInt32 outputBounds = output.Bounds;
    using IBitmapLock<ColorBgra32> outputLock = output.LockBgra32();
    RegionPtr<ColorBgra32> outputSubRegion = outputLock.AsRegionPtr();
    var outputRegion = outputSubRegion.OffsetView(-outputBounds.Location);
    //uint seed = RandomNumber.InitializeSeed(RandomNumberRenderSeed, outputBounds.Location);

    // Generate array One-Dimensional Cellular Automata

    var selection = Environment.Selection.RenderBounds;
    int width =  (selection.Right - selection.Left);
    int height = (selection.Bottom - selection.Top);

    int[,] automata = generateAutomataImageArray(width, height);

    
    //Debug.WriteLine("Top: " + selection.Top);
    //Debug.WriteLine("Bottom: " + selection.Bottom);
    //Debug.WriteLine("Left: " + selection.Left);
    //Debug.WriteLine("Right: " + selection.Right);
    
    
    /*
     // Loop through the output canvas tile
    for (int y = selection.Top; y < selection.Bottom; ++y)
    {
        if (IsCancelRequested) return;

        for (int x = selection.Left; x < selection.Right; ++x)
        {            
            try {
            // Get your source pixel
            ColorBgra32 sourcePixel = sourceRegion[x,y];

            //if(x < width && y < width && x >= selection.Left && y >= selection.Bottom) {
                if(automata[x,y] == 1) {
                   sourcePixel = SrgbColors.Black;
                }
                else {
                   sourcePixel = SrgbColors.Red;
                }
            //}
            // Save your pixel to the output canvas
            outputRegion[x,y] = sourcePixel;

            }
            catch(Exception ex) {
                Debug.Write(ex.Message);
            }
        }
    }
    */
    
    // Loop through the output canvas tile
    for (int y = outputBounds.Top; y < outputBounds.Bottom; ++y)
    {
        if (IsCancelRequested) return;

        for (int x = outputBounds.Left; x < outputBounds.Right; ++x)
        {
            try {
                // Get your source pixel
                ColorBgra32 sourcePixel = sourceRegion[x,y];

                if(automata[x,y] == 1) {
                   sourcePixel = color1;
                }
                else {
                   sourcePixel = color2;
                }

                // Save your pixel to the output canvas
                outputRegion[x,y] = sourcePixel;
            }
            catch(Exception ex) {
                Debug.Write(ex.Message);
            }
        }
    }
}
