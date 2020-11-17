using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class PaletteSwapper : EditorWindow
{
    private Texture2D original;
    private Texture2D reference;
    private Texture2D swapper;

    private string absolutePath;
    private string relPath;

    private float addR, addG, addB;
    private bool useSwapper = true;
    private float tolerance = 0.001f;

    private void OnGUI()
    {
        name = EditorGUILayout.TextField("File name: ", name);
        relPath = EditorGUILayout.TextField("Save location: Assets/", relPath);
        absolutePath = Application.dataPath +"/"+ relPath;
        GUILayout.Space(20);

        original = TextureField("Original texture", original);
        reference = TextureField("Reference texture", reference);

        GUILayout.Space(10);
        tolerance = EditorGUILayout.FloatField("Tolerance: ", tolerance);
        useSwapper = EditorGUILayout.Toggle("Use swapper: ", useSwapper);

        if (useSwapper)
        {
            swapper = TextureField("Swap texture", swapper);
        }
        else
        {
            addR = EditorGUILayout.Slider("R", addR, -1f, 1f);
            addG = EditorGUILayout.Slider("G", addG, -1f, 1f);
            addB = EditorGUILayout.Slider("B", addB, -1f, 1f);

            if (GUILayout.Button("Generate Swapper"))
                GenerateSwapper();
        }

        if (original == null ||
            reference == null ||
            (swapper == null && useSwapper))
        {
            return;
        }

        GUILayout.Space(20);
        if (GUILayout.Button("Generate", GUILayout.Height(50)))
            GenerateTexture();
        if (GUILayout.Button("Reset", GUILayout.Height(50)))
            SaveTextureAsPNG(original, absolutePath);
    }

    private Texture2D TextureField(string name, Texture2D texture)
    {
        texture = (Texture2D)EditorGUILayout.ObjectField(name + ": ", texture, typeof(Texture2D), false);

        if (texture == null)
        {
            EditorGUILayout.HelpBox(name + " cannot be null.", MessageType.Error);
            return texture;
        }

        bool isPoint = (texture.filterMode == FilterMode.Point);
        bool isReadable = (texture.isReadable);

        if (!isPoint) EditorGUILayout.HelpBox("Point FilterMode is recommended.", MessageType.Warning);
        if (!isReadable) EditorGUILayout.HelpBox("Texture has to be readable.", MessageType.Error);

        if (!isPoint || !isReadable)
        {
            if (GUILayout.Button("Fix texture"))
            {
                SetTextureImporterFormat(texture, FilterMode.Point, true);
            }
        }

        return texture;
    }

    private void GenerateTexture()
    {
        //TODO: ERROR HANDLING
        //if null
        //if different size ref & swap
        //if not 1*y size ref & swap
        //ref, swap, orig need same settings

        var tex = new Texture2D(original.width, original.height, TextureFormat.ARGB32, false);

        for (int x = 0; x < original.width; x++)
        {
            for (int y = 0; y < original.height; y++)
            {
                SwapPixel(tex, x, y);
            }
        }

        tex.filterMode = FilterMode.Point;
        tex.Apply();

        SaveTextureAsPNG(tex, absolutePath);
    }

    private void GenerateSwapper()
    {
        var tex = new Texture2D(reference.width, reference.height, TextureFormat.ARGB32, false);

        for (int x = 0; x < reference.width; x++)
        {
            Color refColor = reference.GetPixel(x, 0);
            Color newColor = new Color(refColor.r + addR, refColor.g + addG, refColor.b + addB);
            tex.SetPixel(x, 0, newColor);
        }

        tex.filterMode = FilterMode.Point;
        tex.Apply();

        SaveTextureAsPNG(tex, absolutePath+"swapper");
    }

    private void SwapPixel(Texture2D tex, int x, int y)
    {
        Color color = original.GetPixel(x, y);
        Color swap  = SwapColor(color);
        tex.SetPixel(x, y, swap);
    }

    private Color SwapColor(Color color)
    {
        if (color.a == 0) return Color.clear;
        for (int x = 0; x < reference.width; x++)
        {
            Color cmp = reference.GetPixel(x, 0);
            if ( AreApproximatelyEqual(cmp, color, tolerance) )
                return useSwapper? swapper.GetPixel(x, 0) 
                    : new Color(cmp.r + addR, cmp.g + addG, cmp.b + addB);
        }
        return color;
    }

    private bool AreApproximatelyEqual(Color color1, Color color2, float tolerance)
    {
        return Vector4.SqrMagnitude((Vector4)color1 - (Vector4)color2) < tolerance;
    }

    private static void SetTextureImporterFormat(Texture2D texture, FilterMode filter, bool isReadable)
    {
        if (null == texture) return;

        string assetPath = AssetDatabase.GetAssetPath(texture);
        var tImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        if (tImporter != null)
        {
            tImporter.textureType = TextureImporterType.Default;
            tImporter.filterMode = filter;
            tImporter.isReadable = isReadable;

            AssetDatabase.ImportAsset(assetPath);
            AssetDatabase.Refresh();
        }
    }

    private void SaveTextureAsPNG(Texture2D tex, string fullPath)
    {
        byte[] bytes = tex.EncodeToPNG();
        File.WriteAllBytes(fullPath + name + ".png", bytes);
        AssetDatabase.Refresh();
    }

    [MenuItem("Tools/PaletteSwapper")]
    private static void Init()
    {
        var window = (PaletteSwapper)GetWindow(typeof(PaletteSwapper));
        window.Show();
    }
}
