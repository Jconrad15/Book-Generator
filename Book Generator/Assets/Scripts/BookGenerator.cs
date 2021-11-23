using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class BookGenerator : MonoBehaviour
{

    private static readonly int maxColorDifference = 5;

    [SerializeField]
    private GameObject pixel;

    [SerializeField]
    private Color pageColor1;
    [SerializeField]
    private Color pageColor2;
    [SerializeField]
    private List<Color> bookmarkColor;
    [SerializeField]
    private List<Color> tabColors;

    [SerializeField]
    private Material bookmarkShader;

    [SerializeField]
    private Material tabShader;

    [SerializeField]
    private Material paperShader;

    [SerializeField]
    private List<Material> glowingShader;

    [SerializeField]
    private List<Material> coverMaterials;

    [SerializeField]
    private int xSize = 8;
    [SerializeField]
    private int ySize = 12;

    int pageLength;
    bool shaderToggle = false;

    private List<GameObject> pixelsList = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        List<Color> colors = DetermineColorScheme();
        CreatePixels(colors);

        DesignBook(colors);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            ClearAll();

            List<Color> colors = DetermineColorScheme();

            CreatePixels(colors);

            DesignBook(colors);
        }
        
        if (Input.GetKeyDown(KeyCode.T))
        {
            shaderToggle = !shaderToggle;
            Debug.Log("shader toggle is now = " + shaderToggle);
        }
    }

    private void DesignBook(List<Color> colors)
    {
        CreatePages();

        CreateBackCover();

        CreateSpine(colors);


        CreateBorder(colors);

        CreateTabs();

        if (shaderToggle)
        {
            AddShaders(colors);
        }
        CreateBookMarks();


    }

    private void AddShaders(List<Color> colors)
    {
        int selectedIndex = Random.Range(0, colors.Count);

        Color selectColorForShader = colors[selectedIndex];
        Material shader = glowingShader[Random.Range(0, glowingShader.Count)];
        float seed = Random.Range(0.1f, 10f);

        int coverMaterialIndex = Random.Range(0, coverMaterials.Count);
        int spineIndex = Random.Range(0, coverMaterials.Count);

        foreach (GameObject p in pixelsList)
        {
            foreach (Color color in colors)
            {
                if (color == selectColorForShader &&
                    p.GetComponent<SpriteRenderer>().color == selectColorForShader)
                {
                    p.GetComponent<SpriteRenderer>().material = shader;
                    p.GetComponent<SpriteRenderer>().color = Color.white;
                    p.GetComponent<SpriteRenderer>().material.SetColor("_color", selectColorForShader);
                    p.GetComponent<SpriteRenderer>().material.SetFloat("_seed", seed);
                }
                else if (p.GetComponent<SpriteRenderer>().color == color)
                {
                    p.GetComponent<SpriteRenderer>().material = coverMaterials[coverMaterialIndex];
                    p.GetComponent<SpriteRenderer>().color = Color.white;
                    p.GetComponent<SpriteRenderer>().material.SetColor("_color", color);
                    p.GetComponent<SpriteRenderer>().material.SetFloat("_seed", seed);
                }
            }

            // Spine
            if (p.transform.position.x == 0)
            {
                Color tempColor = p.GetComponent<SpriteRenderer>().color;
                p.GetComponent<SpriteRenderer>().material = coverMaterials[spineIndex];
                p.GetComponent<SpriteRenderer>().color = Color.white;
                p.GetComponent<SpriteRenderer>().material.SetColor("_color", tempColor);
            }
            if (p.transform.position.x == 0 && p.transform.position.y == 0)
            {
                Color color = p.GetComponent<SpriteRenderer>().color;
                color.a = 0;
                p.GetComponent<SpriteRenderer>().color = color;
            }
            if (p.transform.position.x == 0 && p.transform.position.y == ySize - 1)
            {
                Color color = p.GetComponent<SpriteRenderer>().color;
                color.a = 0;
                p.GetComponent<SpriteRenderer>().color = color;
            }

            // Back Cover
            if (p.GetComponent<SpriteRenderer>().material != bookmarkShader)
            {
                if (p.transform.position.y == 0 && p.transform.position.x != 0)
                {
                    Color tempColor = p.GetComponent<SpriteRenderer>().color;
                    p.GetComponent<SpriteRenderer>().material = coverMaterials[coverMaterialIndex];
                    p.GetComponent<SpriteRenderer>().material.SetColor("_color", tempColor);
                }
                if (pageLength == xSize - 1)
                {
                    if (p.transform.position.x == xSize - 1 && p.transform.position.y == 1)
                    {
                        Color tempColor = p.GetComponent<SpriteRenderer>().color;
                        p.GetComponent<SpriteRenderer>().material = coverMaterials[coverMaterialIndex];
                        p.GetComponent<SpriteRenderer>().material.SetColor("_color", tempColor);
                    }
                    if (p.transform.position.x == xSize - 1 && p.transform.position.y == 2)
                    {
                        Color tempColor = p.GetComponent<SpriteRenderer>().color;
                        p.GetComponent<SpriteRenderer>().material = coverMaterials[coverMaterialIndex];
                        p.GetComponent<SpriteRenderer>().material.SetColor("_color", tempColor);
                    }
                }
            }

            // Paper
            if (p.transform.position.y == 1 && p.transform.position.x >= 1 && p.transform.position.x < pageLength)
            {
                Color tempColor = p.GetComponent<SpriteRenderer>().color;
                p.GetComponent<SpriteRenderer>().material = paperShader;
                p.GetComponent<SpriteRenderer>().material.SetColor("_color", tempColor);
            }
            if (p.transform.position.y == 2 && p.transform.position.x >= 1 && p.transform.position.x < pageLength)
            {
                Color tempColor = p.GetComponent<SpriteRenderer>().color;
                p.GetComponent<SpriteRenderer>().material = paperShader;
                p.GetComponent<SpriteRenderer>().material.SetColor("_color", tempColor);
            }
        }
    }

    private void CreateTabs()
    {
        bool isCreateTab = Random.value < 0.5f;
        if (isCreateTab)
        {
            Debug.Log("tabs");
            int numTabs = Random.Range(1, 3);
            for (int i = 0; i < numTabs; i++)
            {
                Color selectedColor = tabColors[Random.Range(0, tabColors.Count)];
                int x = xSize;
                int y = Random.Range(5, ySize - 2);
                GameObject pix = Instantiate(pixel, new Vector3(x, y, 0), Quaternion.identity);
                pixelsList.Add(pix);
                //pix.GetComponent<SpriteRenderer>().color = selectedColor;

                if (shaderToggle)
                {
                    pix.GetComponent<SpriteRenderer>().material = bookmarkShader;
                    pix.GetComponent<SpriteRenderer>().color = Color.white;
                    pix.GetComponent<SpriteRenderer>().material.SetColor("_color", selectedColor);
                }
                else
                {
                    pix.GetComponent<SpriteRenderer>().color = selectedColor;
                }
            }
        }
    }

    private void CreateBorder(List<Color> colors)
    {
        bool isCreateBorder = Random.value < 0.99f;
        Color selectedColor = colors[Random.Range(0, colors.Count)];

        if (isCreateBorder)
        {
            Debug.Log("border is created");
            foreach (GameObject p in pixelsList)
            {
                if (p.transform.position.y == 3 && p.transform.position.x >= 1)
                {
                    p.GetComponent<SpriteRenderer>().color = selectedColor;
                }
                if (p.transform.position.y == ySize - 1 && p.transform.position.x >= 1)
                {
                    p.GetComponent<SpriteRenderer>().color = selectedColor;
                }
                if (p.transform.position.x == xSize - 1 && p.transform.position.y >= 3)
                {
                    p.GetComponent<SpriteRenderer>().color = selectedColor;
                }
                if (p.transform.position.x == 1 && p.transform.position.y >= 3)
                {
                    p.GetComponent<SpriteRenderer>().color = selectedColor;
                }
            }
        }

    }

    private void CreateBookMarks()
    {
        bool isBottomMark = Random.value > 0.5f;
        Color currentColor = bookmarkColor[Random.Range(0, bookmarkColor.Count)];

        if (isBottomMark)
        {
            int numBookMarks = Random.Range(1, 3);

            for (int i = 0; i < numBookMarks; i++)
            {
                int xLocation = Random.Range(2, 6);

                foreach (GameObject p in pixelsList)
                {
                    if (p.transform.position.y == 0 && p.transform.position.x == xLocation)
                    {
                        if (shaderToggle)
                        {
                            p.GetComponent<SpriteRenderer>().material = bookmarkShader;
                            p.GetComponent<SpriteRenderer>().color = Color.white;
                            p.GetComponent<SpriteRenderer>().material.SetColor("_color", currentColor);
                        }
                        else
                        {
                            p.GetComponent<SpriteRenderer>().color = currentColor;
                        }
                    }
                    if (p.transform.position.y == 1 && p.transform.position.x == xLocation)
                    {
                        if (shaderToggle)
                        {
                            p.GetComponent<SpriteRenderer>().material = bookmarkShader;
                            p.GetComponent<SpriteRenderer>().color = Color.white;
                            p.GetComponent<SpriteRenderer>().material.SetColor("_color", currentColor);
                        }
                        else
                        {
                            p.GetComponent<SpriteRenderer>().color = currentColor;
                        }
                    }
                }

                GameObject pix = Instantiate(pixel, new Vector3(xLocation, -1, 0), Quaternion.identity);
                pixelsList.Add(pix);

                if (shaderToggle)
                {
                    pix.GetComponent<SpriteRenderer>().material = bookmarkShader;
                    pix.GetComponent<SpriteRenderer>().color = Color.white;
                    pix.GetComponent<SpriteRenderer>().material.SetColor("_color", currentColor);
                }
                else
                {
                    pix.GetComponent<SpriteRenderer>().color = currentColor;
                }
            }
        }
    }

    private void CreateBackCover()
    {
        foreach (GameObject p in pixelsList)
        {
            if (p.transform.position.y == 0)
            {
                Color color = p.GetComponent<SpriteRenderer>().color;
                color = DarkenColor(color);
                color = DarkenColor(color);
                color = DarkenColor(color);

                p.GetComponent<SpriteRenderer>().color = color;
            }
            if (pageLength == xSize - 1)
            {
                if(p.transform.position.x == xSize - 1 && p.transform.position.y == 1)
                {
                    Color color = p.GetComponent<SpriteRenderer>().color;
                    color = DarkenColor(color);
                    color = DarkenColor(color);
                    color = DarkenColor(color);

                    p.GetComponent<SpriteRenderer>().color = color;
                }
                if (p.transform.position.x == xSize - 1 && p.transform.position.y == 2)
                {
                    Color color = p.GetComponent<SpriteRenderer>().color;
                    color = DarkenColor(color);
                    color = DarkenColor(color);
                    color = DarkenColor(color);

                    p.GetComponent<SpriteRenderer>().color = color;
                }
            }
        }
    }

    private void CreateSpine(List<Color> colors)
    {
        // Same color spine
        bool isSameColor = Random.value > 0.7f;
        Color sameColor = colors[Random.Range(0, colors.Count)];

        foreach (GameObject p in pixelsList)
        {
            if (p.transform.position.x == 0 && p.transform.position.y == 0)
            {
                Color color = p.GetComponent<SpriteRenderer>().color;
                color.a = 0;
                p.GetComponent<SpriteRenderer>().color = color;

            }
            if (p.transform.position.x == 0 && p.transform.position.y == ySize - 1)
            {
                Color color = p.GetComponent<SpriteRenderer>().color;
                color.a = 0;
                p.GetComponent<SpriteRenderer>().color = color;

            }
            if (p.transform.position.x == 0)
            {
                Color color = p.GetComponent<SpriteRenderer>().color;
                color = DarkenColor(color);
                p.GetComponent<SpriteRenderer>().color = color;
            }
            if (isSameColor)
            {
                if (p.transform.position.x == 0 && p.transform.position.y > 0 && p.transform.position.y < ySize - 1)
                {
                    p.GetComponent<SpriteRenderer>().color = DarkenColor(sameColor);
                }
            }
        }
    }

    private void CreatePages()
    {
        pageLength = Random.Range(xSize - 1, xSize + 1);
        foreach (GameObject p in pixelsList)
        {
            if(p.transform.position.y == 1 && p.transform.position.x >= 1 && p.transform.position.x < pageLength)
            { p.GetComponent<SpriteRenderer>().color = pageColor1; }
            if (p.transform.position.y == 2 && p.transform.position.x >= 1 && p.transform.position.x < pageLength)
            { p.GetComponent<SpriteRenderer>().color = pageColor2; }
        }
    }

    private void ClearAll()
    {
        for (int i = 0; i < pixelsList.Count; i++)
        {
            Destroy(pixelsList[i]);
        }
        pixelsList.Clear();
    }

    private List<Color> DetermineColorScheme()
    {
        List<Color> colors = new List<Color>();
        int colorCount = Random.Range(2, 4);

        for (int i = 0; i < colorCount; i++)
        {
            colors.Add(RandomColor());
        }

        return colors;
    }

    private void CreatePixels(List<Color> colors)
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                Vector3 position = new Vector3(x, y, 0);

                GameObject pix = Instantiate(pixel, position, Quaternion.identity);
                pixelsList.Add(pix);

                SpriteRenderer sr = pix.GetComponent<SpriteRenderer>();
                sr.color = colors[Random.Range(0, colors.Count)];

            }
        }
    }

    private Color DarkenColor(Color color)
    {
        float colorChange = 0.8f;

        Color darkerColor = color;
        darkerColor.r *= colorChange;
        darkerColor.g *= colorChange;
        darkerColor.b *= colorChange;

        return darkerColor;
    }

    private Color RandomColor()
    {
        Color color = new Color();
        color.r = Random.value;
        color.g = Random.value;
        color.b = Random.value;
        color.a = 1;

        return color;
    }

    [DllImport("__Internal")]
    private static extern void DownloadFile(byte[] array, int byteLength, string fileName);

    [DllImport("__Internal")]
    private static extern void Hello();

    private IEnumerator DownloadScreenshot()
    {
        yield return new WaitForEndOfFrame();
        Texture2D rawTexture = ScreenCapture.CaptureScreenshotAsTexture(1);

        float width = 0.35f;
        float height = 0.8f;

        int xPos = (int)(Camera.main.pixelWidth * ((1 - width) / 2f));
        int yPos = (int)(Camera.main.pixelHeight * ((1 - height) / 2f));
        int xSize = (int)(Camera.main.pixelWidth * width);
        int ySize = (int)(Camera.main.pixelHeight * height);
        Color[] c = rawTexture.GetPixels(xPos, yPos, xSize, ySize);

        Texture2D croppedTexture = new Texture2D(xSize, ySize);
        croppedTexture.SetPixels(c);
        croppedTexture.Apply();

        croppedTexture = RemoveBackgroundColor(croppedTexture);
/*
        GameObject test = new GameObject("texture");
        SpriteRenderer test_sr = test.AddComponent<SpriteRenderer>();
        test_sr.sprite = Sprite.Create(
            croppedTexture,
            new Rect(0.0f, 0.0f, croppedTexture.width, croppedTexture.height),
            new Vector2(0.5f, 0.5f));
*/
        byte[] textureBytes = croppedTexture.EncodeToPNG();
        DownloadFile(textureBytes, textureBytes.Length, "Book.png");
        Destroy(rawTexture);
    }

    private Texture2D RemoveBackgroundColor(Texture2D texture)
    {
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                Color currentColor = texture.GetPixel(x, y);
                if (CompareColors(currentColor, new Color32(31, 32, 64, 255)))
                {
                    texture.SetPixel(x, y, new Color32(0, 0, 0, 0));
                }
            }

        }

        texture.Apply();
        return texture;
    }

    public void SaveFileButton()
    {
        StartCoroutine(DownloadScreenshot());
    }

    private static bool CompareColors(Color32 color1, Color32 color2)
    {
        int rdiff = Mathf.Abs(color1.r - color2.r);
        int gdiff = Mathf.Abs(color1.g - color2.g);
        int bdiff = Mathf.Abs(color1.b - color2.b);
        int adiff = Mathf.Abs(color1.a - color2.a);

        return rdiff + gdiff + bdiff + adiff < maxColorDifference;
    }
}
