using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Texture2DSizeSetter : AssetPostprocessor
{
    [Obsolete("Obsolete")]
    void OnPreprocessTexture()
    {
        //get a reference to the built-in TextureImporter...
        TextureImporter importer = (TextureImporter)assetImporter;

        //create a new empty TextureImporterSettings struct...
        TextureImporterSettings textureImporterSettings = new TextureImporterSettings();

        //read the current import settings from the Texture Importer
        //into our new importer settings struct (basically filling the empty struct with values)
        importer.ReadTextureSettings(textureImporterSettings);

        //change the maxTextureSize setting in our settings struct
        textureImporterSettings.maxTextureSize = 8192;
        textureImporterSettings.sRGBTexture = true;

        //pass the settings struct, with the changed maxTextureSize value, back into the importer
        //(e.g. apply the changed settings to the importer)
        importer.SetTextureSettings(textureImporterSettings);
    }
}
