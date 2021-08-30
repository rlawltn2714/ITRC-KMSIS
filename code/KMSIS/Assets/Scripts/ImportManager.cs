#pragma warning disable 649
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SimpleFileBrowser;
using TriLibCore.General;

namespace TriLibCore.Samples
{
	public class ImportManager : MonoBehaviour
	{
		// This class manages importing.

		// GameObject component
		private GameObject importedBuildings;
		private GameObject standardBoundBuilding;

		// Bound variable
		private float maxBound;

		void Start()
        {
			importedBuildings = GameObject.Find("ImportedBuildings");
			standardBoundBuilding = GameObject.Find("Building.4028");
			maxBound = standardBoundBuilding.GetComponent<MeshFilter>().mesh.bounds.size.y;
        }

		// Set the size of model appropriately
		public void SetSizeOfModel(GameObject building)
        {
			if (!CheckSizeOfModel(building))
            {
				Debug.Log(maxBound);
				Bounds bounds = building.GetComponent<MeshFilter>().mesh.bounds;
				float ratio = maxBound / bounds.size.x;
				if (maxBound / bounds.size.y > ratio) ratio = maxBound / bounds.size.y;
				if (maxBound / bounds.size.z > ratio) ratio = maxBound / bounds.size.z;

				Debug.Log(ratio);
				building.transform.localScale = new Vector3(ratio, ratio, ratio);
			}
		}

		// Check the size of model
		public bool CheckSizeOfModel(GameObject building)
        {
			Bounds bounds = building.GetComponent<MeshFilter>().mesh.bounds;
			if (bounds.size.x <= maxBound && bounds.size.y <= maxBound && bounds.size.z <= maxBound) return true;
			else return false;
		}

		// Import model using path
		public void ImportFromPath(string path)
        {
			var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions();
			assetLoaderOptions.ImportMeshes = true;
			assetLoaderOptions.GenerateColliders = true;
			AssetLoader.LoadModelFromFile(path, OnLoad, null, null, null, importedBuildings, assetLoaderOptions);
		}

		// Import model using file browser
		public void ImportFromBrowser()
		{
			// Set filters (optional)
			// It is sufficient to set the filters just once (instead of each time before showing the file browser dialog), 
			// if all the dialogs will be using the same filters
			FileBrowser.SetFilters(true, new FileBrowser.Filter("3D Models", ".fbx", ".obj", ".stl"));

			// Set default filter that is selected when the dialog is shown (optional)
			// Returns true if the default filter is set successfully
			// In this case, set 3D Models filter as the default filter
			FileBrowser.SetDefaultFilter(".fbx");

			// Set excluded file extensions (optional) (by default, .lnk and .tmp extensions are excluded)
			// Note that when you use this function, .lnk and .tmp extensions will no longer be
			// excluded unless you explicitly add them as parameters to the function
			FileBrowser.SetExcludedExtensions(".gltf2", ".ply", ".3mf", ".zip");

			// Add a new quick link to the browser (optional) (returns true if quick link is added successfully)
			// It is sufficient to add a quick link just once
			// Name: Users
			// Path: C:\Users
			// Icon: default (folder icon)
			FileBrowser.AddQuickLink("Users", "C:\\Users", null);

			// Coroutine example
			StartCoroutine(ShowLoadDialogCoroutine());
		}

		private void OnLoad(AssetLoaderContext assetLoaderContext)
		{
			SetSizeOfModel(importedBuildings.transform.GetChild(importedBuildings.transform.childCount - 1).gameObject);
		}

		IEnumerator ShowLoadDialogCoroutine()
		{
			yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, false, null, null, "Load Files", "Load");

			// Dialog is closed

			if (FileBrowser.Success)
			{
				// Import files
				for (int i = 0; i < FileBrowser.Result.Length; i++)
				{
					ImportFromPath(FileBrowser.Result[i]);
				}
			}
		}
	}
}